using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock
{
    public class ConfigHelper : IReclaim
    {
        private string mConfigLoading;
        private Action<ConfigsResult> mLoadConfigHandler;
        private Queue<string> mWillLoadNames;
        private List<string> mConfigReady;
        private Dictionary<string, IConfigHolder> mConfigHolders;
        private KeyValueList<string, Func<IConfigHolder>> mConfigHolderCreater;

        public string ConfigResABName { get; set; }
        public List<string> HolderTypes { get; private set; }

        public ConfigHelper()
        {
            HolderTypes = new List<string>();
            mConfigHolders = new Dictionary<string, IConfigHolder>();
            mConfigHolderCreater = new KeyValueList<string, Func<IConfigHolder>>();
        }

        public void Reclaim()
        {
            HolderTypes?.Clear();
            HolderTypes = default;
            mLoadConfigHandler = default;

            Utils.Reclaim(ref mConfigReady);
            Utils.Reclaim(ref mConfigHolders);
            Utils.Reclaim(ref mConfigHolderCreater);
        }

        public void AddHolderType<T>(string configName) where T : IConfig, new()
        {
            if (HolderTypes.Contains(configName)) { }
            {
                static IConfigHolder creater()
                {
                    return new ConfigHolder<T>();
                }
                AddHolderType(configName, creater);
            }
        }

        public void AddHolderType(string configName, Func<IConfigHolder> creater = default)
        {
            if (HolderTypes.Contains(configName)) { }
            {
                HolderTypes.Add(configName);
                mConfigHolderCreater[configName] = creater;
            }
        }

        public void Load(Action<ConfigsResult> target, params string[] configNames)
        {
            if (string.IsNullOrEmpty(ConfigResABName))
            {
                LogLoadConfigResABNameEmpty();
                return;
            }
            else { }

            if (mWillLoadNames != default)
            {
#if ILRUNTIME
                mWillLoadNames.Clear();
#else
                Utils.Reclaim(ref mWillLoadNames, false);
#endif
            }
            else { }

            mConfigReady = new List<string>();
            mWillLoadNames = new Queue<string>();

            mLoadConfigHandler = target;

            CreateConfigHolder(ref configNames);

            LoaderConfirm(default);
        }

        private void CreateConfigHolder(ref string[] configNames)
        {
            string name;
            IConfigHolder configHolder;
            int max = configNames.Length;
            for (int i = 0; i < max; i++)
            {
                name = configNames[i];
                if (mConfigHolders.ContainsKey(name))
                {
                    mConfigReady.Add(name);
                }
                else
                {
                    configHolder = GetHolder(name);
                    configHolder.SetCongfigName(name);
                    mConfigHolders[name] = configHolder;

                    mWillLoadNames.Enqueue(name);
                }
            }
        }

        private IConfigHolder GetHolder(string name)
        {
            Func<IConfigHolder> func = mConfigHolderCreater[name];

            LogConfigHolderEmpty(func == default, ref name);

            return func.Invoke();
        }

        private void LoaderConfirm(byte[] vs)
        {
            if (mWillLoadNames.Count > 0)
            {
                LoadConfigItem(ref vs);
            }
            else
            {
                if (vs != default)
                {
                    ParseConfigHolder(vs);
                }
                else { }

                ConfigResultReady();
            }
        }

        private void LoadConfigItem(ref byte[] vs)
        {
            if (vs != default)
            {
                ParseConfigHolder(vs);
            }
            else { }

            mConfigLoading = mWillLoadNames.Dequeue();
            AssetBundles abs = Framework.UNIT_AB.Unit<AssetBundles>();
            TextAsset data = abs.Get<TextAsset>(ConfigResABName, mConfigLoading);

            LogConfigEmptyAfterLoaded(data == default, ref mConfigLoading);
            LoaderConfirm(data != default ? data.bytes : default);
        }

        private void ParseConfigHolder(byte[] vs)
        {
            IConfigHolder holder = mConfigHolders[mConfigLoading];
            holder.SetSource(ref vs);
            mConfigReady.Add(mConfigLoading);
        }

        private void ConfigResultReady()
        {
            string configName;
            int max = mConfigReady.Count;
            IConfigHolder[] holders = new IConfigHolder[max];
            for (int i = 0; i < max; i++)
            {
                configName = mConfigReady[i];
                holders[i] = mConfigHolders[configName];
            }

#if ILRUNTIME
            mConfigReady?.Clear();
#else
            Utils.Reclaim(ref mConfigReady);
#endif

            ConfigsResult configsResult = new ConfigsResult();
            configsResult.SetConfigHolders(holders);

            mLoadConfigHandler?.Invoke(configsResult);
            mLoadConfigHandler = default;
        }

        #region 日志相关
        [System.Diagnostics.Conditional("G_LOG")]
        private void LogLoadConfigResABNameEmpty()
        {
            "warning".Log("ConfigHelper need set property ConfigResABName for get config res.");
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogConfigHolderEmpty(bool isHandlerEmpty, ref string name)
        {
            "error: Config holder creater is null, name is {0}".Log(isHandlerEmpty, name);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogConfigEmptyAfterLoaded(bool isConfigEmpty, ref string name)
        {
            "log:Config data is null, name is {0}".Log(isConfigEmpty, mConfigLoading);
        }
        #endregion
    }
}
