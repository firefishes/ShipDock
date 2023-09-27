using System;
using System.Threading;

namespace ShipDock
{
    public class ThreadTicks : IReclaim
    {
        public const int UNIT_SEC = 1000;
        public const float UNIT_MSEC_F = 0.001f;

        public int FPS { get; private set; }
        public int SleepTime { get; private set; }

        private Thread mThreader;
        private Action<int> mOnUpdate;

        public ThreadTicks(int fps)
        {
            FPS = fps;
            SleepTime = UNIT_SEC / fps;
            ThreadStart start = new ThreadStart(OnTicks);
            mThreader = new Thread(start);
        }

        public void Reclaim()
        {
            Stop();
            mOnUpdate = null;
            mThreader = null;
        }

        public void RefreshThreadSleepTime(int time)
        {
            SleepTime = time;
            FPS = UNIT_SEC / SleepTime;
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
                Thread.Sleep(SleepTime);
                mOnUpdate?.Invoke(SleepTime);
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
    }
}

