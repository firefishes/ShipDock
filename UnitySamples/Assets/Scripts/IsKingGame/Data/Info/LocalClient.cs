
using ShipDock.Datas;

namespace IsKing
{
    public class LocalClient : ClientData<DeviceLocalInfo, IsKingClientInfo>
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