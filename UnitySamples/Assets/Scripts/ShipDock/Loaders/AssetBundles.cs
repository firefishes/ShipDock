#define _LOG_ASSET_QUOTEDER//是否开启资源引用器的日志

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

        /// <summary>资源引用ID列数，用于以引用方式获取资源的功能</summary>
        private const int IDColumnSize = 10;

        /// <summary>定制资源对象</summary>
        private ICustomAssetBundle mCustomAssets;
        /// <summary>资源包缓存</summary>
        private KeyValueList<string, IAssetBundleInfo> mCaches;
        /// <summary>资源包依赖清单缓存</summary>
        private KeyValueList<string, AssetBundleManifest> mABManifests;
        /// <summary>资源包名自动 id 集合</summary>
        private IntegerID<string> mABNameIDs = new IntegerID<string>();
        /// <summary>资源名自动 id 集合</summary>
        private IntegerID<string> mAssetNameIDs = new IntegerID<string>();
        /// <summary>资源母本对象的数据映射</summary>
        private KeyValueList<int, Object> mRawMapper = new KeyValueList<int, Object>();
        /// <summary>资源副本对应的资源引用对象数据映射</summary>
        private KeyValueList<Object, AssetQuoteder> mAssetMapper = new KeyValueList<Object, AssetQuoteder>();
        /// <summary>资源引用对象的集合</summary>
        private KeyValueList<int, AssetQuoteder> mQuotederMapper = new KeyValueList<int, AssetQuoteder>();
        /// <summary>资源主依赖清单名</summary>
        public string MainManifestName { get; private set; }

        public AssetBundles()
        {
            mCaches = new KeyValueList<string, IAssetBundleInfo>();
            mABManifests = new KeyValueList<string, AssetBundleManifest>();
        }

        public void Dispose()
        {
            mCustomAssets = default;
            Utils.Reclaim(ref mCaches, false, true);
            Utils.Reclaim(ref mABManifests, false, true);
            Utils.Reclaim(ref mAssetMapper, false, true);
            Utils.Reclaim(ref mQuotederMapper, false, true);
            Utils.Reclaim(mABNameIDs);
            Utils.Reclaim(mAssetNameIDs);
            Utils.Reclaim(mRawMapper);
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
            if ((result == default) && HasBundel(name))
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
            if (bundle == default)
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
                info.BeingUsed++;
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
                    info.BeingUsed = 0;
                }
                else
                {
                    info.BeingUsed--;
                }

                "log:Asset bundle still relate {0}".Log(info.BeingUsed > 0);
                if (info.BeingUsed <= 0)
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
                quoteder = new AssetQuoteder(ABBundleQuotedChanged, mAssetMapper, mQuotederMapper, mRawMapper);
                quoteder.SetRaw(mapperID, ref abName, ref assetName, raw);
#if LOG_ASSET_QUOTEDER
                "log:New quote {0}, id is {1}, ab name is {2}, asset name is {3}".Log(typeof(T).Name, id.ToString(), abName, assetName);
#endif
            }
            T result = quoteder.Instantiate<T>();
            return result;
        }

        private int ABBundleQuotedChanged(string abName, bool isIncreaced)
        {
            IAssetBundleInfo info = mCaches[abName];
            if (info != default)
            {
                if (isIncreaced)
                {
                    info.BeingUsed++;
                }
                else
                {
                    info.BeingUsed--;
                }
            }
            else { }
            return info != default ? info.BeingUsed : 0;
        }

        public void UnloadUselessAssetBundles(params string[] abNames)
        {
            bool isCustome = abNames.Length > 0;
            List<string> list = isCustome ? new List<string>(abNames) : mCaches.Keys;

            string abName;
            IAssetBundleInfo info;
            List<string> deletes = new List<string>();
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                abName = list[i];
                info = mCaches[abName];
                if (info.BeingUsed == 0)
                {
                    deletes.Add(abName);
                }
                else
                {
#if LOG_ASSET_QUOTEDER
                    if (isCustome)
                    {
                        "error: {0} is using, count is {1}".Log(abName, mCaches[abName].BeingUsed.ToString());
                    }
                    else { }
#endif
                }
            }
            max = deletes.Count;
            for (int i = 0; i < max; i++)
            {
                abName = deletes[i];
                Remove(abName, true);
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

/// <summary>
/// 
/// 资源包管理器相关的扩展方法类
/// 
/// add by Minghua.ji
/// 
/// </summary>
public static class AssetBundlesExtensions
{
    /// <summary>
    /// 从资源引用器销毁资源
    /// </summary>
    /// <param name="target"></param>
    /// <param name="isAutoDispose"></param>
    public static void DestroyFromQuote(this Object target, bool isAutoDispose = false)
    {
        AssetBundles abs = Framework.Instance.GetUnit<AssetBundles>(Framework.UNIT_AB);
        abs?.DestroyQuotederAsset(target, isAutoDispose);
    }
}

