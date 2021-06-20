using ShipDock.Datas;
using ShipDock.Notices;
using System;

namespace ShipDock.Applications
{
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

        public override void OnDataProxyNotify(IDataProxy data, int keyName) { }

        protected override void UIModularHandler(INoticeBase<int> param) { }

        sealed public override void Init()
        {
            base.Init();

            if (UIInteracterHandler != default)
            {
                mUI.Remove(UIModularHandler);
                mUI.Add(UIInteracterHandler);
            }
            else { }

            HotFixerInteractor interacter = UIInteracterCreater?.Invoke();
            interacter.SetUIModular(this);

            mBridge = UIAgent.Bridge;
            mBridge.SetHotFixInteractor(interacter);

            mUIHotFixer = mBridge.HotFixerInteractor.UIModular;

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