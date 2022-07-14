#define LOG_CLIENT_VERSIONING

using ShipDock.Applications;
using ShipDock.Loader;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ShipDock.Versioning
{
    /// <summary>
    /// 
    /// 远程网关可选项
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    [Serializable]
    public class RemoteGatewayItem
    {
        /// <summary>网关名</summary>
#if ODIN_INSPECTOR
        [LabelText("网关名"), ShowIf("selected", true)]
#endif
        public string name;

        /// <summary>网关</summary>
#if ODIN_INSPECTOR
        [ShowIf("selected", true), Indent(1)]
#endif
        public string gateway;

        /// <summary>是否启用</summary>
#if ODIN_INSPECTOR
        [ToggleGroup("selected", "$name"), Indent(1)]
#endif
        public bool selected;
    }

    /// <summary>
    /// 
    /// 客户端安装包资源版本配置数据对象
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "ClientResVersions", menuName = "ShipDock : 客户端资源版本", order = 100)]
    public class ClientResVersion : ScriptableObject
    {
        [SerializeField, Header("客户端安装包默认的资源版本配置"), Tooltip("子服务名，用于在同一个客户端下获取不同服务器的资源"), HideInInspector()]
#if ODIN_INSPECTOR
        [LabelText("子服务")]
#endif
        private string m_RemoteName = string.Empty;//TODO 用于进一步扩展不同资源网关支持，未完成，暂时隐藏

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("客户端资源版本")]
#endif
        private ResDataVersion m_Res;

        #region 编辑器扩展相关
        /// <summary>本次资源打包变更项</summary>
#if UNITY_EDITOR
        [SerializeField, Header("以下仅生效于编辑器")]
#if ODIN_INSPECTOR
        [LabelText("本次资源打包变更项"), ShowIf("@this.m_ResChanged.Length > 0")]
#endif
        private ResVersion[] m_ResChanged;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("启用备选资源网关")]
#endif
        private bool m_ApplyCurrentResGateway;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("已选的资源网关"), ShowIf("m_ApplyCurrentResGateway", true)]
#endif
        private string m_ResRemoteGateway;

        /// <summary>可选的资源网关项</summary>
        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("资源网关备选项"), ShowIf("m_ApplyCurrentResGateway", true)]
#endif
        private RemoteGatewayItem[] m_OptionalGateways;

        /// <summary>其他客户端资源版本的预览文件</summary>
        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("其他资源版本预览文件")]
#endif
        private TextAsset m_Preview = default;

        private int mRemoteSelected = -1;
        private int mRemoteResVersion;
        private bool mIsShowPreview = false;
        private bool mAskForDeletePersistent = false;
        private ResDataVersion mBeforePreview;
        private AssetsLoader mRemoteResLoader;

#if ODIN_INSPECTOR
        [ShowIf("m_ApplyCurrentResGateway", true), Button(name: "应用 Gateway")]
        private void UpdateRemoteGatewaySelected()
        {
            mRemoteSelected = -1;
            int max = m_OptionalGateways.Length;
            for (int i = 0; i < max; i++)
            {
                if (m_OptionalGateways[i].selected)
                {
                    mRemoteSelected = i;
                    break;
                }
                else { }
            }
            max = m_OptionalGateways.Length;
            for (int i = 0; i < max; i++)
            {
                if (mRemoteSelected != i)
                {
                    m_OptionalGateways[i].selected = false;
                }
                else { }
            }
            if (mRemoteSelected != -1)
            {
                m_ResRemoteGateway = m_OptionalGateways[mRemoteSelected].gateway;
            }
            else { }
        }

        [Button(name: "预览其他"), ShowIf("@this.mIsShowPreview == false && m_Preview != null")]
        private void PreviewVersionsData()
        {
            if (m_Preview != default)
            {
                mBeforePreview = m_Res;
                mIsShowPreview = true;
                m_Res = JsonUtility.FromJson<ResDataVersion>(m_Preview.text);
            }
            else { }
        }

        [Button(name: "返回"), ShowIf("@this.mIsShowPreview == true")]
        private void ClosePreviewVersions()
        {
            mIsShowPreview = false;
            if (mBeforePreview != default)
            {
                m_Res = mBeforePreview;
            }
            else { }
        }

        [Button(name: "清空缓存资源"), ShowIf("@this.mAskForDeletePersistent == false")]
        private void WillDeletePersistent()
        {
            mAskForDeletePersistent = true;
        }

        [ButtonGroup("是否清空缓存资源？"), Button(name: "取消"), ShowIf("@this.mAskForDeletePersistent == true")]
        private void CancelDeletePersistent()
        {
            mAskForDeletePersistent = false;
        }

        [ButtonGroup("是否清空缓存资源？"), Button(name: "确定"), ShowIf("@this.mAskForDeletePersistent == true")]
        private void ConfirmDeletePersistent()
        {
            mAskForDeletePersistent = false;
            ClearPersistent();
        }
#endif

        /// <summary>
        /// 设置本次资源打包的变更
        /// </summary>
        /// <param name="resChanges"></param>
        public void SetChanges(ResVersion[] resChanges)
        {
            m_ResChanged = resChanges;
        }

        private void OnEnable()
        {
#if ODIN_INSPECTOR
            if (mIsShowPreview && 
                (mBeforePreview != default) && (m_Preview == default))
            {
                ClosePreviewVersions();
            }
            else { }
#endif
        }
#endif
        #endregion

        /// <summary>本地缓存的资源版本配置</summary>
        public ResDataVersion CachedVersion { get; private set; }
        /// <summary>单个资源更新完成的回调函数</summary>
        public Action<bool, float> UpdateHandler { get; private set; }
        /// <summary>单个资源更新完成的回调函数</summary>
        public Func<bool> VersionInvalidHandler { get; private set; }
        /// <summary>远端服务器上的应用版本号</summary>
        public string RemoteAppVersion { get; private set; }

        public int UpdatingLoaded
        {
            get
            {
                return CachedVersion != default ? CachedVersion.UpdatingLoaded : 0;
            }
        }

        public int UpdatingMax
        {
            get
            {
                return CachedVersion != default ? CachedVersion.UpdatingMax : 0;
            }
        }

        public string Source
        {
            get
            {
                return JsonUtility.ToJson(m_Res);
            }
            set
            {
                m_Res = JsonUtility.FromJson<ResDataVersion>(value);
                m_Res.resVersionType = ResDataVersionType.Client;
                SyncResGateway();
            }
        }

        public ResDataVersion Versions
        {
            get
            {
#if UNITY_EDITOR && ODIN_INSPECTOR
                ClosePreviewVersions();
#endif
                if (m_Res == default)
                {
                    m_Res = new ResDataVersion();
                }
                else { }

                if (m_Res.resVersionType != ResDataVersionType.Client)
                {
                    m_Res.resVersionType = ResDataVersionType.Client;
                }
                else { }

                SyncResGateway();
                return m_Res;
            }
        }

        public bool ApplyCurrentResGateway
        {
            get
            {
                return m_ApplyCurrentResGateway;
            }
        }

        public string ClientResVersionGateway()
        {
            return m_ResRemoteGateway;
        }

        private void SyncResGateway()
        {
            if (m_ApplyCurrentResGateway)
            {
                m_Res.res_gateway = m_ResRemoteGateway;
            }
            else
            {
                m_ResRemoteGateway = m_Res.res_gateway;
            }
        }

        /// <summary>
        /// 以客户端安装包默认的资源版本配置为基准创建一个新的本地缓存
        /// </summary>
        /// <param name="remoteVersions"></param>
        public void CreateVersionsCached(ref ResDataVersion remoteVersions)
        {
            string path = AppPaths.PersistentResDataRoot.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME);
            string data = FileOperater.ReadUTF8Text(path);

            ResDataVersion temp = JsonUtility.FromJson<ResDataVersion>(data);
            if (temp == default || temp.IsVersionsEmpty())
            {
#if LOG_CLIENT_VERSIONING
                "log".Log("Verision data is empty, will create new one.");
#endif
                temp = new ResDataVersion
                {
                    res_gateway = remoteVersions.res_gateway,
                    app_version = remoteVersions.app_version,
                    res_version = remoteVersions.res_version
                };

                ResDataVersion client = Versions;
                temp.CloneVersionsFrom(ref client);
            }
            else { }

            CachedVersion = temp;
            CachedVersion.resVersionType = ResDataVersionType.Cached;
        }

        /// <summary>
        /// 加载远端资源版本配置
        /// </summary>
        /// <param name="handler"></param>
        public void LoadRemoteVersion(Action<bool, float> updateHandler, Func<bool> versionInvalidHandler, out int statu)
        {
            statu = 0;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                statu = 1;//无网络
                updateHandler?.Invoke(true, 1f);
            }
            else
            {
                UpdateHandler = updateHandler;
                VersionInvalidHandler = versionInvalidHandler;

                Loader.Loader loader = new Loader.Loader();
                loader.CompleteEvent.AddListener(OnLoadComplete);
                loader.Load(Versions.res_gateway.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));
            }
        }

        private void OnLoadComplete(bool success, Loader.Loader target)
        {
            if (success)
            {
                string json = target.TextData;
                target.Dispose();

                ResDataVersion remoteVersions = JsonUtility.FromJson<ResDataVersion>(json);
                remoteVersions.resVersionType = ResDataVersionType.Remote;
                CreateVersionsCached(ref remoteVersions);

                RemoteAppVersion = remoteVersions.app_version;
                if (VersionInvalidHandler != default)
                {
                    bool flag = VersionInvalidHandler();
                    if (flag)
                    {
#if LOG_CLIENT_VERSIONING
                        "warning:There have a newer App inistaller.".Log();
#endif
                        return;
                    }
                    else { }
                }
                else { }

                mRemoteResVersion = remoteVersions.res_version;
                List<ResVersion> resUpdate = CachedVersion.CheckUpdates(Versions, ref remoteVersions);
                CachedVersion.WriteAsCached();

#if LOG_CLIENT_VERSIONING
                "log:Remote res update count is {0}".Log(resUpdate.Count.ToString());
                "log:UpdateHandler is null: {0}".Log((UpdateHandler == default).ToString());
#endif
                if (resUpdate.Count == 0)
                {
                    UpdateHandler?.Invoke(true, 1f);
                }
                else
                {
                    StartLoadPatchRes(ref resUpdate);
                }
            }
            else
            {
                "error: Load remote version failed, url is {0}".Log(target.Url);
            }
        }

        /// <summary>
        /// 更新资源补丁
        /// </summary>
        /// <param name="resUpdate"></param>
        private void StartLoadPatchRes(ref List<ResVersion> resUpdate)
        {
            if (mRemoteResLoader != default)
            {
                mRemoteResLoader.Dispose();
            }
            else { }

            int max = resUpdate.Count;
            if (max > 0)
            {
                mRemoteResLoader = new AssetsLoader();
                mRemoteResLoader.RemoteAssetUpdated.AddListener(OnResItemUpdated);

                ResVersion item;
                string url, resName;
                for (int i = 0; i < max; i++)
                {
                    item = resUpdate[i];
                    url = item.Url;
                    resName = item.name;
                    mRemoteResLoader.AddRemote(url, resName, true);
                }
                mRemoteResLoader.Load(out _);
            }
            else
            {
                UpdateHandler?.Invoke(true, 1f);
            }
        }

        /// <summary>
        /// 单个资源更新加载完成
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="name"></param>
        private void OnResItemUpdated(bool flag, string name)
        {
            if (CachedVersion != default)
            {
                CachedVersion.RemoveUpdate(name);

                float min = CachedVersion.UpdatingLoaded;
                float max = CachedVersion.UpdatingMax;
                bool isCompleted = min >= max;

                UpdateHandler?.Invoke(isCompleted, isCompleted ? 1f : min / max);

                bool isFinished = min >= max;
                if (isFinished)
                {
                    CachedVersion.res_version = mRemoteResVersion;
                    CachedVersion.WriteAsCached();
                    CachedVersion.ResetUpdatingsCount();
                }
                else { }
            }
            else { }
        }

        public void CacheResVersion(bool isExit)
        {
            if (CachedVersion != default)
            {
                if (CachedVersion.resVersionType == ResDataVersionType.Cached)
                {
                    CachedVersion?.WriteAsCached();
                }
                else { }

                if (isExit)
                {
                    CachedVersion.Clean();
                    CachedVersion = default;
                }
                else { }
            }
            else { }
        }

        public void ClearPersistent()
        {
            string path = AppPaths.PersistentResDataRoot;
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else { }
        }
    }

}