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
        [SerializeField, Tooltip("����֡��")]
#if ODIN_INSPECTOR
        [TitleGroup("������Ϣ")]
        [LabelText("Ŀ��֡��"), Indent(1)]
#endif
        private int m_FrameRate = 40;

        [SerializeField, Tooltip("�������߳�ִ�� Ticks")]
#if ODIN_INSPECTOR
        [LabelText("ִ�����߳� Ticks"), Indent(1)]
#endif
        private bool m_ThreadTicksEnabled = true;

        [SerializeField, Tooltip("�����Ա��ػ���ʶ")]
#if ODIN_INSPECTOR
        [LabelText("���Ա��ػ�����"), Indent(1)]
#endif
        private string m_Locals = "CN";

        [SerializeField, Tooltip("������������")]
#if ODIN_INSPECTOR
        [TitleGroup("��������")]
        [LabelText("����"), Indent(1)]
#endif
        private DevelopSubgroup m_DevelopSubgroup;

#if ODIN_INSPECTOR
        [SerializeField]
        [TitleGroup("�¼�")]
        [LabelText("��ʾ�¼�"), Indent(1)]
        private bool m_ShowGameAppEvents;
#endif

        [SerializeField, Tooltip("��ϷӦ������ϵ���¼�")]
#if ODIN_INSPECTOR
        [LabelText("����"), ShowIf("@this.m_ShowGameAppEvents"), Indent(1)]
#endif
        private GameApplicationEvents m_GameAppEvents;

        /// <summary>
        /// ������ڵĳ����������ڹرտ�ܵ�ʱ��ж�س���
        /// </summary>
        private string mFrameworkSceneName;
        /// <summary>
        /// �����ļ�������
        /// </summary>
        private ConfigHelper mConfigHelper;
        /// <summary>
        /// �����ܼ��ص���������
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
        /// UI ���ڵ�����¼�������
        /// </summary>
        /// <param name="root"></param>
        public void UIRootAwaked(IUIRoot root)
        {
#if RELEASE
            Debug.unityLogger.logEnabled = false;//���뷢����ʱ�ر���־
#endif
            ShipDockAppSettings.threadTicksEnabled = m_ThreadTicksEnabled;

            ShipDockApp.Instance.InitUIRoot(root);
            ShipDockApp.StartUp(m_FrameRate, OnShipDockStart);
        }

        /// <summary>
        /// ShipDock ������Ļص�����
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
        /// ��ʼ�����ݴ���
        /// </summary>
        private void InitDataProxy()
        {
            //����������ݴ���
            ConfigData configData = new ConfigData(ShipDockConsts.D_CONFIGS);
            configData.AddToWarehouse();

            //����������ݴ���
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
        /// ��ȡ��Ҫ��ʼ�������ݴ���
        /// </summary>
        /// <returns></returns>
        private IDataProxy[] DataProxyWillInit()
        {
            IDataProxy[] result = CommonEventInovker(m_GameAppEvents.getDataProxyEvent);
            return result;
        }

        /// <summary>
        /// ��ʼ���˻�
        /// </summary>
        /// <param name="proxyNames"></param>
        private void InitProfile(ref int[] proxyNames)
        {
            CommonEventInovker(m_GameAppEvents.initProfileEvent, false, proxyNames);
        }

        /// <summary>
        /// ���ü��������Ϣ������
        /// </summary>
        /// <param name="param"></param>
        private void StartLoadConfig()
        {
#if ULTIMATE
            m_DevelopSubgroup.configInitedNoticeName.Remove(OnConfigLoaded);
#endif

            //IConfigNotice notice = param as IConfigNotice;

            //����Ϣ�����л�ȡ�������ݵ� Sample��ConfigClass ��ʵ�� IConfig��:
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
                m_GameAppEvents.updateRemoteAssetEvent.Invoke();//����Զ����Դ�Ա��¼�����ɺ���� PreLoadAsset �Լ�������
            }
            else
            {
                PreloadAsset();
            }
#else
            //�½����ø�����
            mConfigHelper = new ConfigHelper
            {
                //���������ļ����ڵ���Դ����
                ConfigResABName = m_DevelopSubgroup.configResABName //"sample_configs"
            };

            //�����ø����������Ҫ�󶨵������࣬����Ϊ�����Զ�����
            CommonEventInovker(m_GameAppEvents.initConfigTypesEvent, true, mConfigHelper);
            //����������
            mLoadConfigNames = mConfigHelper.HolderTypes;

            //������������������
            string[] list = mLoadConfigNames.ToArray();
            mConfigHelper.Load(OnConfigLoaded, list);
#endif
        }

        /// <summary>
        /// ���ü������
        /// 
        /// ֮���� ShipDockAppComponent ��������ʹ�÷�װ������չ�������������ݴ����л�ȡ����
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
            //���������ݴ���������Ѽ��ص��������ݣ���ʹ��һ����������Ӧ
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
        /// ������ԴԤ����
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
        /// ��ԴԤ�������
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
        /// ������Ϸ
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
