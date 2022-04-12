#define _LOG_DISABLED

using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Scriptables;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class IsKingStartUp : ShipDockAppComponent
    {
        public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
        {
            base.GetDataProxyHandler(param);

            param.ParamValue = new IDataProxy[]
            {
                new PlayerData(),
                new ConfigsData(),
                new BattleData(),
            };
        }

        public override void ApplicationCloseHandler()
        {
            base.ApplicationCloseHandler();
        }

        public override void EnterGameHandler()
        {
            base.EnterGameHandler();

#if LOG_DISABLED
            Debug.unityLogger.logEnabled = false;
#endif

            IModular[] modulars = new ApplicationModular[]
            {
                new GameDataModular(),
                new BattleModular(),
                new BattleCardModular(),
                new BattleHeroModular(),
                new BattleAIModular(),
            };

            ShipDockApp shipDockApp = ShipDockApp.Instance;
            GameObject configableItems = shipDockApp.ABs.GetAndQuote<GameObject>("is_king_main/configables", "ConfigableItems", out _);
            ConfigableItemsComponent component = configableItems.GetComponent<ConfigableItemsComponent>();

            InitConfigables(ref component);

            Consts.D_PLAYER.GetData<PlayerData>().InitPlayer();

            shipDockApp.AppModulars.AddModular(modulars);
            shipDockApp.AppModulars.NotifyModular(Consts.N_START_BATTLE);
        }

        private void InitConfigables(ref ConfigableItemsComponent component)
        {
            ConfigsData configsData = Consts.D_CONFIGS.GetData<ConfigsData>();
            configsData.Init();

            int itemType;
            ConfigableItems item;
            IScriptableItems infos;
            List<ConfigableItems> configs = component.GetConfigableItems();
            int max = configs.Count;
            for (int i = 0; i < max; i++)
            {
                item = configs[i];
                itemType = item.ItemType();
                infos = item.Collections() as IScriptableItems;
                configsData.LoadItems(itemType, infos);
            }
        }
    }
}