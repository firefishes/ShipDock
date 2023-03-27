using UnityEngine.Events;

namespace ShipDock
{
    public class OnLoaderProgress : UnityEvent<Loader> { };
    public class OnLoaderCompleted : UnityEvent<bool, Loader> { };
    public class OnAssetLoaderCompleted : UnityEvent<bool, AssetsLoader> { };
    public class OnRemoteAssetUpdated : UnityEvent<bool, string> { };
}
