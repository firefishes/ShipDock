
using System;

namespace HotFixClient
{
    /// <summary>
    /// 完全热更（包含资源版本比对至正式进入游戏）模式下的启动器类
    /// </summary>
    public class GameRunner : HotFixRunner
    {
        protected override string GetLoadingUIABName()
        {
            return "ui/ui_loading";
        }

        protected override void OpenDownloadNewestAppUIMudular(Action<bool> callback)
        {
        }

        protected override IStartupLoadingUIModular OpenStartupLoadingUIModular()
        {
            //return Consts.UIM_STARTUP_LOADING.Open<UIStartUpLoadingModular>();
            return default;

        }
    }
}
