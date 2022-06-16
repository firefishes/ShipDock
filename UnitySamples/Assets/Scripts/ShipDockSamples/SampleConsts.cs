using ShipDock.Applications;
using ShipDock.Config;
using System.Collections.Generic;
/// <summary>
/// 静态常量
/// </summary>
public static class SampleConsts
{
    /// <summary>数据代理案例中的数据 ID</summary>
    public const int D_SAMPLE = 1;
    /// <summary>本地客户端账户案例中的数据 ID</summary>
    public const int D_PLAYER = 2;
    /// <summary>MVC案例中的数据 ID</summary>
    public const int D_SAMPLE_MVC = 3;
    /// <summary>游戏配置案例中的数据 ID</summary>
    public const int D_CONFIGS = 4;

    #region 消息分发案例相关的消息名
    public const int N_SAMPLE_NOTICE_BY_PARAM = 1000;
    public const int N_SAMPLE_NOTICE_BY_DEFAULT= 1001;
    public const int N_SAMPLE_NOTICE_BY_OBSERVER = 1002;
    #endregion

    /// <summary>逻辑模块案例消息名：开始游戏</summary>
    public const int N_SAMPLE_GAME_START = 1003;
    /// <summary>逻辑模块案例消息名：进入关卡</summary>
    public const int N_SAMPLE_GAME_ENTER_MISSION = 1004;
    /// <summary>逻辑模块案例消息名：加载关卡</summary>
    public const int N_SAMPLE_GAME_LOAD_MISSION = 1005;
    /// <summary>逻辑模块案例消息名：关卡结束</summary>
    public const int N_SAMPLE_GAME_MISSION_FINISHED = 1006;
    /// <summary>逻辑模块案例消息名：功能模块案例演示完毕</summary>
    public const int N_SAMPLE_MODULARS_END = 1007;

    /// <summary>数据代理案例中的数据变更消息</summary>
    public const int DN_SAMPLE_DATA_NOTIFY = 2000;
    /// <summary>MVC案例中的数据变更消息</summary>
    public const int DN_SAMPLE_MVC_DATA_CHANGED = 2001;

    /// <summary>逻辑模块案例中的模块名：游戏流程模块</summary>
    public const int M_SAMPLE_GAME_START = 3001;
    /// <summary>逻辑模块案例中的模块名：关卡模块</summary>
    public const int M_SAMPLE_GAME_MISSIONS = 3002;

    /// <summary>MVC案例中的资源包名</summary>
    public const string AB_SAMPLES = "sample_res";
    /// <summary>MVC案例中的UI模块名</summary>
    public const string U_SAMPLE = "SampleWindow";

    /// <summary>游戏配置案例中的配置组：默认组</summary>
    public const int CONF_GROUP_CONFIGS = 4000;

    /// <summary>游戏配置案例中的配置名</summary>
    public const string CONF_PUZZLES = "puzzles_config";

    /// <summary>游戏配置案例中获取配置的扩展方法</summary>
    public static Dictionary<int, ConfigT> GetConfigTable<ConfigT>(this string configName) where ConfigT : IConfig, new()
    {
        ConfigData data = D_CONFIGS.GetData<ConfigData>();
        ConfigsResult configs = data.GetConfigs(CONF_GROUP_CONFIGS);
        Dictionary<int, ConfigT> dic = configs.GetConfigRaw<ConfigT>(configName, out _);
        return dic;
    }
}