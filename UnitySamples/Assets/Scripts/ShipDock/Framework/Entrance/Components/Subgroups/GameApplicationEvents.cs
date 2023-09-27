using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock
{

    [Serializable]
    internal class GetDataProxyEvent : UnityEvent<IParamNotice<IDataProxy[]>> { }

    [Serializable]
    internal class GetLocalsConfigItemNotice : UnityEvent<Dictionary<int, string>, IConfigNotice> { }

    [Serializable]
    internal class InitConfigTypesEvent : UnityEvent<IParamNotice<ConfigHelper>> { }

#if ULTIMATE
    [Serializable]
    internal class InitProfileDataEvent : UnityEvent<IConfigNotice> { }

    [Serializable]
    internal class GetServerConfigsEvent : UnityEvent<IParamNotice<IResolvableConfig[]>> { }
    
    [Serializable]
    internal class GetGameServersEvent : UnityEvent<IParamNotice<IServer[]>> { }

    [SerializeField, Header("获取游戏服务容器事件")]
#if ODIN_INSPECTOR
    [Indent(1)]
#endif
    internal GetGameServersEvent getGameServersEvent = new GetGameServersEvent();

    [SerializeField, Header("获取服务容器解析配置事件")]
#if ODIN_INSPECTOR
    [Indent(1)]
#endif
    internal GetServerConfigsEvent getServerConfigsEvent = new GetServerConfigsEvent();

    [SerializeField, Header("用户数据初始化事件")]
#if ODIN_INSPECTOR
    [Indent(1)]
#endif
    internal InitProfileDataEvent initProfileDataEvent = new InitProfileDataEvent();
#endif

    [Serializable]
    internal class InitProfileEvent : UnityEvent<IParamNotice<int[]>> { }

    [Serializable]
    internal class UpdateRemoteAssetEvent : UnityEvent { }

    [Serializable]
    internal class ShipDockCloseEvent : UnityEvent { }

    [Serializable]
    public class GameApplicationEvents
    {
        [SerializeField, Header("创建测试驱动器事件")]
#if ODIN_INSPECTOR
        [Indent(1)]
#endif
        internal UnityEvent createTestersEvent = new UnityEvent();

        [SerializeField, Header("服务容器就绪事件")]
#if ODIN_INSPECTOR
        [Indent(1)]
#endif
        internal UnityEvent serversFinishedEvent = new UnityEvent();

        [SerializeField, Header("用户对象初始化事件")]
#if ODIN_INSPECTOR
        [Indent(1)]
#endif
        internal InitProfileEvent initProfileEvent = new InitProfileEvent();

        [SerializeField, Header("游戏进入事件")]
#if ODIN_INSPECTOR
        [Indent(1)]
#endif
        internal UnityEvent enterGameEvent = new UnityEvent();

        [SerializeField, Header("数据代理初始化事件")]
#if ODIN_INSPECTOR
        [Indent(1)]
#endif
        internal GetDataProxyEvent getDataProxyEvent = new GetDataProxyEvent();

        [SerializeField, Header("获取多语言本地化配置项事件，用于对多语言映射数据赋值")]
#if ODIN_INSPECTOR
    [Indent(1)]
#endif
        internal GetLocalsConfigItemNotice getLocalsConfigItemEvent = new GetLocalsConfigItemNotice();

        [SerializeField, Header("初始化配置数据类型绑定事件")]
#if ODIN_INSPECTOR
        [Indent(1)]
#endif
        internal InitConfigTypesEvent initConfigTypesEvent = new InitConfigTypesEvent();


        [SerializeField, Header("更新远程资源事件")]
#if ODIN_INSPECTOR
        [Indent(1)]
#endif
        internal UpdateRemoteAssetEvent updateRemoteAssetEvent = new UpdateRemoteAssetEvent();

        [SerializeField, Header("框架关闭事件")]
#if ODIN_INSPECTOR
        [Indent(1)]
#endif
        internal ShipDockCloseEvent frameworkCloseEvent = new ShipDockCloseEvent();
    }

}