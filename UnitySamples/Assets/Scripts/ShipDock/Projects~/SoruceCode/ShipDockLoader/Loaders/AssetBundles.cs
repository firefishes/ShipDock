﻿#define _LOG_ASSET_QUOTEDER//是否开启资源引用器的日志

using ShipDock;
using ShipDock.Loader;
using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    /// <summary>
    /// 
    /// 资源包管理器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class AssetBundles : IAssetBundles
    {
        /// <summary>资源包的依赖清单</summary>
        public const string ASSET_BUNDLE_MANIFEST = "AssetBundleManifest";

        private const int IDColumnSize = 10;

        private ICustomAssetBundle mCustomAssets;
        private KeyValueList<string, IAssetBundleInfo> mCaches;
        private KeyValueList<string, AssetBundleManifest> mABManifests;
        private IntegerID<string> mABNameIDs = new IntegerID<string>();
        private IntegerID<string> mAssetNameIDs = new IntegerID<string>();
        private KeyValueList<int, Object> mRawMapper = new KeyValueList<int, Object>();
        private KeyValueList<Object, AssetQuoteder> mAssetMapper = new KeyValueList<Object, AssetQuoteder>();
        private KeyValueList<int, AssetQuoteder> mQuotederMapper = new KeyValueList<int, AssetQuoteder>();
        private KeyValueList<string, int> mBundlesCounter = new KeyValueList<string, int>();

        public string MainManifestName { get; private set; }

        public AssetBundles()
        {
            mCaches = new KeyValueList<string, IAssetBundleInfo>();
            mABManifests = new KeyValueList<string, AssetBundleManifest>();
        }

        public void Dispose()
        {
            Utils.Reclaim(ref mCaches, false, true);
            Utils.Reclaim(ref mABManifests, false, true);
        }

        public void SetCustomBundles(ref ICustomAssetBundle value)
        {
            mCustomAssets = value;
        }

        public bool HasBundel(string name)
        {
            return (mCaches != default) && mCaches.ContainsKey(name);
        }

        public T Get<T>(string name, string path) where T : Object
        {
            T result = (mCustomAssets != default) ? mCustomAssets.GetCustomAsset<T>(name, path) : default;
            if((result == default) && HasBundel(name))
            {
                IAssetBundleInfo assetBundleInfo = mCaches[name];
                result = assetBundleInfo.GetAsset<T>(path);
            }
            else { }

            return result;
        }

        public GameObject Get(string name, string path)
        {
            GameObject result = (mCustomAssets != default) ? mCustomAssets.GetCustomAsset<GameObject>(name, path) : default;
            if ((result == default) && HasBundel(name))
            {
                IAssetBundleInfo assetBundleInfo = mCaches[name];
                result = assetBundleInfo.GetAsset(path);
            }
            else { }

            return result;
        }

        public bool Add(AssetBundle bundle)
        {
            if(bundle == default)
            {
                return false;
            }
            else { }

            return AddBundle(string.Empty, ref bundle);
        }

        public bool Add(string manifestName, AssetBundle bundle)
        {
            if (bundle == default)
            {
                return false;
            }
            else { }

            return AddBundle(manifestName, ref bundle);
        }

        private bool AddBundle(string name, ref AssetBundle bundle)
        {
            name = string.IsNullOrEmpty(name) ? bundle.name : name;
            IAssetBundleInfo info;

            bool result = mCaches.ContainsKey(name);
            if (result)
            {
                info = mCaches[name];
                info.LoadCount++;
            }
            else
            {
                info = new AssetBundleInfo(bundle);
                mCaches[name] = info;

                if (!name.Contains("_unityscene"))
                {
                    mABManifests[name] = bundle.LoadAsset<AssetBundleManifest>(ASSET_BUNDLE_MANIFEST);
                }
                else { }
            }

            return !result;
        }

        public void Remove(string name, bool unloadAllLoaded = false)
        {
            AssetBundle ab = RemoveBundle(ref name, unloadAllLoaded);
            ab?.Unload(unloadAllLoaded);
        }

        public void Remove(AssetBundle bundle, bool unloadAllLoaded = false)
        {
            if (bundle != default)
            {
                string name = bundle.name;
                Remove(name, unloadAllLoaded);
            }
            else { }
        }

        private AssetBundle RemoveBundle(ref string name, bool unloadAllLoaded)
        {
            AssetBundle result = default;
            if (mCaches.ContainsKey(name))
            {
                IAssetBundleInfo info = mCaches.GetValue(name, unloadAllLoaded);
                if (unloadAllLoaded)
                {
                    info.LoadCount = 0;
                }
                else
                {
                    info.LoadCount--;
                }

                "log:Asset bundle still relate {0}".Log(info.LoadCount > 0);
                if (info.LoadCount <= 0)
                {
                    "log:Asset bundle {0} removed".Log(name);
                    mABManifests.Remove(name);

                    result = info.Asset;
                    info.Dispose();
                }
                else { }
            }
            else { }

            return result;
        }

        public void SetMainManifest(string name, AssetBundle bundle)
        {
            if (string.IsNullOrEmpty(MainManifestName))
            {
                MainManifestName = name;
                Add(name, bundle);
                "log:Main manifest asset bundle added, name is {0}".Log(name);
            }
            else { }
        }

        public void RemoveMainManifest()
        {
            if (!string.IsNullOrEmpty(MainManifestName))
            {
                "log:Will remove main manifest asset bundle.".Log();
                Remove(MainManifestName, true);
                MainManifestName = string.Empty;
            }
            else { }
        }

        public AssetBundleManifest GetManifest(string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                name = MainManifestName;//name 参数为空字符串为获取资源包的总依赖
            }
            else { }

            return HasBundel(name) ? mABManifests[name] : default;
        }

        public T GetAndQuote<T>(string abName, string assetName, out AssetQuoteder quoteder) where T : Object
        {
            int abID = mABNameIDs.GetID(ref abName);
            int assetID = mAssetNameIDs.GetID(ref assetName);
            int id = abID * IDColumnSize + assetID;//id 阵列转换
            Object raw;
            if (mRawMapper.ContainsKey(id))
            {
                raw = mRawMapper[id];
            }
            else
            {
                raw = Get<T>(abName, assetName);
                mRawMapper[id] = raw;
            }
            quoteder = default;
            int mapperID = id;
            id = raw.GetInstanceID();
            if (mQuotederMapper.ContainsKey(id))
            {
                quoteder = mQuotederMapper[id];
            }
            else
            {
                quoteder = new AssetQuoteder(mAssetMapper, mQuotederMapper, mBundlesCounter, mRawMapper);
                quoteder.SetRaw(mapperID, ref abName, ref assetName, raw);
#if LOG_ASSET_QUOTEDER
                "log:New quote {0}, id is {1}, ab name is {2}, asset name is {3}"
                    .Log(typeof(T).Name, id.ToString(), abName, assetName);
#endif
            }
            T result = quoteder.Instantiate<T>();
            return result;
        }

        public void UnloadUselessAssetBundles(params string[] abNames)
        {
            bool isCustome = abNames.Length > 0;
            List<string> list = isCustome ? new List<string>(abNames) : mBundlesCounter.Keys;

            string key;
            List<string> deletes = new List<string>();
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                key = list[i];
                if (mBundlesCounter[key] == 0)
                {
                    deletes.Add(key);
                }
                else
                {
#if LOG_ASSET_QUOTEDER
                    if (isCustome)
                    {
                        "error: {0} is using, count is {1}".Log(key, bundlesCounter[key].ToString());
                    }
                    else { }
#endif
                }
            }
            max = deletes.Count;
            for (int i = 0; i < max; i++)
            {
                key = deletes[i];
                mBundlesCounter.Remove(deletes[i]);
                Remove(key, true);
            }
        }

        public void UnloadQuote(string abName, string assetName)
        {
            int abID = mABNameIDs.GetID(ref abName);
            int assetID = mAssetNameIDs.GetID(ref assetName);
            int id = abID * IDColumnSize + assetID;//id 阵列转换
            if (mRawMapper.ContainsKey(id))
            {
                Object raw = mRawMapper[id];
                id = raw.GetInstanceID();
                if (mQuotederMapper.ContainsKey(id))
                {
                    AssetQuoteder quoteder = mQuotederMapper[id];
                    quoteder.Dispose();
                }
                else { }
            }
            else { }
        }

        public void DestroyQuotederAsset(Object target, bool isAutoDispose = false)
        {
            if (mAssetMapper.Keys.Contains(target))
            {
                AssetQuoteder quoteder = mAssetMapper[target];
                quoteder.Destroy(target, false, isAutoDispose);
            }
            else { }
        }
    }
}

public static class AssetBundlesExtensions
{
    public static void DestroyFromQuote(this Object target, bool isAutoDispose = false)
    {
        AssetBundles abs = Framework.Instance.GetUnit<AssetBundles>(Framework.UNIT_AB);
        abs?.DestroyQuotederAsset(target, isAutoDispose);
    }
}

