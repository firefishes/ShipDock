#define _LOG_DISABLED
#define _IS_KING_CONFIGABLES
#define IS_KING_ECS
#define _IS_KING_MONSTERS

using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Scriptables;
using StaticConfig;
using System.Collections.Generic;
#if UNITY_ECS
using Unity.Scenes;
#endif
using UnityEngine;

namespace IsKing
{
    public class IsKingStartUp : ShipDockAppComponent
    {
        private ConfigHelper mConfigHelper;
        private List<string> mLoadConfigNames;

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
                new MessageModular(Consts.M_MESSAGE),
                new GameDataModular(),
                new BattleModular(),
                new BattleCardModular(),
                new BattleHeroModular(),
                new BattleAIModular(),
#if IS_KING_ECS
                new IsKingWorldModular(),
#endif
            };

            ShipDockApp shipDockApp = ShipDockApp.Instance;
#if IS_KING_CONFIGABLES
            GameObject configableItems = shipDockApp.ABs.GetAndQuote<GameObject>("is_king_main/configables", "ConfigableItems", out _);
            ConfigableItemsComponent component = configableItems.GetComponent<ConfigableItemsComponent>();
            InitConfigables(ref component);
#endif

            Consts.D_PLAYER.GetData<PlayerData>().InitPlayer();

            DecorativeModulars appModular = shipDockApp.AppModulars;
            appModular.AddModular(modulars);

#if IS_KING_CONFIGABLES
            appModular.NotifyModular(Consts.N_START_BATTLE);
#else
            LoadConfigs();
#endif
        }

#if !IS_KING_CONFIGABLES
        private void LoadConfigs()
        {
            //新建配置辅助器
            mConfigHelper = new ConfigHelper
            {
                //设置配置文件所在的资源包名
                ConfigResABName = Consts.AB_CONFIGS,
            };

            //向配置辅助器添加需要绑定的配置类，此类为工具自动生成
            mConfigHelper.AddHolderType<IsKingGenerals>(Consts.CONF_GENERALS);
            //mConfigHelper.AddHolderType<PeaceOrganizations>(Consts.CONF_ORGANIZATIONS);
            //设置配置名
            mLoadConfigNames = mConfigHelper.HolderTypes;

            //根据配置名加载配置
            string[] list = mLoadConfigNames.ToArray();
            mConfigHelper.Load(OnConfigLoaded, list);
        }

        //配置加载完成
        private void OnConfigLoaded(ConfigsResult configResult)
        {
            Consts.D_CONFIGS.SetConfigDataDefaultName();
            Consts.CONF_GROUP_CONFIGS.SetConfigGroupDefaultName();

            //向配置数据代理中添加已加载的配置数据，并使用一个组名做对应
            ConfigData data = Consts.D_CONFIGS.GetData<ConfigData>();
            data.AddConfigs(Consts.CONF_GROUP_CONFIGS, configResult);

            Test();

            ShipDockApp shipDockApp = ShipDockApp.Instance;
            DecorativeModulars appModular = shipDockApp.AppModulars;

#if IS_KING_MONSTERS
#if UNITY_ECS
            ParamNotice<SubScene> notice = new()
            {
                ParamValue = GameComponent.UnityECSEntranceScene,
            };
            appModular.NotifyModular(Consts.N_START_BATTLE, notice);
            notice.Reclaim();
#endif
#else
            appModular.NotifyModular(Consts.N_START_BATTLE);
#endif

        }

        private void Test()
        {
            //通过封装过的扩展方法从配置数据代理中获取配置
            Dictionary<int, IsKingGenerals> testConfTable = Consts.CONF_GENERALS.GetConfigTable<IsKingGenerals>();

            ////从配置中读取数据
            foreach (var item in testConfTable)
            {
                Debug.Log(item.Value.name);
                Debug.Log(item.Value.propertyValues);
            }
        }
#endif

#if IS_KING_CONFIGABLES
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
#endif
    }
}