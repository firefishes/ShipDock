using ShipDock.Datas;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.UI;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 基于界面接口方式的 UI 模块实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="InterfaceT"></typeparam>
    public abstract class UIModularImpl<T, InterfaceT> : UIModular<T> where T : MonoBehaviour, InterfaceT, INotificationSender
    {
        protected InterfaceT UIImpl { get; private set; }

        public override void Dispose()
        {
            base.Dispose();

            UIImpl = default;
        }

        protected override void FillUIInstance(ref T ui)
        {
            base.FillUIInstance(ref ui);

            UIImpl = ui;
        }
    }

    /// <summary>
    /// UI模块
    /// </summary>
    public abstract class UIModular<T> : UIStack, IUIModular, IDataExtracter where T : MonoBehaviour, INotificationSender
    {
        protected T mUI;

        protected T UI
        {
            get
            {
                return mUI;
            }
        }

        public override bool IsStackable
        {
            get
            {
                return UILayer == UILayerType.WINDOW;
            }
        }

        protected IAssetBundles ABs { get; private set; }
        protected UIManager UIs { get; private set; }
        protected DataWarehouse Datas { get; private set; }

        /// <summary>UI模块的资源包名</summary>
        public virtual string ABName { get; }
        /// <summary>需要关联的数据代理</summary>
        public abstract int[] DataProxyLinks { get; set; }
        /// <summary>UI 关闭后的回调函数</summary>
        public Action OnUIClose { get; set; }

        public UIModular() { }

        public UIModular(T ui)
        {
            FillUIInstance(ref ui);
        }

        public virtual void Dispose()
        {
            this.DataProxyDelink(DataProxyLinks);
            mUI.Remove(UIModularHandler);

            Purge();

            UnityEngine.Object.Destroy(mUI.gameObject);

            OnUIClose = default;
            UIs = default;
            ABs = default;
            mUI = default;
            Datas = default;
        }

        protected abstract void Purge();

        public override void Init()
        {
            base.Init();

            ShipDockApp app = ShipDockApp.Instance;
            Datas = app.Datas;
            ABs = app.ABs;
            UIs = app.UIs;

            if (mUI == default)
            {
                GameObject prefab = ABs.Get(ABName, UIAssetName);
                GameObject ui = UnityEngine.Object.Instantiate(prefab, UIs.UIRoot.MainCanvas.transform);

                ParamNotice<MonoBehaviour> notice = Pooling<ParamNotice<MonoBehaviour>>.From();

                int id = ui.GetInstanceID();
                id.Broadcast(notice);

                T instance = (T)notice.ParamValue;
                FillUIInstance(ref instance);
                notice.ToPool();
            }
            else { }

            UILayer layer = mUI.GetComponent<UILayer>();
            SetUIParent(ref layer);
            mUI.Add(UIModularHandler);
        }

        protected virtual void FillUIInstance(ref T ui)
        {
            mUI = ui;
        }

        private void SetUIParent(ref UILayer layer)
        {
            if (layer != default)
            {
                UILayer = layer.UILayerValue;
            }
            else { }

            Transform parent = default;
            IUIRoot root = UIs.UIRoot;
            switch (UILayer)
            {
                case UILayerType.WINDOW:
                    parent = root.Windows;
                    break;
                case UILayerType.POPUPS:
                    parent = root.Popups;
                    break;
                case UILayerType.WIDGET:
                    parent = root.Widgets;
                    break;
                default:
                    parent = root.MainCanvas.transform;
                    break;
            }

            mUI.transform.SetParent(parent);

            if (UILayer == UILayerType.POPUPS)
            {
                mUI.transform.SetAsLastSibling();
            }
            else { }
        }

        public override void Enter()
        {
            base.Enter();

            CheckDisplay();
        }

        public override void Renew()
        {
            base.Renew();

            CheckDisplay();
        }

        private void CheckDisplay()
        {
            this.DataProxyLink(DataProxyLinks);

            if (mUI != default)
            {
                ShowUI();
            }
            else { }
        }

        /// <summary>
        /// 覆盖此方法，重载界面显示的逻辑
        /// </summary>
        protected virtual void ShowUI()
        {
            if (UILayer == UILayerType.POPUPS)
            {
                mUI.transform.SetAsLastSibling();
            }
            else { }

            mUI.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 覆盖此方法，重载界面隐藏的逻辑
        /// </summary>
        protected virtual void HideUI()
        {
            mUI.transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// UI模块注册在UI资源中的消息器处理函数，用于模块与UI资源体的通信
        /// </summary>
        protected abstract void UIModularHandler(INoticeBase<int> param);

        /// <summary>
        /// UI模块注册在数据代理中的消息处理器函数，用于模块与数据的通信
        /// </summary>
        public abstract void OnDataProxyNotify(IDataProxy data, int keyName);

        public override void Exit(bool isDestroy)
        {
            base.Exit(isDestroy);

            if (mUI != default)
            {
                if (isDestroy)
                {
                    Dispose();
                }
                else
                {
                    this.DataProxyDelink(DataProxyLinks);

                    HideUI();

                    OnUIClose?.Invoke();
                    OnUIClose = default;
                }
            }
            else { }
        }
    }
}