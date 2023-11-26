#pragma warning disable

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ShipDock
{
    [RequireComponent(typeof(UpdatesComponent))]
    public abstract class ShipDockWechatGame : MonoBehaviour
    {
        [SerializeField, Tooltip("运行帧率")]
#if ODIN_INSPECTOR
        [TitleGroup("基本信息")]
        [LabelText("目标帧率"), Indent(1)]
#endif
        private int m_FrameRate = 40;

        [SerializeField, Tooltip("资源包主依赖名")]
#if ODIN_INSPECTOR
        [TitleGroup("资源包信息")]
        [LabelText("资源根目录名"), SuffixLabel("Assets/"), Indent(1)]
#endif
        private string m_ABManifestName = "res_data";

        [SerializeField, Tooltip("预加载的资源列表")]
#if ODIN_INSPECTOR
        [LabelText("预加载资源"), Indent(1)]
#endif
        private string[] m_AssetNamePreload;

        [SerializeField, Tooltip("配置文件所在的资源包名")]
#if ODIN_INSPECTOR
        [TitleGroup("配置信息")]
        [LabelText("资源包名"), Indent(1)]
#endif
        private string m_ConfigABName;

        /// <summary>配置文件辅助器</summary>
        protected ConfigHelper mConfigHelper;
        
        /// <summary>框架所在的场景名，用于关闭框架的时候卸载场景</summary>
        private string mFrameworkSceneName;
        /// <summary>所有能加载到的配置名</summary>
        private List<string> mLoadConfigNames;

        private void OnDestroy()
        {
            mConfigHelper?.Reclaim();
            mLoadConfigNames?.Clear();

            Framework.Instance.Clean();
            LogShipDockAppClose();
        }

        private void Awake()
        {
            CreateGame();
        }

        private void CreateGame()
        {
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
            Tester tester = Tester.Instance;
            tester.Init(new TesterBaseApp());

            UIManager uis = new UIManager();
            uis.SetRoot(root);

            //SoundEffects sounds = new SoundEffects();
            //sounds.Init();

            Framework framework = Framework.Instance;
            framework.OnUpdateComponentReady(UpdateComponentReadyHandler);
            
            IFrameworkUnit[] units = new IFrameworkUnit[]
            {
                framework.CreateUnitByBridge(Framework.UNIT_UI, uis),
                framework.CreateUnitByBridge(Framework.UNIT_TESTER, tester),
                framework.CreateUnitByBridge(Framework.UNIT_MSG_LOOPER, new MessageLooper()),
                framework.CreateUnitByBridge(Framework.UNIT_DATA, new DataWarehouse()),
                framework.CreateUnitByBridge(Framework.UNIT_AB, new AssetBundles()),
                framework.CreateUnitByBridge(Framework.UNIT_ASSET_POOL, new AssetsPooling()),
                framework.CreateUnitByBridge(Framework.UNIT_SOUND, new SoundEffects()),
                framework.CreateUnitByBridge(Framework.UNIT_FX, new Effects()),
            };
            framework.Init(m_FrameRate, units, OnShipDockStart);
        }

        private void UpdateComponentReadyHandler()
        {
            Framework framework = Framework.Instance;
            framework.RemoveUpdateComponentReady(UpdateComponentReadyHandler);

            SoundEffects soundEffects = Framework.UNIT_SOUND.Unit<SoundEffects>();
            soundEffects.Init();

            MessageLooper messageLooper = Framework.UNIT_MSG_LOOPER.Unit<MessageLooper>();
            messageLooper.Init();
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
        protected virtual IDataProxy[] DataProxyWillInit()
        {
            return default;
        }

        /// <summary>
        /// 初始化账户
        /// </summary>
        /// <param name="willRelateToProfileProxyNames"></param>
        protected virtual void InitProfile(ref int[] willRelateToProfileProxyNames)
        {
        }

        /// <summary>
        /// 配置加载完成消息处理函数
        /// </summary>
        /// <param name="param"></param>
        private void StartLoadConfig()
        {
            //新建配置辅助器
            mConfigHelper = new ConfigHelper
            {
                //设置配置文件所在的资源包名
                ConfigResABName = m_ConfigABName//"sample_configs"
            };

            //向配置辅助器添加需要绑定的配置类，此类为工具自动生成
            InitConfigTypes();

            //设置配置名
            mLoadConfigNames = mConfigHelper.HolderTypes;

            //根据配置名加载配置
            string[] list = mLoadConfigNames.ToArray();
            if (list.Length > 0)
            {
                mConfigHelper.Load(OnConfigLoaded, list);
            }
            else
            {
                PreloadAsset();
            }
        }

        protected virtual void InitConfigTypes()
        {
            //mConfigHelper.AddHolderType<ConfigDataClass>("sample_config");
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

        /// <summary>
        /// 启动资源预加载
        /// </summary>
        public void PreloadAsset()
        {
            AssetsLoader assetsLoader = new AssetsLoader();
            int max = m_AssetNamePreload.Length;
            if (max > 0)
            {
                assetsLoader.CompleteEvent.AddListener(OnPreloadComplete);
                assetsLoader.AddManifest(m_ABManifestName, false);

                string item;
                for (int i = 0; i < max; i++)
                {
                    item = m_AssetNamePreload[i];
                    assetsLoader.Add(item, true, false);
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

        /// <summary>
        /// 进入游戏
        /// </summary>
        protected virtual void EnterGame()
        {
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
