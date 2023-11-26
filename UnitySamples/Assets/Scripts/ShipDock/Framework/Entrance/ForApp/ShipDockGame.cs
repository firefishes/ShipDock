#pragma warning disable

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ShipDock
{
    [RequireComponent(typeof(UpdatesComponent))]
    public class ShipDockGame : MonoBehaviour
    {
        [SerializeField, Tooltip("运行帧率")]
#if ODIN_INSPECTOR
        [TitleGroup("基本信息")]
        [LabelText("目标帧率"), Indent(1)]
#endif
        private int m_FrameRate = 40;

        [SerializeField, Tooltip("启用子线程执行 Ticks")]
#if ODIN_INSPECTOR
        [LabelText("执行子线程 Ticks"), Indent(1)]
#endif
        private bool m_ThreadTicksEnabled = true;

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

#if ODIN_INSPECTOR
        [SerializeField]
        [TitleGroup("事件")]
        [LabelText("显示事件"), Indent(1)]
        private bool m_ShowGameAppEvents;
#endif

        [SerializeField, Tooltip("游戏应用启动系列事件")]
#if ODIN_INSPECTOR
        [LabelText("详情"), ShowIf("@this.m_ShowGameAppEvents"), Indent(1)]
#endif
        private GameApplicationEvents m_GameAppEvents;

        /// <summary>
        /// 框架所在的场景名，用于关闭框架的时候卸载场景
        /// </summary>
        private string mFrameworkSceneName;
        /// <summary>
        /// 配置文件辅助器
        /// </summary>
        private ConfigHelper mConfigHelper;
        /// <summary>
        /// 所有能加载到的配置名
        /// </summary>
        private List<string> mLoadConfigNames;

        private void OnDestroy()
        {
            m_GameAppEvents.createTestersEvent.RemoveAllListeners();
            m_GameAppEvents.enterGameEvent.RemoveAllListeners();
            m_GameAppEvents.getDataProxyEvent.RemoveAllListeners();
            m_GameAppEvents.initProfileEvent.RemoveAllListeners();
            m_GameAppEvents.serversFinishedEvent.RemoveAllListeners();
            m_GameAppEvents.getLocalsConfigItemEvent.RemoveAllListeners();
            m_GameAppEvents.initConfigTypesEvent.RemoveAllListeners();
#if ULTIMATE
            m_GameAppEvents.getGameServersEvent.RemoveAllListeners();
            m_GameAppEvents.initProfileDataEvent.RemoveAllListeners();
            m_GameAppEvents.getServerConfigsEvent.RemoveAllListeners();
#endif

            ShipDockApp.Close();

            LogShipDockAppClose();

            m_GameAppEvents.frameworkCloseEvent.Invoke();
            m_GameAppEvents.frameworkCloseEvent.RemoveAllListeners();
        }

        private void Awake()
        {
            CreateGame();
        }

        private void CreateGame()
        {
            ShipDockAppComponent component = GetComponent<ShipDockAppComponent>();
            if (component != default)
            {
                //DontDestroyOnLoad(gameObject);
                component.SetShipDockGame(this);
                m_GameAppEvents.createTestersEvent.AddListener(component.CreateTestersHandler);
                m_GameAppEvents.enterGameEvent.AddListener(component.EnterGameHandler);
                m_GameAppEvents.getDataProxyEvent.AddListener(component.GetDataProxyHandler);
                m_GameAppEvents.initProfileEvent.AddListener(component.InitProfileHandler);
                m_GameAppEvents.serversFinishedEvent.AddListener(component.ServerFinishedHandler);
                m_GameAppEvents.frameworkCloseEvent.AddListener(component.ApplicationCloseHandler);
                m_GameAppEvents.getLocalsConfigItemEvent.AddListener(component.GetLocalsConfigItemHandler);
                m_GameAppEvents.initConfigTypesEvent.AddListener(component.InitConfigTypesHandler);
#if ULTIMATE
                m_GameAppEvents.updateRemoteAssetEvent.AddListener(component.UpdateRemoteAssetHandler);
                m_GameAppEvents.getGameServersEvent.AddListener(component.GetGameServersHandler);
                m_GameAppEvents.initProfileDataEvent.AddListener(component.InitProfileDataHandler);
                m_GameAppEvents.getServerConfigsEvent.AddListener(component.GetServerConfigsHandler);

                LogGameComponentCreated();
#endif
            }
            else { }

            mFrameworkSceneName = SceneManager.GetActiveScene().name;
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

        /// <summary>
        /// UI 根节点就绪事件处理函数
        /// </summary>
        /// <param name="root"></param>
        public void UIRootAwaked(IUIRoot root)
        {
#if RELEASE
            Debug.unityLogger.logEnabled = false;//编译发布版时关闭日志
#endif
            ShipDockAppSettings.threadTicksEnabled = m_ThreadTicksEnabled;

            ShipDockApp.Instance.InitUIRoot(root);
            ShipDockApp.StartUp(m_FrameRate, OnShipDockStart);
        }

        /// <summary>
        /// ShipDock 启动后的回调函数
        /// </summary>
        private void OnShipDockStart()
        {
#if ULTIMATE
            AssertShipDockGameInit(0);
#endif
            InitDataProxy();
#if ULTIMATE
            InitServerContainers();
#endif
            InitLocals();
            InitProfileData();
            StartLoadConfig();
        }

        /// <summary>
        /// 初始化数据代理
        /// </summary>
        private void InitDataProxy()
        {
            //添加配置数据代理
            ConfigData configData = new ConfigData(ShipDockConsts.D_CONFIGS);
            configData.AddToWarehouse();

            //添加其他数据代理
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
        /// 获取需要初始化的数据代理
        /// </summary>
        /// <returns></returns>
        private IDataProxy[] DataProxyWillInit()
        {
            IDataProxy[] result = CommonEventInovker(m_GameAppEvents.getDataProxyEvent);
            return result;
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
        /// 配置加载完成消息处理函数
        /// </summary>
        /// <param name="param"></param>
        private void StartLoadConfig()
        {
#if ULTIMATE
            m_DevelopSubgroup.configInitedNoticeName.Remove(OnConfigLoaded);
#endif

            //IConfigNotice notice = param as IConfigNotice;

            //从消息参数中获取配置数据的 Sample（ConfigClass 须实现 IConfig）:
            //Dictionary<int, ConfigClass> mapper = notice.GetConfigRaw<ConfigClass>("ConfigName");
            //var a = mapper[1].a;
            //var b = mapper[12].b;
#if ULTIMATE
            AssertShipDockGameInit(3);//Assert IoC
            AssertShipDockGameInit(4);//Assert IoC
            AssertShipDockGameInit(5);//Assert IoC
#endif

            //notice?.ToPool();

#if ULTIMATE
            bool applyRemoteAssets = m_DevelopSubgroup.ApplyRemoteAssets;
            AssetsLoader.IgnoreRemote = !applyRemoteAssets;

            if (applyRemoteAssets)
            {
                m_GameAppEvents.updateRemoteAssetEvent.Invoke();//触发远程资源对比事件，完成后调用 PreLoadAsset 以继续流程
            }
            else
            {
                PreloadAsset();
            }
#else
            //新建配置辅助器
            mConfigHelper = new ConfigHelper
            {
                //设置配置文件所在的资源包名
                ConfigResABName = m_DevelopSubgroup.configResABName //"sample_configs"
            };

            //向配置辅助器添加需要绑定的配置类，此类为工具自动生成
            CommonEventInovker(m_GameAppEvents.initConfigTypesEvent, true, mConfigHelper);
            //设置配置名
            mLoadConfigNames = mConfigHelper.HolderTypes;

            //根据配置名加载配置
            string[] list = mLoadConfigNames.ToArray();
            mConfigHelper.Load(OnConfigLoaded, list);
#endif
        }

        /// <summary>
        /// 配置加载完成
        /// 
        /// 之后在 ShipDockAppComponent 的子类中使用封装过的扩展方法从配置数据代理中获取配置
        /// 
        /// Sample in child of ShpDockAppComponent class when load config:
        /// 
        /// Dictionary<int, ClassConfig> confTable = SampleConsts.CONF_NAME.GetConfigTable<ClassConfig>();
        /// int id = 1;
        /// ClassConfig config = confTable[id];
        /// Debug.Log(config.question);
        /// 
        /// </summary>
        /// <param name="configResult"></param>
        private void OnConfigLoaded(ConfigsResult configResult)
        {
            //向配置数据代理中添加已加载的配置数据，并使用一个组名做对应
            ShipDockConsts.D_CONFIGS.SetConfigDataDefaultName();
            ShipDockConsts.CONF_GROUP_CONFIGS.SetConfigGroupDefaultName();

            ConfigData data = ShipDockConsts.D_CONFIGS.GetData<ConfigData>();
            data.AddConfigs(ShipDockConsts.CONF_GROUP_CONFIGS, configResult);

            PreloadAsset();
        }

        protected virtual void InitLocals()
        {
            Locals locals = ShipDockApp.Instance.Locals;
            locals.SetLocalName(m_Locals);

            //if (notice != default)
            //{
            //    Dictionary<int, string> raw = new Dictionary<int, string>();
            //    m_GameAppEvents.getLocalsConfigItemEvent.Invoke(raw, notice);
            //    locals.SetLocal(raw);
            //}
            //else { }

#if ULTIMATE
            AssertShipDockGameInit(1);//Assert Configs
#endif
        }

        private void InitProfileData()
        {
            //m_GameAppEvents.initProfileDataEvent?.Invoke();

#if ULTIMATE
            AssertShipDockGameInit(2);//Assert Profile Data
#endif
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
                bool applyManifestAutoPath = m_DevelopSubgroup.applyManifestAutoPath;
                assetsLoader.CompleteEvent.AddListener(OnPreloadComplete);
                assetsLoader.AddManifest(m_DevelopSubgroup.assetNameResData, applyManifestAutoPath);

                string item;
                for (int i = 0; i < max; i++)
                {
                    item = m_DevelopSubgroup.assetNamePreload[i];
                    assetsLoader.Add(item, true, applyManifestAutoPath);
                }
                assetsLoader.Load(out _);
            }
            else
            {
                OnPreloadComplete(true, assetsLoader);
            }
        }

        /// <summary>
        /// 资源预加载完成
        /// </summary>
        /// <param name="successed"></param>
        /// <param name="target"></param>
        private void OnPreloadComplete(bool successed, AssetsLoader target)
        {
            target.Reclaim();

            EnterGame();
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
                notice.Reclaim();
            }
            return result;
        }

        /// <summary>
        /// 进入游戏
        /// </summary>
        private void EnterGame()
        {
            m_GameAppEvents.enterGameEvent?.Invoke();
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogUIRootCreated()
        {
            "debug".Log("UI Root created..");
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogShipDockAppClose()
        {
            "debug".Log("ShipDock close.");
        }
    }
}
