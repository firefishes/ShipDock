﻿using ShipDock.Datas;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class GameDataModular : ApplicationModular
    {
        public override void Purge()
        {
        }

        public GameDataModular()
        {
            ModularName = Consts.M_GAME_DATA;
        }

        protected override void InitCustomHandlers()
        {
            base.InitCustomHandlers();

            AddNoticeHandler(OnSetGeneralIntoBattleRatio);
        }

        private T GetGameData<T>(int dataName) where T : DataProxy
        {
            T data = dataName.GetData<T>();
            return data;
        }

        [ModularNoticeListener(Consts.N_SET_GENERAL_INTO_BATTLE_RATIO, 1)]
        private void OnSetGeneralIntoBattleRatio(INoticeBase<int> param)
        {
            Debug.Log("OnSetGeneralIntoBattleRatio 1");

            BattleData battleData = GetGameData<BattleData>(Consts.D_BATTLE);

            AIRatioNotice notice = param as AIRatioNotice;
            notice.Heros = battleData.GetIdleBattleHeros(notice.camp);
        }
    }
}