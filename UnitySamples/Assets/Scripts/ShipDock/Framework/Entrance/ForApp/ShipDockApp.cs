using System;
using UnityEngine;

namespace ShipDock
{
    public class ShipDockApp : Singletons<ShipDockApp>, ICustomCore
    {

        /// <summary>
        /// 启动 ShipDock（原生应用） 框架
        /// </summary>
        /// <param name="ticks">子线程心跳帧率</param>
        /// <param name="onStartUp">初始化后调用的回调函数</param>
        public static void StartUp(int ticks, Action onStartUp = default)
        {
            //将内核加入框架
            Framework.Instance.InitByCustomCore(Instance, ticks, onStartUp);
        }

        public static void Close()
        {
            Framework.Instance.Clean();
        }

        public bool IsStarted { get; private set; }
        public IFrameworkUnit[] FrameworkUnits { get; private set; }
        public IUpdatesComponent UpdatesComponent { get; private set; }
        public Action MergeCallOnMainThread { get; set; }

        public Tester Tester { get; private set; }
        public Notifications<int> Notificater { get; private set; }
        public AssetBundles ABs { get; private set; }
        public Effects Effects { get; private set; }
        public AssetsPooling AssetsPooling { get; private set; }
        public Locals Locals { get; private set; }
        public UIManager UIs { get; private set; }
        public TicksUpdater TicksUpdater { get; private set; }
        public ConfigHelper Configs { get; private set; }
        public Tenons Tenons { get; private set; }
        public MessageLooper Messages { get; private set; }

        public DataWarehouse Datas
        {
            get
            {
                return Framework.Instance.GetUnit<DataWarehouse>(Framework.UNIT_DATA);
            }
        }

        private MethodUpdater mTennonsUpdater;

        public void Clean()
        {
            if (IsStarted) { }
            else
            {
                return;
            }

            IsStarted = false;

            UpdaterNotice.RemoveSceneUpdate(mTennonsUpdater);
            mTennonsUpdater.Reclaim();
            mTennonsUpdater = default;

            ShipDockConsts.NOTICE_APPLICATION_CLOSE.Broadcast();
            AllPools.ResetAllPooling();

            DuringClean();

            GC.Collect();
        }

        private void DuringClean()
        {
#if ULTIMATE
            ILRuntimeHotFix?.Clear();
            Utils.Reclaim(ref mFSMUpdaters);
            Utils.Reclaim(ref mStateUpdaters);
#endif

            Locals?.Reclaim();
            Notificater?.Reclaim();
            TicksUpdater?.Reclaim();
            ABs?.Reclaim();
            AssetsPooling?.Reclaim();
            Effects?.Reclaim();
            Tenons?.Reclaim();
            Messages?.Reclaim();
#if ULTIMATE
            ECSContext?.Reclaim();
            Servers?.Reclaim();
            StateMachines?.Reclaim();
            Datas?.Reclaim();
            PerspectivesInputer?.Reclaim();
            AppModulars?.Reclaim();
#endif
            Tester?.Reclaim();

            Notificater = default;
            TicksUpdater = default;
            AssetsPooling = default;
            ABs = default;
            UIs = default;
            Locals = default;
            Tester = default;
            Effects = default;
#if ULTIMATE
            ECSContext = default;
            Servers = default;
            StateMachines = default;
            PerspectivesInputer = default;
            ILRuntimeHotFix = default;
            AppModulars = default;
#endif
        }

        public void SetStarted(bool value)
        {
            IsStarted = value;
        }

        public void SetUpdatesComponent(IUpdatesComponent component)
        {
            UpdatesComponent = component;
        }

        public void Run(int ticks)
        {
            Application.targetFrameRate = ticks <= 0 ? 10 : ticks;
            if (IsStarted)
            {
#if ULTIMATE
                LogStartedError();
#endif
                return;
            }
            else { }

            Tester = Tester.Instance;
            Tester.Init(new TesterBaseApp());

            AssertFrameworkInit(int.MaxValue);
            AssertFrameworkInit(0);

            Notificater = NotificatonsInt.Instance.Notificater;//新建消息中心
            ABs = new AssetBundles();//新建资源包管理器
            DataWarehouse datas = new();//新建数据管理器
            AssetsPooling = new AssetsPooling();//新建场景资源对象池
            Locals = new Locals();//新建语言本地化管理器
            Effects = new Effects();//新建特效管理器
            Tenons = new Tenons();//能力组件管理器
            Messages = new MessageLooper();//消息队列

            mTennonsUpdater = new MethodUpdater()
            {
                Update = OnTenonsUpdate,
                FixedUpdate = OnTenonsFixedUpdate,
                LateUpdate = OnTenonsLateUpdate,
            };
#if ULTIMATE
            Servers = new Servers();//新建服务容器管理器

            int frameTimeInScene = (int)(Time.deltaTime * UpdatesCacher.UPDATE_CACHER_TIME_SCALE);
            ECSContext = new ECSContext(frameTimeInScene);//新建ECS世界上下文

            StateMachines = new StateMachines//新建有限状态机管理器
            {
                FSMFrameUpdater = OnFSMFrameUpdater,
                StateFrameUpdater = OnStateFrameUpdater
            };
            PerspectivesInputer = new PerspectiveInputer();//新建透视物体交互器
            AppModulars = new DecorativeModulars();//新建装饰模块管理器
            Configs = new ConfigHelper();//新建配置管理器
#endif

            #region 向定制框架中填充框架功能单元
            Framework framework = Framework.Instance;
            FrameworkUnits = new IFrameworkUnit[]
            {
                framework.CreateUnitByBridge(Framework.UNIT_DATA, datas),
                framework.CreateUnitByBridge(Framework.UNIT_AB, ABs),
                framework.CreateUnitByBridge(Framework.UNIT_CONFIG, Configs),
#if ULTIMATE
                framework.CreateUnitByBridge(Framework.UNIT_MODULARS, AppModulars),
                framework.CreateUnitByBridge(Framework.UNIT_ECS, ECSContext),
                framework.CreateUnitByBridge(Framework.UNIT_IOC, Servers),
                framework.CreateUnitByBridge(Framework.UNIT_FSM, StateMachines),
#endif
                framework.CreateUnitByBridge(Framework.UNIT_UI, UIs),
            };
            framework.LoadUnit(FrameworkUnits);
            #endregion

#if ULTIMATE
            mFSMUpdaters = new KeyValueList<IStateMachine, IUpdate>();
            mStateUpdaters = new KeyValueList<IState, IUpdate>();
#endif
            if (ShipDockAppSettings.threadTicksEnabled)
            {
                //新建客户端运行于子线程的帧更新器
                TicksUpdater = new TicksUpdater(Application.targetFrameRate);
            }
            else { }

            AssertFrameworkInit(1);
            AssertFrameworkInit(2);

            IsStarted = true;
#if ULTIMATE
            mAppStarted?.Invoke();
            mAppStarted = default;
#endif

            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Add(OnSceneUpdateReady);
            UpdatesComponent?.Init();

            //框架启动完成
            ShipDockConsts.NOTICE_APPLICATION_STARTUP.Broadcast();

            AssertFrameworkInit(3);
        }

        private void OnTenonsFixedUpdate(float deltaTime)
        {
            Tenons?.SimulateFixtedUpdate(deltaTime);
        }

        private void OnTenonsUpdate(float deltaTime)
        {
            Tenons?.SimulateUpdateInit(deltaTime);
            Tenons?.SimulateUpdate(deltaTime);
        }

        private void OnTenonsLateUpdate()
        {
            Tenons?.SimulateLateUpdate();
            Tenons?.SimulateUpdateEnd();
            //Tenons?.RunSystems();
            //UpdaterNotice.AddUpdater(mTennonsSystemUpdater);

            //ECS.Instance.RunSystems(0f);
            TicksUpdater?.CallLater(ECS.Instance.RunSystems);
        }

        private void OnSceneUpdateReady(INoticeBase<int> time)
        {
            ShipDockConsts.NOTICE_SCENE_UPDATE_READY.Remove(OnSceneUpdateReady);
#if ULTIMATE
            IsSceneUpdateReady = true;
            SceneUpdaterReady?.Invoke();
#endif
            Messages.Init();
            UpdaterNotice.AddSceneUpdate(mTennonsUpdater);
        }

        public void InitUIRoot(IUIRoot root)
        {
            if (UIs == default)
            {
                //新建 UI 管理器
                UIs = new UIManager();
                UIs.SetRoot(root);

#if ULTIMATE
                LogUIRootReady();
#endif
            }
            else { }
        }

        public void SyncToUpdates(Action method)
        {
            UpdatesComponent.SyncToFrame(method);
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
    }

}