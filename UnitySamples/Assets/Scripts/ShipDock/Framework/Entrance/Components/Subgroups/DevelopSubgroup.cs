using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock
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
#if ULTIMATE
        [Tooltip("用于确定是否删除本地缓存的数据或资源，版本号顺序：\r\n持久化数据, 私有目录资源, 账户数据")]
#if ODIN_INSPECTOR
        [LabelText("持久数据版本")]
#endif
        public string playerPrefsVersion = "0.0.0";
#endif

#if ODIN_INSPECTOR
        [SerializeField]
        [LabelText("已缓存的持久数据版本"), InlineButton("GetPlayerPrefsVersionCached", "更新")]//, Indent(1)]
        private string m_playerPrefsVersionCached;

        private void GetPlayerPrefsVersionCached()
        {
            string identifier = Application.identifier;
            string clientInfoKey = identifier.Append("_clientData");
            string persistentResKey = identifier.Append("_persistentRes");
            string allPrefsKey = identifier.Append("_allPrefs");

            int allPrefsCode = PlayerPrefs.HasKey(allPrefsKey) ? PlayerPrefs.GetInt(allPrefsKey) : 0;
            int privistionResCode = PlayerPrefs.HasKey(persistentResKey) ? PlayerPrefs.GetInt(persistentResKey) : 0;
            int clientDataCode = PlayerPrefs.HasKey(clientInfoKey) ? PlayerPrefs.GetInt(clientInfoKey) : 0;

            int[] list = new int[] { allPrefsCode, privistionResCode, clientDataCode };
            m_playerPrefsVersionCached = list.Joins(StringUtils.DOT).Append(" (App Version: ", Application.version, ")");
        }
#endif

        [Tooltip("资源总依赖清单所在的ab包文件")]
#if ODIN_INSPECTOR
        [LabelText("资源根目录名"), SuffixLabel("Assets/")]
#endif
        public string assetNameResData = "res_data";

#if ULTIMATE
        [Tooltip("是否启动 IoC 模块")]
#if ODIN_INSPECTOR
        [LabelText("启用 IoC")]
#endif
        public bool startUpIoC = false;

        [Tooltip("服务容器子组")]
#if ODIN_INSPECTOR
        [LabelText("服务容器子组 - 配置文件管理容器"), ShowIf("@this.startUpIoC == true")]
#endif
        public ServerContainerSubgroup loadConfig;
#endif

#if ULTIMATE
        [Tooltip("是否包含多语言本地化配置")]
#if ODIN_INSPECTOR
        [LabelText("启用IoC初始化配置"), ShowIf("@this.startUpIoC")]
#endif
        public bool applyIoCLoadConfigs = false;

        [Tooltip("配置初始化完成的消息名")]
#if ODIN_INSPECTOR
        [LabelText("消息名 - 配置预加载"), ShowIf("@this.applyIoCLoadConfigs == true && startUpIoC == true")]
#endif
        public int configInitedNoticeName = ShipDockConsts.NOTICE_CONFIG_PRELOADED;
#endif

        [Tooltip("加载资源时是否自动识别使用本地缓存还是Streaming目录")]
#if ODIN_INSPECTOR
#if ULTIMATE
        [LabelText("自动识别资源主依赖路径"), ShowIf("@this.remoteAssetVersions != null")]
#else
        [LabelText("自动识别资源主依赖路径"), ReadOnly]
#endif
#endif
        public bool applyManifestAutoPath = false;

#if ULTIMATE
        [Tooltip("远程资源客户端配置")]
#if ODIN_INSPECTOR
        [LabelText("客户端资源版本配置")]
#endif
        public ClientResVersion remoteAssetVersions;

        [Tooltip("位于 Assets/Resource 目录的首屏加载弹窗资源路径")]
#if ODIN_INSPECTOR
        [LabelText("资源热更提示弹窗所在路径"), SuffixLabel("资源位于 Assets/Resource/", true), ShowIf("@this.remoteAssetVersions != null")]
#endif
        public string resUpdatePopupPath = "res_update_popup/ResUpdatePopup";
#endif

        [Tooltip("AB资源包名")]
#if ODIN_INSPECTOR
        [LabelText("配置资源包名")]
#endif
        public string configResABName = "configs";

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

#if ODIN_INSPECTOR
        [LabelText("日志颜色")]
        public bool changeDebugSettings;
#endif

        [Tooltip("框架中日志模块启动前的日志颜色")]
#if ODIN_INSPECTOR
        [LabelText("Debug 日志"), ShowIf("@this.changeDebugSettings == true"), ColorPalette(ShowAlpha = false), Indent(1)]
#endif
        public Color logColorDefault;

        [Tooltip("框架中日志模块启动后的普通日志颜色")]
#if ODIN_INSPECTOR
        [LabelText("Normal 日志"), ShowIf("@this.changeDebugSettings == true"), ColorPalette(ShowAlpha = false), Indent(1)]
#endif
        public Color logColorDebug;

        [Tooltip("框架中日志模块启动后的警告日志颜色")]
#if ODIN_INSPECTOR
        [LabelText("Warning 日志"), ShowIf("@this.changeDebugSettings == true"), ColorPalette(ShowAlpha = false), Indent(1)]
#endif
        public Color logColorWarning;

        [Tooltip("框架中日志模块启动后的报错日志颜色")]
#if ODIN_INSPECTOR
        [LabelText("Error 日志"), ShowIf("@this.changeDebugSettings == true"), ColorPalette(ShowAlpha = false), Indent(1)]
#endif
        public Color logColorError;

        [Tooltip("框架中日志模块启动后的待开发日志颜色")]
#if ODIN_INSPECTOR
        [LabelText("TODO 日志"), ShowIf("@this.changeDebugSettings == true"), ColorPalette(ShowAlpha = false), Indent(1)]
#endif
        public Color logColorTodo;

#if ULTIMATE
        /// <summary>是否应用远程资源服务器</summary>
        public bool ApplyRemoteAssets
        {
            get
            {
                return remoteAssetVersions != default;
            }
        }
#endif
    }
}