using System;
using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Loader;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;

namespace IsKing
{
    public class BattleModular : ApplicationModular, IDataExtracter
    {

        public override void Purge()
        {
        }

        public BattleModular()
        {
            ModularName = Consts.M_BATTLE;

            NoticeCreates = new ModularNoticeCreater[]
            {
                new ModularNoticeCreater(Consts.N_START_BATTLE, OnCreateStartBattleNotice),
            };

            //NoticeDecoraters = new ModularNoticeDecorater[]
            //{
            //    new ModularNoticeDecorater(Consts.N_PLAYER_CARD_GENERATE, OnPlayerCardGenerateDecorater),
            //};

            NoticeListeners = new ModularNoticeListener[]
            {
                new ModularNoticeListener(Consts.N_START_BATTLE, OnStartBattle),
            };

            this.DataProxyLink(Consts.D_BATTLE);
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
                        NotifyModular(Consts.N_AI_CHOOSE_PLAYER_CARD_HERO);
                        NotifyModular(Consts.N_PLAYER_CARD_GENERATE);
                        break;
                }
            }
        }

        //private void OnPlayerCardGenerateDecorater(int noticeName, INoticeBase<int> param)
        //{
        //    CardNotice notice = param as CardNotice;
        //    notice.camp = Consts.CAMP_PLAYER;
        //}

        private INoticeBase<int> OnCreateStartBattleNotice(int arg)
        {
            Notice notice = new Notice();
            notice.SetNoticeName(arg);
            return notice;
        }

        private void OnStartBattle(INoticeBase<int> obj)
        {
            Consts.D_BATTLE.GetData<BattleData>().StartBattle();

            AssetBundles abs = ShipDockApp.Instance.ABs;
            GameObject map = abs.GetAndQuote<GameObject>("is_king_map/mission_1", "Map", out AssetQuoteder quoteder);

            Consts.UIM_BATTLE.LoadAndOpenUI<UIBattleModular>(default, Consts.AB_UI_BATTLE);
        }
    }

}