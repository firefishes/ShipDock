using System.Collections.Generic;

namespace ShipDock
{
    public static class ConfigableExtension
    {
        /// <summary>默认的配置数据代理名</summary>
        private static int configDataDefaultName = int.MaxValue;
        /// <summary>默认的配置分组名</summary>
        private static int configGroupDefaultName = int.MaxValue;

        /// <summary>
        /// 设置默认的配置分组名
        /// </summary>
        /// <param name="groupName"></param>
        public static void SetConfigGroupDefaultName(this int groupName)
        {
            configGroupDefaultName = groupName;
        }

        /// <summary>
        /// 设置默认的配置数据代理名
        /// </summary>
        /// <param name="dataName"></param>
        public static void SetConfigDataDefaultName(this int dataName)
        {
            configDataDefaultName = dataName;
        }

        /// <summary>获取配置资源名参数指定的配置数据，并以字典的方式返回</summary>
        public static Dictionary<int, ConfigT> GetConfigTable<ConfigT>(this string configName) where ConfigT : IConfig, new()
        {
            if (configDataDefaultName == int.MaxValue || configGroupDefaultName == int.MaxValue)
            {
                return default;
            }
            else { }

            ConfigData data = configDataDefaultName.GetData<ConfigData>();
            ConfigsResult configs = data.GetConfigs(configGroupDefaultName);
            Dictionary<int, ConfigT> dic = configs.GetConfigRaw<ConfigT>(configName, out _);
            return dic;
        }
    }
}
