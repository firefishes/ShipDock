namespace ShipDock
{
    public interface ICustomAssetBundle : IReclaim
    {
        T GetCustomAsset<T>(string name, string path) where T : UnityEngine.Object;
    }
}
