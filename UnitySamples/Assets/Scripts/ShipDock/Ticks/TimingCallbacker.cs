using ShipDock.Interfaces;
using System;

namespace ShipDock.Ticks
{
    public class TimingCallbacker : IReclaim
    {
        public float timing;
        public Action callback;

        public void Reclaim()
        {
            callback = default;
        }
    }
}