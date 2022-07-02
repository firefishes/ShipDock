using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using StaticConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public class PeaceStartUp : ShipDockAppComponent
    {
        private ConfigHelper mConfigHelper;
        private List<string> mLoadConfigNames;

        public override void ApplicationCloseHandler()
        {
            base.ApplicationCloseHandler();
        }

        public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
        {
            base.GetDataProxyHandler(param);

            param.ParamValue = new IDataProxy[]
            {
                new ConfigData(Consts.D_CONFIGS),
                new PlayerData(),
            };
        }

        public override void EnterGameHandler()
        {
            base.EnterGameHandler();

            PlayerData playerData = Consts.D_PLAYER.GetData<PlayerData>();
            playerData.InitPlayer();

            IModular[] modulars = new IModular[]
            {
                new MessageModular(),
                new BattleModular(),
            };
            DecorativeModulars modluars = ShipDockApp.Instance.AppModulars;
            modluars.AddModular(modulars);

            LoadConfig();
        }

        private void LoadConfig()
        {
            //新建配置辅助器
            mConfigHelper = new ConfigHelper
            {
                //设置配置文件所在的资源包名
                ConfigResABName = Consts.AB_CONFIGS,
            };

            //向配置辅助器添加需要绑定的配置类，此类为工具自动生成
            mConfigHelper.AddHolderType<PeaceEquipment>(Consts.CONF_EQUIPMENT);
            //设置配置名
            mLoadConfigNames = mConfigHelper.HolderTypes;

            //根据配置名加载配置
            string[] list = mLoadConfigNames.ToArray();
            mConfigHelper.Load(OnConfigLoaded, list);
        }

        //配置加载完成
        private void OnConfigLoaded(ConfigsResult configResult)
        {
            //向配置数据代理中添加已加载的配置数据，并使用一个组名做对应
            ConfigData data = Consts.D_CONFIGS.GetData<ConfigData>();
            data.AddConfigs(Consts.CONF_GROUP_CONFIGS, configResult);

            //通过封装过的扩展方法从配置数据代理中获取配置
            //Dictionary<int, PeaceEquipment> equipmentConfTable = Consts.CONF_EQUIPMENT.GetConfigTable<PeaceEquipment>();

            ////从配置中读取数据
            //int id = 20000;
            //PeaceEquipment config = equipmentConfTable[id];
            //Debug.Log(config.name);
            //Debug.Log(config.propertyValues);

            IParamNotice<string> notice = new ParamNotice<string>()
            {
                ParamValue = "！！！",
            };

            DecorativeModulars modluars = ShipDockApp.Instance.AppModulars;
            modluars.NotifyModular(Consts.N_MSG_ADD, MessageNotice.Create(Consts.MSG_GAME_READY, notice));

        }
    }
}