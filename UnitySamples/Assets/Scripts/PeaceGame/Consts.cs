using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public static class Consts
    {
        #region 数据层代理名
        /// <summary>战斗数据</summary>
        public const int D_BATTLE = 0;
        /// <summary>玩家数据</summary>
        public const int D_PLAYER = 1;
        /// <summary>配置数据</summary>
        public const int D_CONFIGS = 2;
        #endregion

        public const int M_MESSAGE = 2000;
        public const int M_BATTLE = 2001;

        public const int N_MSG_ADD = 10000;
        public const int N_MSG_QUEUE = 10001;

        public const int MSG_ENTER_BATTLE = 20000;


    }
}