using ShipDock.Applications;
using ShipDock.Config;
using System.Collections.Generic;

namespace Peace
{
    public static class Consts
    {
        #region 数据层代理名
        /// <summary>战斗数据</summary>
        public const int D_BATTLE = 0;
        /// <summary>玩家数据</summary>
        public const int D_PLAYER = 1;
        /// <summary>配置数据</summary>
        public const int D_CONFIGS = 2;
        #endregion

        public const int M_MESSAGE = 2000;
        public const int M_BATTLE = 2001;

        public const int N_MSG_ADD = 10000;
        public const int N_MSG_QUEUE = 10001;

        public const int MSG_GAME_READY = 20000;
        public const int MSG_ENTER_BATTLE = 20001;

        public const int CONF_GROUP_CONFIGS = 1;

        public const string CONF_EQUIPMENT = "peaceequipment";

        public const string AB_CONFIGS = "peace/configs";

        #region 部队类型
        /// <summary>常规部队</summary>
        public const int TROOP_TYPE_COMMON = 1;
        /// <summary>侦察部队</summary>
        public const int TROOP_TYPE_RECONNOITRE = 2;
        /// <summary>火力部队</summary>
        public const int TROOP_TYPE_FIREPOWER = 3;
        /// <summary>运输部队</summary>
        public const int TROOP_TYPE_TRANSPORT = 4;
        /// <summary>装甲部队</summary>
        public const int TROOP_TYPE_ARMOURED = 5;
        /// <summary>海军部队</summary>
        public const int TROOP_TYPE_NAVY = 6;
        /// <summary>空天部队</summary>
        public const int TROOP_TYPE_SPACE_AIR = 7;
        #endregion

        /// <summary>游戏配置案例中获取配置的扩展方法</summary>
        public static Dictionary<int, ConfigT> GetConfigTable<ConfigT>(this string configName) where ConfigT : IConfig, new()
        {
            ConfigData data = D_CONFIGS.GetData<ConfigData>();
            ConfigsResult configs = data.GetConfigs(CONF_GROUP_CONFIGS);
            Dictionary<int, ConfigT> dic = configs.GetConfigRaw<ConfigT>(configName, out _);
            return dic;
        }
    }
}