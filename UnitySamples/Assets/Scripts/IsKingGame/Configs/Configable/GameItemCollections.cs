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

namespace IsKing
{
    [Serializable]
    public abstract class GameItemCollections<T> : ScriptableObject, IGameItemCollections where T : IGameItem
    {
        [SerializeField]
        private TextAsset m_RawData;
        [SerializeField]
        protected List<T> m_Collections;

        protected Dictionary<int, T> mMapper;

#if ODIN_INSPECTOR && UNITY_EDITOR
        [Button(name: "保存"), ShowIf("@this.m_RawData != null")]
        private void SaveGameItems()
        {
            string source = JsonMapper.ToJson(m_Collections);
            string relativePath = AssetDatabase.GetAssetPath(m_RawData);

            int pathLen = Application.dataPath.Length;
            int end = "Assets/".Length;
            string path = Application.dataPath.Substring(0, pathLen - end);
            path = path + "/" + relativePath;
            File.WriteAllText(path, source);
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
    }
}