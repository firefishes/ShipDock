using ShipDock.Datas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    /// <summary>
    /// 属性字段常量
    /// </summary>
    public static class FieldsConsts
    {
        /// <summary>基于客户端的 ID</summary>
        public const int F_ID = 1;
        /// <summary>基于服务端的 ID</summary>
        public const int F_S_ID = 2;
        /// <summary>名称</summary>
        public const int F_NAME = 3;

        /// <summary>总部 ID</summary>
        public const int F_HEADQUARTERS_ID = 4;

        /// <summary>信用点</summary>
        public const int F_CREDIT_POINT = 5;
        /// <summary>金属</summary>
        public const int F_METAL = 6;
        /// <summary>能源</summary>
        public const int F_ENERGY = 7;
        /// <summary>物资</summary>
        public const int F_SUPPLIES = 8;

        /// <summary>军团指挥官 ID</summary>
        public const int F_LEGION_COMMANDER_ID = 9;

        /// <summary>忠诚</summary>
        public const int F_LOYAL = 10;
        /// <summary>战功</summary>
        public const int F_ACHIEVEMENT = 11;
        /// <summary>军衔</summary>
        public const int F_MILITARY_RANK = 12;
        /// <summary>军职</summary>
        public const int F_STRIPES = 13;

        /// <summary>指挥</summary>
        public const int F_ROLE_COMMAND = 14;
        /// <summary>思维</summary>
        public const int F_ROLE_THINKING = 15;
        /// <summary>情报</summary>
        public const int F_ROLE_INFORMATION = 16;
        /// <summary>科学</summary>
        public const int F_ROLE_SCIENTIFIC = 17;
        /// <summary>交际</summary>
        public const int F_ROLE_SOCIAL = 18;

        /// <summary>破坏</summary>
        public const int F_EQUIPMENT_RUIN = 18;
        /// <summary>杀伤</summary>
        public const int F_EQUIPMENT_KILL = 19;
        /// <summary>防空/summary>
        public const int F_EQUIPMENT_AIR_DEF = 20;
        /// <summary>强化/summary>
        public const int F_EQUIPMENT_INTENSIFY = 21;
        /// <summary>电子战/summary>
        public const int F_EQUIPMENT_ECM_WAR = 22;
        /// <summary>运载/summary>
        public const int F_EQUIPMENT_TRANSPORT = 24;
        /// <summary>科技/summary>
        public const int F_EQUIPMENT_SCIENCE_TECH = 25;
    }

}
