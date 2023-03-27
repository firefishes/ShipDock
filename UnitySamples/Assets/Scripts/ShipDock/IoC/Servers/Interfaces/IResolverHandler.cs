namespace ShipDock
{
    public interface IResolverHandler : IReclaim
    {
        void SetID(int id);
        void InvokeResolver();
        void SetParam<T>(ref T param);
        object ResolverParam { get; }
        bool OnlyOnce { get; set; }
        int ID { get; }
    }
}