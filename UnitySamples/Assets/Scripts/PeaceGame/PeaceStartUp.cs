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
            //�½����ø�����
            mConfigHelper = new ConfigHelper
            {
                //���������ļ����ڵ���Դ����
                ConfigResABName = Consts.AB_CONFIGS,
            };

            //�����ø����������Ҫ�󶨵������࣬����Ϊ�����Զ�����
            mConfigHelper.AddHolderType<PeaceEquipment>(Consts.CONF_EQUIPMENT);
            //����������
            mLoadConfigNames = mConfigHelper.HolderTypes;

            //������������������
            string[] list = mLoadConfigNames.ToArray();
            mConfigHelper.Load(OnConfigLoaded, list);
        }

        //���ü������
        private void OnConfigLoaded(ConfigsResult configResult)
        {
            //���������ݴ���������Ѽ��ص��������ݣ���ʹ��һ����������Ӧ
            ConfigData data = Consts.D_CONFIGS.GetData<ConfigData>();
            data.AddConfigs(Consts.CONF_GROUP_CONFIGS, configResult);

            //ͨ����װ������չ�������������ݴ����л�ȡ����
            //Dictionary<int, PeaceEquipment> equipmentConfTable = Consts.CONF_EQUIPMENT.GetConfigTable<PeaceEquipment>();

            ////�������ж�ȡ����
            //int id = 20000;
            //PeaceEquipment config = equipmentConfTable[id];
            //Debug.Log(config.name);
            //Debug.Log(config.propertyValues);

            IParamNotice<string> notice = new ParamNotice<string>()
            {
                ParamValue = "������",
            };

            DecorativeModulars modluars = ShipDockApp.Instance.AppModulars;
            modluars.NotifyModular(Consts.N_MSG_ADD, MessageNotice.Create(Consts.MSG_GAME_READY, notice));

        }
    }
}