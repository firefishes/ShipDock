﻿using ShipDock.Notices;
using ShipDock.Server;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class ServerContainerSubgroup
    {
        [Tooltip("服务容器名")]
        public string serverName = ShipDockConsts.SERVER_CONFIG;
        [Tooltip("容器方法名")]
        public string deliverName = "LoadConfig";
        [Tooltip("调用容器方法所需的参数别名")]
        public string alias = "ConfigNotice";

        public void Delive<T>(ResolveDelegate<T> customResolver = default)
        {
            serverName.Delive(deliverName, alias, customResolver);//调用容器方法
        }
    }
}