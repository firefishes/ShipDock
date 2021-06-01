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
#if ODIN_INSPECTOR
        [TitleGroup("模板信息")]
#endif
        [SerializeField, Tooltip("运行帧率")]
#if ODIN_INSPECTOR
        [LabelText("帧率"), Indent(1)]
#endif 
        private int m_FrameRate = 40;

        [SerializeField, Tooltip("多语言本地化标识")]
#if ODIN_INSPECTOR
        [LabelText("语言本地化参数"), Indent(1), ShowIf("@this.m_DevelopSubgroup.hasLocalsConfig == true")]
#endif 
        private string m_Locals = "CN";

        [SerializeField, Tooltip("开发设置子组")]
#if ODIN_INSPECTOR
        [TitleGroup("开发参数"), LabelText("详情"), Indent(1)]
#endif
        private DevelopSubgroup m_DevelopSubgroup;

        [SerializeField, Tooltip("ILRuntime热更子组")]
#if ODIN_INSPECTOR
        [TitleGroup("ILRuntime热更子组"), LabelText("详情"), Indent(1)]
#endif 
        private HotFixSubgroup m_HotFixSubgroup;

        [SerializeField, Tooltip("游戏应用启动系列事件")]
#if ODIN_INSPECTOR
        [TitleGroup("事件"), LabelText("详情"), Indent(1)]
#endif 
        private GameApplicationEvents m_GameAppEvents;

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
            ShipDockApp.Instance.InitUIRoot(root);
#if RELEASE
            Debug.unityLogger.logEnabled = false;//编译发布版时关闭日志
#endif
            ShipDockApp.StartUp(m_FrameRate, OnShipDockStart);
        }

        private void OnDestroy()
        {
            m_GameAppEvents.frameworkCloseEvent.Invoke();

            m_GameAppEvents.frameworkCloseEvent.RemoveAllListeners();
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

            "debug".Log("ShipDock close.");
        }

        private void Awake()
        {
            if (m_DevelopSubgroup.isDeletePlayerPref)
            {
                PlayerPrefs.DeleteAll();
            }
            else { }

            CreateGame();
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

                "debug".Log("Game Component created..");
            }
            else { }

            GameObject target = GameObject.Find("UIRoot");
            if (target != default)
            {
                UIRoot ui = target.GetComponent<UIRoot>();
                if (ui != default)
                {
                    "debug".Log("UI Root created..");
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
        }

        protected virtual void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                BackgroundOperation(false);
            }
            else { }

            ParamNotice<bool> notice = Pooling<ParamNotice<bool>>.From();
            notice.ParamValue = focus;
            ShipDockConsts.NOTICE_APPLICATION_PAUSE.Broadcast(notice);
            Pooling<ParamNotice<bool>>.To(notice);
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
            CreateTesters();
            InitDataProxy();
            InitServerContainers();
        }

        /// <summary>
        /// 初始化IoC服务容器
        /// </summary>
        private void InitServerContainers()
        {
            ShipDockApp app = ShipDockApp.Instance;
            bool flag = m_DevelopSubgroup.startUpIOC;//根据开发参数确定是否启动IoC模块
            IServer[] servers = flag ? GetGameServers() : default;
            Action[] onInited = flag ? new Action[] { AddResolvableConfigs } : default;
            Action[] onFinished = flag ? new Action[] { OnServersFinished } : default;
            app.StartIOC(servers, MainThreadServerReady, onInited, onFinished);
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

            "log".AssertLog("game", "ServerInit");
        }

        /// <summary>
        /// 服务容器初始化完成后在主线程上的回调
        /// </summary>
        private void MainThreadServerReady()
        {
            "log".AssertLog("game", "ServerFinished");

            int configInitedNoticeName = m_DevelopSubgroup.configInitedNoticeName;
            if (m_DevelopSubgroup.hasLocalsConfig && (configInitedNoticeName != int.MaxValue))
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

            //从消息参数中获取配置数据的 Sample:
            //Dictionary<int, ConfigClass> mapper = notice.GetConfigRaw<ConfigClass>("ConfigName");
            //var a = mapper[1].a;
            //var b = mapper[12].b;

            InitConfigs(ref notice);
            InitProfileData(ref notice);

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

            "log".AssertLog("game", "LocalsInited");
        }

        private void InitProfileData(ref IConfigNotice notice)
        {
            m_GameAppEvents.initProfileDataEvent?.Invoke(notice);

            "log".AssertLog("game", "ProfileDataInited");
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

            "debug".AssertLog("game", "Game start.");
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
            "log".Log(servers != default, servers != default ? "Servers count is ".Append(servers.Length.ToString()) : "Servers is empty..");
            return servers;
        }

        private void OnServersFinished()
        {
            m_GameAppEvents.serversFinishedEvent?.Invoke();
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
            "log".AssertLog("game", "ServerInit");
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
    }
}