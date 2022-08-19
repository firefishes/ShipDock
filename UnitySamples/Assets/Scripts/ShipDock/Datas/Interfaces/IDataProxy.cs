using ShipDock.Interfaces;

namespace ShipDock.Datas
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
