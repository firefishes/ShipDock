using ShipDock.Notices;
using ShipDock.Server;
using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.Applications
{
    [Serializable]
    public class ServerContainerSubgroup
    {
        [Tooltip("服务容器名")]
#if ODIN_INSPECTOR
        [LabelText("服务容器名")]
#endif 
        public string serverName = ShipDockConsts.SERVER_CONFIG;

        [Tooltip("容器方法名")]
#if ODIN_INSPECTOR
        [LabelText("外派方法名")]
#endif 
        public string deliverName = "LoadConfig";

        [Tooltip("用于从容器中解析一个对象")]
#if ODIN_INSPECTOR
        [LabelText("解析器别名")]
#endif 
        public string alias = "ConfigNotice";

        /// <summary>
        /// 调用子组已定义的服务容器中的外派方法
        /// </summary>
        /// <typeparam name="T">将要解析的对象类型</typeparam>
        /// <param name="customResolver">外部定制的解析器函数，用于加工解析出的对象</param>
        public void Delive<T>(ResolveDelegate<T> customResolver = default)
        {
            serverName.Delive(deliverName, alias, customResolver);//调用容器方法
        }
    }
}