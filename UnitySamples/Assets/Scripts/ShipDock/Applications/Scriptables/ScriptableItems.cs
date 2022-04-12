#define _APPLY_AUTO_FILL

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using LitJson;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.Scriptables
{
    [Serializable]
    public abstract class ScriptableItems<T> : ScriptableObject, IScriptableItems where T : IScriptableItem
    {
        [SerializeField]
        protected bool m_ApplyAutoID;
        [SerializeField]
        private TextAsset m_RawData;
        [SerializeField]
        protected List<T> m_Collections;

        protected Dictionary<int, T> mMapper;

#if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(name: "保存"), ShowIf("@this.m_RawData != null")]
        private void SaveGameItems()
        {
            int max;
            if (m_ApplyAutoID)
            {
                max = m_Collections.Count;
                for (int i = 0; i < max; i++)
                {
                    SetAutoID(i);
                }
            }
            else { }

            T item = default;
            max = m_Collections.Count;
            for (int i = 0; i < max; i++)
            {
                item = m_Collections[i];
                BeforeSaveGameItems(ref item);
            }

            string source = JsonMapper.ToJson(GetJsonSource());
            string relativePath = AssetDatabase.GetAssetPath(m_RawData);

            int pathLen = Application.dataPath.Length;
            int end = "Assets/".Length;
            string path = Application.dataPath.Substring(0, pathLen - end);
            path = path + "/" + relativePath;
            File.WriteAllText(path, source);

            AssetDatabase.Refresh();
        }

#if APPLY_AUTO_FILL
        [Button(name: "重填充并保存"), ShowIf("@this.m_RawData != null")]
        private void FillAndSaveGameItems()
        {
            m_Collections?.Clear();
            m_Collections = new List<T>();

            InitCollections();
            EditorUtility.SetDirty(this);
            SaveGameItems();
        }
#endif

        [Button(name: "载入"), ShowIf("@this.m_RawData != null")]
        private void LoadRaw()
        {
            int pathLen = Application.dataPath.Length;
            int end = "Assets/".Length;
            string path = Application.dataPath.Substring(0, pathLen - end);
            string relativePath = AssetDatabase.GetAssetPath(m_RawData);
            path = path + "/" + relativePath;
            string source = File.ReadAllText(path);

            m_Collections?.Clear();
            m_Collections = new List<T>();

            FillFromDataRaw(ref source);

            EditorUtility.SetDirty(this);
        }
#endif

        public abstract void FillFromDataRaw(ref string source);
        public abstract void InitCollections();
        public abstract T GetItem(int id);

        protected virtual void SetAutoID(int index)
        {
            T item = m_Collections[index];
            int id = item.GetHashCode();
            item.SetID(id);
        }

        protected virtual void BeforeSaveGameItems(ref T item)
        {
        }

        protected virtual object GetJsonSource()
        {
            return m_Collections;
        }
    }
}