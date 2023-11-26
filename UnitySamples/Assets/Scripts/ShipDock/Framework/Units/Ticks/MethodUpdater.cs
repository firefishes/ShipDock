using System;

namespace ShipDock
{
    public class MethodUpdater : IUpdate, IReclaim
    {
        public float TimeScale { get; set; } = 1f;
        public Action<float> Update { get; set; }
        public Action<float> FixedUpdate { get; set; }
        public Action LateUpdate { get; set; }
        public bool IsUpdate { get; set; } = true;
        public bool IsFixedUpdate { get; set; } = true;
        public bool IsLateUpdate { get; set; } = true;
        public bool Asynced { get; set; }

        public int Index { get; set; }

        public bool WillDelete { get; set; } = false;

        public void SetIndex(int value)
        {
            Index = value;
        }

        public virtual void Reclaim()
        {
            Asynced = false;
            Update = default;
            FixedUpdate = default;
            LateUpdate = default;
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

        public void OnFixedUpdate(float dTime)
        {
            dTime *= TimeScale;
            Asynced = false;
            FixedUpdate?.Invoke(dTime);
        }

        public virtual void OnUpdate(float dTime)
        {
            dTime *= TimeScale;
            Update?.Invoke(dTime);
        }

        public void OnLateUpdate()
        {
            Asynced = true;
            LateUpdate?.Invoke();
        }

        public void AfterAddUpdate() { }

        public void AfterRemoveUpdate() { }
    }

}
