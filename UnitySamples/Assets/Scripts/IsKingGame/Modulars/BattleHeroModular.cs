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
        }

        protected override void InitCustomHandlers()
        {
            AddNoticeCreater(OnAIChoosePlayerCardHeroCreate);
            AddNoticeHandler(OnAIChoosePlayerCardHero);
        }

        [ModularNoticeCreate(Consts.N_AI_CHOOSE_PLAYER_CARD_HERO)]
        private INoticeBase<int> OnAIChoosePlayerCardHeroCreate(int noticeName)
        {
            HeroNotice heroNotice = Pooling<HeroNotice>.From();
            heroNotice.camp = Consts.CAMP_PLAYER;
            return heroNotice;
        }

        [ModularNoticeListener(Consts.N_AI_CHOOSE_PLAYER_CARD_HERO, 1)]
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

}