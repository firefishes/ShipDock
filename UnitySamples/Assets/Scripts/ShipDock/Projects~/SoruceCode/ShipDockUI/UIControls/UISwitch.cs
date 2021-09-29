using ShipDock.Tools;
using System;
using UnityEngine.UI;

namespace ShipDock.UIControls
{
    /// <summary>
    /// 
    /// 开关控件
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class UISwitch : UIBase
    {
        /// <summary>内容为开启的标题</summary>
        public string LabelOn { get; private set; } = "ON";
        /// <summary>内容为关闭的标题</summary>
        public string LabelOff { get; private set; } = "OFF";
        /// <summary>开关状态发生变化时的回调</summary>
        public Action<bool> OnChanged { get; set; }

        /// <summary>是否为开启状态</summary>
        private bool mTurnOn;
        /// <summary>标题文本</summary>
        private Text mLabel;
        /// <summary>按钮组</summary>
        private Button[] mButtons; 

        /// <summary>切换开启或关闭状态</summary>
        public bool TurnOn
        {
            get
            {
                return mTurnOn;
            }
            set
            {
                mTurnOn = value;
                UIValid();
            }
        }

        public UISwitch(Button UIBtnOn, Button UIBtnOff, Text label = default) : base()
        {
            mLabel = label;
            mButtons = new Button[] { UIBtnOn, UIBtnOff };

            Init();
        }

        protected override void Purge()
        {
            Utils.Reclaim(ref mButtons, true);

            mLabel = default;
            OnInited = default;
            OnChanged = default;
        }

        protected override void InitUI()
        {
            AddReferenceUI(UIControlReferenceName.UI_LABEL, mLabel != default ? mLabel.gameObject : default);
            AddReferenceUI(UIControlReferenceName.UI_BTN_ON, mButtons[0].gameObject);
            AddReferenceUI(UIControlReferenceName.UI_BTN_OFF, mButtons[1].gameObject);
        }

        protected override void InitEvents()
        {
            mButtons[0].onClick.AddListener(OnSwtichClick);
            mButtons[1].onClick.AddListener(OnSwtichClick);

            AddClean<UIBase>(OnCleanUISwitch);

            if (mLabel != default)
            {
                AddUIRaw<string>(UIControlNameRaws.RAW_SET_LABEL, OnSetLabel);
            }
            else { }

            OnInited?.Invoke();
        }

        private void OnSetLabel(string value)
        {
            mLabel.text = value;
        }

        private void OnCleanUISwitch(UIBase ui)
        {
            mButtons[0].onClick.RemoveListener(OnSwtichClick);
            mButtons[1].onClick.RemoveListener(OnSwtichClick);
        }

        private void OnSwtichClick()
        {
            TurnOn = !TurnOn;
        }

        protected override void PropertiesChanged()
        {
            string value = TurnOn ? LabelOff : LabelOn;
            CallRaw(value, UIControlNameRaws.RAW_SET_LABEL);

            SetVisible(false == TurnOn, UIControlReferenceName.UI_BTN_ON);
            SetVisible(true == TurnOn, UIControlReferenceName.UI_BTN_OFF);

            OnChanged?.Invoke(TurnOn);
        }
    }
}
