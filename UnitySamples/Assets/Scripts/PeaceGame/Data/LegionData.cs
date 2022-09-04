using ShipDock.Datas;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using User;

namespace Peace
{
    public interface IServiceCaller
    {
        void InitIDAdvanced(int IDAdvanced);
    }

    public interface ILegionData : IServiceCaller, IDataProxy
    {
        void InitPlayerLegion(bool isNewPlayer);
        void Save(PlayerData playerData);
    }

    public class LegionData : DataProxy, ILegionData
    {
        private bool mIsNewPlayer;
        private Legion mPlayerLegion;
        private KeyValueList<int, Legion> mNeutralLegions;

        public LegionData() : base(Consts.D_LEGION)
        {
            mNeutralLegions = new KeyValueList<int, Legion>();

            LegionData legionData = Consts.D_LEGION.GetData<LegionData>();

        }

        public int IDAdvanced => throw new NotImplementedException();

        public void InitIDAdvanced()
        {
            throw new NotImplementedException();
        }

        public void InitPlayerLegion(bool isNewPlayer)
        {
            if (mPlayerLegion == default)
            {
                mPlayerLegion = new Legion();
            }
            else { }

            mIsNewPlayer = isNewPlayer;

            mPlayerLegion.Init();
            mPlayerLegion.SetRelationType(LegionRelationType.LEGION_SELF);

            Protocal protocal = new Protocal();
            protocal.Encode(new InitPlayer() {
                IsNewPlayer = mIsNewPlayer,
            });

            ProtocalNotice notice = Pooling<ProtocalNotice>.From();
            notice.ParamValue = protocal;
            MessageModular.AddMessage(Consts.MSG_S_INIT_PLAYER, notice);
        }

        public void Save(PlayerData playerData)
        {
            //playerData.LocalClient.ClientInfo.;
        }

        public void InitIDAdvanced(int IDAdvanced)
        {
            if (mIsNewPlayer)
            {
                mPlayerLegion.SyncServiceID(IDAdvanced);
            }
            else
            {
                //mPlayerLegion.SyncServiceID(value);
            }
        }
    }
}
