﻿namespace ShipDock.Notices
{
    public static class ShipDockConsts
    {
        /// <summary>应用启动</summary>
        public const int NOTICE_APPLICATION_STARTUP = -1000;
        /// <summary>应用关闭</summary>
        public const int NOTICE_APPLICATION_CLOSE = -1001;
        /// <summary>添加帧更新项</summary>
        public const int NOTICE_ADD_UPDATE = -1002;
        /// <summary>移除帧更新项</summary>
        public const int NOTICE_REMOVE_UPDATE = -1003;
        /// <summary>帧更新组件已就绪</summary>
        public const int NOTICE_FRAME_UPDATER_COMP_READY = -1004;
        /// <summary>添加以Unity主线程为驱动的帧更新项</summary>
        public const int NOTICE_ADD_SCENE_UPDATE = -1005;
        /// <summary>移除以Unity主线程为驱动的帧更新项</summary>
        public const int NOTICE_REMOVE_SCENE_UPDATE = -1006;
        /// <summary>以Unity主线程为驱动的帧更新组件已就绪</summary>
        public const int NOTICE_SCENE_UPDATE_READY = -1007;
        /// <summary>触发以Unity主线程为驱动的帧延迟</summary>
        public const int NOTICE_SCENE_CALL_LATE = -1008;
        /// <summary>触发配置预加载</summary>
        public const int NOTICE_CONFIG_PRELOADED = -1009;
        /// <summary>应用暂停</summary>
        public const int NOTICE_APPLICATION_PAUSE = -1010;
        /// <summary>显示帧率</summary>
        public const int NOTICE_FPS_SHOW = -1011;
        /// <summary>加入消息队列</summary>
        public const int NOTICE_MSG_ADD = -1012;
        /// <summary>处理消息队列</summary>
        public const int NOTICE_MSG_QUEUE = -1013;

        /// <summary>配置功能相关的服务容器名</summary>
        public const string SERVER_CONFIG = "ShipDockConfigServer";
    }
}
