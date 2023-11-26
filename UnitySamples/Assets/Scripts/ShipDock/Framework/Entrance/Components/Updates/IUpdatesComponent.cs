using System;

namespace ShipDock
{
    public interface IUpdatesComponent
    {
        void Init();
        void SyncToFrame(Action method);
        void AddCallLate(Action<float> target);
        void AddUpdate(IUpdate target);
        void RemoveUpdate(IUpdate target);
        void AddFixedUpdate(IUpdate target);
        void RemoveFixedUpdate(IUpdate target);
        void AddLateUpdate(IUpdate target);
        void RemoveLateUpdate(IUpdate target);
    }
}
