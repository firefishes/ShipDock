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
    public class UIButton : UIBase
    {
        /// <summary>按钮</summary>
        private Button mButton;
        /// <summary>按钮标签</summary>
        private Text mLabel;
        /// <summary>按钮标签值</summary>
        private string mLabelValue;
        /// <summary>按钮点击后的回调函数</summary>
        private UnityAction mOnClicked;

        public bool AutoReset { get; set; }

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

        public string Label
        {
            get
            {
                return mLabelValue;
            }
            set
            {
                mLabelValue = value;
                if (mLabel != default)
                {
                    mLabel.text = mLabelValue;
                }
                else { }
            }
        }

        public UIButton(Button button, UnityAction onClick, string labelValue = "", Text label = default, bool autoReset = true) : base()
        {
            mButton = button;

            AddReferenceUI("btn", button.gameObject);

            if (label != default)
            {
                AddReferenceUI("label", label.gameObject);
            }
            else { }

            mLabel = label;
            Label = labelValue;

            AddClick(onClick);

            button.onClick.AddListener(OnClick);

            AddClean(
                (target) => {
                    button.onClick.RemoveAllListeners();
                });

            AutoReset = autoReset;
        }

        protected override void Purge()
        {
            mButton = default;
            mLabel = default;
            mOnClicked = default;
        }

        /// <summary>
        /// 添加点击后的回调函数
        /// </summary>
        /// <param name="onClick"></param>
        public void AddClick(UnityAction onClick)
        {
            mOnClicked += onClick;
        }

        /// <summary>
        /// 移除点击后的回调函数
        /// </summary>
        /// <param name="onClick"></param>
        public void RemoveClick(UnityAction onClick)
        {
            mOnClicked -= onClick;
        }

        /// <summary>
        /// 设置标签子元素
        /// </summary>
        /// <param name="target"></param>
        public void SetLabel(Text target)
        {
            if (target != default)
            {
                mLabel = target;
                mLabel.text = Label;
                AddReferenceUI("label", target.gameObject);
            }
            else { }
        }

        /// <summary>
        /// 点击按钮，普通模式下下一帧才响应功能，防止多次点击出现的问题
        /// </summary>
        private void OnClick()
        {
            UIValid();
        }

        public void ResetClick()
        {
            Interactable = true;
        }

        protected override void InitUI() { }

        protected override void PropertiesChanged()
        {
            if (!AutoReset)
            {
                Interactable = false;
            }
            else { }

            mOnClicked?.Invoke();
        }

        public override void SetVisible(bool value, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "btn";
            }
            else { }

            base.SetVisible(value, name);
        }
    }
}
