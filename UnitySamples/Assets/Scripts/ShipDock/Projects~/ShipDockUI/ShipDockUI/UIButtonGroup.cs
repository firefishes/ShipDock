using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShipDock.UIControls
{
    public class UIButtonGroup : UIBase
    {
        private class UISelectable
        {
            public bool enabled = true;
            public Sprite normal;
            public Sprite hightlighted;
            public Sprite disabled;
            public Image buttonState;

            public void Clean()
            {
                normal = default;
                hightlighted = default;
                disabled = default;
                buttonState = default;
            }

            public void Init(Button btn)
            {
                SpriteState spriteState = btn.spriteState;

                normal = spriteState.pressedSprite;
                hightlighted = spriteState.highlightedSprite;
                disabled = spriteState.disabledSprite;

                spriteState.pressedSprite = default;
                spriteState.highlightedSprite = default;
                spriteState.disabledSprite = default;

                btn.spriteState = spriteState;
                btn.transition = Selectable.Transition.None;
            }

            private void CheckStateVisible(ref Sprite state, bool visible)
            {
                if (state != default)
                {
                    if (visible)
                    {
                        buttonState.overrideSprite = state;
                    }
                    else
                    {
                        buttonState.overrideSprite = default;
                    }
                }
                else { }
            }

            public void Unselected()
            {
                if (enabled)
                {
                    CheckStateVisible(ref normal, true);
                    CheckStateVisible(ref hightlighted, false);
                }
                else { }
            }

            public void Selected()
            {
                if (enabled)
                {
                    CheckStateVisible(ref normal, false);
                    CheckStateVisible(ref hightlighted, true);
                }
                else { }
            }

            public void Active(bool willSelected = false)
            {
                enabled = true;

                CheckStateVisible(ref disabled, false);

                if (willSelected)
                {
                    Selected();
                }
                else
                {
                    Unselected();
                }
            }

            public void Deactive()
            {
                enabled = false;

                CheckStateVisible(ref normal, false);
                CheckStateVisible(ref hightlighted, false);
                CheckStateVisible(ref disabled, true);
            }
        }

        private int mSelectedIndex = -1;
        private bool mSelectedChanged;
        private bool mButtonClicked;
        private bool mIsDataProviderChanged;
        private UISelectable mSelected;
        private UnityAction<UIButton> mOnButtonClick;
        private List<UIButton> mButtons;
        private List<UISelectable> mSelectables;
        private UIControlData[] mDataProvider;

        public int SelectedIndex
        {
            get
            {
                return mSelectedIndex;
            }
            set
            {
                if (mSelectedIndex != value)
                {
                    mSelectedIndex = value;
                    mSelectedIndex = Mathf.Max(-1, mSelectedIndex);
                    mSelectedIndex = Mathf.Min(mButtons.Count - 1, mSelectedIndex);

                    mSelectedChanged = true;

                    UIValid();
                }
                else { }
            }
        }

        public UIControlData[] DataProvider
        {
            set
            {
                mIsDataProviderChanged = true;
                mDataProvider = value;

                UIValid();
            }
            get
            {
                return mDataProvider;
            }
        }

        public override UIControlData Data
        {
            get
            {
                return base.Data;
            }
            set
            {
                throw new System.Exception("UIButtonGroup do not allow set data, please use DataProvider to set buttonGroup data..");
            }
        }

        public UIControlData SelectedData
        {
            get
            {
                return Data;
            }
        }

        public UIButtonGroup(Button[] btns, Image[] states, UnityAction<UIButton> onClick, string[] labelValues = default, Text[] labels = default)
        {
            mButtons = new List<UIButton>(btns.Length);
            mSelectables = new List<UISelectable>(btns.Length);

            mOnButtonClick += onClick;

            Text label;
            Button btn;
            UISelectable selectable;
            UIButton button;
            string labelValue;
            int max = btns.Length;
            for (int i = 0; i < max; i++)
            {
                label = labels != default ? labels[i] : default;
                labelValue = labelValues != default ? labelValues[i] : string.Empty;
                btn = btns[i];

                button = new UIButton(btn, OnButtonClick, labelValue, label, true);
                mButtons.Add(button);

                selectable = new UISelectable()
                {
                    buttonState = states[i],
                };
                selectable.Init(btn);
                mSelectables.Add(selectable);

                BindChildControl(button);
            }
        }

        protected override void Purge()
        {
            int max = mButtons.Count;
            for (int i = 0; i < max; i++)
            {
                mButtons[i].Clean();
            }

            Utils.Reclaim(ref mButtons);

            mOnButtonClick = default;
        }

        protected override void InitEvents()
        {
        }

        protected override void InitUI()
        {
        }

        protected override void PropertiesChanged()
        {
            bool clickCommited = false;
            if (mSelectedChanged)
            {
                mSelectedChanged = false;

                if (mSelectedIndex >= 0)
                {
                    if (mSelected != default)
                    {
                        mSelected.Unselected();
                    }
                    else { }

                    mSelected = mSelectables[mSelectedIndex];
                    mSelected.Selected();
                }
                else { }

                if (mButtonClicked)
                {
                    mButtonClicked = false;
                    clickCommited = true;
                }
                else { }
            }
            else { }

            if (mIsDataProviderChanged)
            {
                mIsDataProviderChanged = false;

                int max = mButtons.Count;
                for (int i = 0; i < max; i++)
                {
                    mButtons[i].Data = i < mDataProvider.Length ? mDataProvider[i] : default;
                }
            }
            else { }

            UpdateData();

            if (clickCommited)
            {
                UIButton button = mButtons[mSelectedIndex];
                mOnButtonClick?.Invoke(button);
                button.ResetClick();
            }
            else { }
        }

        private void UpdateData()
        {
            if (mDataProvider != default)
            {
                mData = mSelectedIndex > 0 && mSelectedIndex < mDataProvider.Length ? mDataProvider[mSelectedIndex] : default;
            }
            else { }
        }

        private void OnButtonClick(UIButton button)
        {
            int index = mButtons.IndexOf(button);
            if (index >= 0)
            {
                if (SelectedIndex != index)
                {
                    mButtonClicked = true;
                    SelectedIndex = index;
                }
                else { }
            }
            else { }
        }

        public void AddClick(UnityAction<UIButton> click)
        {
            mOnButtonClick += click;
        }

        public void RemoveClick(UnityAction<UIButton> click)
        {
            mOnButtonClick -= click;
        }

        public void SetButtonSkin(int buttonIndex, Sprite normal, Sprite hightlighted, Sprite disabled = default)
        {
            mSelectables[buttonIndex].normal = normal;
            mSelectables[buttonIndex].hightlighted = hightlighted;
            mSelectables[buttonIndex].disabled = disabled;
        }

        public void OnlyChangeSelected()
        {
            mButtonClicked = false;
        }

        public void ButtonEnabled(int buttonIndex, bool enabled)
        {
            UISelectable selectable = mSelectables[buttonIndex];
            if (enabled)
            {
                if (selectable.disabled)
                {
                    selectable.Active();
                }
                else { }
            }
            else
            {
                selectable.Deactive();
            }
        }
    }
}
