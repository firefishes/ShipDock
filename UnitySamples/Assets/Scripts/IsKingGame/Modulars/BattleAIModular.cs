using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class BattleAIModular : ApplicationModular
    {
        private float[] mWeights;
        private HeroNotice mHeroNotice;
        private AIGeneralIntoBattleRatio mPlayerIntoBattleRatio;

        public override void Purge()
        {
        }

        public BattleAIModular()
        {
            mWeights = new float[] { 1f };
            ModularName = Consts.M_BATTLE_AI;

            NoticeDecoraters = new ModularNoticeDecorater[]
            {
                new ModularNoticeDecorater(Consts.N_PLAYER_CARD_GENERATE, OnPlayerCardGenerateDecorate)
            };

            NoticeListeners = new ModularNoticeListener[]
            {
                new ModularNoticeListener(Consts.N_SET_GENERAL_INTO_BATTLE_RATIO, OnSetGeneralIntoBattleRatio, 2),
                //new ModularNoticeListener(Consts.N_AI_CHOOSE_PLAYER_CARD_HERO, OnAIChoosePlayerCardHero, 2),
                new ModularNoticeListener(Consts.N_COMMIT_PLAYER_AI, OnAIPlayerAICommit, 1),
            };

            mPlayerIntoBattleRatio = new AIGeneralIntoBattleRatio();
        }

        private void OnPlayerCardGenerateDecorate(int noticeName, INoticeBase<int> param)
        {
            CardNotice notice = param as CardNotice;
            notice.heroControllerFrom = mHeroNotice.heroController;
            mHeroNotice.ToPool();
        }

        private void OnAIChoosePlayerCardHero(INoticeBase<int> param)
        {
            Debug.Log("OnAIChoosePlayerCardHero 2");
        }

        private void OnSetGeneralIntoBattleRatio(INoticeBase<int> param)
        {
            Debug.Log("OnSetGeneralIntoBattleRatio 2");
            AIRatioNotice notice = param as AIRatioNotice;
            List<BattleHeroController> list = notice.Heros;
            if (list.Count > 0)
            {
                float ratio = list.Count / Consts.CAMP_HERO_MAX * mWeights[0];
                mPlayerIntoBattleRatio.Ratio = ratio;
                mPlayerIntoBattleRatio.Heros = list;
            }
            else { }
        }

        private void OnAIPlayerAICommit(INoticeBase<int> param)
        {
            Debug.Log("OnAIPlayerAICommit");

            mPlayerIntoBattleRatio.Execute();

            mHeroNotice = param as HeroNotice;
            mHeroNotice.id = mPlayerIntoBattleRatio.HeroID;
            mHeroNotice.heroController = mPlayerIntoBattleRatio.Hero;
        }
    }

    public class AINotice : Notice
    {
        public int camp;
    }

    public class AIRatioNotice : AINotice
    {
        public List<BattleHeroController> Heros { get; set; }
    }

    public abstract class AIExectuter
    {
        public abstract void Execute();
    }

    public class AIGeneralIntoBattleRatio : AIExectuter
    {
        public List<BattleHeroController> Heros { get; set; }
        public float Ratio { get; set; }
        public int HeroID { get; set; }
        public BattleHeroController Hero { get; set; }

        public override void Execute()
        {
            int index = Utils.UnityRangeRandom(0, Heros.Count);

            Hero = Heros[index];
            HeroID = Hero.Info.GetIntData(Consts.FN_ID);

            "log:AI chooset player card from {0} heros, selected index is {1}, id is {2}".Log(Heros.Count.ToString(), index.ToString(), HeroID.ToString());
        }
    }
}