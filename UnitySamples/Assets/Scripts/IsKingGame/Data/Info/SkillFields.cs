using LitJson;
using System.Collections.Generic;

namespace IsKing
{
    public class SkillFields : DataInfo
    {
        /// <summary>
        /// 从JSON源数据转换为数据项
        /// </summary>
        /// <param name="item"></param>
        /// <param name="data"></param>
        public static void FillFromJSON(ref JsonData item, out SkillItem data)
        {
            data = new SkillItem
            {
                id = int.Parse(item["id"].ToString()),
                name = item["name"].ToString(),
                level = int.Parse(item["level"].ToString()),
                skillType = int.Parse(item["skillType"].ToString()),
                targetType = int.Parse(item["targetType"].ToString()),
                needSetTarget = bool.Parse(item["needSetTarget"].ToString()),
            };

            JsonData list = item["effects"];

            int n = list.Count;
            data.effects = new int[n];
            for (int j = 0; j < n; j++)
            {
                data.effects[j] = int.Parse(list[j].ToString());
            }

            data.AfterInitFromJSON();
        }

        //public override List<int> FloatFieldNames { get; protected set; } = Consts.FN_BATTLE_INFO_FLOAT;
        public override List<int> IntFieldNames { get; protected set; } = Consts.FieldsSkillInfoInt;


    }

}