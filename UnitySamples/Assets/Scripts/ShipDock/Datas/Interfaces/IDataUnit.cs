namespace ShipDock.Datas
{
    public interface IDataUnit
    {
        int InstanceID { get; }
        void SetInstanceID(int value);
    }
}