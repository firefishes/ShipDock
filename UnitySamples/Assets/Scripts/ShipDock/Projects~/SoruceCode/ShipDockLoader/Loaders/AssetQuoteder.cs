#define _LOG_ASSET_QUOTEDER//是否开启资源引用器的日志

using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    /// <summary>
    /// 
    /// 资源引用器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public sealed class AssetQuoteder
    {
        public int MapperID { get; private set; }
        public int Count { get; private set; }
        public string ABName { get; private set; }
        public string AssetName { get; private set; }

        /// <summary>资源母本</summary>
        private Object Raw { get; set; }
        private System.Func<string, bool, int> OnABBundleInUsed { get; set; }
        private KeyValueList<Object, AssetQuoteder> AssetMapper { get; set; }
        private KeyValueList<int, AssetQuoteder> QuotederMapper { get; set; }
        private KeyValueList<int, Object> RawMapper { get; set; }

        public AssetQuoteder(
            System.Func<string, bool, int> onABBundleInUsed,
            KeyValueList<Object, AssetQuoteder> assetMapper,
            KeyValueList<int, AssetQuoteder> quotederMapper,
            KeyValueList<int, Object> rawMapper)
        {
            AssetMapper = assetMapper;
            QuotederMapper = quotederMapper;
            RawMapper = rawMapper;
            OnABBundleInUsed = onABBundleInUsed;
        }

        public void Dispose()
        {
            int id = MapperID;
            if (RawMapper.ContainsKey(id))
            {
                Object raw = RawMapper[id];
                RawMapper.Remove(id);

                id = raw.GetInstanceID();
                if (QuotederMapper.ContainsKey(id))
                {
                    QuotederMapper.Remove(id);
                    CleanAllAsset();
                    RemoveAssetQuoted();
                }
                else { }
            }
            else { }

            Raw = default;
            RawMapper = default;
            QuotederMapper = default;
            AssetMapper = default;
            OnABBundleInUsed = default;
            Count = 0;
        }

        private void CleanAllAsset()
        {
            List<Object> keys = AssetMapper.Keys;
            List<AssetQuoteder> list = AssetMapper.Values;
            List<Object> deletes = new List<Object>();
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                if (list[i].MapperID == MapperID)
                {
                    deletes.Add(keys[i]);
                }
                else { }
            }
            Object asset;
            max = deletes.Count;
            for (int i = 0; i < max; i++)
            {
                asset = deletes[i];
                if (asset != default)
                {
#if LOG_ASSET_QUOTEDER
                    "log:Asset quoteder {0} clean all asset, ({1})".Log(mRaw.name, asset.name);
#endif
                    AssetMapper.Remove(asset);
                    Object.Destroy(asset);
                }
                else { }
            }
        }

        public void SetRaw(int id, ref string abName, ref string assetName, Object value)
        {
            if (Raw == default)
            {
                MapperID = id;
                Raw = value;
                ABName = abName;
                AssetName = assetName;
                Count = 0;

                int rawID = Raw.GetInstanceID();
                QuotederMapper[rawID] = this;

                AddAssetQuoted();
            }
            else { }
        }

        public T Instantiate<T>() where T : Object
        {
            T result = default;
            bool flag = Raw != default;
            if (flag)
            {
                result = Object.Instantiate((T)Raw);
                AssetMapper[result] = this;
                Count++;

                AddAssetQuoted();
#if LOG_ASSET_QUOTEDER
                "log:Get asset from quote {0}, total is {1}".Log(mRaw.name, Count.ToString());
#endif
            }
            else { }
            return result;
        }

        public void Destroy(Object target, bool checkIsExists = true, bool isAutoDispose = false)
        {
            if (checkIsExists)
            {
                if (AssetMapper != default)
                {
                    bool flag = AssetMapper.Keys.Contains(target);
#if LOG_ASSET_QUOTEDER
                    "error:Asset {0} do not contains in quoteder, raw name is {1}".Log(!flag, target.name, mRaw.name);
#endif
                    if (!flag)
                    {
                        return;
                    }
                    else { }
                }
                else { }
            }
            else { }

            AssetMapper.Remove(target);

#if LOG_ASSET_QUOTEDER
            "log".Log("Destroy asset".Append(target.name, " count remains ", Count.ToString()));
#endif
            Object.Destroy(target);

            RemoveAssetQuoted();

            Count--;
            if (Count <= 0)
            {
                if (isAutoDispose)
                {
                    Dispose();
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 增加资源引用
        /// </summary>
        private void AddAssetQuoted()
        {
            int used = OnABBundleInUsed.Invoke(ABName, true);
#if LOG_ASSET_QUOTEDER
            "log:Asset bundle {0} quoted increaced {1}".Log(ABName, used.ToString());
#endif
        }

        /// <summary>
        /// 移除资源引用
        /// </summary>
        private void RemoveAssetQuoted()
        {
            int used = OnABBundleInUsed.Invoke(ABName, false);
#if LOG_ASSET_QUOTEDER
            "log:Asset bundle {0} quoted reduced to {1}".Log(ABName, used.ToString());
#endif
        }
    }
}
