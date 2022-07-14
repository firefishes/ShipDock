using System;
using UnityEngine;
using UnityEngine.UI;

namespace ShipDock.UIControls
{
    public interface ILabel
    {
        string Label { get; set; }
        void SetLabelTarget(Text target);
    }

    public class UILabel : UIBase
    {
        /// <summary>按钮标签</summary>
        private Text mText;
        /// <summary>按钮标签值</summary>
        private string mTextValue;
        
        /// <summary>按钮标题</summary>
        public string Text
        {
            get
            {
                return mTextValue;
            }
            set
            {
                mTextValue = value;

                UIValid();
            }
        }

        public Action<string> OnTextChanged
        {
            get
            {
                return GetUIRaw<string>(UIControlNameRaws.RAW_SET_LABEL);
            }
            set
            {
                InsertUIRaw<string>(UIControlNameRaws.RAW_SET_LABEL, value, true);
            }
        }

        public UILabel(Text label)
        {
            mText = label;

            Action<string> setLabel = (param) => { };

            AddUIRaw(UIControlNameRaws.RAW_SET_LABEL, setLabel);

            Init();
        }

        protected override void Purge()
        {
            mText = default;
        }

        protected override void InitUI()
        {
            if(mText != default)
            {
                UITransform = mText.transform as RectTransform;
            }
            else { }

            AddReferenceUI(UIControlReferenceName.UI_LABEL, mText != default ? mText.gameObject : default);

            InsertUIRaw<string>(UIControlNameRaws.RAW_SET_LABEL, OnSetLabel);
        }

        private void OnSetLabel(string value)
        {
            if (mText != default)
            {
                mText.text = mTextValue;
            }
            else { }
        }

        protected override void InitEvents() { }

        protected override void PropertiesChanged()
        {
            CallRaw(mTextValue, UIControlNameRaws.RAW_SET_LABEL);
        }

        public override void SetVisible(bool value, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = UIControlReferenceName.UI_LABEL;
            }
            else { }

            base.SetVisible(value, name);
        }

        /// <summary>
        /// 设置标签子元素
        /// </summary>
        /// <param name="target"></param>
        public void SetLabelTarget(Text target)
        {
            if (target != default)
            {
                if(mText != default)
                {
                    RemoveReferenceUI(UIControlReferenceName.UI_LABEL, mText.gameObject);
                }
                else { }

                mText = target;
                mText.text = Text;
                AddReferenceUI(UIControlReferenceName.UI_LABEL, target.gameObject);
            }
            else { }
        }


        /// <summary>
        /// 重定向组件
        /// </summary>
        public void RedirectControl(Text label)
        {
            RedirectControl();

            mText = label;

            if (mText != default)
            {
                mText.text = Text;
            }
            else { }

            Init();
        }

        protected override void OnRedirect(UIBase control)
        {
            base.OnRedirect(control);

            RemoveReferenceUI(UIControlReferenceName.UI_LABEL, mText != default ? mText.gameObject : default);

            mText = default;
        }
    }
}
