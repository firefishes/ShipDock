using ShipDock.Interfaces;
using System;
using System.Threading;

namespace ShipDock.Ticks
{
    public class ThreadTicks : IReclaim
    {
        public const int UNIT_SEC = 1000;

        private int mSleepTime;
        private Thread mThreader;
        private Action<int> mOnUpdate;

        public ThreadTicks(int fps)
        {
            FPS = fps;
            ThreadStart start = new ThreadStart(OnTicks);
            mThreader = new Thread(start);
        }

        public void Reclaim()
        {
            Stop();
            mOnUpdate = null;
            mThreader = null;
        }

        public void Add(Action<int> method)
        {
            mOnUpdate += method;
        }

        public void Remove(Action<int> method)
        {
            mOnUpdate -= method;
        }

        private void OnTicks()
        {
            while (true)
            {
                Thread.Sleep(mSleepTime);
                mOnUpdate?.Invoke(mSleepTime);
            }
        }

        public void Start()
        {
            mThreader?.Start();
        }

        public void Stop()
        {
            mThreader?.Abort();
        }

        public int FPS
        {
            set => mSleepTime = UNIT_SEC / value;
        }
    }
}

