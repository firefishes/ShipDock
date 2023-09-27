using UnityEngine;

namespace ShipDock
{

    public class UIArea : MonoBehaviour
    {
        public const int N_UI_AREA_CHANGED = 0;
        public const int N_UI_AREA_INITED = 1;

        [SerializeField]
        private UIDisplay[] m_Displays;

        private UIDisplay mCurrent;
        private ParamNotice<UIDisplay> mUIAreaNotice;
        private StringIntValueList mMapper;

        public NoticesObserver Events { get; private set; }

        private void Awake()
        {
            Events = new NoticesObserver();
            mMapper = new StringIntValueList();

            mUIAreaNotice = Pooling<ParamNotice<UIDisplay>>.From();
            mUIAreaNotice.SetNoticeName(N_UI_AREA_CHANGED);
        }

        private void Start()
        {
            UIDisplay item;
            int max = m_Displays.Length;
            for (int i = 0; i < max; i++)
            {
                item = m_Displays[i];
                item.Events.AddListener(UIDisplay.N_UI_DISPLAY_SELECTED, OnUIDisplayChanged);
                mMapper[item.GroupID] = i;
            }

            Events.Dispatch(N_UI_AREA_INITED);
        }

        private void OnDestroy()
        {
            Events?.Clean();

            mMapper?.Reclaim();
            mUIAreaNotice?.ToPool();
            mUIAreaNotice = default;
            mCurrent = default;

            UIDisplay item;
            int max = m_Displays.Length;
            for (int i = 0; i < max; i++)
            {
                item = m_Displays[i];
                item.Events?.RemoveListener(UIDisplay.N_UI_DISPLAY_SELECTED, OnUIDisplayChanged);
            }
            Utils.Reclaim(ref m_Displays);
        }

        private void OnUIDisplayChanged(INoticeBase<int> param)
        {
            mUIAreaNotice.ParamValue = (param as IParamNotice<UIDisplay>).ParamValue;
            Events.Dispatch(mUIAreaNotice);
            mUIAreaNotice.ParamValue = default;
        }

        public UIDisplay GetAreaDisplay(string name)
        {
            UIDisplay result = default;
            if (mMapper.ContainsKey(name))
            {
                int index = mMapper[name];
                result = m_Displays[index];
            }
            else { }

            return result;
        }

        public void DisplayView(string groupID, bool displayFlag, bool invalidPrevAllTargets = false)
        {
            if (mUIAreaNotice != default & mMapper.ContainsKey(groupID))
            {
                int index = mMapper[groupID];
                UIDisplay item = m_Displays[index];
                if (item != default)
                {
                    if (mCurrent != default)
                    {
                        if (invalidPrevAllTargets)
                        {
                            mCurrent.AllTargetInvalid();
                        }
                        else { }
                    }
                    else { }

                    if (item.IsRadio)
                    {
                        mCurrent = item;
                    }
                    else { }

                    item.Valid = displayFlag;
                }
                else { }
            }
            else
            {
                "error:Do not contais UIDisplay whitch named {0}".Log(groupID);
            }
        }
    }

}