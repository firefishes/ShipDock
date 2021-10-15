using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShipDock.UIControls
{
    /// <summary>
    /// 
    /// 按钮控件
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class UIButton : UIBase, ILabel
    {
        /// <summary>按钮</summary>
        private Button mButton;
        /// <summary>按钮索引</summary>
        private int mIndex = -1;
        /// <summary>按钮点击后的回调函数</summary>
        private UnityAction<UIButton> mButtonClickHandler;
        /// <summary>按钮是否已被按下</summary>
        private bool mClicked;

        private UILabel mLabel;

        public int Index
        {
            get
            {
                return mIndex;
            }
            set
            {
                mIndex = value;
                UIValid();
            }
        }

        /// <summary>按钮是否可交互</summary>
        public bool Interactable
        {
            get
            {
                return mButton != default ? mButton.interactable : false;
            }
            set
            {
                if (mButton != default)
                {
                    mButton.interactable = value;
                }
                else { }
            }
        }

        /// <summary>按钮标题</summary>
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

        /// <summary>按钮在交互后是否自动重置可用性</summary>
        public bool AutoReset { get; set; }

        public UIButton(Button button, UnityAction<UIButton> onClick, string labelValue = "", Text label = default, bool autoReset = true) : base()
        {
            mLabel = new UILabel(label);

            mButton = button;
            Label = labelValue;
            AutoReset = autoReset;

            AddClick(onClick);

            Init();
        }

        protected override void Purge()
        {
            mLabel.Clean();

            mButton = default;
            mLabel = default;
            mButtonClickHandler = default;
        }

        protected override void InitUI()
        {
            UITransform = mButton.transform as RectTransform;

            AddReferenceUI(UIControlReferenceName.UI_BTN, mButton.gameObject);

            BindChildControl(mLabel);
        }

        protected override void InitEvents()
        {
            mButton.onClick.AddListener(OnClick);
            AddClean<UIBase>(OnCleanBtn);
        }

        private void OnCleanBtn(UIBase ui)
        {
            if (mButton != default)
            {
                mButton.onClick.RemoveAllListeners();
            }
            else { }
        }

        /// <summary>
        /// 重定向组件
        /// </summary>
        public void RedirectControl(Button button, Text label = default)
        {
            RedirectControl();

            mLabel.RedirectControl(label);

            mButton = button;

            Init();
        }

        protected override void OnRedirect(UIBase control)
        {
            base.OnRedirect(control);

            mButton.onClick.RemoveAllListeners();
            RemoveClean<UIBase>(OnCleanBtn);

            RemoveReferenceUI(UIControlReferenceName.UI_BTN, mButton.gameObject);

            mButton = default;
        }

        /// <summary>
        /// 添加点击后的回调函数
        /// </summary>
        /// <param name="onClick"></param>
        public void AddClick(UnityAction<UIButton> onClick, bool isReset = false)
        {
            if (isReset)
            {
                mButtonClickHandler = default;
            }
            else { }

            if (onClick != default)
            {
                mButtonClickHandler += onClick;
            }
            else { }
        }

        /// <summary>
        /// 移除点击后的回调函数
        /// </summary>
        /// <param name="onClick"></param>
        public void RemoveClick(UnityAction<UIButton> onClick)
        {
            mButtonClickHandler -= onClick;
        }

        /// <summary>
        /// 设置标签子元素
        /// </summary>
        /// <param name="target"></param>
        public void SetLabelTarget(Text target)
        {
            mLabel.SetLabelTarget(target);
        }

        /// <summary>
        /// 点击按钮，普通模式下下一帧才响应功能，防止多次点击出现的问题
        /// </summary>
        private void OnClick()
        {
            if (mClicked)
            {
                return;
            }
            else { }

            mClicked = true;

            UIValid();
        }

        public void ResetClick()
        {
            Interactable = true;
        }

        protected override void PropertiesChanged()
        {
            if (mClicked)
            {
                if (Interactable)
                {
                    mButtonClickHandler?.Invoke(this);
                }
                else { }

                if (!AutoReset)
                {
                    Interactable = false;
                }
                else { }
            }
            else { }

            mClicked = false;
        }

        public override void SetVisible(bool value, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = UIControlReferenceName.UI_BTN;
            }
            else { }

            base.SetVisible(value, name);
        }
    }
}
