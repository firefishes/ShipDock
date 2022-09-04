using ShipDock.Applications;
using ShipDock.Config;
using System.Collections.Generic;

namespace Peace
{
    /// <summary>
    /// 常量类
    /// </summary>
    public static class Consts
    {
        #region 数据层代理名
        /// <summary>战斗数据</summary>
        public const int D_BATTLE = 0;
        /// <summary>玩家数据</summary>
        public const int D_PLAYER = 1;
        /// <summary>配置数据</summary>
        public const int D_CONFIGS = 2;
        /// <summary>军团数据</summary>
        public const int D_LEGION = 3;
        /// <summary>军团数据</summary>
        public const int D_TROOPS = 4;
        /// <summary>服务数据</summary>
        public const int D_SERVICE = 5;
        /// <summary>UI 数据</summary>
        public const int D_UI = 6;
        #endregion

        #region 模块名
        public const int M_MESSAGE = 2000;
        public const int M_BATTLE = 2001;
        public const int M_WORLD = 2002;
        public const int M_DATA = 2003;
        public const int M_SERVICE = 2004;
        public const int M_VIEW = 2005;
        #endregion

        #region 消息名
        //public const int N_MSG_ADD = 10000;
        //public const int N_MSG_QUEUE = 10001;
        #endregion

        #region 管道消息名
        public const int MSG_GAME_READY = 20000;
        public const int MSG_ENTER_BATTLE = 20001;
        public const int MSG_ADD_UPDATER = 20002;
        public const int MSG_RM_UPDATER = 20003;
        public const int MSG_S_INIT_PLAYER = 20004;
        #endregion

        #region 数据消息名
        public const int DN_NEW_GAME_CREATED = 30000;
        public const int DN_GAME_LOADED = 30001;
        public const int DN_NEW_GAME_SAVE = 30002;
        public const int DN_NEXT_UI_MODULAR = 30003;
        #endregion

        #region 配置
        public const int CONF_GROUP_CONFIGS = 1;

        public const string CONF_EQUIPMENT = "equipments";
        public const string CONF_ORGANIZATIONS = "organizations";
        #endregion

        #region 资源包名
        public const string AB_CONFIGS = "peace/configs";
        public const string AB_LOGOIN = "peace/ui_login";
        public const string AB_UI_MAIN = "peace/ui_main";
        public const string AB_UI_HEADQUARTERS = "peace/ui_headquarters";
        #endregion

        #region UI资源名
        public const string U_LOGIN = "UILogin";
        public const string U_LOADING = "UILoading";
        public const string U_HEADQUARTERS = "UIHeadquarters";
        #endregion

        #region UI模块名
        public const string UM_LOGIN = "UMLogin";
        public const string UM_HEADQUARTERS = "UMHeadquarters";
        public const string UM_LOADING = "UMLoading";
        #endregion

        #region 军衔
        /// <summary>少尉衔</summary>
        public const int ORG_TYPE_P = 1;
        /// <summary>中尉衔</summary>
        public const int ORG_TYPE_C = 2;
        /// <summary>上尉衔</summary>
        public const int ORG_TYPE_B = 3;
        /// <summary>少校衔</summary>
        public const int ORG_TYPE_R = 4;
        /// <summary>中校衔</summary>
        public const int ORG_TYPE_BR = 5;
        /// <summary>上校衔</summary>
        public const int ORG_TYPE_D = 6;
        /// <summary>准将衔</summary>
        public const int ORG_TYPE_A = 7;
        /// <summary>少将衔</summary>
        public const int ORG_TYPE_GA = 8;
        /// <summary>中将衔</summary>
        public const int ORG_TYPE_MGA = 9;
        /// <summary>上将衔</summary>
        public const int ORG_TYPE_LGA = 10;
        /// <summary>二等上将衔</summary>
        public const int ORG_TYPE_FA = 11;
        /// <summary>一等上将衔</summary>
        public const int ORG_TYPE_MFA = 12;
        /// <summary>特等上将衔</summary>
        public const int ORG_TYPE_LFA = 13;
        #endregion

        #region 部队类型
        /// <summary>常规部队</summary>
        public const int TROOP_TYPE_COMMON = 1;
        /// <summary>侦察部队</summary>
        public const int TROOP_TYPE_RECONNOITRE = 2;
        /// <summary>火力部队</summary>
        public const int TROOP_TYPE_FIREPOWER = 3;
        /// <summary>运输部队</summary>
        public const int TROOP_TYPE_TRANSPORT = 4;
        /// <summary>装甲部队</summary>
        public const int TROOP_TYPE_ARMOURED = 5;
        /// <summary>海军部队</summary>
        public const int TROOP_TYPE_NAVY = 6;
        /// <summary>空天部队</summary>
        public const int TROOP_TYPE_SPACE_AIR = 7;
        #endregion

        /// <summary>ECS世界上下文</summary>
        public const int ECS_CONTEXT_PEACE = 1;

        /// <summary>世界系统</summary>
        public const int SYSTEM_WORLD = 1;

        /// <summary>位移组件</summary>
        public const int COMP_MOVEMENT = 1;
    }
}