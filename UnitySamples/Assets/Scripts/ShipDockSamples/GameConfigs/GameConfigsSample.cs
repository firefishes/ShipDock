using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using StaticConfig;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigsSample : ShipDockAppComponent
{
    /// <summary>从配置辅助器中绑定配置类型后，获取到的配置名，为后续加载配置文件做准备</summary>
    private List<string> mLoadConfigNames;
    /// <summary>存储需要加载的配置名</summary>
    private ConfigHelper mConfigHelper;

    public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
    {
        base.GetDataProxyHandler(param);

        param.ParamValue = new IDataProxy[]
        {
            //添加配置数据代理
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
        //新建配置辅助器
        mConfigHelper = new ConfigHelper
        {
            //设置配置文件所在的资源包名
            ConfigResABName = "sample_configs"
        };

        //向配置辅助器添加需要绑定的配置类，此类为工具自动生成
        mConfigHelper.AddHolderType<PuzzlesConfig>(SampleConsts.CONF_PUZZLES);
        //设置配置名
        mLoadConfigNames = mConfigHelper.HolderTypes;

        //根据配置名加载配置
        string[] list = mLoadConfigNames.ToArray();
        mConfigHelper.Load(OnConfigLoaded, list);
    }

    //配置加载完成
    private void OnConfigLoaded(ConfigsResult configResult)
    {
        SampleConsts.D_CONFIGS.SetConfigDataDefaultName();
        SampleConsts.CONF_GROUP_CONFIGS.SetConfigGroupDefaultName();

        ConfigData data = SampleConsts.D_CONFIGS.GetData<ConfigData>();

        //向配置数据代理中添加已加载的配置数据，并使用一个组名做对应
        data.AddConfigs(SampleConsts.CONF_GROUP_CONFIGS, configResult);

        //通过封装过的扩展方法从配置数据代理中获取配置
        Dictionary<int, PuzzlesConfig> puzzleConfTable = SampleConsts.CONF_PUZZLES.GetConfigTable<PuzzlesConfig>();

        //从配置中读取数据
        int id = 1;
        PuzzlesConfig config = puzzleConfTable[id];
        Debug.Log(config.question);
    }
}
