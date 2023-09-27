using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// 
    /// ��ϷӦ���������
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class ShipDockAppComponent : MonoBehaviour, IShipDockApp
    {
#if ULTIMATE
        /// <summary>�ͻ��˰汾</summary>
        protected ClientResVersion mClientVersions;
        /// <summary>�������浯��������ͬ����Դ���½��ȵȣ�</summary>
        protected IStartingUpdatePopup mStartingUpdatePopup;
#endif

        /// <summary>��Ϸģ�����</summary>
        protected ShipDockGame GameComponent { get; private set; }

        protected virtual void OnDestroy()
        {
            GameComponent = default;
#if ULTIMATE
            mClientVersions = default;
            mStartingUpdatePopup = default;
#endif
        }

        /// <summary>
        /// ��Ӧ�����������¼��Ĵ�����
        /// </summary>
        public virtual void CreateTestersHandler() { }

        /// <summary>
        /// ��Ӧ������Ϸ�¼��Ĵ�����
        /// </summary>
        public virtual void EnterGameHandler() { }

        /// <summary>
        /// ��Ӧ��ȡ���ݲ������������¼��Ĵ�����
        /// </summary>
        /// <param name="param">��Ϣ���󣬳��д�����ĸ������ݴ������</param>
        public virtual void GetDataProxyHandler(IParamNotice<IDataProxy[]> param) { }

        /// <summary>
        /// ��Ӧ��ȡ���Ա��ػ������¼��Ĵ�����
        /// </summary>
        /// <param name="raw">���ڱ������Ա��ػ�ӳ�����ݵĶ���</param>
        /// <param name="param">��Ϣ���󣬳������Ա��ػ�����������</param>
        public virtual void GetLocalsConfigItemHandler(Dictionary<int, string> raw, IConfigNotice param) { }

#if ULTIMATE
        /// <summary>
        /// ��Ӧ��ȡIoC�������������¼��Ĵ�����
        /// </summary>
        /// <param name="param">��Ϣ���󣬳��д�����ĸ���IoC������������</param>
        public virtual void GetGameServersHandler(IParamNotice<IServer[]> param) { }

        /// <summary>
        /// �����Ա��ػ����ö�ȡ���ݲ��������Ա��ػ�������ӳ��
        /// </summary>
        /// <param name="locals">���Ա��ػ���ǣ��������ֲ�ͬ�����ı��ػ�����</param>
        /// <param name="localsConfigName">���������Ա��ػ����õ������������ڴ�������Ϣ�����ж�ȡ���ػ���������</param>
        /// <param name="param">��Ϣ���󣬳������Ա��ػ�����������</param>
        /// <param name="raw">���ڱ������Ա��ػ�ӳ�����ݵĶ���</param>
        protected void SetDataFromLocalsConfig<T>(ref string locals, ref string localsConfigName, ref IConfigNotice param, ref Dictionary<int, string> raw) where T : IConfig, new()
        {
            Dictionary<int, T> configs = param.GetConfigRaw<T>(localsConfigName);

            int id;
            T item;
            KeyValuePair<int, T> pair;
            Dictionary<int, T>.Enumerator localsEnumer = configs.GetEnumerator();
            int max = configs.Count;
            for (int i = 0; i < max; i++)
            {
                localsEnumer.MoveNext();
                pair = localsEnumer.Current;
                item = pair.Value;
                id = item.GetID();//���õļ���
                raw[id] = GetLocalsDescription(ref locals, ref item);
            }
        }

        /// <summary>
        /// ��ȡ���Ա��ػ����õ�����
        /// </summary>
        /// <param name="locals">���Ա��ػ���ǣ��������ֲ�ͬ�����ı��ػ�����</param>
        /// <param name="item">���Ա��ػ����õ����ݶ���ʵ�� IConfig �ӿ�</param>
        /// <returns></returns>
        protected virtual string GetLocalsDescription<T>(ref string locals, ref T item) where T : IConfig, new()
        {
            return string.Empty;
        }

        /// <summary>
        /// ��Ӧ��ʼ����Ϸ�û������¼��Ĵ��������������û��������Դ�������ļ����ֵ�����
        /// </summary>
        /// <param name="param">��Ϣ���󣬳��д�����ĸ������ݴ������</param>
        public virtual void InitProfileDataHandler(IConfigNotice param) { }

        /// <summary>
        /// ��Ӧ��ȡ���������������������¼��Ĵ�����
        /// </summary>
        /// <param name="param">��Ϣ���󣬳��д�����ı�����������</param>
        public virtual void GetServerConfigsHandler(IParamNotice<IResolvableConfig[]> param) { }
#endif

        /// <summary>
        /// ��Ӧ��ʼ���û������¼��Ĵ��������������û��������Դ�����ݴ����ֵ�����
        /// </summary>
        /// <param name="param">��Ϣ���󣬳��п����õ������ݴ�����</param>
        public virtual void InitProfileHandler(IParamNotice<int[]> param) { }

        /// <summary>
        /// ��Ӧ��ʼ����ʼ�������������Ͱ󶨵��¼�
        /// 
        /// Sample: params.ParamValue.AddHolderType<ClassConfig>(SampleConsts.CONF_NAME);
        /// 
        /// </summary>
        /// <param name="param"></param>
        public virtual void InitConfigTypesHandler(IParamNotice<ConfigHelper> param) { }

        /// <summary>
        /// ��������������ɵĻص�����
        /// </summary>
        public virtual void ServerFinishedHandler() { }

        /// <summary>
        /// Ӧ�ó���ر��¼��Ĵ�����
        /// </summary>
        public virtual void ApplicationCloseHandler()
        {
            ShipDockApp.Instance.Clean();
        }

        /// <summary>
        /// ������Ϸģ���������
        /// </summary>
        /// <param name="comp"></param>
        public void SetShipDockGame(ShipDockGame comp)
        {
            GameComponent = comp;
        }

#if ULTIMATE
        protected virtual void StartILRuntime()
        {
            ShipDockApp.Instance.InitILRuntime<AppHotFixConfigBase>();
        }

        /// <summary>
        /// ���ڸ���Զ����Դ�¼��Ĵ�����
        /// </summary>
        public virtual void UpdateRemoteAssetHandler()
        {
            HotFixSubgroup hotFixSubgroup = GameComponent.HotFixSubgroup;
            if (hotFixSubgroup.applyILRuntime && !string.IsNullOrEmpty(hotFixSubgroup.initerNameInResource))
            {
#if ILRUNTIME
                StartILRuntime();
#endif
                GameObject mainBridge = Resources.Load<GameObject>(hotFixSubgroup.initerNameInResource);
                mainBridge = Instantiate(mainBridge);

                HotFixerComponent hotfixer = mainBridge.GetComponent<HotFixerComponent>();

                object ILCls = hotfixer.ShellBridge;
                string clsName = hotFixSubgroup.initerClassName;
                string initerMethodName = hotFixSubgroup.initerGameCompSetter;
                ShipDockGame paramValue = GameComponent;

                ILRuntimeUtils.InvokeMethodILR(ILCls, clsName, initerMethodName, 1, paramValue);
            }
            else
            {
                mClientVersions = GameComponent.DevelopSetting.remoteAssetVersions;
                if (mClientVersions != default)
                {
                    UIManager uis = ShipDockApp.Instance.UIs;
                    mStartingUpdatePopup = (IStartingUpdatePopup)uis.OpenResourceUI<MonoBehaviour>(GameComponent.DevelopSetting.resUpdatePopupPath);
                    mClientVersions.LoadRemoteVersion(OnLoadComplete, OnVersionInvalid, out _);//����Զ�˷���������Դ�汾
                }
                else
                {
                    AfterStartingLoadComplete();
                }
            }
        }

        /// <summary>
        /// ���Ӧ�ó���汾�Ƿ����
        /// </summary>
        private bool OnVersionInvalid()
        {
            string version = mClientVersions.RemoteAppVersion;
            string[] splits = version.Split('.');
            int v1 = int.Parse(splits[0]);
            int v2 = int.Parse(splits[1]);

            version = Application.version;
            splits = version.Split(StringUtils.DOT_CHAR);

            bool result = v1 > int.Parse(splits[0]) || v2 > int.Parse(splits[1]);
            if (result)
            {
                AfterVersionInvalid();
            }
            else { }
            return result;
        }

        /// <summary>
        /// ������Դ�汾��Ч�����
        /// </summary>
        protected virtual void AfterVersionInvalid() { }

        /// <summary>
        /// ��Դ�汾�ȶ����
        /// </summary>
        /// <param name="isComplete">�Ƿ����</param>
        /// <param name="progress">����</param>
        private void OnLoadComplete(bool isComplete, float progress)
        {
            if (isComplete)
            {
                AfterStartingLoadComplete();
            }
            else
            {
                StartingResLoadProgress();
            }
        }

        /// <summary>
        /// Զ����Դ���½���׷��
        /// </summary>
        protected virtual void StartingResLoadProgress()
        {
            mStartingUpdatePopup.Loaded = mClientVersions.UpdatingLoaded;
            mStartingUpdatePopup.LoadingCount = mClientVersions.UpdatingMax;
            mStartingUpdatePopup.LoadingUpdate();
        }

        /// <summary>
        /// ���Զ����Դ����
        /// </summary>
        protected virtual void AfterStartingLoadComplete()
        {
            if (mStartingUpdatePopup != default)
            {
                mStartingUpdatePopup.Close();
                mStartingUpdatePopup = default;
            }
            else { }

            GameComponent.PreloadAsset();//Ԥ����������Դ

        }

        /// <summary>
        /// ����
        /// </summary>
        public virtual void ReloadFrameworkScene()
        {
            GameComponent?.RestartAndReloadScene();
        }
#endif
    }
}