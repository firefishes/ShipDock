#define _IS_KING_MONSTERS

using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.ECS;
using ShipDock.Interfaces;
using ShipDock.Loader;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
#if UNITY_ECS
using Unity.Scenes;
#endif
using UnityEngine;

namespace IsKing
{
    public class BattleModular : BaseModular, IDataExtracter
    {
        private QueueExecuter mBattleQueue;

        public override void Purge()
        {
        }

        public BattleModular() : base(Consts.M_BATTLE)
        {
            this.DataProxyLink(Consts.D_BATTLE);

            mBattleQueue = new QueueExecuter(false);
        }

        protected override void InitCustomHandlers()
        {
            base.InitCustomHandlers();

            AddNoticeCreater(OnCreateStartBattleNotice);
            AddNoticeHandler(OnStartBattle);

            AddPipelineNotifies(OnPlayerIntelligentalFinished);
        }

        public void OnDataProxyNotify(IDataProxy data, int DCName)
        {
            if (data is BattleData battleData)
            {
                switch (DCName)
                {
                    case Consts.DN_PLAYER_INTELLIGENTAL_FINISHED:
#if G_LOG
                        "log".Log("Card generating..");
#endif
                        NotifyModularPipeline(OnPlayerIntelligentalFinished);
                        break;
                }
            }
        }

        [ModularNotify(Consts.N_AI_CHOOSE_PLAYER_CARD_HERO, Consts.N_PLAYER_CARD_GENERATE, NotifyTiming = ModularNotifyTiming.BEFORE)]
        private void OnPlayerIntelligentalFinished(INoticeBase<int> param)
        {

        }

        [ModularNoticeCreate(Consts.N_START_BATTLE)]
        private INoticeBase<int> OnCreateStartBattleNotice(int arg)
        {
            Notice notice = new Notice();
            notice.SetNoticeName(arg);
            return notice;
        }

        [ModularNoticeListener(Consts.N_START_BATTLE)]
        private void OnStartBattle(INoticeBase<int> param)
        {
#if IS_KING_MONSTERS
#if UNITY_ECS
            if (param is IParamNotice<SubScene> notice)
            {
                notice.ParamValue.enabled = true;
            }
            else { }
#endif
#else
            ECSContext ecs = ShipDockApp.Instance.ECSContext;
            ILogicEntities allEntitas = ecs.CurrentContext.AllEntitas;
            allEntitas.AddEntitas(out int entitasMain);

            Consts.entitasMain = entitasMain;
            allEntitas.AddComponent(Consts.entitasMain, Consts.COMP_WORLD_RES);

            AssetBundles abs = ShipDockApp.Instance.ABs;
            GameObject map = abs.GetAndQuote<GameObject>("is_king_map/mission_1", "Map_1", out AssetQuoteder quoteder);
            map.transform.position = Vector3.zero;
            map.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            //Consts.D_BATTLE.GetData<BattleData>().InitBattleData();
            //Consts.UIM_BATTLE.LoadAndOpenUI<UIBattleModular>(default, Consts.AB_UI_BATTLE);

            GameObject hero = abs.GetAndQuote<GameObject>("is_king_main/heros", "Hero", out AssetQuoteder heroQuoteder);
            hero.transform.parent = map.transform;
            //hero.transform.localScale = Vector3.one * 0.35f;
            hero.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            hero.transform.localPosition = new Vector3(0f, 0f, 3f);

            hero.gameObject.GetInstanceID().BroadcastWithParam(EntityComponent.ENTITY_SETUP, true);

            GameObject monsterSpwan = abs.Get<GameObject>("is_king_monsters/spawners", "MonsterSpawner");
            for (int i = 0; i < 1; i++)
            {
                GameObject a = UnityEngine.Object.Instantiate(monsterSpwan);
                a.transform.SetParent(map.transform);
            }
#endif
        }

        [ModularNoticeListener(Consts.N_ADD_BATTLE_EXECUTER_UNIT)]
        private void OnAddBattleExecuterUnit(INoticeBase<int> param)
        {
            IParamNotice<IQueueExecuter> notice = param as IParamNotice<IQueueExecuter>;
            IQueueExecuter unit = notice.ParamValue;
            mBattleQueue.Add(unit);
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
        }
    }

}