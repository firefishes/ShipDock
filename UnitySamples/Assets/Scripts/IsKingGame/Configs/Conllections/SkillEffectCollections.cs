using LitJson;
using ShipDock.Scriptables;
using UnityEngine;

namespace IsKing
{
    [CreateAssetMenu]
    public class SkillEffectCollections : ScriptableItems<SkillEffectItem>
    {
        public override void InitCollections()
        {
            ScriptableItem.InitCollections(ref mMapper, ref m_Collections);
        }

        public override SkillEffectItem GetItem(int id)
        {
            return mMapper[id];
        }

        public override void FillFromDataRaw(ref string source)
        {
            JsonData jsonData = JsonMapper.ToObject(source);

            SkillEffectItem data;
            JsonData item;
            int count = jsonData.Count;

            for (int i = 0; i < count; i++)
            {
                item = jsonData[i];

                data = new SkillEffectItem
                {
                    id = int.Parse(item["id"].ToString()),
                    name = item["name"].ToString(),
                    effectField = int.Parse(item["effectField"].ToString()),
                    effectType = int.Parse(item["effectType"].ToString()),
                    effectCount = int.Parse(item["effectCount"].ToString()),
                    effectValue = float.Parse(item["effectValue"].ToString()),
                };
                data.AfterInitFromJSON();
                m_Collections.Add(data);
            }
        }
    }

}