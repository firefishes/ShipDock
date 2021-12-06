using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
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

            NoticeListeners = new ModularNoticeListener[]
            {
                new ModularNoticeListener(Consts.N_AI_CHOOSE_PLAYER_CARD_HERO, OnAIChoosePlayerCardHero, 1),
            };
        }

        private void OnAIChoosePlayerCardHero(INoticeBase<int> param)
        {
            Debug.Log("OnAIChoosePlayerCardHero 1");
            HeroNotice notice = param as HeroNotice;
            notice.camp = Consts.CAMP_PLAYER;

            BattleData battleData = Consts.D_BATTLE.GetData<BattleData>();
            List<BattleHeroController> list = battleData.GetIdleBattleHeros(notice.camp);

            AIRatioNotice AIRatioNotice = Pooling<AIRatioNotice>.From();
            AIRatioNotice.camp = notice.camp;
            AIRatioNotice.Heros = list;
            NotifyModular(Consts.N_SET_GENERAL_INTO_BATTLE_RATIO, AIRatioNotice);
            //AIRatioNotice.ToPool();

            UpdaterNotice.SceneCallLater((t) =>
            {
                NotifyModular(Consts.N_COMMIT_PLAYER_AI);
                notice.ToPool();
            });
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