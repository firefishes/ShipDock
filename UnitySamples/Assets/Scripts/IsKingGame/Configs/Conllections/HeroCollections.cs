﻿using LitJson;
using ShipDock.Loader;
using ShipDock.Scriptables;
using UnityEngine;

namespace IsKing
{
    [CreateAssetMenu]
    public class HeroCollections : ScriptableItems<HeroItem>
    {
        public override void InitCollections()
        {
            ScriptableItem.InitCollections(ref mMapper, ref m_Collections);
        }

        public override HeroItem GetItem(int id)
        {
            return mMapper[id];
        }

        public override void FillFromDataRaw(ref string source)
        {
            JsonData jsonData = JsonMapper.ToObject(source);

            HeroItem data;
            JsonData item;
            int count = jsonData.Count;

            for (int i = 0; i < count; i++)
            {
                item = jsonData[i];

                HeroFields.FillFromJSON(ref item, out data);

                m_Collections.Add(data);
            }
        }
    }
}