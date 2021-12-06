using LitJson;
using UnityEngine;

namespace IsKing
{
    [CreateAssetMenu]
    public class SkillCollections : GameItemCollections<SkillItem>
    {
        public override SkillItem GetItem(int id)
        {
            return mMapper[id];
        }

        public override void InitCollections()
        {
            GameItem.InitCollections(ref mMapper, ref m_Collections);
        }

        public override void FillFromDataRaw(ref string source)
        {
            JsonData jsonData = JsonMapper.ToObject(source);

            SkillItem data;
            JsonData item;
            int count = jsonData.Count;

            for (int i = 0; i < count; i++)
            {
                item = jsonData[i];
                SkillFields.FillFromJSON(ref item, out data);
                m_Collections.Add(data);
            }
        }
    }
}