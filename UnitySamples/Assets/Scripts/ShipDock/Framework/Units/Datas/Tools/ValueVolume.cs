using System;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 值容量结构
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public struct ValueVolume
    {
        private float mMax;
        private float mCurrent;
        private float mScaler;
        private bool mIsRestrict;

        public float Max
        {
            get
            {
                return mScaler > 0 ? mMax / mScaler : mMax;
            }
            set
            {
                mMax = value * mScaler;
            }
        }

        public float Current
        {
            get
            {
                return mScaler > 0 ? mCurrent / mScaler : mCurrent;
            }
            set
            {
                mCurrent = value * mScaler;

                if (mIsRestrict)
                {
                    mCurrent = Math.Max(0, mCurrent);
                    mCurrent = Math.Min(mCurrent, mMax);
                }
                else { }
            }
        }

        public void Volumn(float maxValue, float scalerValue = 1f, bool isRestrict = true)
        {
            mScaler = scalerValue;
            mMax = maxValue * mScaler;
            mCurrent = mMax;
            mIsRestrict = isRestrict;
        }
    }
}
