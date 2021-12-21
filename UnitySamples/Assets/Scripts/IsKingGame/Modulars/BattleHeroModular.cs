using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class BattleHeroModular : ApplicationModular
    {
        public override void Purge()
        {
        }

        public BattleHeroModular()
        {
            ModularName = Consts.M_BATTLE_HERO;

            NoticeCreates = new ModularNoticeCreater[]
            {
                new ModularNoticeCreater(Consts.N_AI_CHOOSE_PLAYER_CARD_HERO, OnAIChoosePlayerCardHeroCreate)
            };

            NoticeListeners = new ModularNoticeListener[]
            {
                new ModularNoticeListener(Consts.N_AI_CHOOSE_PLAYER_CARD_HERO, OnAIChoosePlayerCardHero, 1),
            };
        }

        private INoticeBase<int> OnAIChoosePlayerCardHeroCreate(int noticeName)
        {
            HeroNotice heroNotice = Pooling<HeroNotice>.From();
            heroNotice.camp = Consts.CAMP_PLAYER;
            return heroNotice;
        }

        private void OnAIChoosePlayerCardHero(INoticeBase<int> param)
        {
            Debug.Log("OnAIChoosePlayerCardHero 1");
            HeroNotice notice = param as HeroNotice;

            AIRatioNotice AIRatioNotice = Pooling<AIRatioNotice>.From();
            AIRatioNotice.camp = notice.camp;
            NotifyModular(Consts.N_SET_GENERAL_INTO_BATTLE_RATIO, AIRatioNotice);
            NotifyModular(Consts.N_COMMIT_PLAYER_AI, notice);

            AIRatioNotice.ToPool();
        }
    }

    public class HeroNotice : Notice
    {
        public int id;
        public int camp;
        public BattleHeroController heroController;

        public override void ToPool()
        {
            base.ToPool();

            heroController = default;
        }
    }

}