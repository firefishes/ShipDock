using System.Collections;
using System.Collections.Generic;
using ShipDock.Datas;
using ShipDock.Notices;
using UnityEngine;

namespace Peace
{
    public class DataModular : BaseModular, IDataExtracter
    {
        private PlayerData mPlayerData;

        public DataModular() : base(Consts.M_DATA)
        {
        }

        public override void InitModular()
        {
            base.InitModular();

            IDataProxy data = Consts.D_PLAYER.GetData<IDataProxy>();

            data.Register(this);
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
        }

        public void OnDataProxyNotify(IDataProxy data, int DCName)
        {
            mPlayerData = data as PlayerData;
            if (mPlayerData.NotNull())
            {
                switch (DCName)
                {
                    case Consts.DN_NEW_GAME_CREATED:
                        Consts.D_LEGION.GetData<IDataProxy>();
                        break;
                }
            }
            else { }
        }
    }
}
