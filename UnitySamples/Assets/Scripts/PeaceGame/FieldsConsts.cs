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
        /// <summary>配置 ID</summary>
        public const int F_CONF_ID = 4;

        /// <summary>总部 ID</summary>
        //public const int F_HEADQUARTERS_ID = 5;
        /// <summary>军团指挥官 ID</summary>
        //public const int F_LEGION_OFFICER_ID = 6;

        /// <summary>信用点</summary>
        public const int F_CREDIT_POINT = 7;
        /// <summary>金属</summary>
        public const int F_METAL = 8;
        /// <summary>能源</summary>
        public const int F_ENERGY = 9;
        /// <summary>物资</summary>
        public const int F_SUPPLIES = 10;

        /// <summary>部队编号</summary>
        public const int F_TROOPS_SERIAL_NUM = 11;
        /// <summary>部队种类</summary>
        public const int F_TROOPS_TYPE = 12;
        /// <summary>部队长官 ID</summary>
        public const int F_TROOPS_OFFICER_ID = 13;
        /// <summary>战绩</summary>
        public const int F_TROOPS_ANNIHILATION = 14;
        /// <summary>兵力</summary>
        public const int F_TROOPS = 15;
        /// <summary>精锐</summary>
        public const int F_CRACK = 16;
        /// <summary>意志</summary>
        public const int F_WILL = 17;
        /// <summary>给养</summary>
        public const int F_PROVISIONS = 18;
        /// <summary>主装备 ID</summary>
        public const int F_MAIN_EQUIPMENT_ID = 19;

        /// <summary>部队编制类型</summary>
        public const int F_ORG_LEVEL_NAME = 20;
        /// <summary>权限值</summary>
        public const int F_ORGANIZATION_VALUE = 21;
        /// <summary>是否基础编制</summary>
        public const int F_IS_BASE_ORGANIZATION = 22;

        /// <summary>姓名</summary>
        public const int F_FULL_NAME = 23;
        /// <summary>性别</summary>
        public const int F_GENDER = 24;
        /// <summary>忠诚</summary>
        public const int F_LOYAL = 25;
        /// <summary>战功</summary>
        public const int F_ACHIEVEMENT = 26;
        /// <summary>军衔</summary>
        //public const int F_MILITARY_RANK = 27;
        /// <summary>军职</summary>
        //public const int F_STRIPES = 28;
        /// <summary>指挥权</summary>
        //public const int F_COMMAND_RIGHT = 29;

        /// <summary>指挥</summary>
        public const int F_ROLE_COMMAND = 30;
        /// <summary>思维</summary>
        public const int F_ROLE_THINKING = 31;
        /// <summary>情报</summary>
        public const int F_ROLE_INFORMATION = 32;
        /// <summary>科学</summary>
        public const int F_ROLE_SCIENTIFIC = 33;
        /// <summary>交际</summary>
        public const int F_ROLE_SOCIAL = 34;

        /// <summary>破坏</summary>
        public const int F_RUIN = 35;
        /// <summary>杀伤</summary>
        public const int F_KILL = 36;
        /// <summary>防空/summary>
        public const int F_AIR_DEF = 37;
        /// <summary>强化/summary>
        public const int F_INTENSIFY = 38;
        /// <summary>电子战/summary>
        public const int F_ECM_WAR = 39;
        /// <summary>运载/summary>
        public const int F_TRANSPORT = 40;
        /// <summary>科技/summary>
        public const int F_SCIENCE_TECH = 41;

        /// <summary>隐形/summary>
        public const int F_INVISIBLE = 42;
        /// <summary>扫描/summary>
        public const int F_SCAN = 43;
        /// <summary>探测/summary>
        public const int F_PROBE = 44;
        /// <summary>视野/summary>
        public const int F_VISION = 45;
        /// <summary>视野/summary>
        public const int F_STORAGE = 46;
        /// <summary>能耗/summary>
        public const int F_ENERY_CONSUMPTION = 47;

        /// <summary>建造时间/summary>
        public const int F_BUILD_TIME = 48;
        /// <summary>总价值/summary>
        public const int F_TOTAL_VALUE = 49;
        /// <summary>重建次数/summary>
        public const int F_REBUILD_COUNT = 50;

        /// <summary>商业/summary>
        public const int F_BUSINESS = 51;
        /// <summary>矿业/summary>
        public const int F_MINING = 52;
        /// <summary>工业/summary>
        public const int F_INDUSTRY = 53;
        /// <summary>防御/summary>
        public const int F_DEFENCE = 54;
        /// <summary>纵深/summary>
        public const int F_DEPTH = 55;
        /// <summary>地形/summary>
        public const int F_LANDFORM = 56;

        /// <summary>军团威望/summary>
        public const int F_PRESTIGE = 57;
        /// <summary>耐久/summary>
        public const int F_STAMINA = 58;
        /// <summary>精密/summary>
        public const int F_PRECISE = 59;

        #region 基础信息字段
        public readonly static List<int> IntFieldsBase = new List<int>
        {
            F_ID,
            F_S_ID,
            F_CONF_ID,
        };

        public readonly static List<int> StringFieldsBase = new List<int>
        {
            F_NAME,
        };
        #endregion

        #region 军团信息字段
        public readonly static List<int> IntFieldsLegion = new List<int>
        {
            F_PRESTIGE,
            F_TROOPS,
        };
        #endregion

        #region 资源信息字段
        public readonly static List<int> IntFieldsResources = new List<int>
        {
            F_CREDIT_POINT,
            F_METAL,
            F_ENERGY,
            F_SUPPLIES,
        };
        #endregion

        #region 部队信息字段
        public readonly static List<int> IntFieldsTroops = new List<int>
        {
            F_TROOPS_SERIAL_NUM,
            F_TROOPS_TYPE,
            F_TROOPS_OFFICER_ID,
            F_TROOPS_ANNIHILATION,
            F_TROOPS,
            F_CRACK,
            F_WILL,
            F_PROVISIONS,
            F_MAIN_EQUIPMENT_ID,
            F_STAMINA,
        };
        #endregion

        #region 编制信息字段
        public readonly static List<int> IntFieldsOrganization = new List<int>
        {
            F_ORGANIZATION_VALUE,
            F_IS_BASE_ORGANIZATION,
        };

        public readonly static List<int> StringFieldsOrganization = new List<int>
        {
            F_ORG_LEVEL_NAME,
        };
        #endregion

        #region 军官信息字段
        public readonly static List<int> IntFieldsOfficer = new List<int>
        {
            F_GENDER,
            F_LOYAL,
            F_ACHIEVEMENT,
        };

        public readonly static List<int> StringFieldsOfficer = new List<int>
        {
            F_FULL_NAME,
        };
        #endregion

        #region 人物属性信息字段
        public readonly static List<int> IntFieldsRole = new List<int>
        {
            F_ROLE_COMMAND,
            F_ROLE_THINKING,
            F_ROLE_INFORMATION,
            F_ROLE_SCIENTIFIC,
            F_ROLE_SOCIAL,
        };
        #endregion

        #region 装备属性信息字段
        public readonly static List<int> IntFieldsEquipment = new List<int>
        {
            F_RUIN,
            F_KILL,
            F_AIR_DEF,
            F_INTENSIFY,
            F_ECM_WAR,
            F_TRANSPORT,
            F_SCIENCE_TECH,
        };
        #endregion

        #region 据点属性信息字段
        public readonly static List<int> IntFieldsFortifiedPoint = new List<int>
        {
            F_BUSINESS,
            F_MINING,
            F_INDUSTRY,
            F_DEFENCE,
            F_DEPTH,
            F_LANDFORM,
        };
        #endregion
    }

}
