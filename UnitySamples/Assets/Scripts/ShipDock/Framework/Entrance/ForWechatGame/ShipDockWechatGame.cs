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
        [SerializeField, Tooltip("����֡��")]
#if ODIN_INSPECTOR
        [TitleGroup("������Ϣ")]
        [LabelText("Ŀ��֡��"), Indent(1)]
#endif
        private int m_FrameRate = 40;

        [SerializeField, Tooltip("��Դ����������")]
#if ODIN_INSPECTOR
        [TitleGroup("��Դ����Ϣ")]
        [LabelText("��Դ��Ŀ¼��"), SuffixLabel("Assets/"), Indent(1)]
#endif
        private string m_ABManifestName = "res_data";

        [SerializeField, Tooltip("Ԥ���ص���Դ�б�")]
#if ODIN_INSPECTOR
        [LabelText("Ԥ������Դ"), Indent(1)]
#endif
        private string[] m_AssetNamePreload;

        [SerializeField, Tooltip("�����ļ����ڵ���Դ����")]
#if ODIN_INSPECTOR
        [TitleGroup("������Ϣ")]
        [LabelText("��Դ����"), Indent(1)]
#endif
        private string m_ConfigABName;

        /// <summary>�����ļ�������</summary>
        protected ConfigHelper mConfigHelper;
        
        /// <summary>������ڵĳ����������ڹرտ�ܵ�ʱ��ж�س���</summary>
        private string mFrameworkSceneName;
        /// <summary>�����ܼ��ص���������</summary>
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
        /// UI ���ڵ�����¼�������
        /// </summary>
        /// <param name="root"></param>
        public void UIRootAwaked(IUIRoot root)
        {
#if RELEASE
            Debug.unityLogger.logEnabled = false;//���뷢����ʱ�ر���־
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
        /// ShipDock ������Ļص�����
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
        protected virtual IDataProxy[] DataProxyWillInit()
        {
            return default;
        }

        /// <summary>
        /// ��ʼ���˻�
        /// </summary>
        /// <param name="willRelateToProfileProxyNames"></param>
        protected virtual void InitProfile(ref int[] willRelateToProfileProxyNames)
        {
        }

        /// <summary>
        /// ���ü��������Ϣ������
        /// </summary>
        /// <param name="param"></param>
        private void StartLoadConfig()
        {
            //�½����ø�����
            mConfigHelper = new ConfigHelper
            {
                //���������ļ����ڵ���Դ����
                ConfigResABName = m_ConfigABName//"sample_configs"
            };

            //�����ø����������Ҫ�󶨵������࣬����Ϊ�����Զ�����
            InitConfigTypes();

            //����������
            mLoadConfigNames = mConfigHelper.HolderTypes;

            //������������������
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

        /// <summary>
        /// ������ԴԤ����
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
        /// ��ԴԤ�������
        /// </summary>
        /// <param name="successed"></param>
        /// <param name="target"></param>
        private void OnPreloadComplete(bool successed, AssetsLoader target)
        {
            target.Reclaim();

            EnterGame();
        }

        /// <summary>
        /// ������Ϸ
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
