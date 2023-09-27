namespace ShipDock
{
    public interface IDataUnit
    {
        int InstanceID { get; }
        void SetInstanceID(int value);
    }
}