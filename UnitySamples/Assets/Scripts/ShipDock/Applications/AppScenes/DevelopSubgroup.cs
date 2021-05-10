using ShipDock.Notices;
using ShipDock.Versioning;
using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// 开发参数子组
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    [Serializable]
    public class DevelopSubgroup
    {
        [Tooltip("是否包含多语言本地化配置")]
#if ODIN_INSPECTOR
        [LabelText("删除本地的用户偏好数据")]
#endif
        public bool isDeletePlayerPref;

        [Tooltip("资源总依赖清单所在的ab包文件")]
#if ODIN_INSPECTOR
        [LabelText("资源根目录名"), SuffixLabel("Assets/", true)]
#endif 
        public string assetNameResData = "res_data";

        [Tooltip("是否启动IOC功能")]
#if ODIN_INSPECTOR
        [LabelText("启用IoC模块")]
#endif
        public bool startUpIOC = false;

        [Tooltip("服务容器子组")]
#if ODIN_INSPECTOR
        [LabelText("服务容器子组 - 配置文件管理容器"), ShowIf("@this.startUpIOC == true")]
#endif
        public ServerContainerSubgroup loadConfig;

        [Tooltip("是否包含多语言本地化配置")]
#if ODIN_INSPECTOR
        [LabelText("启用多语言本地化配置"), ShowIf("@this.startUpIOC")]
#endif
        public bool hasLocalsConfig = false;

        [Tooltip("配置初始化完成的消息名")]
#if ODIN_INSPECTOR
        [LabelText("消息名 - 配置预加载"), ShowIf("@this.hasLocalsConfig == true && startUpIOC == true")]
#endif 
        public int configInitedNoticeName = ShipDockConsts.NOTICE_CONFIG_PRELOADED;

        [Tooltip("远程资源客户端配置")]
#if ODIN_INSPECTOR
        [LabelText("客户端资源版本配置")]
#endif
        public ClientResVersion remoteAssetVersions;

        [Tooltip("位于 Assets/Resource 目录的首屏加载弹窗资源路径")]
#if ODIN_INSPECTOR
        [LabelText("资源热更提示弹窗所在路径"), SuffixLabel("Assets/Resource/", true), ShowIf("@this.remoteAssetVersions != null")]
#endif 
        public string resUpdatePopupPath = "res_update_popup/ResUpdatePopup";

        [Tooltip("加载资源时是否自动识别使用本地缓存还是Streaming目录")]
#if ODIN_INSPECTOR
        [LabelText("自动识别资源主依赖路径"), ShowIf("@this.remoteAssetVersions != null")]
#endif
        public bool applyManifestAutoPath = false;

        [Tooltip("预加载的配置列表")]
#if ODIN_INSPECTOR
        [LabelText("预加载配置")]
#endif
        public string[] configNames;

        [Tooltip("预加载的资源列表")]
#if ODIN_INSPECTOR
        [LabelText("预加载资源")]
#endif
        public string[] assetNamePreload;

        /// <summary>是否应用远程资源服务器</summary>
        public bool ApplyRemoteAssets
        {
            get
            {
                return remoteAssetVersions != default;
            }
        }
    }
}