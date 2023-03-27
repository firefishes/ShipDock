﻿using System;

namespace ShipDock
{
    [Serializable]
    public class ClientLocalInfo
    {
        /// <summary>是否已初始化</summary>
        public bool isInited;
        /// <summary>是否已注册</summary>
        public bool isRegistered;
        /// <summary>用户id</summary>
        public string accountID;
        /// <summary>客户端id</summary>
        public string clientID;
        /// <summary>背景音量</summary>
        public float volumnBGM = 1f;
        /// <summary>音效音量</summary>
        public float volumnSound = 1f;
        /// <summary>构建版本次数，可用于确定是否删除本地缓存的数据或资源</summary>
        public int builds;

        public bool isNewUser;

        /// <summary>
        /// 检测后续增补的字段信息
        /// </summary>
        public virtual void CheckInfoPatch() { }
    }
}
