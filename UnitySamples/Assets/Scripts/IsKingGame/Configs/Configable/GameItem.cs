using LitJson;
using System;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using UnityEngine;
using Sirenix.OdinInspector;
#endif

namespace IsKing
{
    [Serializable]
    public abstract class GameItem : IGameItem
    {
        public static void InitCollections<T>(ref Dictionary<int, T> mapper, ref List<T> collections) where T : IGameItem
        {
            mapper = new Dictionary<int, T>();

            T item;
            int max = collections.Count;
            for (int i = 0; i < max; i++)
            {
                item = collections[i];
                mapper[item.GetID()] = item;
                item.AutoFill();
            }
        }

#if ODIN_INSPECTOR
        [EnableIf("@this.m_EditEnabled == true")]
#endif
        public string name;

#if ODIN_INSPECTOR
        [SerializeField, LabelText("修改")]
        private bool m_EditEnabled;
        [EnableIf("@this.m_EditEnabled == true")]
#endif
        public int id;

        public int GetID()
        {
            return id;
        }

        public JsonData ToJSON()
        {
            return JsonMapper.ToJson(this);
        }

        public abstract void AutoFill();
    }
}