namespace ShipDock
{
    public interface IUpdate
	{
		void AfterAddUpdate();
		void AfterRemoveUpdate();
        void OnLateUpdate();
        void OnUpdate(float dTime);
		void OnFixedUpdate(float dTime);
        bool IsUpdate { get; }
        bool IsFixedUpdate { get; }
        bool IsLateUpdate { get; }
    }
}