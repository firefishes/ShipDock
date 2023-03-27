using System;

namespace ShipDock
{
    public class MethodUpdater : IUpdate, IReclaim
    {

        public Action<int> Update { get; set; }
        public Action<int> FixedUpdate { get; set; }
        public Action LateUpdate { get; set; }
        public bool IsUpdate { get; set; } = true;
        public bool IsFixedUpdate { get; set; } = true;
        public bool IsLateUpdate { get; set; } = true;
        public bool Asynced { get; set; }

        public virtual void Reclaim()
        {
            Asynced = false;
            Update = default;
            FixedUpdate = default;
            LateUpdate = default;
        }

        public void AddUpdate()
        {
        }

        public void OnFixedUpdate(int dTime)
        {
            Asynced = false;
            FixedUpdate?.Invoke(dTime);
        }

        public void OnLateUpdate()
        {
            Asynced = true;
            LateUpdate?.Invoke();
        }

        public virtual void OnUpdate(int dTime)
        {
            Update?.Invoke(dTime);
        }

        public void RemoveUpdate()
        {
        }

        public void Enabled()
        {
            IsUpdate = true;
            IsLateUpdate = true;
            IsFixedUpdate = true;
        }

        public void DisEnabled()
        {
            IsUpdate = false;
            IsLateUpdate = false;
            IsFixedUpdate = false;
        }
    }

}
