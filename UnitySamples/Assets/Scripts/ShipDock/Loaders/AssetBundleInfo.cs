
using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 资源包信息类
    /// 
    /// 用于资源包的管理和资源引用计数
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class AssetBundleInfo : IAssetBundleInfo
    {
        public int BeingUsed { get; set; }
        public AssetBundle Asset { get; private set; }

        public string Name
        {
            get
            {
                return Asset != default ? Asset.name : string.Empty;
            }
        }

        public AssetBundleInfo(AssetBundle asset)
        {
            BeingUsed = 1;
            Asset = asset;
        }

        public void Reclaim()
        {
            Asset = default;
        }

        public T GetAsset<T>(string path) where T : Object
        {
            T result = default;
            if (Asset != default)
            {
                result = Asset.LoadAsset<T>(path);
            }
            else { }
            return  result;
        }

        public GameObject GetAsset(string path)
        {
            GameObject result = default;
            if (Asset != default)
            {
                result = Asset.LoadAsset<GameObject>(path);
            }
            else { }
            return result;
        }
    }
}

