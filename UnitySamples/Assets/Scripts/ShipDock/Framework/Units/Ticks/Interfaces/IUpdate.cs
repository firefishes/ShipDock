namespace ShipDock
{
    public interface IUpdate
	{
        int Index { get; }
        bool WillDelete { get; set; }
        bool IsUpdate { get; }
        bool IsFixedUpdate { get; }
        bool IsLateUpdate { get; }

        void SetIndex(int value);
		void AfterAddUpdate();
		void AfterRemoveUpdate();
        void OnLateUpdate();
        void OnUpdate(float dTime);
		void OnFixedUpdate(float dTime);
    }
}