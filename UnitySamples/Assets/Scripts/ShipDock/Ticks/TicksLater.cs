using ShipDock.Interfaces;
using ShipDock.Tools;
using System;

namespace ShipDock.Ticks
{
    public class TicksLater : IReclaim
    {
        private DoubleBuffers<Action<int>> mDoubleBuffer;

        public TicksLater() : base()
        {
            mDoubleBuffer = new DoubleBuffers<Action<int>>();
            mDoubleBuffer.OnDequeue += OnTicksLater;
        }

        public void Reclaim()
        {
            mDoubleBuffer?.Reclaim();
        }

        private void OnTicksLater(int time, Action<int> current)
        {
            current?.Invoke(time);
        }

        /// <summary>
        /// 添加在下一帧只需要执行一次的函数
        /// </summary>
        public void CallLater(Action<int> method)
        {
            mDoubleBuffer.Enqueue(method);
        }

        public void Update(int time)
        {
            mDoubleBuffer.Update(time);
        }
    }

}
