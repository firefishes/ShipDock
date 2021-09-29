#define _G_LOG

using ShipDock.Datas;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using ShipDock.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ShipDock.Testers;
using ShipDock.Tools;
using UnityEngine.SceneManagement;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// 游戏模板组件
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    [RequireComponent(typeof(UpdatesComponent))]
    public class ShipDockGame : MonoBehaviour
    {
        [SerializeField, Tooltip("运行帧率")]
#if ODIN_INSPECTOR
        [TitleGroup("模板信息")]
        [LabelText("目标帧率"), Indent(1)]
#endif 
        private int m_FrameRate = 40;

        [SerializeField, Tooltip("多语言本地化标识")]
#if ODIN_INSPECTOR
        [LabelText("语言本地化参数"), Indent(1)]
#endif 
        private string m_Locals = "CN";

        [SerializeField, Tooltip("开发设置子组")]
#if ODIN_INSPECTOR
        [TitleGroup("开发参数")]
        [LabelText("详情"), Indent(1)]
#endif
        private DevelopSubgroup m_DevelopSubgroup;

        [SerializeField, Tooltip("ILRuntime热更子组")]
#if ODIN_INSPECTOR
        [TitleGroup("ILRuntime热更子组")]
        [LabelText("详情"), Indent(1)]
#endif 
        private HotFixSubgroup m_HotFixSubgroup;

        [SerializeField]
#if ODIN_INSPECTOR
        [TitleGroup("事件")]
        [LabelText("显示事件"), Indent(1)]
#endif 
        private bool m_ShowGameAppEvents;

        [SerializeField, Tooltip("游戏应用启动系列事件")]
#if ODIN_INSPECTOR
        [LabelText("详情"), ShowIf("@this.m_ShowGameAppEvents"), Indent(1)]
#endif 
        private GameApplicationEvents m_GameAppEvents;

        private string mFrameworkSceneName;

        public DevelopSubgroup DevelopSetting
        {
            get
            {
                return m_DevelopSubgroup;
            }
        }

        public HotFixSubgroup HotFixSubgroup
        {
            get
            {
                return m_HotFixSubgroup;
            }
        }

        /// <summary>
        /// UI 根节点就绪事件处理函数
        /// </summary>
        /// <param name="root"></param>
        public void UIRootAwaked(IUIRoot root)
        {
            CreateTesters();
            CreateShipDockApp(ref root);
        }

        private void CreateShipDockApp(ref IUIRoot UIRoot)
        {
            ShipDockApp.Instance.InitUIRoot(UIRoot);
#if RELEASE
            Debug.unityLogger.logEnabled = false;//编译发布版时关闭日志
#endif
            ShipDockApp.StartUp(m_FrameRate, OnShipDockStart);
        }

        private void OnDestroy()
        {
            m_GameAppEvents.createTestersEvent.RemoveAllListeners();
            m_GameAppEvents.enterGameEvent.RemoveAllListeners();
            m_GameAppEvents.getDataProxyEvent.RemoveAllListeners();
            m_GameAppEvents.getGameServersEvent.RemoveAllListeners();
            m_GameAppEvents.initProfileDataEvent.RemoveAllListeners();
            m_GameAppEvents.getLocalsConfigItemEvent.RemoveAllListeners();
            m_GameAppEvents.getServerConfigsEvent.RemoveAllListeners();
            m_GameAppEvents.initProfileEvent.RemoveAllListeners();
            m_GameAppEvents.serversFinishedEvent.RemoveAllListeners();

            ShipDockApp.Close();
            LogShipDockAppClose();

            m_GameAppEvents.frameworkCloseEvent.Invoke();
            m_GameAppEvents.frameworkCloseEvent.RemoveAllListeners();
        }

        private void Awake()
        {
            mFrameworkSceneName = SceneManager.GetActiveScene().name;

            CheckLogColorSettings();

            CreateGame();
        }

        public void RestartAndReloadScene()
        {
            m_GameAppEvents.frameworkCloseEvent.AddListener(OnFrameworkCloseHandler);

            Destroy(this);
        }

        private void OnFrameworkCloseHandler()
        {
            Scene scene = SceneManager.CreateScene("ShipDockScene_FrameworkClose");
            SceneManager.SetActiveScene(scene);//使用新场景做为卸载中转，防止停止编辑器的运行后发生场景文件无法覆盖的报错

            SceneManager.sceneUnloaded += RemoveUnloadFrameworkScene;
            SceneManager.UnloadSceneAsync(scene);
        }

        private void RemoveUnloadFrameworkScene(Scene scene)
        {
            SceneManager.sceneUnloaded -= RemoveUnloadFrameworkScene;

            if (Application.isPlaying)
            {
                SceneManager.sceneLoaded += OnReloadStartUpScene;
                SceneManager.LoadScene(mFrameworkSceneName);
            }
            else { }
        }

        private void OnReloadStartUpScene(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnReloadStartUpScene;
            SceneManager.SetActiveScene(scene);
        }

        public void CheckLogColorSettings()
        {
            if (m_DevelopSubgroup.changeDebugSettings)
            {
                string colorValue = ColorUtility.ToHtmlStringRGBA(m_DevelopSubgroup.logColorDebug);
                DebugUtils.SetLogDebugColor(colorValue);

                colorValue = ColorUtility.ToHtmlStringRGBA(m_DevelopSubgroup.logColorDefault);
                DebugUtils.SetLogDefaultColor(colorValue);

                colorValue = ColorUtility.ToHtmlStringRGBA(m_DevelopSubgroup.logColorWarning);
                DebugUtils.SetLogWarningColor(colorValue);

                colorValue = ColorUtility.ToHtmlStringRGBA(m_DevelopSubgroup.logColorTodo);
                DebugUtils.SetLogTodoColor(colorValue);

                colorValue = ColorUtility.ToHtmlStringRGBA(m_DevelopSubgroup.logColorError);
                DebugUtils.SetLogErrorColor(colorValue);
            }
            else { }
        }

        /// <summary>
        /// 创建游戏应用组件
        /// </summary>
        private void CreateGame()
        {
            ShipDockAppComponent component = GetComponent<ShipDockAppComponent>();
            if (component != default)
            {
                component.SetShipDockGame(this);
                m_GameAppEvents.createTestersEvent.AddListener(component.CreateTestersHandler);
                m_GameAppEvents.enterGameEvent.AddListener(component.EnterGameHandler);
                m_GameAppEvents.getDataProxyEvent.AddListener(component.GetDataProxyHandler);
                m_GameAppEvents.getGameServersEvent.AddListener(component.GetGameServersHandler);
                m_GameAppEvents.initProfileDataEvent.AddListener(component.InitProfileDataHandler);
                m_GameAppEvents.getLocalsConfigItemEvent.AddListener(component.GetLocalsConfigItemHandler);
                m_GameAppEvents.getServerConfigsEvent.AddListener(component.GetServerConfigsHandler);
                m_GameAppEvents.initProfileEvent.AddListener(component.InitProfileHandler);
                m_GameAppEvents.serversFinishedEvent.AddListener(component.ServerFinishedHandler);
                m_GameAppEvents.frameworkCloseEvent.AddListener(component.ApplicationCloseHandler);
                m_GameAppEvents.updateRemoteAssetEvent.AddListener(component.UpdateRemoteAssetHandler);

                LogGameComponentCreated();
            }
            else { }

            GameObject target = GameObject.Find("UIRoot");
            if (target != default)
            {
                UIRoot ui = target.GetComponent<UIRoot>();
                if (ui != default)
                {
                    LogUIRootCreated();
                    ui.AddAwakedHandler(UIRootAwaked);
                }
                else { }
            }
            else { }

            Loom loom = Loom.Current;
        }

        protected virtual void OnApplicationQuit()
        {
            BackgroundOperation(true);
            ShipDockApp.Instance.Clean();
        }

        protected virtual void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                BackgroundOperation(false);
            }
            else { }

            ShipDockConsts.NOTICE_APPLICATION_PAUSE.BroadcastWithParam(focus, true);
        }

        protected virtual void BackgroundOperation(bool isCleanVersionCache)
        {
#if !UNITY_EDITOR
            if (m_DevelopSubgroup.ApplyRemoteAssets)
            {
                m_DevelopSubgroup.remoteAssetVersions.CacheResVersion(isCleanVersionCache);
            }
            else { }
#endif
        }

        /// <summary>
        /// ShipDock 启动后的回调函数
        /// </summary>
        private void OnShipDockStart()
        {
            AssertShipDockGameInit(0);
            InitDataProxy();
            InitServerContainers();
        }

        /// <summary>
        /// 初始化IoC服务容器
        /// </summary>
        private void InitServerContainers()
        {
            ShipDockApp app = ShipDockApp.Instance;
            bool isOn = m_DevelopSubgroup.startUpIoC;//根据开发参数确定是否启动IoC模块
            Action[] onInited = isOn ? new Action[] { AddResolvableConfigs } : default;
            Action[] onFinished = isOn ? new Action[] { OnServersFinished } : default;
            IServer[] servers = isOn ? GetGameServers() : default;
            app.StartIoC(servers, MainThreadServerReady, onInited, onFinished);
        }

        /// <summary>
        /// 初始化数据代理
        /// </summary>
        private void InitDataProxy()
        {
            IDataProxy[] list = DataProxyWillInit();
            int max = list != default ? list.Length : 0;
            int[] proxyNames = new int[max];
            for (int i = 0; i < max; i++)
            {
                list[i].AddToWarehouse();
                proxyNames[i] = list[i].DataName;
            }

            InitProfile(ref proxyNames);
        }

        /// <summary>
        /// 添加服务容器解析器配置
        /// </summary>
        private void AddResolvableConfigs()
        {
            ShipDockApp app = ShipDockApp.Instance;

            IResolvableConfig[] resolvableConfs = GetServerConfigs();
            app.Servers.AddResolvableConfig(resolvableConfs);

            AssertShipDockGameInit(1);//Assert IoC
        }

        /// <summary>
        /// 服务容器初始化完成后在主线程上的回调
        /// </summary>
        private void MainThreadServerReady()
        {
            int configInitedNoticeName = m_DevelopSubgroup.configInitedNoticeName;
            if (m_DevelopSubgroup.applyIoCLoadConfigs && (configInitedNoticeName != int.MaxValue))
            {
                configInitedNoticeName.Add(OnConfigLoaded);//订阅一个配置初始化完成的消息
                ServerContainerSubgroup server = m_DevelopSubgroup.loadConfig;
                server.Delive<IConfigNotice>(OnGetConfigNotice);//加载配置
                //调用配置服务容器方法 Sample: "ServerConfig".Delive<IConfigNotice>("LoadConfig", "ConfigNotice", OnGetConfigNotice);
            }
            else
            {
                OnConfigLoaded(default);//未开启相关的本地化配置开发参数选项则直接跳到配置加载完成环节
            }
        }

        /// <summary>
        /// 获取配置的外派方法的装饰器方法
        /// </summary>
        /// <param name="target"></param>
        private void OnGetConfigNotice(ref IConfigNotice target)
        {
            target.SetNoticeName(m_DevelopSubgroup.configInitedNoticeName);
            target.ParamValue = m_DevelopSubgroup.configNames;
        }

        /// <summary>
        /// 配置加载完成消息处理函数
        /// </summary>
        /// <param name="param"></param>
        private void OnConfigLoaded(INoticeBase<int> param)
        {
            m_DevelopSubgroup.configInitedNoticeName.Remove(OnConfigLoaded);

            IConfigNotice notice = param as IConfigNotice;

            //从消息参数中获取配置数据的 Sample（ConfigClass 须实现 IConfig）:
            //Dictionary<int, ConfigClass> mapper = notice.GetConfigRaw<ConfigClass>("ConfigName");
            //var a = mapper[1].a;
            //var b = mapper[12].b;

            InitConfigs(ref notice);
            InitProfileData(ref notice);

            AssertShipDockGameInit(3);//Assert IoC
            AssertShipDockGameInit(4);//Assert IoC
            AssertShipDockGameInit(5);//Assert IoC

            if (notice != default)
            {
                notice.IsClearHolderList = true;
                notice.ToPool();
            }
            else { }

            if (m_DevelopSubgroup.ApplyRemoteAssets)
            {
                m_GameAppEvents.updateRemoteAssetEvent.Invoke();//触发远程资源对比事件，完成后调用 PreLoadAsset 以继续流程
            }
            else
            {
                PreloadAsset();
            }
        }

        /// <summary>
        /// 启动资源预加载
        /// </summary>
        public void PreloadAsset()
        {
            AssetsLoader assetsLoader = new AssetsLoader();
            int max = m_DevelopSubgroup.assetNamePreload.Length;
            if (max > 0)
            {
                assetsLoader.CompleteEvent.AddListener(OnPreloadComplete);
                assetsLoader.AddManifest(m_DevelopSubgroup.assetNameResData, m_DevelopSubgroup.applyManifestAutoPath);

                string item;
                for (int i = 0; i < max; i++)
                {
                    item = m_DevelopSubgroup.assetNamePreload[i];
                    assetsLoader.Add(item, true, m_DevelopSubgroup.applyManifestAutoPath);
                }
                assetsLoader.Load(out _);
            }
            else
            {
                OnPreloadComplete(true, assetsLoader);
            }
        }

        protected virtual void InitConfigs(ref IConfigNotice notice)
        {
            Locals locals = ShipDockApp.Instance.Locals;
            locals.SetLocalName(m_Locals);

            if (notice != default)
            {
                Dictionary<int, string> raw = new Dictionary<int, string>();
                m_GameAppEvents.getLocalsConfigItemEvent.Invoke(raw, notice);
                locals.SetLocal(raw);
            }
            else { }

            AssertShipDockGameInit(1);//Assert Configs
        }

        private void InitProfileData(ref IConfigNotice notice)
        {
            m_GameAppEvents.initProfileDataEvent?.Invoke(notice);

            AssertShipDockGameInit(2);//Assert Profile Data
        }

        /// <summary>
        /// 资源预加载完成
        /// </summary>
        /// <param name="successed"></param>
        /// <param name="target"></param>
        private void OnPreloadComplete(bool successed, AssetsLoader target)
        {
            target.Dispose();

            EnterGame();
        }

        /// <summary>
        /// 进入游戏
        /// </summary>
        private void EnterGame()
        {
            m_GameAppEvents.enterGameEvent?.Invoke();
        }

        /// <summary>
        /// 创建测试器
        /// </summary>
        private void CreateTesters()
        {
            m_GameAppEvents.createTestersEvent?.Invoke();
            AssertShipDockGameInit(int.MaxValue);
        }

        private T CommonEventInovker<T>(UnityEvent<IParamNotice<T>> commonEvent, bool applyPooling = false, T param = default)
        {
            IParamNotice<T> notice = applyPooling ? Pooling<ParamNotice<T>>.From() : new ParamNotice<T>();
            if (param != null)
            {
                notice.ParamValue = param;
            }
            else { }
            commonEvent?.Invoke(notice);

            T result = notice.ParamValue;
            if (applyPooling)
            {
                notice.ToPool();
            }
            else
            {
                notice.Dispose();
            }
            return result;
        }

        private IServer[] GetGameServers()
        {
            IServer[] servers = CommonEventInovker(m_GameAppEvents.getGameServersEvent);
            LogServersLength(ref servers);
            return servers;
        }

        private void OnServersFinished()
        {
            m_GameAppEvents.serversFinishedEvent?.Invoke();
            AssertShipDockGameInit(2);//Assert IoC
        }

        /// <summary>
        /// 初始化账户
        /// </summary>
        /// <param name="proxyNames"></param>
        private void InitProfile(ref int[] proxyNames)
        {
            CommonEventInovker(m_GameAppEvents.initProfileEvent, false, proxyNames);
        }

        /// <summary>
        /// 获取需要初始化的服务容器解析器配置
        /// </summary>
        /// <returns></returns>
        private IResolvableConfig[] GetServerConfigs()
        {
            IResolvableConfig[] serverConfigs = CommonEventInovker(m_GameAppEvents.getServerConfigsEvent);
            return serverConfigs;
        }

        /// <summary>
        /// 获取需要初始化的数据代理
        /// </summary>
        /// <returns></returns>
        private IDataProxy[] DataProxyWillInit()
        {
            IDataProxy[] result = CommonEventInovker(m_GameAppEvents.getDataProxyEvent);
            return result;
        }

        /// <summary>
        /// 检测本地偏好数据及资源的版本
        /// </summary>
        /// <param name="onClientDataChecked"></param>
        public void CheckPlayerPrefsVersion(Action afterPlayerPrefsDel = default, Action onPersistentResChecked = default, Action onClientDataChecked = default)
        {
            string value = m_DevelopSubgroup.playerPrefsVersion;
            string[] list = value.Split(StringUtils.DOT_CHAR);
            int allPrefsCode = int.Parse(list[0]);
            int privistionResCode = int.Parse(list[1]);
            int clientDataCode = int.Parse(list[2]);

            string identifier = Application.identifier;
            string allPrefsKey = identifier.Append("_allPrefs");
            string persistentResKey = identifier.Append("_persistentRes");
            string clientInfoKey = identifier.Append("_clientData");

            int codeCached;
            bool hasDelAllPrefs = false;

            if (PlayerPrefs.HasKey(allPrefsKey))
            {
                if (allPrefsCode > 0)
                {
                    codeCached = PlayerPrefs.GetInt(allPrefsKey);
                    hasDelAllPrefs = codeCached != allPrefsCode;//只要不同即可清空偏好

                    if (hasDelAllPrefs)
                    {
                        PlayerPrefs.DeleteAll();//清空偏好数据
                        afterPlayerPrefsDel?.Invoke();
                    }
                    else { }
                }
                else//小于0则忽略用户偏好的清除
                {
                    LogPlayerPrefsClearIgnore();
                }
            }
            else { }

            PlayerPrefs.SetInt(allPrefsKey, allPrefsCode);

            if (!hasDelAllPrefs)
            {
                if (PlayerPrefs.HasKey(clientInfoKey))
                {
                    codeCached = PlayerPrefs.GetInt(clientInfoKey);
                    if (codeCached < clientDataCode)
                    {
                        onClientDataChecked?.Invoke();//变更客户端信息
                    }
                    else { }
                }
                else { }

                if (PlayerPrefs.HasKey(persistentResKey))
                {
                    codeCached = PlayerPrefs.GetInt(persistentResKey);
                    if (codeCached < privistionResCode)
                    {
                        onPersistentResChecked?.Invoke();//处理持久化目录中的资源
                    }
                    else { }
                }
                else
                {
                    onPersistentResChecked?.Invoke();//处理持久化目录中的资源
                }

                PlayerPrefs.SetInt(clientInfoKey, clientDataCode);
                PlayerPrefs.SetInt(persistentResKey, privistionResCode);
            }
            else { }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogPlayerPrefsClearIgnore()
        {
            "log".Log("Player prefs clear ignored.");
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogShipDockAppClose()
        {
            "debug".Log("ShipDock close.");
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogGameComponentCreated()
        {
            "debug".Log("Game Component created..");
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogUIRootCreated()
        {
            "debug".Log("UI Root created..");
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogServersLength(ref IServer[] servers)
        {
            "log".Log(servers != default, servers != default ? "Servers count is ".Append(servers.Length.ToString()) : "Servers is empty..");
        }

        /// <summary>
        /// 断言测试框架的初始化流程是否正确
        /// </summary>
        /// <param name="step">断言步骤，不在预定值内时为构筑断言流程</param>
        [System.Diagnostics.Conditional("G_LOG")]
        private void AssertShipDockGameInit(int step)
        {
            #region 断言内容
            const string LOG = "log";
            const string ASSERT_GAME = "game";
            const string GAME_START = "Assert Game start.";
            const string SERVER_CONFIG_INIT = "ServerConfigInit";
            const string LOCALS_INITED = "LocalsInited";
            const string SERVER_INIT = "ServerInit";
            const string PROFILE_DATA_INITED = "ProfileDataInited";
            const string SERVER_FINISHED = "ServerFinished";
            #endregion

            switch (step)
            {
                default:
                    Queue<string> asserts = new Queue<string>();
                    asserts.Enqueue(GAME_START);

                    if (m_DevelopSubgroup.startUpIoC)
                    {
                        asserts.Enqueue(SERVER_CONFIG_INIT);
                        asserts.Enqueue(SERVER_INIT);
                        asserts.Enqueue(SERVER_FINISHED);
                    }
                    else { }

                    asserts.Enqueue(LOCALS_INITED);
                    asserts.Enqueue(PROFILE_DATA_INITED);

                    Tester.Instance.AddAsserter(ASSERT_GAME, false, asserts.ToArray());//构筑断言流程
                    break;

                case 0:
                    LOG.AssertLog(ASSERT_GAME, GAME_START);
                    break;
                case 1:
                    if (m_DevelopSubgroup.startUpIoC)
                    {
                        LOG.AssertLog(ASSERT_GAME, SERVER_CONFIG_INIT);
                    }
                    else
                    {
                        LOG.AssertLog(ASSERT_GAME, LOCALS_INITED);
                    }
                    break;
                case 2:
                    if (m_DevelopSubgroup.startUpIoC)
                    {
                        LOG.AssertLog(ASSERT_GAME, SERVER_INIT);
                    }
                    else
                    {
                        LOG.AssertLog(ASSERT_GAME, PROFILE_DATA_INITED);
                    }
                    break;
                case 3:
                    if (m_DevelopSubgroup.startUpIoC)
                    {
                        LOG.AssertLog(ASSERT_GAME, SERVER_FINISHED);
                    }
                    else { }
                    break;
                case 4:
                    if (m_DevelopSubgroup.startUpIoC)
                    {
                        LOG.AssertLog(ASSERT_GAME, LOCALS_INITED);
                    }
                    else { }
                    break;
                case 5:
                    if (m_DevelopSubgroup.startUpIoC)
                    {
                        LOG.AssertLog(ASSERT_GAME, PROFILE_DATA_INITED);
                    }
                    else { }
                    break;
            }
        }
    }
}