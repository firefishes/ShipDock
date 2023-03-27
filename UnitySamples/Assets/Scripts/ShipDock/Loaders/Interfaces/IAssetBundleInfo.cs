using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 资源包信息接口
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public interface IAssetBundleInfo : IReclaim
    {
        int BeingUsed { get; set; }
        T GetAsset<T>(string path) where T : Object;
        GameObject GetAsset(string path);
        AssetBundle Asset { get; }
        string Name { get; }
    }
}

