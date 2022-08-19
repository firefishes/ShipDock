using ShipDock;
using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Loader;
using ShipDock.UI;
using System;

public static class ShipDockUIExtensions
{
    /// <summary>
    /// 打开热更界面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stackName">界面名</param>
    /// <returns></returns>
    public static T OpenHotFixUI<T>(this string stackName) where T : UIModularHotFixer, new()
    {
        UIManager uis = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        T result = uis.Open<T>(stackName, () =>
        {
            "log:Open hotfix UI {0}".Log(stackName);
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
    public static void LoadAndOpenHotFixUI<T>(this string stackName, Action<T> onUIOpen, params string[] UIABNames) where T : UIModularHotFixer, new()
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
                        ld?.Reclaim();
                        uis.OnLoadingShower?.Invoke(false);

                        T result = OpenHotFixUI<T>(stackName);
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
            T result = OpenHotFixUI<T>(stackName);
            onUIOpen?.Invoke(result);
        }
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="stackName"></param>
    /// <param name="isDestroy"></param>
    public static void Close(this string stackName, bool isDestroy = false)
    {
        UIManager uis = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
        IUIStack stack = uis.GetUI<IUIStack>(stackName);
        uis.Close(stackName, isDestroy);
    }

    /// <summary>
    /// 热更界面模块添加要监听的数据代理，并返回此数据代理的引用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="modular"></param>
    /// <param name="dataName"></param>
    /// <returns></returns>
    public static T UIModularRegisterData<T>(this UIModularHotFixer modular, int dataName) where T : DataProxy
    {
        T result = dataName.GetData<T>();
        UIModularRegisterData(modular, dataName);
        return result;
    }

    /// <summary>
    /// 热更界面模块添加多个需要监听的数据代理
    /// </summary>
    /// <param name="modular"></param>
    /// <param name="dataNames"></param>
    /// <returns></returns>
    public static DataProxyCacher UIModularRegisterData(this UIModularHotFixer modular, params int[] dataNames)
    {
        DataProxyCacher result = default;
        int max = dataNames.Length;
        if (max > 0)
        {
            DataProxy data;
            DataProxy[] list = new DataProxy[max];
            for (int i = 0; i < max; i++)
            {
                data = dataNames[i].GetData<DataProxy>();
                data.AddDataProxyNotify(modular.OnDataProxyNotify);
                list[i] = data;
            }
            result = new DataProxyCacher(list);
        }
        else { }

        return result;
    }

    /// <summary>
    /// 热更界面模块移除多个不再监听的数据代理
    /// </summary>
    /// <param name="modular"></param>
    /// <param name="dataNames"></param>
    public static void UIModularUnregisterData(this UIModularHotFixer modular, params int[] dataNames)
    {
        DataProxy data;
        int max = dataNames.Length;
        for (int i = 0; i < max; i++)
        {
            data = dataNames[i].GetData<DataProxy>();
            data.RemoveDataProxyNotify(modular.OnDataProxyNotify);
        }
    }
}