using ShipDock.Datas;
using ShipDock.Notices;
using System;
using UnityEngine;

namespace Peace
{
    public class PlayerData : DataProxy
    {
        public NoticesObserver Events { get; private set; }
        public LocalClient LocalClient { get; private set; }

        public PlayerData() : base(Consts.D_PLAYER)
        {
            Events = new NoticesObserver();
            
            ShipDockConsts.NOTICE_APPLICATION_CLOSE.Add(OnApplicationClose);
        }

        public void InitPlayer()
        {
            LocalClient = new LocalClient();
            LocalClient.Sync();
        }

        public void LoadNewPlayer()
        {
            LocalClient.CreateNewClient();
            LocalClient.ClientInfo.accountID = DateTime.UtcNow.ToLongTimeString();
            LocalClient.ClientInfo.IDAdvanced = 0;
            Save();

            DataNotify(Consts.DN_NEW_GAME_CREATED);
        }

        public void ContinueGame()
        {
            DataNotify(Consts.DN_GAME_LOADED);
        }

        private void OnApplicationClose(INoticeBase<int> obj)
        {
            PeaceClientInfo clientInfo = LocalClient.ClientInfo;

            ILegionData legionData = Consts.D_LEGION.GetData<ILegionData>();

            legionData.Save(this);

            Save();
        }

        public void Save()
        {
            LocalClient?.FlushInfos();
        }
    }

}