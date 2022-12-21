#define _G_LOG
#define _SHIPDOCK_MODULARS

using ShipDock.Commons;
using ShipDock.Datas;
using ShipDock.ECS;
using ShipDock.FSM;
using ShipDock.Loader;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using ShipDock.Testers;
using ShipDock.Ticks;
using ShipDock.Tools;
using ShipDock.UI;
using System;
using UnityEngine;

namespace ShipDock.Applications 
{

    /// <summary>
    /// 
    /// ShipDock 门面单例
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class ShipDockApp : Singletons<ShipDockApp>, IAppILRuntime, ICustomFramework
    {
        /// <summary>
        /// 启动 ShipDock 框架
        /// </summary>
        /// <param name="ticks">子线程心跳帧率</param>
        /// <param name="onStartUp">初始化后调用的回调函数</param>
        public static void StartUp(int ticks, Action onStartUp = default)
        {
            ICustomFramework app = Instance;
            Framework.Instance.InitCustomFramework(app, ticks, onStartUp);//将 ShipDock 门面对象加入框架定制
        }

        /// <summary>
        /// 让指定方法在子线程延迟一帧后执行
        /// </summary>
        public static void CallLater(Action<int> method)
        {
            Instance.TicksUpdater?.CallLater(method);
        }

        public static void Close()
        {
            Framework.Instance.Clean();
        }

        private Action mAppStarted;
        private KeyValueList<IStateMachine, IUpdate> mFSMUpdaters;
        private KeyValueList<IState, IUpdate> mStateUpdaters;
        private MethodUpdater mMainThreadReadyChecker;
        private IServer[] mServersWillAdd;

        private IHotFixConfig HotFixConfig { get; set; }

        public bool IsStarted { get; private set; }
        public bool IsSceneUpdateReady { get; private set; }
        public UIManager UIs { get; private set; }
        public Tester Tester { get; private set; }
        public Notifications<int> Notificater { get; private set; }
        public IUpdatesComponent UpdatesComponent { get; private set; }
        public TicksUpdater TicksUpdater { get; private set; }
        public Servers Servers { get; private set; }
        public ConfigHelper Configs { get; private set; }
        public Locals Locals { get; private set; }
        public AssetBundles ABs { get; private set; }
        public AssetsPooling AssetsPooling { get; private set; }
        public Effects Effects { get; private set; }
        public StateMachines StateMachines { get; private set; }
        public PerspectiveInputer PerspectivesInputer { get; private set; }
        public DecorativeModulars AppModulars { get; private set; }
        public ECSContext ECSContext { get; private set; }
        public ILRuntimeHotFix ILRuntimeHotFix { get; private set; }
        public IFrameworkUnit[] FrameworkUnits { get; private set; }
        public Action MergeCallOnMainThread { get; set; }
        public Action SceneUpdaterReady { get; set; }

        public DataWarehouse Datas
        {
            get
            {
                return Framework.Instance.GetUnit<DataWarehouse>(Framework.UNIT_DATA);
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogStartedError()
        {
            "error".Log("ShipDockApplication has started");
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogUIRootReady()
        {
            "debug".Log("UI root ready");
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void AssertFrameworkInit(int step)
        {
            const string LOG = "log";
            const string ASSERT_FRAMEWORK_START = "framework start";
            const string WELCOME = "Welcom..";
            const string TICKS_READY = "Ticks Ready";
            const string MANAGERS_READY = "Managers Ready";
            const string FRAMEWORK_STARTED = "Framework started";

            switch (step)
            {
                default:
                    Tester.AddAsserter(ASSERT_FRAMEWORK_START, false, WELCOME, TICKS_READY, MANAGERS_READY, FRAMEWORK_STARTED);
                    break;
                case 0:
                    LOG.AssertLog(ASSERT_FRAMEWORK_START, WELCOME);
                    break;
                case 1:
                    LOG.AssertLog(ASSERT_FRAMEWORK_START, TICKS_READY);
                    break;
                case 2:
                    LOG.AssertLog(ASSERT_FRAMEWORK_START, MANAGERS_READY);
                    break;
                case 3:
                    LOG.AssertLog(ASSERT_FRAMEWORK_START, FRAMEWORK_STARTED);
                    break;
            }
        }

        public void Clean()
        {
            IsStarted = false;

            ShipDockConsts.NOTICE_APPLICATION_CLOSE.Broadcast();

            ILRuntimeHotFix?.Clear();

            Utils.Reclaim(ref mFSMUpdaters);
            Utils.Reclaim(ref mStateUpdaters);

            Locals?.Reclaim();
            Effects?.Reclaim();
            Notificater?.Reclaim();
            TicksUpdater?.Reclaim();
            ECSContext?.Reclaim();
            Servers?.Reclaim();
            StateMachines?.Reclaim();
            Datas?.Reclaim();
            AssetsPooling?.Reclaim();
            ABs?.Reclaim();
            PerspectivesInputer?.Reclaim();
            AppModulars?.Reclaim();

            Tester?.Reclaim();

            AllPools.ResetAllPooling();

            Notificater = default;
            TicksUpdater = default;
            ECSContext = default;
            Servers = default;
            StateMachines = default;
            AssetsPooling = default;
            ABs = default;
            UIs = default;
            Locals = default;
            Effects = default;
            Tester = default;
            PerspectivesInputer = default;
            ILRuntimeHotFix = default;
            AppModulars = default;

            GC.Collect();
        }

        public void Start(int ticks)
        {
            Application.targetFrameRate = ticks <= 0 ? 10 : ticks;
            if (IsStarted)
            {
                LogStartedError();
                return;
            }
            else { }

            Tester = Tester.Instance;
            Tester.Init(new TesterBaseApp());

            AssertFrameworkInit(int.MaxValue);
            AssertFrameworkInit(0);

            Notificater = NotificatonsInt.Instance.Notificater;//新建消息中心
            ABs = new AssetBundles();//新建资源包管理器
            Servers = new Servers();//新建服务容器管理器
            DataWarehouse datas = new DataWarehouse();//新建数据管理器
            AssetsPooling = new AssetsPooling();//新建场景资源对象池
            int frameTimeInScene = (int)(Time.deltaTime * UpdatesCacher.UPDATE_CACHER_TIME_SCALE);
            ECSContext = new ECSContext(frameTimeInScene);//新建ECS世界上下文

            StateMachines = new StateMachines//新建有限状态机管理器
            {
                FSMFrameUpdater = OnFSMFrameUpdater,
                StateFrameUpdater = OnStateFrameUpdater
            };
            Effects = new Effects();//新建特效管理器
            Locals = new Locals();//新建本地化管理器
            PerspectivesInputer = new PerspectiveInputer();//新建透视物体交互器
            AppModulars = new DecorativeModulars();//新建装饰模块管理器
            Configs = new ConfigHelper();//新建配置管理器

            #region 向定制框架中填充框架功能单元
            Framework framework = Framework.Instance;
            FrameworkUnits = new IFrameworkUnit[]
            {
                framework.CreateUnitByBridge(Framework.UNIT_DATA, datas),
                framework.CreateUnitByBridge(Framework.UNIT_AB, ABs),
                framework.CreateUnitByBridge(Framework.UNIT_MODULARS, AppModulars),
                framework.CreateUnitByBridge(Framework.UNIT_ECS, ECSContext),
                framework.CreateUnitByBridge(Framework.UNIT_IOC, Servers),
                framework.CreateUnitByBridge(Framework.UNIT_CONFIG, Configs),
                framework.CreateUnitByBridge(Framework.UNIT_UI, UIs),
                framework.CreateUnitByBridge(Framework.UNIT_FSM, StateMachines),
            };
            framework.LoadUnit(FrameworkUnits);
            #endregion

            mFSMUpdaters = new KeyValueList<IStateMachine, IUpdate>();
            mStateUpdaters = new KeyValueList<IState, IUpdate>();
            TicksUpdater = new TicksUpdater(Application.targetFrameRate);//新建客户端心跳帧更新器

            AssertFrameworkInit(1);
            AssertFrameworkInit(2);

            IsStarted = true;
            mAppStarted?.Invoke();
            mAppStarted = default;

            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Add(OnSceneUpdateReady);
            UpdatesComponent?.Init();

            ShipDockConsts.NOTICE_APPLICATION_STARTUP.Broadcast();//框架启动完成
            AssertFrameworkInit(3);
        }

        private void OnSceneUpdateReady(INoticeBase<int> time)
        {
            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Remove(OnSceneUpdateReady);

            IsSceneUpdateReady = true;
            SceneUpdaterReady?.Invoke();
        }

        private void OnStateFrameUpdater(IState state, bool isAdd)
        {
            if (isAdd)
            {
                if (!mStateUpdaters.IsContainsKey(state))
                {
                    MethodUpdater updater = new MethodUpdater
                    {
                        Update = state.UpdateState
                    };
                    mStateUpdaters[state] = updater;
                    UpdaterNotice.AddSceneUpdater(updater);
                }
                else { }
            }
            else
            {
                IUpdate updater = mStateUpdaters.GetValue(state, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        private void OnFSMFrameUpdater(IStateMachine fsm, bool isAdd)
        {
            if (isAdd)
            {
                if (!mFSMUpdaters.ContainsKey(fsm))
                {
                    MethodUpdater updater = new MethodUpdater
                    {
                        Update = fsm.UpdateState
                    };
                    mFSMUpdaters[fsm] = updater;
                    UpdaterNotice.AddSceneUpdater(updater);
                }
                else { }
            }
            else
            {
                IUpdate updater = mFSMUpdaters.GetValue(fsm, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        /// <summary>
        /// 启动 IOC 功能
        /// </summary>
        /// <param name="servers">需要添加的服务容器</param>
        /// <param name="mainThreadServersReady">服务容器初始化完成后在主线程上的回调</param>
        /// <param name="onInitedCallbacks">服务容器初始化完成后在子线程上的一组回调函数</param>
        /// <param name="onFinishedCallbacks">服务容器初始化完成后在子线程上的一组回调函数</param>
        public void StartIoC(IServer[] servers, Action mainThreadServersReady, Action[] onInitedCallbacks = default, Action[] onFinishedCallbacks = default)
        {
            if (mainThreadServersReady != default)
            {
                MergeCallOnMainThread += mainThreadServersReady;
            }
            else { }

            SetServersCallback(ref onInitedCallbacks, ref onFinishedCallbacks);

            MergeToMainThread();

            if (!IsSceneUpdateReady)
            {
                SceneUpdaterReady += () =>
                {
                    StartIoC(default, default);
                };
            }
            else { }

            if (mServersWillAdd == default)
            {
                mServersWillAdd = servers;
            }
            else { }
        }

        private void MergeToMainThread()
        {
            if (IsSceneUpdateReady)
            {
                mMainThreadReadyChecker = new MethodUpdater();
                mMainThreadReadyChecker.Update += OnCheckMainThreadReady;
                UpdaterNotice.AddSceneUpdater(mMainThreadReadyChecker);//将调用并入主线程调用
            }
            else { }
        }

        private void SetServersCallback(ref Action[] onInitedCallbacks, ref Action[] onFinishedCallbacks)
        {
            int max = onInitedCallbacks != default ? onInitedCallbacks.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    Servers.AddOnServerInited(onInitedCallbacks[i]);
                }
            }
            else { }

            max = onFinishedCallbacks != default ? onFinishedCallbacks.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    Servers.AddOnServerFinished(onFinishedCallbacks[i]);
                }
            }
            else { }
        }

        private void OnCheckMainThreadReady(int time)
        {
            if (Servers.IsServersReady)
            {
                UpdaterNotice.RemoveSceneUpdater(mMainThreadReadyChecker);

                AddServers();
                MergeCallOnMainThread?.Invoke();
                MergeCallOnMainThread = default;
            }
            else
            {
                bool hasPreset = mServersWillAdd != default;
                if (hasPreset)
                {
                    AddServers();
                }
                else
                {
                    Servers.ServersInited();
                }
            }
        }

        private void AddServers()
        {
            int max = mServersWillAdd != default ? mServersWillAdd.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    Servers.Add(mServersWillAdd[i]);
                }
            }
            else { }

            Utils.Reclaim(ref mServersWillAdd);
        }

        #region ECS 启动和初始化相关
        /// <summary>
        /// 启动 ECS 功能
        /// </summary>
        public void StartECS()
        {
            if (IsSceneUpdateReady)
            {
                InitECSUpdateModes();
            }
            else
            {
                SceneUpdaterReady += StartECS;
            }
        }

        /// <summary>
        /// 初始化ECS模块的更新模式
        /// </summary>
        private void InitECSUpdateModes()
        {
            //子线程更新器
            MethodUpdater updater = new MethodUpdater
            {
                //框架默认为此模式
                Update = OnFrameUpdate
            };
            UpdaterNotice.AddUpdater(updater);

            //主线程更新器
            updater = new MethodUpdater
            {
                //框架默认为此模式
                Update = OnFramUpdateInScene
            };
            UpdaterNotice.AddSceneUpdater(updater);
        }

        /// <summary>
        /// 交替更新帧模式。运行于子线程下，框架默认为交替帧更新模式
        /// 根据框架ECS模组的设置决定是异步执行还是同步执行，默认为同步执行
        /// 
        /// 每个循环帧必执行的流程：
        /// 更新组件
        /// 
        /// 每个奇数帧额外执行的流程：
        /// 检查组件需要释放的数据、检测可移除的组件并销毁
        /// 
        /// </summary>
        private void OnFrameUpdate(int time)
        {
            ILogicContext context = ECSContext.CurrentContext;
            if (context != default)
            {
                if (ShipDockECSSetting.isUpdateByCallLate)
                {
                    context.UpdateECSUnits(time, ComponentUnitUpdate);
                }
                else
                {
                    //框架默认为此模式
                    context.UpdateECSUnits(time);
                }
            }
            else { }
        }

        /// <summary>
        /// 异步方式更新单个组件时的回调方法
        /// </summary>
        private void ComponentUnitUpdate(Action<int> method)
        {
            TicksUpdater?.CallLater(method);
        }

        /// <summary>
        /// 交替更新帧模式。运行于主线程下的场景帧，框架默认为交替帧更新模式
        /// 根据框架ECS模组的设置决定是异步执行还是同步执行，默认为同步执行
        /// 
        /// 每个循环帧必执行的流程：
        /// 更新组件
        /// 
        /// 每个奇数帧额外执行的流程：
        /// 检查组件需要释放的数据、检测可移除的组件并销毁
        /// </summary>
        private void OnFramUpdateInScene(int time)
        {
            ILogicContext context = ECSContext.CurrentContext;
            if (context != default)
            {
                if (ShipDockECSSetting.isUpdateByCallLate)
                {
                    context.UpdateECSUnitsInScene(time, ComponentUnitUpdateInScene);
                }
                else
                {
                    context.UpdateECSUnitsInScene(time);
                }
            }
            else { }
        }

        /// <summary>
        /// 异步方式更新单个组件时的回调方法
        /// </summary>
        private void ComponentUnitUpdateInScene(Action<int> target)
        {
            UpdaterNotice.SceneCallLater(target);
        }
        #endregion

        public void AddStart(Action method)
        {
            if (method != default)
            {
                if (IsStarted)
                {
                    method();
                }
                else
                {
                    mAppStarted += method;
                }
            }
            else { }
        }

        public void RemoveStart(Action method)
        {
            mAppStarted -= method;
        }

        public void InitUIRoot(IUIRoot root)
        {
            if (UIs == default)
            {
                UIs = new UIManager();//新建 UI 管理器
                UIs.SetRoot(root);

                LogUIRootReady();
            }
            else { }
        }

        public KeyValueList<int, IDataProxy> DataProxyLink(IDataExtracter target, params int[] dataNames)
        {
            int name, max = dataNames == default ? 0 : dataNames.Length;
            IDataProxy proxy;
            KeyValueList<int, IDataProxy> result = new KeyValueList<int, IDataProxy>();
            for (int i = 0; i < max; i++)
            {
                name = dataNames[i];
                proxy = Datas.GetData<IDataProxy>(name);
                proxy.Register(target);
                result[name] = proxy;
            }
            return result;
        }

        public void DataProxyDelink(IDataExtracter target, params int[] dataNames)
        {
            int max = dataNames == default ? 0 : dataNames.Length;
            IDataProxy proxy;
            for (int i = 0; i < max; i++)
            {
                proxy = Datas.GetData<IDataProxy>(dataNames[i]);
                proxy.Unregister(target);
            }
        }

        /// <summary>
        /// 初始化 ILRuntime 热更
        /// </summary>
        /// <param name="value"></param>
        /// <param name="config"></param>
        public void SetHotFixSetting(ILRuntimeHotFix value, IHotFixConfig config)
        {
            if (value != default)
            {
                ILRuntimeHotFix = value;//新建IL热更方案的管理器
                if (ILRuntimeHotFix.GetAppILRuntime() == default)
                {
                    ILRuntimeHotFix.SetOwner(this);
                }
                else { }
            }
            else { }

            HotFixConfig = config;
        }

        /// <summary>
        /// 以传入热更配置的泛型参数作为方式初始化 ILRuntime 热更
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void InitILRuntime<T>() where T : IHotFixConfig, new()
        {
            T hotFixConfig = new T();
            ILRuntimeHotFix hotFixCore = new ILRuntimeHotFix(this);

            SetHotFixSetting(hotFixCore, hotFixConfig);
        }

        public IHotFixConfig GetHotFixConfig()
        {
            return HotFixConfig;
        }

        public void SetStarted(bool value)
        {
            IsStarted = value;
        }

        public void SetUpdatesComp(IUpdatesComponent component)
        {
            UpdatesComponent = component;
        }

        public void SetTimeScale(float value)
        {
            Time.timeScale = value;
        }
    }
}
