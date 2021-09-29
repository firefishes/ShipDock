using ShipDock.Notices;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.UIControls
{
    /// <summary>
    /// 
    /// UI控件基类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class UIBase
    {
        /// <summary>控件子元素名称</summary>
        private List<string> mNameReferens;
        /// <summary>控件子元素物体引用</summary>
        private List<GameObject> mUIReferens;
        /// <summary>控件子元素源功能名称</summary>
        private List<string> mNameRaws;
        /// <summary>控件子元素源功能引用</summary>
        private List<IUIRaw> mUIRaws;
        /// <summary>控件数据</summary>
        private UIControlData mData;

        public virtual UIControlData Data
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;

                UIValid();
            }
        }

        /// <summary>是否已被清除</summary>
        public bool IsClean { get; protected set; }
        /// <summary>是否开启控件的延后执行功能</summary>
        public bool ApplyCallLate { get; set; }
        /// <summary>控件初始化完成后的回调函数</summary>
        public Action OnInited { get; set; }
        /// <summary>控件对应的UI变换组件</summary>
        public RectTransform UITransform { get; protected set; }

        public UIBase()
        {
            ApplyCallLate = true;

            mNameReferens = new List<string>();
            mUIReferens = new List<GameObject>();

            mNameRaws = new List<string>();
            mUIRaws = new List<IUIRaw>();

            void clean(UIBase param) { }
            Action<UIBase> redirect = (param) => { };
            redirect += OnRedirect;

            AddUIRaw<UIBase>(UIControlNameRaws.RAW_DURING_CLEAN, clean);//增加自动回收的源功能
            AddUIRaw(UIControlNameRaws.REDIRECT_CONTROL, redirect);//增加自动回收的源功能

            //子类须在构造函数中主动调用以下代码执行各自的初始化流程
            //Init();
        }

        /// <summary>
        /// 清除控件
        /// </summary>
        public virtual void Clean()
        {
            if (IsClean)
            {
                return;
            }
            else { }

            IsClean = true;

            Purge();

            CallRaw(this, UIControlNameRaws.RAW_DURING_CLEAN);//调用自动回收的方法

            mNameReferens?.Clear();
            mUIReferens?.Clear();
            mNameRaws?.Clear();
            mUIRaws?.Clear();

            OnInited = default;
            UITransform = default;
        }

        /// <summary>
        /// 控件子类的回收
        /// </summary>
        protected abstract void Purge();

        /// <summary>
        /// 初始化控件，须由各子类在构造函数中主动调用
        /// </summary>
        protected virtual void Init()
        {
            InitUI();
            InitEvents();
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        protected abstract void InitUI();

        /// <summary>
        /// 初始化事件
        /// </summary>
        protected abstract void InitEvents();

        /// <summary>
        /// 添加自动回收时需要调用的方法
        /// </summary>
        /// <param name="method"></param>
        protected void AddClean<T>(Action<T> method)
        {
            if (method != default)
            {
                Action<T> target = GetUIRaw<T>(UIControlNameRaws.RAW_DURING_CLEAN);
                target += method;
            }
            else { }
        }

        /// <summary>
        /// 移除自动回收时需要调用的方法
        /// </summary>
        /// <param name="method"></param>
        protected void RemoveClean<T>(Action<T> method)
        {
            if (method != default)
            {
                Action<T> target = GetUIRaw<T>(UIControlNameRaws.RAW_DURING_CLEAN);
                target -= method;
            }
            else { }
        }

        /// <summary>
        /// 添加控件子元素引用
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        protected void AddReferenceUI(string name, GameObject target)
        {
            if (target != default)
            {
                mNameReferens.Add(name);
                mUIReferens.Add(target);
            }
            else { }
        }

        /// <summary>
        /// 移除控件子元素引用
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        protected void RemoveReferenceUI(string name, GameObject target)
        {
            if (target != default)
            {
                mNameReferens.Remove(name);
                mUIReferens.Remove(target);
            }
            else { }
        }

        /// <summary>
        /// 添加控件源功能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="method"></param>
        protected void AddUIRaw<T>(string name, Action<T> method)
        {
            if (method != default)
            {
                mNameRaws.Add(name);
                mUIRaws.Add(new UIRaw<Action<T>>(method));
            }
            else { }
        }

        /// <summary>
        /// 设置子元素显示与否
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public virtual void SetVisible(bool value, string name = "")
        {
            int index = mNameReferens.IndexOf(name);
            if (index >= 0)
            {
                mUIReferens[index]?.SetActive(value);
            }
            else { }
        }

        /// <summary>
        /// 调用控件源功能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="name"></param>
        protected void CallRaw<T>(T param, string name)
        {
            Action<T> method = GetUIRaw<T>(name);
            method?.Invoke(param);
        }

        /// <summary>
        /// 获取控件的源功能引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        private Action<T> GetUIRaw<T>(string name)
        {
            Action<T> result = default;
            int index = mNameRaws.IndexOf(name);
            if (index >= 0)
            {
                IUIRaw item = mUIRaws[index];
                if (item is UIRaw<Action<T>> target)
                {
                    if (target != default)
                    {
                        result = target.raw;
                    }
                    else { }
                }
                else { }
            }
            else { }
            return result;
        }

        /// <summary>
        /// 标记控件的变更为生效状态
        /// </summary>
        public virtual void UIValid()
        {
            if (ApplyCallLate)
            {
                UpdaterNotice.SceneCallLater(OnNextFrameUpdate);
            }
            else
            {
                OnNextFrameUpdate(0);
            }
        }

        private void OnNextFrameUpdate(int time)
        {
            DataChanged();
            PropertiesChanged();
            AfterUIValid();
        }

        /// <summary>
        /// 数据发生变化
        /// </summary>
        protected virtual void DataChanged() { }

        /// <summary>
        /// 控件属性发生变更
        /// </summary>
        protected abstract void PropertiesChanged();

        /// <summary>
        /// 处理控件更新后的流程
        /// </summary>
        protected virtual void AfterUIValid() { }

        protected void RedirectControl()
        {
            CallRaw(this, UIControlNameRaws.REDIRECT_CONTROL);//调用重定向方法
        }

        /// <summary>
        /// 子类覆盖此方法实现子类控件的重定向功能
        /// </summary>
        protected virtual void OnRedirect(UIBase control)
        {
            UITransform = default;
        }
    }
}