
using ShipDock;
using UnityEngine;
using UnityEngine.UI;

public static class ShipDockAppExtension
{
    public static void MakeResolver<I>(this string serverName, string alias, string resolverName, ResolveDelegate<I> handler)
    {
        serverName.GetServer().MakeResolver(alias, resolverName, handler);
    }

    public static string Language(this string target, params string[] args)
    {
        return ShipDockApp.Instance.Locals.Language(target, args);
    }

    public static GameObject Create(this GameObject target, int poolID = int.MaxValue, bool selfActive = true)
    {
        if (poolID != int.MaxValue)
        {
            return ShipDockApp.Instance.AssetsPooling.FromPool(poolID, ref target, default, selfActive);
        }
        else
        {
            return Object.Instantiate(target);
        }
    }

    public static void Terminate(this GameObject target, int poolID = int.MaxValue)
    {
        if (poolID == int.MaxValue)
        {
            Object.Destroy(target);
        }
        else
        {
            ShipDockApp.Instance.AssetsPooling.ToPool(poolID, target);
        }
    }

    public static bool ToUIPosition(this Vector3 worldPosition, ref Camera worldCamera, out Vector3 UIPos, Camera camerInUI = default)
    {
        Vector3 viewPos = worldCamera.WorldToViewportPoint(worldPosition);
        bool result = viewPos.z >= 0f;
        if (result)
        {
            viewPos.x -= 0.5f;
            viewPos.y -= 0.5f;

            UIManager manager = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
            Camera camera = camerInUI == default ? manager.UIRoot.UICamera : camerInUI;
            UIPos = new Vector3(camera.pixelWidth * viewPos.x, camera.pixelHeight * viewPos.y, 0f);
            UIPos *= manager.UIRoot.ScaleRatio;
        }
        else
        {
            UIPos = Vector3.zero;
        }
        return result;
    }

    public static bool ToUILocalPosition(this RectTransform translateTo, RectTransform translateFrom, out Vector2 localInTranslateTo, Camera camInTranslateTo = default)
    {
        UIManager manager = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        if (camInTranslateTo == default)
        {
            camInTranslateTo = manager.UIRoot.UICamera;
        }
        else { }

        Vector2 screenPos = camInTranslateTo.WorldToScreenPoint(translateFrom.position);
        bool isSucess = RectTransformUtility.ScreenPointToLocalPointInRectangle(translateTo, screenPos, camInTranslateTo, out localInTranslateTo);

        localInTranslateTo *= manager.UIRoot.ScaleRatio;

        return isSucess;
    }

    public static T OpenFromResouce<T>(this string name, bool isUnique = true, bool isShow = true, bool activeSelfControlShow = true) where T : Component
    {
        UIManager uis = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        T result = uis.OpenResourceUI<T>(name, isUnique, isShow, activeSelfControlShow);
        return result;
    }

    public static void CloseResouceUI(this string name, bool willDestroy = true, bool activeSelfControlHide = true)
    {
        UIManager uis = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        uis.CloseResourceUI(name, willDestroy, activeSelfControlHide);
    }

    private const string LOG_OPEN_UI = "log:Open UI {0}";

    /// <summary>
    /// 打开热更界面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stackName">界面名</param>
    /// <returns></returns>
    public static T OpenUI<T>(this string stackName) where T : IUIStack, new()
    {
        UIManager uis = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        T result = uis.Open<T>(stackName, () =>
        {
            LOG_OPEN_UI.Log(stackName);
            return new T();
        });
        return result;
    }

    /// <summary>
    /// 加载UI资源并打开热更界面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stackName">界面名</param>
    /// <param name="onUIOpen">打开热更界面后的回调函数</param>
    /// <param name="UIABNames">需要加载的资源列表</param>
    public static void LoadAndOpenUI<T>(this string stackName, System.Action<T> onUIOpen, params string[] UIABNames) where T : IUIStack, new()
    {
        int max = UIABNames != default ? UIABNames.Length : 0;
        if (max > 0)
        {
            Framework framework = Framework.Instance;
            UIManager uis = framework.GetUnit<UIManager>(Framework.UNIT_UI);
            AssetBundles abs = framework.GetUnit<AssetBundles>(Framework.UNIT_AB);
            if (abs != default)
            {
                AssetsLoader loader = new AssetsLoader();
                loader.CompleteEvent.AddListener((suc, ld) =>
                {
                    if (suc)
                    {
                        ld?.Reclaim();
                        uis.OnLoadingShower?.Invoke(false);

                        T result = OpenUI<T>(stackName);
                        onUIOpen?.Invoke(result);
                    }
                    else { }
                });
                string abName;
                for (int i = 0; i < max; i++)
                {
                    abName = UIABNames[i];
                    if (!abs.HasBundel(abName))
                    {
                        loader.Add(abName, true, true);
                    }
                    else { }
                }
                if (loader.ResList.Count > 0)
                {
                    uis.OnLoadingShower?.Invoke(true);
                }
                else { }
                loader.Load(out _);
            }
            else { }
        }
        else
        {
            T result = OpenUI<T>(stackName);
            onUIOpen?.Invoke(result);
        }
    }

    public static void SyncMatchWidthOrHeight(this CanvasScaler target)
    {
        UIManager uis = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        target.matchWidthOrHeight = uis.UIRoot.MatchWidthOrHeight;
    }

    public static void SyncOorthographicSize(this Camera target, float rawSize)
    {
        UIManager uis = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        target.orthographicSize = rawSize * uis.UIRoot.FOVRatio;
    }

    public static void SyncFOVByRatio(this Camera target, float FOVRaw)
    {
        UIManager uis = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        target.fieldOfView = FOVRaw * uis.UIRoot.FOVRatio;
    }
}
