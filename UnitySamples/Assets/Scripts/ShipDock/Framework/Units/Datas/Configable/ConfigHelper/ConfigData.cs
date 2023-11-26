using System.Collections.Generic;

namespace ShipDock
{
    public class ConfigData : DataProxy
    {
        private Dictionary<int, ConfigsResult> mConfigs;

        public ConfigData(int name) : base(name)
        {
            mConfigs = new Dictionary<int, ConfigsResult>();
        }

        public bool HasConfigGroup(int name)
        {
            return mConfigs != default && mConfigs.ContainsKey(name);
        }

        public void AddConfigs(int name, ConfigsResult results)
        {
            if (HasConfigGroup(name))
            {
                ConfigsResult temp = GetConfigs(name);
                temp.Clean();
            }
            mConfigs[name] = results;
        }

        public void RemoveConfigs(int name)
        {
            bool flag = mConfigs.TryGetValue(name, out ConfigsResult configsResult);
            if (flag)
            {
                mConfigs.Remove(name);
                configsResult.Clean();
            }
            else { }
        }

        public ConfigsResult GetConfigs(int name)
        {
            return HasConfigGroup(name) ? mConfigs[name] : default;
        }
    }
}
