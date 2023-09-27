using System.Collections.Generic;

namespace ShipDock
{
    public static class ConfigableExtension
    {
        /// <summary>Ĭ�ϵ��������ݴ�����</summary>
        private static int configDataDefaultName = int.MaxValue;
        /// <summary>Ĭ�ϵ����÷�����</summary>
        private static int configGroupDefaultName = int.MaxValue;

        /// <summary>
        /// ����Ĭ�ϵ����÷�����
        /// </summary>
        /// <param name="groupName"></param>
        public static void SetConfigGroupDefaultName(this int groupName)
        {
            configGroupDefaultName = groupName;
        }

        /// <summary>
        /// ����Ĭ�ϵ��������ݴ�����
        /// </summary>
        /// <param name="dataName"></param>
        public static void SetConfigDataDefaultName(this int dataName)
        {
            configDataDefaultName = dataName;
        }

        /// <summary>��ȡ������Դ������ָ�����������ݣ������ֵ�ķ�ʽ����</summary>
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
