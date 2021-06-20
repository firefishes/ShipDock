
using ShipDock;
using ShipDock.Applications;
using ShipDock.Loader;
using ShipDock.Server;
using ShipDock.UI;
using UnityEngine;

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

    public static bool WorldToUIPosition(this Vector3 worldPosition, ref Camera worldCamera, out Vector3 localPos)
    {
        Vector3 viewPos = worldCamera.WorldToViewportPoint(worldPosition);
        if (viewPos.z < 0f)
        {
            localPos = Vector3.zero;
            return false;
        }

        viewPos.x -= 0.5f;
        viewPos.y -= 0.5f;

        UIManager ui = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        Camera UICamera = ui.UIRoot.UICamera;
        localPos = new Vector3(UICamera.pixelWidth * viewPos.x, UICamera.pixelHeight * viewPos.y, 0);
        return true;
    }

    public static T OpenFromResouce<T>(this string name) where T : Component
    {
        UIManager uis = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        T result = uis.OpenResourceUI<T>(name);
        return result;
    }

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
            "log:Open UI {0}".Log(stackName);
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
        int max = UIABNames.Length;
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
                        ld?.Dispose();
                        uis.OnLoadingAlert?.Invoke(false);

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
                    uis.OnLoadingAlert?.Invoke(true);
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
}
