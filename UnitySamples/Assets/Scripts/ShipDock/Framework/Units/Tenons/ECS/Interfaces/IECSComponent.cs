namespace ShipDock
{
    public interface IECSComponent<T> : IECSComponentBase where T : struct// IECSData
    {
        T GetEntityData(int entity, out bool hasData);
        T GetNullData();
    }
}
