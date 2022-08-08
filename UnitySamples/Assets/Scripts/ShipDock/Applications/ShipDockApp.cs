﻿#define _G_LOG
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

        private int mFrameSign;
        private int mFrameSignInScene;
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

            Locals?.Dispose();
            Effects?.Dispose();
            Notificater?.Dispose();
            TicksUpdater?.Dispose();
            ECSContext?.Dispose();
            Servers?.Dispose();
            StateMachines?.Dispose();
            Datas?.Dispose();
            AssetsPooling?.Dispose();
            ABs?.Dispose();
            PerspectivesInputer?.Dispose();
            AppModulars?.Dispose();

            Tester?.Dispose();

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
            MethodUpdater updater = new MethodUpdater();
            if (ShipDockECSSetting.isMergeUpdateMode)
            {
                updater.Update = MergeUpdateMode;
            }
            else
            {
                updater.Update = AlternateFrameUpdateMode;//框架默认为此模式
            }
            UpdaterNotice.AddUpdater(updater);

            updater = new MethodUpdater();
            if (ShipDockECSSetting.isMergeUpdateMode)
            {
                updater.Update = MergeUpdateModeInScene;
            }
            else
            {
                updater.Update = AlternateFramUpdateModeInScene;//框架默认为此模式
            }
            UpdaterNotice.AddSceneUpdater(updater);
        }

        /// <summary>
        /// 交替更新帧模式
        /// </summary>
        private void AlternateFrameUpdateMode(int time)
        {
            IShipDockComponentContext context = ECSContext.CurrentContext;
            if (context == default)
            {
                return;
            }
            else { }

            if (ShipDockECSSetting.isUpdateByCallLate)
            {
                context.UpdateComponentUnit(time, ComponentUnitUpdate);
                if (mFrameSign > 0)
                {
                    context.FreeComponentUnit(time, ComponentUnitUpdate);//奇数帧检测是否有需要释放的实体
                    context.RemoveSingedComponents();
                }
                else { }
            }
            else
            {
                context.UpdateComponentUnit(time);//框架默认为此模式
                if (mFrameSign > 0)
                {
                    context.FreeComponentUnit(time);//奇数帧检测是否有需要释放的实体，框架默认为此模式
                    context.RemoveSingedComponents();
                }
                else { }
            }
            mFrameSign++;
            mFrameSign = mFrameSign > 1 ? 0 : mFrameSign;
        }

        /// <summary>
        /// 合并更新帧模式
        /// </summary>
        private void MergeUpdateMode(int time)
        {
            IShipDockComponentContext context = ECSContext.CurrentContext;
            if (ShipDockECSSetting.isUpdateByCallLate)
            {
                context.UpdateComponentUnit(time, ComponentUnitUpdate);
                context.FreeComponentUnit(time, ComponentUnitUpdate);
                context.RemoveSingedComponents();
            }
            else
            {
                context.UpdateComponentUnit(time);
                context.FreeComponentUnit(time);
                context.RemoveSingedComponents();
            }
        }

        /// <summary>
        /// 更新单个组件
        /// </summary>
        private void ComponentUnitUpdate(Action<int> method)
        {
            TicksUpdater?.CallLater(method);
        }

        /// <summary>
        /// 主线程的场景更新已就绪，用于需要在主线程中更新的组件
        /// </summary>
        //private void OnSceneUpdateReady2(INoticeBase<int> obj)
        //{
        //    ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Remove(OnSceneUpdateReady2);

        //    MethodUpdater updater = ShipDockECSSetting.isMergeUpdateMode ?
        //        new MethodUpdater
        //        {
        //            Update = MergeUpdateModeInScene
        //        } :
        //        new MethodUpdater
        //        {
        //            Update = AlternateFramUpdateModeInScene
        //        };

        //    UpdaterNotice notice = Pooling<UpdaterNotice>.From();
        //    notice.ParamValue = updater;
        //    ShipDockConsts.NOTICE_ADD_SCENE_UPDATE.Broadcast(notice);
        //    notice.ToPool();
        //}

        /// <summary>
        /// 交替更新帧模式（用于主线程的场景更新）
        /// </summary>
        private void AlternateFramUpdateModeInScene(int time)
        {
            IShipDockComponentContext context = ECSContext.CurrentContext;
            if (ShipDockECSSetting.isUpdateByCallLate)
            {
                context.UpdateComponentUnitInScene(time, ComponentUnitUpdateInScene);
                if (mFrameSignInScene > 0)
                {
                    context.FreeComponentUnitInScene(time, ComponentUnitUpdateInScene);//奇数帧检测是否有需要释放的实体
                    context.RemoveSingedComponents();
                }
                else { }
            }
            else
            {
                context.UpdateComponentUnitInScene(time);
                if (mFrameSignInScene > 0)
                {
                    context.FreeComponentUnitInScene(time);//奇数帧检测是否有需要释放的实体
                    context.RemoveSingedComponents();
                }
                else { }
            }
            mFrameSignInScene++;
            mFrameSignInScene = mFrameSignInScene > 1 ? 0 : mFrameSignInScene;
        }

        /// <summary>
        /// 合并更新帧模式（用于主线程的场景更新）
        /// </summary>
        private void MergeUpdateModeInScene(int time)
        {
            IShipDockComponentContext context = ECSContext.CurrentContext;
            if (ShipDockECSSetting.isUpdateByCallLate)
            {
                context.UpdateComponentUnitInScene(time, ComponentUnitUpdateInScene);
                context.FreeComponentUnitInScene(time, ComponentUnitUpdateInScene);
                context.RemoveSingedComponents();
            }
            else
            {
                context.UpdateComponentUnitInScene(time);
                context.FreeComponentUnitInScene(time);
                context.RemoveSingedComponents();
            }
        }

        private void ComponentUnitUpdateInScene(Action<int> target)
        {
            UpdaterNotice.SceneCallLater(target);
        }

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
