using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class BattleCardModular : ApplicationModular
    {

        public override void Purge()
        {
        }

        public BattleCardModular()
        {
            ModularName = Consts.M_BATTLE_CARD;
        }

        protected override void InitCustomHandlers()
        {
            base.InitCustomHandlers();

            AddNoticeCreater(OnPlayerCardGenerateCreat);
            AddNoticeHandler(OnPlayerCardGenerateListener);
        }

        [ModularNoticeCreate(Consts.N_PLAYER_CARD_GENERATE)]
        private INoticeBase<int> OnPlayerCardGenerateCreat(int noticeName)
        {
            CardNotice cardNotice = Pooling<CardNotice>.From();
            cardNotice.camp = Consts.CAMP_PLAYER;
            return cardNotice;
        }

        /// <summary>
        /// 生成玩家手牌：将领
        /// </summary>
        /// <param name="param"></param>
        [ModularNoticeListener(Consts.N_PLAYER_CARD_GENERATE)]
        private void OnPlayerCardGenerateListener(INoticeBase<int> param)
        {
            Debug.Log("OnPlayerCardGenerateListener 1");
            CardNotice notice = param as CardNotice;
            BattleHeroController heroController = notice.heroControllerFrom;
            //heroController.
            //TODO 根据将领能力生成手牌
            BattleData data = Consts.D_BATTLE.GetData<BattleData>();
            data.AddPlayerCard(ref notice);
            //这里生成其他牌

            notice.ToPool();
        }
    }
}