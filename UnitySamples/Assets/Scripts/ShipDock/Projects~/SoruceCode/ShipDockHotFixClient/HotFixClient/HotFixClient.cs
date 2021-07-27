using ShipDock.Applications;
using ShipDock.Commons;
using ShipDock.Config;
using ShipDock.Datas;
using ShipDock.FSM;
using ShipDock.HotFix;
using ShipDock.Modulars;
using ShipDock.Network;
using ShipDock.Notices;
using ShipDock.Sounds;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

public static class HotFixClientExtensions
{
    public static Dictionary<int, ConfigT> GetConfig<ConfigT>(this string configName) where ConfigT : IConfig, new()
    {
        Dictionary<int, ConfigT> result = default;
        int dataName = HotFixClient.configsDataName;
        int modularName = HotFixClient.configsModularName;
        if (dataName != int.MaxValue && modularName != int.MaxValue)
        {
            result = HotFixClient.Instance.GetConfig<ConfigT>(dataName, modularName, configName, out _);
        }
        else { }

        return result;
    }

    public static void SetSceneUpdate(this object target, Action<int> handler, bool isAdd)
    {
        if (isAdd)
        {
            HotFixClient.Instance.AddUpdate(handler);
        }
        else
        {
            HotFixClient.Instance.RemoveUpdate(handler);
        }
    }
}

//namespace ShipDock.Applications
//{
//    public class CommonUpdaters
//    {
//        /// <summary>普通帧更新器的映射</summary>
//        private KeyValueList<Action<int>, MethodUpdater> mUpdaterMapper;

//        public CommonUpdaters()
//        {
//            mUpdaterMapper = new KeyValueList<Action<int>, MethodUpdater>();
//        }

//        public void Clean()
//        {
//            MethodUpdater updater;
//            int max = mUpdaterMapper.Size;
//            for (int i = 0; i < max; i++)
//            {
//                updater = mUpdaterMapper.GetValueByIndex(i);
//                UpdaterNotice.RemoveSceneUpdater(updater);
//                updater.Dispose();
//            }
//            mUpdaterMapper?.Clear();
//        }

//        public void AddUpdate(Action<int> method)
//        {
//            if (!mUpdaterMapper.ContainsKey(method))
//            {
//                MethodUpdater updater = new MethodUpdater
//                {
//                    Update = method
//                };
//                mUpdaterMapper[method] = updater;
//                UpdaterNotice.AddSceneUpdater(updater);
//            }
//            else { }
//        }

//        public void RemoveUpdate(Action<int> method)
//        {
//            if (mUpdaterMapper.ContainsKey(method))
//            {
//                MethodUpdater updater = mUpdaterMapper.GetValue(method, true);
//                UpdaterNotice.RemoveSceneUpdater(updater);
//                updater.Dispose();
//            }
//            else { }
//        }
//    }
//}

namespace ShipDock.HotFix
{
    /// <summary>
    /// 
    /// 完全热更新客户端单例类
    /// 
    /// 用于支持完全使用ILRuntime热更方式运行应用程序
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class HotFixClient
    {
        /// <summary>配置相关的数据代理名</summary>
        public static int configsDataName = int.MaxValue;
        /// <summary>配置相关的模块名</summary>
        public static int configsModularName = int.MaxValue;

        private static HotFixClient instance;

        public static HotFixClient Instance
        {
            get
            {
                if (instance == default)
                {
                    instance = new HotFixClient();
                }
                else { }
                return instance;
            }
        }

        /// <summary>状态机中各个状态帧更新器的映射</summary>
        private KeyValueList<IState, IUpdate> mStateMapper;
        /// <summary>各个状态机帧更新器的映射</summary>
        private KeyValueList<IStateMachine, IUpdate> mFSMMapper;
        /// <summary>通用的帧更新管理器</summary>
        private CommonUpdaters mCommonUpdaters;
        /// <summary>业务逻辑的全局配置</summary>
        private IConfig SettingsConfig { get; set; }

        /// <summary>业务逻辑配置解析容器绑定辅助器</summary>
        public ConfigHelper Configs { get; private set; }
        /// <summary>业务逻辑大模块管理器</summary>
        public DecorativeModulars Modulars { get; private set; }
        /// <summary>数据曾总管理器</summary>
        public DataWarehouse Datas { get; private set; }
        /// <summary>网络请求分发器</summary>
        public NetDistributer NetDistributer { get; private set; }
        /// <summary>状态机管理器</summary>
        public StateMachines FSMs { get; private set; }
        /// <summary>声音管理器</summary>
        public SoundEffects Sounds { get; private set; }
        /// <summary>主工程的热更入口组件</summary>
        public HotFixerComponent HotFixEnter { get; set; }

        public int PlaySoundNoticeName { get; set; } = int.MaxValue;
        public int StopSoundNoticeName { get; set; } = int.MaxValue;
        public int PlayBGMNoticeName { get; set; } = int.MaxValue;
        public int StopBGMNoticeName { get; set; } = int.MaxValue;
        public int AddSoundsNoticeName { get; set; } = int.MaxValue;
        public int RemoveSoundsNoticeName { get; set; } = int.MaxValue;

        private HotFixClient()
        {
            "log".Log("Application will run almost in hot fix client..");
            mCommonUpdaters = new CommonUpdaters();

            mStateMapper = new KeyValueList<IState, IUpdate>();
            mFSMMapper = new KeyValueList<IStateMachine, IUpdate>();

            Modulars = new DecorativeModulars();
            Configs = new ConfigHelper();
            Datas = new DataWarehouse();
            FSMs = new StateMachines
            {
                FSMFrameUpdater = OnFSMFrameUpdater,
                StateFrameUpdater = OnStateFrameUpdater,
            };

            Sounds = new SoundEffects();
            Sounds.Init();

            #region 对主工程框架中热更端需要覆盖的各功能单元做重填充，以使得相同功能的代码定义转移到热更端
            Framework framework = Framework.Instance;
            framework.ReloadUnit(new IFrameworkUnit[] {
                framework.CreateUnitByBridge(Framework.UNIT_MODULARS, Modulars),
                framework.CreateUnitByBridge(Framework.UNIT_CONFIG, Configs),
                framework.CreateUnitByBridge(Framework.UNIT_DATA, Datas),
                framework.CreateUnitByBridge(Framework.UNIT_FSM, FSMs),
                framework.CreateUnitByBridge(Framework.UNIT_SOUND, Sounds),
            });
            #endregion
        }

        public void SetSoundNotices(int playSound, int stopSound, int playBGM, int stopBGM, int addSounds, int removeSounds)
        {
            PlaySoundNoticeName = playSound;
            StopSoundNoticeName = stopSound;
            PlayBGMNoticeName = playBGM;
            StopBGMNoticeName = stopBGM;
            AddSoundsNoticeName = addSounds;
            RemoveSoundsNoticeName = removeSounds;

            ListenNotice(PlaySoundNoticeName, OnPlaySound);
            ListenNotice(StopSoundNoticeName, OnStopSound);
            ListenNotice(PlayBGMNoticeName, OnPlayBGM);
            ListenNotice(StopBGMNoticeName, OnStopBGM);
            ListenNotice(AddSoundsNoticeName, OnAddSounds);
            ListenNotice(RemoveSoundsNoticeName, OnRemoveSounds);
        }

        private void ListenNotice(int noticeName, Action<INoticeBase<int>> handler)
        {
            if (noticeName != int.MaxValue)
            {
                noticeName.Add(handler);
            }
            else { }
        }

        private void OnRemoveSounds(INoticeBase<int> param)
        {
            if (param is IParamNotice<int> soundNotice)
            {
                Sounds.RemoveSound(soundNotice.ParamValue);
            }
            else { }
        }

        private void OnAddSounds(INoticeBase<int> param)
        {
            if (param is IParamNotice<List<SoundItem>> soundNotice)
            {
                Sounds.SetPlayList(soundNotice.ParamValue.ToArray());
                soundNotice.ParamValue.Clear();
            }
            else { }
        }

        private void OnPlaySound(INoticeBase<int> param)
        {
            if (param is IParamNotice<string> soundNotice)
            {
                Sounds.PlaySound(soundNotice.ParamValue);
            }
            else { }
        }

        private void OnStopSound(INoticeBase<int> param)
        {
            if (param is IParamNotice<string> soundNotice)
            {
                Sounds.StopSound(soundNotice.ParamValue);
            }
            else { }
        }

        private void OnPlayBGM(INoticeBase<int> param)
        {
            if (param is IParamNotice<string> soundNotice)
            {
                Sounds.PlayBGM(soundNotice.ParamValue);
            }
            else { }
        }

        private void OnStopBGM(INoticeBase<int> param)
        {
            Sounds.StopBGM();
        }

        public void RunSoundsUnit()
        {
            AddUpdate(OnSoundsUpdate);
        }

        private void OnSoundsUpdate(int time)
        {
            Sounds?.Update();
        }

        private void OnStateFrameUpdater(IState state, bool isAdd)
        {
            if (isAdd)
            {
                if (!mStateMapper.ContainsKey(state))
                {
                    MethodUpdater updater = new MethodUpdater()
                    {
                        Update = state.UpdateState
                    };
                    mStateMapper[state] = updater;
                    UpdaterNotice.AddSceneUpdater(updater);
                }
                else { }
            }
            else
            {
                IUpdate updater = mStateMapper.GetValue(state, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        private void OnFSMFrameUpdater(IStateMachine fsm, bool isAdd)
        {
            if (isAdd)
            {
                if (!mFSMMapper.ContainsKey(fsm))
                {
                    MethodUpdater updater = new MethodUpdater()
                    {
                        Update = fsm.UpdateState
                    };
                    mFSMMapper[fsm] = updater;
                    UpdaterNotice.AddSceneUpdater(updater);
                }
                else { }
            }
            else
            {
                IUpdate updater = mFSMMapper.GetValue(fsm, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        public void Clean()
        {
            PlaySoundNoticeName.Remove(OnPlaySound);
            PlayBGMNoticeName.Remove(OnPlayBGM);
            StopBGMNoticeName.Remove(OnStopBGM);
            AddSoundsNoticeName.Remove(OnAddSounds);
            RemoveSoundsNoticeName.Remove(OnRemoveSounds);

            Modulars.Dispose();
            Datas.Dispose();
        }

        public void InitGroupConfigs(int dataName, int groupName, ref ConfigsResult result)
        {
            ConfigData data = Datas.GetData<ConfigData>(dataName);
            data.AddConfigs(groupName, result);
        }

        public Dictionary<int, ConfigT> GetConfig<ConfigT>(int dataName, int groupName, string configName, out int statu) where  ConfigT : IConfig, new()
        {
            ConfigData data = Datas.GetData<ConfigData>(dataName);
            ConfigsResult configs = data.GetConfigs(groupName);
            Dictionary<int, ConfigT> dic = configs.GetConfigRaw<ConfigT>(configName, out statu);
            return dic;
        }

        public ConfigT GetConfig<ConfigT>(int dataName, int groupName, string configName, int id) where ConfigT : IConfig, new()
        {
            ConfigData data = Datas.GetData<ConfigData>(dataName);
            ConfigsResult configs = data.GetConfigs(groupName);
            Dictionary<int, ConfigT> mapper = configs.GetConfigRaw<ConfigT>(configName, out int statu);
            return statu == 0 ? mapper[id] : default;
        }

        public void SetGlobalSettings(IConfig value)
        {
            SettingsConfig = value;
        }

        public T GlobalSettins<T>() where T : IConfig
        {
            return (T)SettingsConfig;
        }

        public void AddUpdate(Action<int> method)
        {
            mCommonUpdaters.AddUpdate(method);
        }

        public void RemoveUpdate(Action<int> method)
        {
            mCommonUpdaters.RemoveUpdate(method);
        }

        public void StartCorutine(System.Collections.IEnumerator target)
        {
            HotFixEnter.StartCoroutine(target);
        }
    }
}
