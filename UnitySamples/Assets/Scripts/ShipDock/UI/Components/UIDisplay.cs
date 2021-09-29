using System;
using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.UI
{
    public class UIDisplay : MonoBehaviour
    {
        public const int N_UI_DISPLAY_SELECTED = 0;

        [SerializeField]
        private string m_GroupID;
        [SerializeField]
        private bool m_Valid;
        [SerializeField]
        private bool m_IsRadio = true;
        [SerializeField, HideInInspector]
        private bool m_ApplyScaleInsteadActive;//还没想好
        [SerializeField]
        private GameObject[] m_ShowGroup;
        [SerializeField]
        private GameObject[] m_HideGroup;

        private bool mValid;
        private bool mChangedValid;
        private ParamNotice<UIDisplay> mChangedNotice;

        public string GroupID
        {
            get
            {
                return m_GroupID;
            }
        }

        public bool Valid
        {
            set
            {
                if (mValid != value)
                {
                    mValid = value;
                    OnUIDisplayValid();
                }
                else { }
            }
            get
            {
                return mValid;
            }
        }

        public bool IsRadio
        {
            set
            {
                m_IsRadio = value;
            }
            get
            {
                return m_IsRadio;
            }
        }

        public NoticesObserver Events { get; private set; }

        private void Awake()
        {
            Events = new NoticesObserver();
            mChangedNotice = Pooling<ParamNotice<UIDisplay>>.From();
            mChangedNotice.SetNoticeName(N_UI_DISPLAY_SELECTED);

            if (string.IsNullOrEmpty(m_GroupID))
            {
                throw new Exception("UIDisplay group id do not allow empty.");
            }
            else { }
        }

        private void OnDestroy()
        {
            Events?.Clean();
            mChangedNotice?.ToPool();
            Utils.Reclaim(ref m_ShowGroup);
            Utils.Reclaim(ref m_HideGroup);
        }

        private void OnShow(int time)
        {
            if (mChangedNotice == default)
            {
                return;
            }
            else { }

            m_Valid = mValid;

            GameObject item;
            if (mValid)
            {
                int max = m_ShowGroup.Length;
                for (int i = 0; i < max; i++)
                {
                    item = m_ShowGroup[i];
                    TargetVisible(ref item, true);
                }
                max = m_HideGroup.Length;
                for (int i = 0; i < max; i++)
                {
                    item = m_HideGroup[i];
                    TargetVisible(ref item, false);
                }
            }
            else
            {
                int max = m_ShowGroup.Length;
                for (int i = 0; i < max; i++)
                {
                    item = m_ShowGroup[i];
                    TargetVisible(ref item, false);
                }
                max = m_HideGroup.Length;
                for (int i = 0; i < max; i++)
                {
                    item = m_HideGroup[i];
                    TargetVisible(ref item, true);
                }
            }

            "log:UIDisplay {0} get {1}".Log(GroupID, mValid ? "show" : "hide");

            mChangedNotice.ParamValue = this;
            Events.Dispatch(mChangedNotice);
            mChangedNotice.ParamValue = default;

            mChangedValid = false;
        }

        private void TargetVisible(ref GameObject target, bool flag)
        {
            if (target.activeSelf != flag)
            {
                target.SetActive(flag);
            }
            else { }
        }

        private void OnUIDisplayValid()
        {
            if (mChangedValid)
            {
                return;
            }
            else { }

            mChangedValid = true;
            UpdaterNotice.SceneCallLater(OnShow);
        }

        public void AllTargetInvalid()
        {
            GameObject item;
            int max = m_ShowGroup.Length;
            for (int i = 0; i < max; i++)
            {
                item = m_ShowGroup[i];
                TargetVisible(ref item, false);
            }
            max = m_HideGroup.Length;
            for (int i = 0; i < max; i++)
            {
                item = m_HideGroup[i];
                TargetVisible(ref item, false);
            }
        }
    }

}