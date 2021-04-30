﻿using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// 特效资源管理器
    /// 
    /// </summary>
    public class Effects
    {
        private class Effect
        {
            public int total;
            public int poolID;
            public GameObject source;

            private int CacheIndex { get; set; }
            private List<GameObject> UniqueCache { get; set; }

            public int Surplus { get; private set; }

            public void Init()
            {
                Surplus = total;
                UniqueCache = new List<GameObject>();
            }

            public void Clean()
            {
                total = 0;
                Surplus = 0;
                source = default;

                List<GameObject> list = UniqueCache;
                Utils.Reclaim(ref list);

                UniqueCache = default;
            }

            public void CreateAndFill(out GameObject result, bool isFromPool = false, bool selfActive = true)
            {
                result = default;
                if (ShouldCreate())
                {
                    Surplus--;
                    int poolIDValue = isFromPool ? poolID : int.MaxValue;
                    result = source.Create(poolIDValue, selfActive);
                    UniqueCache.Add(result);
                }
                else { }
            }

            public bool ShouldCreate()
            {
                return Surplus > 0;
            }

            public GameObject GetUniqueCache()
            {
                int index = CacheIndex;
                CacheIndex++;
                CacheIndex = CacheIndex >= total - 1 ? 0 : CacheIndex;

                GameObject result = (index >= 0) && (index < UniqueCache.Count) ? UniqueCache[index] : default;

                if (result == default && source != default)
                {
                    result = Object.Instantiate(source);
                    UniqueCache[index] = result;
                }
                else { }

                return result;
            }

            public void CollectEffect(GameObject target)
            {
                UniqueCache.Remove(target);
                target.Terminate(poolID);
            }
        }

        private KeyValueList<int, Effect> mPrefabRaw;

        public Effects() : base()
        {
            mPrefabRaw = new KeyValueList<int, Effect>();
        }

        public void Dispose()
        {
            int max = mPrefabRaw != default ? mPrefabRaw.Size : 0;
            if (max > 0)
            {
                List<Effect> list = mPrefabRaw.Values;
                for (int i = 0; i < max; i++)
                {
                    list[i].Clean();
                }
            }
            else { }

            Utils.Reclaim(ref mPrefabRaw);
        }

        public bool HasEffectRaw(int id)
        {
            return mPrefabRaw.ContainsKey(id);
        }

        public void CreateSource(int id, ref GameObject source, int total, int preCreate = 0)
        {
            Effect effect;
            if (mPrefabRaw.ContainsKey(id))
            {
                effect = mPrefabRaw[id];
            }
            else
            {
                effect = new Effect
                {
                    source = source,
                    total = total,
                    poolID = id,
                };
                effect.Init();
                mPrefabRaw[id] = effect;
            }

            int max = preCreate;
            for (int i = 0; i < max; i++)
            {
                CreateEffect(id, out _);
            }
        }

        public void CreateSource(int id, ref ResPrefabBridge source, int total, int preCreate = 0)
        {
            GameObject prefab = source.Prefab;
            CreateSource(id, ref prefab, total, preCreate);
        }

        public void FillToPrefabBridge(int id, ref ResPrefabBridge source)
        {
            if (mPrefabRaw.ContainsKey(id))
            {
                source.SetPoolID(id);
                source.FillRaw(mPrefabRaw[id].source);
            }
            else { }
        }

        public void CreateEffect(int id, out GameObject result, bool isFromPool = true, bool selfActive = true)
        {
            result = default;
            if (mPrefabRaw.ContainsKey(id))
            {
                Effect effect = mPrefabRaw[id];
                if (effect.ShouldCreate())
                {
                    effect.CreateAndFill(out result, isFromPool, selfActive);
                }
                else
                {
                    result = effect.GetUniqueCache();
                }
            }
            else { }
        }

        public void CollectEffect(int id, GameObject target)
        {
            if (mPrefabRaw.ContainsKey(id))
            {
                Effect effect = mPrefabRaw[id];
                effect.CollectEffect(target);
            }
            else
            {
                Object.Destroy(target);
            }
        }

        public void RemoveEffectRaw(params int[] ids)
        {
            Effect effect;
            int max = ids.Length;
            for (int i = 0; i < max; i++)
            {
                int id = ids[i];
                if (mPrefabRaw != default && mPrefabRaw.ContainsKey(id))
                {
                    effect = mPrefabRaw.Remove(id);
                    effect?.Clean();
                }
                else { }
            }
        }
    }
}