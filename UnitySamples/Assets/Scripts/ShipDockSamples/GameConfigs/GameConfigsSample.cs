using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using StaticConfig;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigsSample : ShipDockAppComponent
{
    /// <summary>�����ø������а��������ͺ󣬻�ȡ������������Ϊ�������������ļ���׼��</summary>
    private List<string> mLoadConfigNames;
    /// <summary>�洢��Ҫ���ص�������</summary>
    private ConfigHelper mConfigHelper;

    public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
    {
        base.GetDataProxyHandler(param);

        param.ParamValue = new IDataProxy[]
        {
            //����������ݴ���
            new ConfigData(SampleConsts.D_CONFIGS),
        };
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        LoadConfig();
    }

    private void LoadConfig()
    {
        //�½����ø�����
        mConfigHelper = new ConfigHelper
        {
            //���������ļ����ڵ���Դ����
            ConfigResABName = "sample_configs"
        };

        //�����ø����������Ҫ�󶨵������࣬����Ϊ�����Զ�����
        mConfigHelper.AddHolderType<PuzzlesConfig>(SampleConsts.CONF_PUZZLES);
        //����������
        mLoadConfigNames = mConfigHelper.HolderTypes;

        //������������������
        string[] list = mLoadConfigNames.ToArray();
        mConfigHelper.Load(OnConfigLoaded, list);
    }

    //���ü������
    private void OnConfigLoaded(ConfigsResult configResult)
    {
        SampleConsts.D_CONFIGS.SetConfigDataDefaultName();
        SampleConsts.CONF_GROUP_CONFIGS.SetConfigGroupDefaultName();

        ConfigData data = SampleConsts.D_CONFIGS.GetData<ConfigData>();

        //���������ݴ���������Ѽ��ص��������ݣ���ʹ��һ����������Ӧ
        data.AddConfigs(SampleConsts.CONF_GROUP_CONFIGS, configResult);

        //ͨ����װ������չ�������������ݴ����л�ȡ����
        Dictionary<int, PuzzlesConfig> puzzleConfTable = SampleConsts.CONF_PUZZLES.GetConfigTable<PuzzlesConfig>();

        //�������ж�ȡ����
        int id = 1;
        PuzzlesConfig config = puzzleConfTable[id];
        Debug.Log(config.question);
    }
}
