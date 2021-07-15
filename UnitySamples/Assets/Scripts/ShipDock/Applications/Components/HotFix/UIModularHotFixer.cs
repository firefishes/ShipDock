using ShipDock.Datas;
using ShipDock.Notices;
using System;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// 热更端 UI 模块基类
    /// 
    /// 对标于纯主工程下开发所使用的 UIModular<T> 类 
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class UIModularHotFixer : UIModular<HotFixerUIAgent>
    {
        private HotFixerUI mBridge;
        private UIModularHotFixer mUIHotFixer;

        protected Func<HotFixerInteractor> UIInteracterCreater { get; set; }
        protected Action<INoticeBase<int>> UIInteracterHandler { get; set; }

        public override int[] DataProxyLinks { get; set; }

        protected HotFixerUIAgent UIAgent
        {
            get
            {
                return UI;
            }
        }

        /// <summary>
        /// 
        /// 数据层消息处理器函数，子类覆盖此方法修改 UI 模块响应数据的处理逻辑
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keyName"></param>
        public override void OnDataProxyNotify(IDataProxy data, int keyName) { }

        /// <summary>
        /// 
        /// UI 消息处理器函数，子类覆盖此方法修改 UI 消息处理逻辑
        /// 
        /// 需要注意：
        /// 由于 ILRuntime 的热更端不与主工程共享一个定义域UI消息被派发时，
        /// 所有子类的此方法都会被调用，故 UI 消息也必须和广播消息一样保证全局唯一
        /// 
        /// </summary>
        /// <param name="param"></param>
        protected override void UIModularHandler(INoticeBase<int> param) { }

        sealed public override void Init()
        {
            base.Init();

            mUIHotFixer = this;

            HotFixerInteractor interacter = UIInteracterCreater?.Invoke();
            interacter.SetUIModular(mUIHotFixer);

            if (UIInteracterHandler != default)
            {
                mUI.Remove(UIModularHandler);
                mUI.Add(UIInteracterHandler);
                "log: UI {0} add modular handler (UIInteracterHandler), UI type is ".Log(mUI.ToString());
            }
            else { }

            mBridge = UIAgent.Bridge;
            mBridge.SetHotFixInteractor(interacter);

            ILRuntimeUtils.InvokeMethodILR(mUIHotFixer, UIAgent.UIModularName, "UIInit", 0);
        }

        sealed public override void Enter()
        {
            base.Enter();

            ILRuntimeUtils.InvokeMethodILR(mUIHotFixer, UIAgent.UIModularName, "UIEnter", 0);
        }

        protected sealed override void Purge()
        {
            if (UIInteracterHandler != default)
            {
                mUI.Remove(UIInteracterHandler);
            }
            else { }

            ILRuntimeUtils.InvokeMethodILR(mUIHotFixer, UIAgent.UIModularName, "UIExit", 0);

            mBridge = default;
            mUIHotFixer = default;
        }

        public sealed override void Exit(bool isDestroy)
        {
            base.Exit(isDestroy);
        }

        public sealed override void Renew()
        {
            base.Renew();

            if (!IsExited)
            {
                ILRuntimeUtils.InvokeMethodILR(mUIHotFixer, UIAgent.UIModularName, "UIRenew", 0);
            }
            else { }
        }

        public sealed override void Interrupt()
        {
            base.Interrupt();

            if (!IsExited)
            {
                ILRuntimeUtils.InvokeMethodILR(mUIHotFixer, UIAgent.UIModularName, "UIInterrupt", 0);
            }
            else { }
        }

        protected sealed override void HideUI()
        {
            base.HideUI();

            ILRuntimeUtils.InvokeMethodILR(mUIHotFixer, UIAgent.UIModularName, "UIHide", 0);
        }

        protected sealed override void ShowUI()
        {
            base.ShowUI();

            ILRuntimeUtils.InvokeMethodILR(mUIHotFixer, UIAgent.UIModularName, "UIShow", 0);
        }
    }
}