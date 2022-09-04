using System.Collections;
using System.Collections.Generic;
using ShipDock.Datas;
using ShipDock.Notices;
using UnityEngine;

namespace Peace
{
    public class DataModular : BaseModular, IDataExtracter
    {
        private List<IServiceData> mServiceDatas;
        private PlayerData mPlayerData;

        public DataModular() : base(Consts.M_DATA)
        {
            mServiceDatas = new List<IServiceData>();
        }

        public override void InitModular()
        {
            base.InitModular();

            IDataProxy data = Consts.D_PLAYER.GetData<IDataProxy>();

            data.Register(this);
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
            switch (message)
            {
                case Consts.MSG_S_INIT_PLAYER:
                    //IParamNotice<IServiceCaller> serviceDataNotice = notice as IParamNotice<IServiceCaller>;
                    //IServiceCaller caller = serviceDataNotice.ParamValue;

                    //IServiceData serviceData = Consts.D_SERVICE.GetData<IServiceData>();
                    //caller.InitIDAdvanced(serviceData.IDAdvanced);
                    break;
            }
        }

        public void OnDataProxyNotify(IDataProxy data, int DCName)
        {
            mPlayerData = data as PlayerData;
            if (mPlayerData.NotNull())
            {
                ILegionData legionData = Consts.D_LEGION.GetData<ILegionData>();
                IServiceData serviceData = Consts.D_SERVICE.GetData<IServiceData>();

                switch (DCName)
                {
                    case Consts.DN_NEW_GAME_CREATED:
                        legionData.InitPlayerLegion(true);
                        break;

                    case Consts.DN_GAME_LOADED:
                        PeaceClientInfo info = mPlayerData.LocalClient.ClientInfo;
                        int IDAdvacned = info.IDAdvanced;
                        serviceData.SyncIDAdvanced(IDAdvacned);
                        legionData.InitPlayerLegion(false);
                        break;
                }
            }
            else { }
        }
    }
}
