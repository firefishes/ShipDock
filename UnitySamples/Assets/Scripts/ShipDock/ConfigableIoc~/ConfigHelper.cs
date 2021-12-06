using ShipDock.Config;
using ShipDock.Loader;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public class ConfigHelper
    {
        private string mConfigLoading;
        private Action<ConfigsResult> mLoadConfigNotice;
        private Queue<string> mWillLoadNames;
        private List<string> mConfigReady;
        private KeyValueList<string, IConfigHolder> mConfigHolders;
        private readonly KeyValueList<string, Func<IConfigHolder>> mConfigHolderCreater;

        public string ConfigResABName { get; set; }
        public List<string> HolderTypes { get; private set; }

        public ConfigHelper()
        {
            HolderTypes = new List<string>();
            mConfigHolders = new KeyValueList<string, IConfigHolder>();
            mConfigHolderCreater = new KeyValueList<string, Func<IConfigHolder>>();
        }

        public void AddHolderType<T>(string name) where T : IConfig, new()
        {
            if (!HolderTypes.Contains(name))
            {
                IConfigHolder creater()
                {
                    return new ConfigHolder<T>();
                }
                AddHolderType(name, creater);
            }
            else { }
        }

        public void AddHolderType(string name, Func<IConfigHolder> creater = default)
        {
            if (!HolderTypes.Contains(name))
            {
                HolderTypes.Add(name);
                mConfigHolderCreater[name] = creater;
            }
            else { }
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

            mLoadConfigNotice = target;

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
            AssetBundles abs = Framework.Instance.GetUnit<AssetBundles>(Framework.UNIT_AB);
            TextAsset data = abs.Get<TextAsset>(ConfigResABName, mConfigLoading);

            LogConfigEmptyAfterLoaded(data == default, ref mConfigLoading);
            LoaderConfirm(data.bytes);
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

            mLoadConfigNotice?.Invoke(configsResult);
            mLoadConfigNotice = default;
        }

        #region 日志相关
        [System.Diagnostics.Conditional("G_LOG")]
        private void LogLoadConfigResABNameEmpty()
        {
            "error".Log("ConfigHelper need set property ConfigResABName for get config res.");
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
