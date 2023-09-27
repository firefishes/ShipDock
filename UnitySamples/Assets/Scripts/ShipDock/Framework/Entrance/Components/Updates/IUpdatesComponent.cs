using System;

namespace ShipDock
{
    public interface IUpdatesComponent
    {
        void Init();
        void SyncToFrame(Action method);
    }
}
