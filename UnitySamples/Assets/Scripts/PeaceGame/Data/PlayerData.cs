using ShipDock.Datas;
using ShipDock.Notices;

namespace Peace
{
    public class PlayerData : DataProxy
    {
        private LocalClient mLocalClient;

        public NoticesObserver Events { get; private set; }

        public PlayerData() : base(Consts.D_PLAYER)
        {
            Events = new NoticesObserver();
            //Events.AddListener(N_GET_NEW_HEROS, OnGetNewHeros);

            ShipDockConsts.NOTICE_APPLICATION_CLOSE.Add(OnApplicationClose);

        }

        public void InitPlayer()
        {
            mLocalClient = new LocalClient();
            mLocalClient.Sync();

            PeaceClientInfo clientInfo = mLocalClient.ClientInfo;
            //Heros.Init(ref clientInfo);
        }

        private void OnGetNewHeros(INoticeBase<int> param)
        {
            ParamNotice<int[]> notice = param as ParamNotice<int[]>;
            //notice.ParamValue = Heros.GetNewHeroIDs();
        }

        private void OnApplicationClose(INoticeBase<int> obj)
        {
            PeaceClientInfo clientInfo = mLocalClient.ClientInfo;

            //Heros.SyncHerosToClient(ref clientInfo);

            Save();
        }

        public void Save()
        {
            mLocalClient?.FlushInfos();
        }
    }

}