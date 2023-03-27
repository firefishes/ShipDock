using System;

namespace ShipDock
{
    public class HotFixData : IDataExtracter
    {
        public Action<IDataProxy, int> OnData { get; set; }

        public void OnDataProxyNotify(IDataProxy data, int DCName)
        {
            OnData?.Invoke(data, DCName);
        }
    }
}
