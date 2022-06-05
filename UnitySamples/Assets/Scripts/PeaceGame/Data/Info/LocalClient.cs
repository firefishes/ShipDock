
using ShipDock.Datas;

namespace Peace
{
    public class LocalClient : ClientData<DeviceLocalInfo, PeaceClientInfo>
    {
        public void Sync()
        {
            Init();

            if (string.IsNullOrEmpty(ClientInfo.accountID))
            {
                ClientInfo.accountID = ClientInfo.accountStart.ToString();
                ClientInfo.accountStart++;
            }
            else { }
        }
    }
}