using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.Scriptables
{
    [Serializable]
    public abstract class ScriptableItem : IScriptableItem
    {
        public static void InitCollections<T>(ref Dictionary<int, T> mapper, ref List<T> collections) where T : IScriptableItem
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
        [EnableIf("@m_EditEnabled && ShouldNameEnabled()"), LabelText("@this.mTitleFieldName")]
#endif
        public string name;

#if ODIN_INSPECTOR
        [EnableIf("@m_EditEnabled && mIDEditEnabled")]
#endif
        public int id;

#if ODIN_INSPECTOR
        [LabelText("修改"), OnValueChanged("OnEditEnabledChanged"), ShowIf("@ShouldEditChooseShow()")]
#endif
        [SerializeField]
        private bool m_EditEnabled;

        protected bool mIDEditEnabled = true;
        protected string mTitleFieldName = "Name";

        public bool EditEnabled
        {
            get
            {
                return m_EditEnabled;
            }
        }

        public int GetID()
        {
            return id;
        }

        public JsonData ToJSON()
        {
            return JsonMapper.ToJson(this);
        }

        public abstract void AutoFill();

        public void SetID(int value)
        {
            id = value;
        }

        protected virtual void OnEditEnabledChanged()
        {
        }

        protected virtual bool ShouldNameEnabled()
        {
            return true;
        }

        protected virtual bool ShouldEditChooseShow()
        {
            return true;
        }
    }
}