using System;
using UnityEngine;

namespace ShipDock
{
    public class ShipDockApp : Singletons<ShipDockApp>, ICustomCore
    {

        /// <summary>
        /// ���� ShipDock��ԭ��Ӧ�ã� ���
        /// </summary>
        /// <param name="ticks">���߳�����֡��</param>
        /// <param name="onStartUp">��ʼ������õĻص�����</param>
        public static void StartUp(int ticks, Action onStartUp = default)
        {
            //���ں˼�����
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

            Notificater = NotificatonsInt.Instance.Notificater;//�½���Ϣ����
            ABs = new AssetBundles();//�½���Դ��������
            DataWarehouse datas = new();//�½����ݹ�����
            AssetsPooling = new AssetsPooling();//�½�������Դ�����
            Locals = new Locals();//�½����Ա��ػ�������
            Effects = new Effects();//�½���Ч������
            Tenons = new Tenons();//�������������
            Messages = new MessageLooper();//��Ϣ����

            mTennonsUpdater = new MethodUpdater()
            {
                Update = OnTenonsUpdate,
                FixedUpdate = OnTenonsFixedUpdate,
                LateUpdate = OnTenonsLateUpdate,
            };
#if ULTIMATE
            Servers = new Servers();//�½���������������

            int frameTimeInScene = (int)(Time.deltaTime * UpdatesCacher.UPDATE_CACHER_TIME_SCALE);
            ECSContext = new ECSContext(frameTimeInScene);//�½�ECS����������

            StateMachines = new StateMachines//�½�����״̬��������
            {
                FSMFrameUpdater = OnFSMFrameUpdater,
                StateFrameUpdater = OnStateFrameUpdater
            };
            PerspectivesInputer = new PerspectiveInputer();//�½�͸�����彻����
            AppModulars = new DecorativeModulars();//�½�װ��ģ�������
            Configs = new ConfigHelper();//�½����ù�����
#endif

            #region ���ƿ��������ܹ��ܵ�Ԫ
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
                //�½��ͻ������������̵߳�֡������
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

            //����������
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
                //�½� UI ������
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