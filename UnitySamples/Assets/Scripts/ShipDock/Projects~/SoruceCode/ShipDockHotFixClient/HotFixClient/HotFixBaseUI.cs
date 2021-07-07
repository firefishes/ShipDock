
using ShipDock.Applications;
using ShipDock.Notices;

/// <summary>
/// 
/// UI热更相关的扩展方法
/// 
/// add by Minghua.ji
/// 
/// </summary>
public static class HotFixBaseUIExtensions
{
    /// <summary>
    /// 从界面热更代理组件获取指定类型的热更UI交互器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="UIAgent"></param>
    /// <returns></returns>
    public static T GetUIInteracter<T>(this HotFixerUIAgent UIAgent) where T : HotFixBaseUI
    {
        return (T)UIAgent.Bridge.HotFixerInteractor;
    }
}

namespace ShipDock.Applications
{

    /// <summary>
    /// 
    /// 热更UI交互器
    /// 
    /// 用于贯通 UIModularHotFixer（热更版UI模块）、HotFixerUI（热更版UI）、HotFixerUIAgent（界面热更代理组件）的对接与交互
    /// 并通过其子类实现UI的热更功能
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class HotFixBaseUI : HotFixerInteractor
    {
        public INotificationSender UINotificationSender()
        {
            return Agent as INotificationSender;
        }

        public void Close(bool isDestroy)
        {
            UIModular.Name.Close(isDestroy);
        }
    }
}
