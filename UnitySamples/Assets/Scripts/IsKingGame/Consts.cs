using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public static class Consts
    {
        #region 业务层消息名
        /// <summary>开始战斗</summary>
        public const int N_START_BATTLE = 10000;
        /// <summary>开始情报收集</summary>
        public const int N_START_COLLECT_INTELLIGENTAL = 10001;
        /// <summary>情报收集完成</summary>
        public const int N_INTELLIGENTAL_FINISHED = 10001;
        /// <summary>情报收集中</summary>
        public const int N_INTELLIGENTAL_UPDATE = 10002;
        /// <summary>创建玩家手牌</summary>
        public const int N_PLAYER_CARD_GENERATE = 10003;
        /// <summary>AI 选择生成牌的玩家将领</summary>
        public const int N_AI_CHOOSE_PLAYER_CARD_HERO = 10004;
        /// <summary>获得将令</summary>
        public const int N_GAIN_GENERAL_ORDER = 10005;
        /// <summary>设置玩家获得将领出阵牌的概率</summary>
        public const int N_SET_GENERAL_INTO_BATTLE_RATIO = 10006;
        /// <summary>提交玩家AI结果</summary>
        public const int N_COMMIT_PLAYER_AI = 10007;
        #endregion

        #region 数据层消息
        /// <summary>我方情报收集中</summary>
        public const int DN_PLAYER_INTELLIGENTAL_UPDATE = 20000;
        /// <summary>我方情报收集完成</summary>
        public const int DN_PLAYER_INTELLIGENTAL_FINISHED = 20001;
        /// <summary>敌方情报收集完成</summary>
        public const int DN_ENEMY_INTELLIGENTAL_FINISHED = 20002;
        /// <summary>更新战斗数据</summary>
        public const int DN_BATTLE_DATA_UPDATE = 20003;
        /// <summary>获取将领配置</summary>
        public const int DN_GET_HEROS_ITEMS = 20004;
        #endregion

        #region 数据层代理名
        /// <summary>战斗数据</summary>
        public const int D_BATTLE = 0;
        /// <summary>玩家数据</summary>
        public const int D_PLAYER = 1;
        /// <summary>配置数据</summary>
        public const int D_CONFIGS = 2;
        #endregion

        #region 业务层模块名
        /// <summary>战斗模块</summary>
        public const int M_BATTLE = 0;
        /// <summary>战斗中的卡牌模块</summary>
        public const int M_BATTLE_CARD = 1;
        /// <summary>战斗中的将领模块</summary>
        public const int M_BATTLE_HERO = 2;
        /// <summary>战斗中的AI模块</summary>
        public const int M_BATTLE_AI = 3;
        #endregion

        #region 数据层字段名
        /// <summary>情报值</summary>
        public const int FN_INTELLIGENTIAL = 1000;
        /// <summary>情报最大值</summary>
        public const int FN_INTELLIGENTIAL_MAX = 1001;
        /// <summary>士气值</summary>
        public const int FN_MORALE = 1002;
        /// <summary>士气值</summary>
        public const int FN_INTELLIGENTIAL_DELTA = 1003;
        /// <summary>士气最大值</summary>
        public const int FN_MORALE_MAX = 1004;
        /// <summary>兵力</summary>
        public const int FN_TROOPS = 1005;
        /// <summary>兵力最大值</summary>
        public const int FN_TROOPS_MAX = 1006;

        /// <summary>卡牌类别</summary>
        public const int FN_CARD_TYPE = 2000;
        /// <summary>是否需要释放目标</summary>
        public const int FN_NEED_TARGET = 2001;
        /// <summary>释放目标类别</summary>
        public const int FN_TARGET_TYPE = 2002;

        /// <summary>体力</summary>
        public const int FN_HP = 3000;
        /// <summary>防御</summary>
        public const int FN_DEF = 3001;
        /// <summary>武力</summary>
        public const int FN_ATK = 3002;
        /// <summary>智力</summary>
        public const int FN_INTELLECT = 3003;
        /// <summary>兵法（Art of war）</summary>
        public const int FN_AOW = 3004;

        /// <summary>阵营</summary>
        public const int FN_CAMP = 4000;
        /// <summary>ID</summary>
        public const int FN_ID = 4001;
        /// <summary>名称</summary>
        public const int FN_NAME = 4002;
        /// <summary>等级</summary>
        public const int FN_LEVEL = 4003;

        public readonly static KeyValueList<int, string> FieldNames = new KeyValueList<int, string>()
        {
            [FN_MORALE] = "士气",
            [FN_TROOPS] = "兵力",
            [FN_HP] = "体力",
            [FN_DEF] = "防御",
            [FN_ATK] = "武力",
            [FN_INTELLECT] = "智力",
            [FN_AOW] = "兵法",
        };

        public readonly static KeyValueList<int, string> CommonNames = new KeyValueList<int, string>()
        {
            [FN_ID] = "ID",
            [FN_NAME] = "名称",
            [FN_CAMP] = "阵营",
        };

        public readonly static KeyValueList<int, string> SkillTypeValues = new KeyValueList<int, string>()
        {
            [SKIL_TYPE_CIC] = "主帅技",
            [SKIL_TYPE_GENERAL] = "武将技",
            [SKIL_TYPE_STRATAGEM] = "锦囊计",
            [SKIL_TYPE_COUNSELLOR] = "军师计",
        };

        public readonly static KeyValueList<int, string> SkillEffectTypeValues = new KeyValueList<int, string>()
        {
            [SKILL_EFFECT_TYPE_SUM] = "增加",
            [SKILL_EFFECT_TYPE_REDUCE] = "减少",
            [SKILL_EFFECT_TYPE_TIME] = "倍率",
        };

        #region 战场信息
        public readonly static List<int> FieldsBattleInfoFloat = new List<int>
        {
            FN_INTELLIGENTIAL,
            FN_INTELLIGENTIAL_MAX,
            FN_MORALE,
            FN_INTELLIGENTIAL_DELTA,
            FN_MORALE_MAX,
            FN_TROOPS,
            FN_TROOPS_MAX,
        };

        public readonly static List<int> FieldsBattleInfoInt = new List<int>
        {
            FN_TROOPS,
            FN_TROOPS_MAX,
        };
        #endregion

        #region 手牌信息
        public readonly static List<int> FieldsCardInfoInt = new List<int>()
        {
            FN_CARD_TYPE,
            FN_NEED_TARGET,
            FN_TARGET_TYPE,
        };
        #endregion

        #region 将领信息
        public readonly static List<int> FieldsHeroInfoInt = new List<int>
        {
            FN_ID,
            FN_CAMP,
            FN_LEVEL,
            FN_TROOPS,
        };

        public readonly static List<int> FieldsHeroInfoFloat = new List<int>
        {
            FN_HP,
            FN_ATK,
            FN_DEF,
            FN_INTELLECT,
            FN_AOW,
        };

        public readonly static List<int> FieldsHeroInfoString = new List<int>
        {
            FN_NAME,
        };
        #endregion

        #region 技能信息
        public readonly static List<int> FieldsSkillInfoInt = new List<int>
        {
            FN_ID,
            FN_NEED_TARGET,
            FN_TARGET_TYPE,
            FN_LEVEL,
        };
        #endregion
        #endregion

        #region 显示层字段名（资源包、UI预制体名、UI模块名）
        public const string KEY_AB_UI = "is_king_ui/";

        public readonly static string AB_UI_BATTLE = KEY_AB_UI.Append("battle");
        public const string UI_BATTLE = "UIBattle";
        public const string UIM_BATTLE = "UIMBattle";
        #endregion

        /// <summary>阵营包含的总将领数</summary>
        public const int CAMP_HERO_MAX = 8;
        /// <summary>玩家阵营</summary>
        public const int CAMP_PLAYER = 0;
        /// <summary>敌方阵营</summary>
        public const int CAMP_ENEMY = 1;

        public const int ITEM_HERO = 0;
        public const int ITEM_SKILL = 1;
        public const int ITEM_SKILL_EFFECT = 2;

        /// <summary>主帅</summary>
        public const int HERO_COMMANDER_IN_CHIEF = 0;
        /// <summary>军师</summary>
        public const int HERO_MILITARY_COUNSELLOR = 1;
        /// <summary>左先锋</summary>
        public const int HERO_LEFT_PIONEER = 2;
        /// <summary>右先锋</summary>
        public const int HERO_RIGHT_PIONEER = 3;
        /// <summary>左翼</summary>
        public const int HERO_LEFT_FLANK = 4;
        /// <summary>辎重队</summary>
        public const int HERO_BAGGAGE_TRANSPORT = 5;
        /// <summary>后军</summary>
        public const int HERO_REAR_ARMY = 6;

        /// <summary>主帅技</summary>
        public const int SKIL_TYPE_CIC = 100;
        /// <summary>武将技</summary>
        public const int SKIL_TYPE_GENERAL = 101;
        /// <summary>锦囊计</summary>
        public const int SKIL_TYPE_STRATAGEM = 102;
        /// <summary>军师计</summary>
        public const int SKIL_TYPE_COUNSELLOR = 103;

        /// <summary>增加</summary>
        public const int SKILL_EFFECT_TYPE_SUM = 200;
        /// <summary>减少</summary>
        public const int SKILL_EFFECT_TYPE_REDUCE = 201;
        /// <summary>倍率</summary>
        public const int SKILL_EFFECT_TYPE_TIME = 202;
    }
}