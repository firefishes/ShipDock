using System;

namespace ShipDock
{
    public interface IUISubgroup
    {
        string ChangerTaskName { get; }
        float ChangerTaskerDuring { get; }
        Action<TimeGapper> ChangerTaskerHandler { get; }
    }
}