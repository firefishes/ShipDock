
using ShipDock;
using ShipDock.Applications;
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
            return UnityEngine.Object.Instantiate(target);
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
}
