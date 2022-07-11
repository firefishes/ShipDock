using System;

namespace ShipDock.Tools
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
        private int mMax;
        private int mCurrent;
        private int mScaler;
        private bool mIsRestrict;

        public int Max
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

        public int Current
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

        public void Volumn(int maxValue, int scalerValue = 1, bool isRestrict = true)
        {
            mScaler = scalerValue;
            mMax = maxValue * mScaler;
            mCurrent = mMax;
            mIsRestrict = isRestrict;
        }
    }
}
