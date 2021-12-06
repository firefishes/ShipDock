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

            NoticeListeners = new ModularNoticeListener[]
            {
                new ModularNoticeListener(Consts.N_PLAYER_CARD_GENERATE, OnPlayerCardGenerateListener),
            };
        }

        private void OnPlayerCardGenerateListener(INoticeBase<int> param)
        {
            CardNotice notice = param as CardNotice;
            BattleHeroController heroController = notice.heroControllerFrom;
            //heroController.
            //TODO 根据将领能力生成手牌
            
            notice.ToPool();
        }

        private INoticeBase<int> OnChooseCardHero(int param)
        {
            return Pooling<CardNotice>.From();
        }

        //private void OnPlayerCardGenerateDecorater(int arg1, INoticeBase<int> arg2)
        //{

        //}
    }

    public class CardNotice : Notice
    {
        public int camp;
        public BattleHeroController heroControllerFrom;

        public override void ToPool()
        {
            Pooling<CardNotice>.To(this);
        }
    }
}