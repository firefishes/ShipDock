using LitJson;
using System.Collections.Generic;

namespace IsKing
{
    public class HeroFields : DataInfo
    {
        /// <summary>
        /// 从JSON源数据转换为数据项
        /// </summary>
        /// <param name="item"></param>
        /// <param name="data"></param>
        public static void FillFromJSON(ref JsonData item, out HeroItem data)
        {
            data = new HeroItem
            {
                id = int.Parse(item["id"].ToString()),
                name = item["name"].ToString(),
                level = int.Parse(item["level"].ToString()),
                hp = double.Parse(item["hp"].ToString()),
                atk = double.Parse(item["atk"].ToString()),
                def = double.Parse(item["def"].ToString()),
                intellect = double.Parse(item["intellect"].ToString()),
                aow = double.Parse(item["aow"].ToString()),
                troops = int.Parse(item["troops"].ToString()),

                skillCIC = int.Parse(item["skillCIC"].ToString()),
                skillCounsellor = int.Parse(item["skillCounsellor"].ToString()),
            };

            JsonData list = item["skillGeneral"];

            int n = list.Count;
            data.skillGeneral = new int[n];
            for (int j = 0; j < n; j++)
            {
                data.skillGeneral[j] = int.Parse(list[j].ToString());
            }
        }

        private HeroItem mHeroItem;
        private SkillFields mSkillCIC;

        public override List<int> FloatFieldNames { get; protected set; } = Consts.FieldsHeroInfoFloat;
        public override List<int> IntFieldNames { get; protected set; } = Consts.FieldsHeroInfoInt;
        public override List<int> StringFieldNames { get; protected set; } = Consts.FieldsHeroInfoString;

        public int SkillCIC { get; private set; }
        public int SkillCounsellor { get; private set; }
        public int[] SkillGeneral { get; private set; }

        public HeroFields() { }

        /// <summary>
        /// 从数据项（配置、本地缓存等）转换为字段数据
        /// </summary>
        /// <param name="heroItem"></param>
        public void InitFormItem(ref HeroItem heroItem)
        {
            "error".Log(heroItem == default, "Hero item is null");

            mHeroItem = heroItem;

            mStringFieldSource = new List<string> { heroItem.name, };

            mIntFieldSource = new List<int> {
                heroItem.id,
                Consts.CAMP_PLAYER,
                heroItem.level,
                heroItem.troops,
            };

            mFloatFieldSource = new List<float> {
                (float)heroItem.hp,
                (float)heroItem.atk,
                (float)heroItem.def,
                (float)heroItem.intellect,
                (float)heroItem.aow,
            };

            FillValues();
            LogFields();

            SkillCIC = heroItem.skillCIC;
            SkillCounsellor = heroItem.skillCounsellor;
            SkillGeneral = heroItem.skillGeneral;

            mSkillCIC = new SkillFields();
        }

        public void CopyToFields(ref HeroFields heroFields)
        {
            heroFields?.InitFormItem(ref mHeroItem);
        }

        /// <summary>
        /// 从数据字段转换为数据项（本地缓存）
        /// </summary>
        /// <param name="heroItem"></param>
        public void ParseToHeroItem(ref HeroItem heroItem)
        {
            heroItem.name = GetStringData(Consts.FN_NAME);
            heroItem.id = GetIntData(Consts.FN_ID);
            heroItem.level = GetIntData(Consts.FN_LEVEL);
            heroItem.hp = GetFloatData(Consts.FN_HP);
            heroItem.atk = GetFloatData(Consts.FN_ATK);
            heroItem.def = GetFloatData(Consts.FN_DEF);
            heroItem.intellect = GetFloatData(Consts.FN_INTELLECT);
            heroItem.aow = GetFloatData(Consts.FN_AOW);
            heroItem.troops = GetIntData(Consts.FN_TROOPS);
        }
    }
}
