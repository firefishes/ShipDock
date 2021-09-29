using ShipDock.Tools;
using System;

namespace ShipDock.UI
{
    public interface IUISubgroup
    {
        string ChangerTaskName { get; }
        float ChangerTaskerDuring { get; }
        Action<TimeGapper> ChangerTaskerHandler { get; }
    }
}