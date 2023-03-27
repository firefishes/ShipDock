using System;

namespace ShipDock
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