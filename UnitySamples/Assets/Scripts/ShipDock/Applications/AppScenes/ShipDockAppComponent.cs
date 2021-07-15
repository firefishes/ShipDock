using ShipDock.Config;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Server;
using ShipDock.Tools;
using ShipDock.UI;
using ShipDock.Versioning;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// 游戏应用启动组件
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class ShipDockAppComponent : MonoBehaviour, IShipDockApp
    {
        /// <summary>客户端版本</summary>
        protected ClientResVersion mClientVersions;
        /// <summary>启动界面弹窗（用于同步资源更新进度等）</summary>
        protected IStartingUpdatePopup mStartingUpdatePopup;

        /// <summary>游戏模板组件</summary>
        protected ShipDockGame GameComponent { get; private set; }

        protected virtual void OnDestroy()
        {
            GameComponent = default;
            mClientVersions = default;
            mStartingUpdatePopup = default;
        }

        /// <summary>
        /// 响应创建测试器事件的处理函数
        /// </summary>
        public virtual void CreateTestersHandler() { }

        /// <summary>
        /// 响应进入游戏事件的处理函数
        /// </summary>
        public virtual void EnterGameHandler() { }

        /// <summary>
        /// 响应获取数据层各个代理对象事件的处理函数
        /// </summary>
        /// <param name="param">消息对象，持有待处理的各个数据代理对象</param>
        public virtual void GetDataProxyHandler(IParamNotice<IDataProxy[]> param) { }

        /// <summary>
        /// 响应获取IoC各个服务容器事件的处理函数
        /// </summary>
        /// <param name="param">消息对象，持有待处理的各个IoC服务容器对象</param>
        public virtual void GetGameServersHandler(IParamNotice<IServer[]> param) { }

        /// <summary>
        /// 响应获取语言本地化配置事件的处理函数
        /// </summary>
        /// <param name="raw">用于保存语言本地化映射数据的对象</param>
        /// <param name="param">消息对象，持有语言本地化的配置数据</param>
        public virtual void GetLocalsConfigItemHandler(Dictionary<int, string> raw, IConfigNotice param) { }

        /// <summary>
        /// 从语言本地化配置读取数据并设置语言本地化的数据映射
        /// </summary>
        /// <param name="locals">语言本地化标记，用于区分不同地区的本地化配置</param>
        /// <param name="localsConfigName">保存了语言本地化配置的配置名，用于从配置消息对象中读取本地化配置数据</param>
        /// <param name="param">消息对象，持有语言本地化的配置数据</param>
        /// <param name="raw">用于保存语言本地化映射数据的对象</param>
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
                id = item.GetID();//配置的键名
                raw[id] = GetLocalsDescription(ref locals, ref item);
            }
        }

        /// <summary>
        /// 获取语言本地化配置的内容
        /// </summary>
        /// <param name="locals">语言本地化标记，用于区分不同地区的本地化配置</param>
        /// <param name="item">语言本地化配置的数据对象，实现 IConfig 接口</param>
        /// <returns></returns>
        protected abstract string GetLocalsDescription<T>(ref string locals, ref T item) where T : IConfig, new();

        /// <summary>
        /// 响应初始化游戏用户数据事件的处理函数，用于向用户数据填充源自配置文件部分的内容
        /// </summary>
        /// <param name="param">消息对象，持有待处理的各个数据代理对象</param>
        public virtual void InitProfileDataHandler(IConfigNotice param) { }

        /// <summary>
        /// 响应获取服务容器别名解析配置事件的处理函数
        /// </summary>
        /// <param name="param">消息对象，持有待处理的别名解析配置</param>
        public virtual void GetServerConfigsHandler(IParamNotice<IResolvableConfig[]> param) { }

        /// <summary>
        /// 响应初始化用户对象事件的处理函数，用于向用户数据填充源自数据代理部分的内容
        /// </summary>
        /// <param name="param">消息对象，持有可能用到的数据代理名</param>
        public virtual void InitProfileHandler(IParamNotice<int[]> param) { }

        /// <summary>
        /// 服务容器处理完成的回调函数
        /// </summary>
        public virtual void ServerFinishedHandler() { }

        /// <summary>
        /// 相应应用程序关闭事件的处理函数
        /// </summary>
        public virtual void ApplicationCloseHandler()
        {
            ShipDockApp.Instance.Clean();
        }

        protected virtual void StartILRuntime()
        {
            ShipDockApp.Instance.InitILRuntime<AppHotFixConfigBase>();
        }

        /// <summary>
        /// 相应更新远程资源事件的处理函数
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
                ILRuntimeUtils.InvokeMethodILR(hotfixer.ShellBridge, hotFixSubgroup.initerClassName, hotFixSubgroup.initerGameCompSetter, 1, GameComponent);
            }
            else
            {
                mClientVersions = GameComponent.DevelopSetting.remoteAssetVersions;
                if (mClientVersions != default)
                {
                    UIManager uis = ShipDockApp.Instance.UIs;
                    mStartingUpdatePopup = (IStartingUpdatePopup)uis.OpenResourceUI<MonoBehaviour>(GameComponent.DevelopSetting.resUpdatePopupPath);
                    mClientVersions.LoadRemoteVersion(OnLoadComplete, OnVersionInvalid, out _);//加载远端服务器的资源版本
                }
                else
                {
                    AfterStartingLoadComplete();
                }
            }
        }

        /// <summary>
        /// 检测应用程序版本是否过期
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
        /// 处理资源版本无效的情况
        /// </summary>
        protected virtual void AfterVersionInvalid() { }

        /// <summary>
        /// 资源版本比对完成
        /// </summary>
        /// <param name="isComplete">是否完成</param>
        /// <param name="progress">进度</param>
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
        /// 远端资源更新进度追踪
        /// </summary>
        protected virtual void StartingResLoadProgress()
        {
            mStartingUpdatePopup.Loaded = mClientVersions.UpdatingLoaded;
            mStartingUpdatePopup.LoadingCount = mClientVersions.UpdatingMax;
            mStartingUpdatePopup.LoadingUpdate();
        }

        /// <summary>
        /// 完成远端资源更新
        /// </summary>
        protected virtual void AfterStartingLoadComplete()
        {
            if (mStartingUpdatePopup != default)
            {
                mStartingUpdatePopup.Close();
                mStartingUpdatePopup = default;
            }
            else { }

            GameComponent.PreloadAsset();//预加载首批资源

        }

        /// <summary>
        /// 设置游戏模板组件对象
        /// </summary>
        /// <param name="comp"></param>
        public void SetShipDockGame(ShipDockGame comp)
        {
            GameComponent = comp;
        }

        /// <summary>
        /// 重启
        /// </summary>
        public virtual void ReloadFrameworkScene()
        {
            GameComponent.RestartAndReloadScene();
        }
    }
}