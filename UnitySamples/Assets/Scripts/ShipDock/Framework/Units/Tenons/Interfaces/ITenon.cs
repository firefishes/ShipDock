namespace ShipDock
{
    public interface ITenon
    {
        int[] SystemIDs { get; }
        void Clear();
        void SetupTenon(Tenons tenons);
        void DataValid();
        bool IsDataChanged();
        void ResetDataChangedMark();
        void PerFrameInit(float deltaTime);
        void PerFrameReady(float deltaTime);
        void PerFrameFixed(float deltaTime);
        void PerFrame(float deltaTime);
        void PerFrameLate();
        void PerFrameEnd();
        void Drop();
        void CancelDrop();
        bool IsDroped();
        bool IsBlock();
        bool IsEnabled();
        void SetEnabled(bool value);
        int GetTenonID();
        void SetTenonID(int value);
        int GetInstanceIndex();
        void SetInstanceIndex(int value);
        int GetTenonType();
        void SetTenonType(int value);
    }

    public interface IECSComponent<T> : ITenon where T : IECSData
    {
        T GetData();
    }
}