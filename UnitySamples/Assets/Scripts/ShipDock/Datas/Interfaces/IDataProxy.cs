namespace ShipDock
{
    public interface IDataProxy : IReclaim
    {
        int DataName { get; }
        void Register(IDataExtracter dataHandler);
        void Unregister(IDataExtracter dataHandler);
    }

    public interface IDataExtracter
    {
        void OnDataProxyNotify(IDataProxy data, int DCName);
    }
}
