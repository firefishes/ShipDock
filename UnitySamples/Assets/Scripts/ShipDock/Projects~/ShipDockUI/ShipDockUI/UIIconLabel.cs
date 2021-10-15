using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShipDock.UIControls
{
    /// <summary>
    /// 
    /// 附带图标的文本控件
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class UIIconLabel : UIBase, ILabel
    {
        private Sprite mImageSource;
        private Image mIcon;
        private UILabel mLabel;
        private UIButton mButton;

        /// <summary>值发生变化时的回调</summary>
        public Action<string> OnTextChanged
        {
            get
            {
                return mLabel.OnTextChanged;
            }
            set
            {
                mLabel.OnTextChanged = value;
            }
        }

        public string Label
        {
            get
            {
                return mLabel.Text;
            }
            set
            {
                mLabel.Text = value;
            }
        }

        public Sprite ImageSource
        {
            get
            {
                return mIcon != default ? mIcon.overrideSprite : default;
            }
            set
            {
                mImageSource = value;

                UIValid();
            }
        }

        public UIIconLabel(Image icon, Text label = default, Button button = default, UnityAction<UIButton> onClick = default)
        {
            mLabel = new UILabel(label);

            if (button != default)
            {
                mButton = new UIButton(button, onClick);
            }
            else { }

            mIcon = icon;

            Init();
        }

        protected override void Purge()
        {
            mLabel?.Clean();
            mButton?.Clean();

            mLabel = default;
            mButton = default;
        }

        protected override void InitUI()
        {
            UITransform = mIcon.rectTransform;

            BindChildControl(mLabel);
            BindChildControl(mButton);
        }

        protected override void InitEvents()
        {
        }

        protected override void PropertiesChanged()
        {
            if (mImageSource != default)
            {
                mIcon.overrideSprite = mImageSource;
                mImageSource = default;
            }
            else { }
        }

        public void SetLabelTarget(Text target)
        {
            mLabel.SetLabelTarget(target);
        }

        public override void SetVisible(bool value, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                mIcon.gameObject.SetActive(value);
                mLabel.SetVisible(value);

                mButton?.SetVisible(value);
            }
            else
            {
                base.SetVisible(value, name);
            }
        }
    }
}
