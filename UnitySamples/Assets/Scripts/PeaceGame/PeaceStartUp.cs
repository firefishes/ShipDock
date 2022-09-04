using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using StaticConfig;
using System;
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
                new UIData(),
                new PlayerData(),
                new ServiceData(),
                new ConfigData(Consts.D_CONFIGS),
                new LegionData(),
                new TroopsData(),
            };
        }

        public override void EnterGameHandler()
        {
            base.EnterGameHandler();

            PlayerData playerData = Consts.D_PLAYER.GetData<PlayerData>();
            playerData.InitPlayer();

            IModular[] modulars = new IModular[]
            {
                new MessageModular(Consts.M_MESSAGE),
                new ServiceModular(),
                new ViewModular(),
                new DataModular(),
                new BattleModular(),
                new PeaceWorldModular(),
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
            mConfigHelper.AddHolderType<PeaceOrganizations>(Consts.CONF_ORGANIZATIONS);
            //����������
            mLoadConfigNames = mConfigHelper.HolderTypes;

            //������������������
            string[] list = mLoadConfigNames.ToArray();
            mConfigHelper.Load(OnConfigLoaded, list);
        }

        //���ü������
        private void OnConfigLoaded(ConfigsResult configResult)
        {
            Consts.D_CONFIGS.SetConfigDataDefaultName();
            Consts.CONF_GROUP_CONFIGS.SetConfigGroupDefaultName();

            //���������ݴ���������Ѽ��ص��������ݣ���ʹ��һ����������Ӧ
            ConfigData data = Consts.D_CONFIGS.GetData<ConfigData>();
            data.AddConfigs(Consts.CONF_GROUP_CONFIGS, configResult);

            Consts.UM_LOGIN.OpenUI<UILoginModular>();

            Test();

        }

        private void Test()
        {
            //ͨ����װ������չ�������������ݴ����л�ȡ����
            Dictionary<int, PeaceOrganizations> testConfTable = Consts.CONF_ORGANIZATIONS.GetConfigTable<PeaceOrganizations>();

            ////�������ж�ȡ����
            foreach (var item in testConfTable)
            {
                OrganizationFields fields = new OrganizationFields();
                fields.InitFieldsFromConfig(item.Value);

                Debug.Log(fields.GetStringData(FieldsConsts.F_ORG_LEVEL_NAME));
                Debug.Log(fields.GetIntData(FieldsConsts.F_ORGANIZATION_VALUE));
                Debug.Log(fields.GetIntData(FieldsConsts.F_IS_BASE_ORGANIZATION) == 0 ? false : true);
            }

            TroopFields troopFields = new TroopFields();
            troopFields.InitFields();
            troopFields.SetTroops(1000, 1000);
            Debug.Log("ID ".Append(troopFields.GetID().ToString(), ":", troopFields.TroopLevelName(), " ���� ", troopFields.GetTroops().ToString()));
        }
    }
}