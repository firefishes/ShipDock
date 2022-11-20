using System;
using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Interfaces;
using ShipDock.Loader;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
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
            Consts.D_BATTLE.GetData<BattleData>().InitBattleData();

            AssetBundles abs = ShipDockApp.Instance.ABs;
            GameObject map = abs.GetAndQuote<GameObject>("is_king_map/mission_1", "Map", out AssetQuoteder quoteder);

            Consts.UIM_BATTLE.LoadAndOpenUI<UIBattleModular>(default, Consts.AB_UI_BATTLE);
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