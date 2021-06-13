using System;
using UnityEngine.UI;

namespace ShipDock.UIControls
{
    public class UISwitch : UIBase
    {
        public string LabelOn { get; private set; } = "ON";
        public string LabelOff { get; private set; } = "OFF";
        public Action<bool> OnChanged { get; set; }

        private bool mTurnOn;

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
            if (label != default)
            {
                AddReferenceUI("Label", label.gameObject);

                AddUIRaw("SetLabel", (string value) =>
                {
                    label.text = value;
                });
            }
            else { }

            AddReferenceUI("BtnOn", UIBtnOn.gameObject);
            AddReferenceUI("BtnOff", UIBtnOff.gameObject);

            UIBtnOn.onClick.AddListener(OnSwtichClick);
            UIBtnOff.onClick.AddListener(OnSwtichClick);

            AddClean((v) => {
                UIBtnOn.onClick.RemoveListener(OnSwtichClick);
                UIBtnOff.onClick.RemoveListener(OnSwtichClick);
            });
        }

        protected override void Purge()
        {
            OnChanged = default;
        }

        private void OnSwtichClick()
        {
            TurnOn = !TurnOn;
        }

        protected override void PropertiesChanged()
        {
            string value = TurnOn ? LabelOff : LabelOn;
            CallRaw(value, "SetLabel");

            SetVisible(!TurnOn, "BtnOn");
            SetVisible(TurnOn, "BtnOff");

            OnChanged?.Invoke(TurnOn);
        }

        protected override void InitUI()
        {
            OnInited?.Invoke();
        }
    }
}
