using System;

namespace ShipDock
{
    public class SyncUpdater : IReclaim
    {
        //双缓冲回调组织器
        private DoubleBuffers<Action<float>> mDoubleBuffer;

        public SyncUpdater() : base()
        {
            mDoubleBuffer = new DoubleBuffers<Action<float>>();
            mDoubleBuffer.OnDequeue += OnTicksLater;
        }

        public void Reclaim()
        {
            mDoubleBuffer?.Reclaim();
        }

        private void OnTicksLater(float time, Action<float> current)
        {
            current?.Invoke(time);
        }

        /// <summary>
        /// 添加在下一帧只需要执行一次的函数
        /// </summary>
        public void CallLater(Action<float> method)
        {
            mDoubleBuffer.Enqueue(method);
        }

        public void Update(float time)
        {
            mDoubleBuffer.UpdateBuffer(time);
        }
    }

}
