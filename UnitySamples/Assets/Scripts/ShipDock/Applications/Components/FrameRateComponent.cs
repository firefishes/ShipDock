
using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// 帧率统计控件
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class FrameRateComponent : MonoBehaviour
    {
        public bool isActive = true;
        public float showTimeGap = 0.7f;

        [SerializeField]
        private Text m_LabelText;
        [SerializeField]
        private Text m_FrameRateText;

        private int mFrameRate;
        private string mFrameRateValue;
        private float mFPS = 0;
        private float mTimeDelta;
        private float mFrameCount = 0;
        private float mLastShowTime;
        private ParamNotice<int> mFPSNotice;

        private void Start()
        {
            showTimeGap = 0.7f;
            mLastShowTime = Time.realtimeSinceStartup;

            if (m_LabelText != default)
            {
                m_LabelText.text = "FPS ";
            }
            else { }
        }

        private void Update()
        {
            if (isActive)
            {
                if (mFPSNotice == null)
                {
                    mFPSNotice = Pooling<ParamNotice<int>>.From();
                }
                else { }

                mFrameCount++;
                mTimeDelta = Time.realtimeSinceStartup - mLastShowTime;
                if (mTimeDelta >= showTimeGap)
                {
                    mFPS = mFrameCount / mTimeDelta;
                    mFrameCount = 0;
                    mLastShowTime = Time.realtimeSinceStartup;
                }
                else { }

                mFrameRate = (int)mFPS;
                ShipDockConsts.NOTICE_FPS_SHOW.BroadcastWithParam(mFrameRate);
                if (m_FrameRateText != default)
                {
                    mFrameRateValue = mFrameRate.ToString();
                    m_FrameRateText.text = mFrameRateValue;
                }
                else { }
            }
            else { }
        }
    }
}