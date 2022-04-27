using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class BattleData : DataProxy
    {
        private KeyValueList<int, BattleCamp> mCamps;

        public Queue<BattleHeroController> PlayerHeroCardGenerated
        {
            get; private set;
        }

        public BattleData() : base(Consts.D_BATTLE)
        {
            mCamps = new KeyValueList<int, BattleCamp>();
            PlayerHeroCardGenerated = new Queue<BattleHeroController>();
        }

        public BattleCamp GetCamp(int campType)
        {
            return mCamps[campType];
        }

        private void InitPlayerTeam(ref BattleCamp camp)
        {
            PlayerData playerData = Consts.D_PLAYER.GetData<PlayerData>();
            List<HeroFields> playerHeros = playerData.Heros.GetTeamHeros();

            int n = playerHeros.Count;
            int max = Consts.CAMP_HERO_MAX;
            for (int i = 0; i < max; i++)
            {
                HeroFields copy = new HeroFields();
                if (i < n)
                {
                    HeroFields hero = playerHeros[i];
                    hero.CopyToFields(ref copy);

                    BattleHeroController heroContorller = new BattleHeroController(copy);
                    camp.SetCampHero(i, heroContorller);
                }
                else { }
            }

            BattleFields battleFields = camp.BattleInfoController.Info;
            int troops = battleFields.GetIntData(Consts.FN_TROOPS);
            int troopsMax = battleFields.GetIntData(Consts.FN_TROOPS_MAX);
            "log:Player troops is {0}/{1}".Log(troops.ToString(), troopsMax.ToString());
            "log:Player intelligential is {0}".Log(battleFields.GetFloatData(Consts.FN_INTELLIGENTIAL).ToString());
            "log:Player intelligential delta is {0}".Log(battleFields.GetFloatData(Consts.FN_INTELLIGENTIAL_DELTA).ToString());
        }

        private void InitPlayerCamp()
        {
            //初始化玩家阵营
            BattleCamp camp = mCamps[Consts.CAMP_PLAYER];
            BattleInfoController controller = camp.BattleInfoController;
            BattleFields info = controller.Info;

            InitPlayerTeam(ref camp);

            controller.AddListener(Consts.N_INTELLIGENTAL_UPDATE, OnPlayerBattleInfoEventHandler);
            controller.AddListener(Consts.N_INTELLIGENTAL_FINISHED, OnPlayerBattleInfoEventHandler);
            controller.Dispatch(Consts.N_START_COLLECT_INTELLIGENTAL);
        }

        private void InitEnemyCamp()
        {
            //初始化敌军阵营
            BattleCamp camp = mCamps[Consts.CAMP_ENEMY];
            BattleInfoController controller = camp.BattleInfoController;
            BattleFields info = controller.Info;

            BattleHeroController heroContorller;
            int max = Consts.CAMP_HERO_MAX;
            for (int i = 0; i < max; i++)
            {
                heroContorller = new BattleHeroController(new HeroFields());
                //heroContorller.Info.SetIntData(Consts.FN_ID, id);
                camp.SetCampHero(i, heroContorller);
            }

            controller.AddListener(Consts.N_INTELLIGENTAL_UPDATE, OnEnemyBattleInfoEventHandler);
            controller.AddListener(Consts.N_INTELLIGENTAL_FINISHED, OnEnemyBattleInfoEventHandler);
            controller.Dispatch(Consts.N_START_COLLECT_INTELLIGENTAL);
        }

        public void InitBattleData()
        {
            mCamps[Consts.CAMP_PLAYER] = new BattleCamp();
            mCamps[Consts.CAMP_ENEMY] = new BattleCamp();

            InitPlayerCamp();
            InitEnemyCamp();
        }

        private void OnPlayerBattleInfoEventHandler(INoticeBase<int> param)
        {
            switch (param.Name)
            {
                case Consts.N_INTELLIGENTAL_UPDATE:
                    DataNotify(Consts.DN_PLAYER_INTELLIGENTAL_UPDATE);
                    break;
                case Consts.N_INTELLIGENTAL_FINISHED:
                    DataNotify(Consts.DN_PLAYER_INTELLIGENTAL_FINISHED);
                    break;
            }
        }

        private void OnEnemyBattleInfoEventHandler(INoticeBase<int> param)
        {
            switch (param.Name)
            {
                case Consts.N_INTELLIGENTAL_FINISHED:
                    DataNotify(Consts.DN_ENEMY_INTELLIGENTAL_FINISHED);
                    break;
            }
        }

        /// <summary>
        /// 当前玩家情报
        /// </summary>
        /// <returns></returns>
        public Vector2 CurrentPlayerIntelligental()
        {
            BattleCamp camp = GetCamp(Consts.CAMP_PLAYER);
            BattleFields info = camp.BattleInfoController.Info;
            float cur = info.GetFloatData(Consts.FN_INTELLIGENTIAL);
            float max = info.GetFloatData(Consts.FN_INTELLIGENTIAL_MAX);
            return new Vector2(cur, max);
        }

        public Vector2 CurrentMorale(int campType)
        {
            BattleCamp camp = GetCamp(campType);
            BattleFields fields = camp.BattleInfoController.Info;
            float cur = fields.GetFloatData(Consts.FN_MORALE);
            float max = fields.GetFloatData(Consts.FN_MORALE_MAX);
            return new Vector2(cur, max);
        }

        public Vector2Int CurrentTroops(int campType)
        {
            BattleCamp camp = GetCamp(campType);
            BattleFields fields = camp.BattleInfoController.Info;
            int cur = fields.GetIntData(Consts.FN_TROOPS);
            int max = fields.GetIntData(Consts.FN_TROOPS_MAX);
            return new Vector2Int(cur, max);
        }

        public List<BattleHeroController> GetIntoBattleHeros(int campType)
        {
            BattleCamp camp = GetCamp(campType);
            List<BattleHeroController> result = new List<BattleHeroController>();
            camp.FillIntoBattleHeros(ref result);
            return result;
        }

        public List<BattleHeroController> GetIdleBattleHeros(int campType)
        {
            BattleCamp camp = GetCamp(campType);
            List<BattleHeroController> result = new List<BattleHeroController>();
            camp.FillIdleBattleHeros(ref result);
            return result;
        }

        public void AddPlayerHeroCard(params BattleHeroController[] heroControllers)
        {
            BattleHeroController item;
            int max = heroControllers.Length;
            for (int i = 0; i < max; i++)
            {
                item = heroControllers[i];
                PlayerHeroCardGenerated.Enqueue(item);
            }
            DataNotify(Consts.DN_PLAYER_HERO_CARD_ADDED);
        }
    }
}
