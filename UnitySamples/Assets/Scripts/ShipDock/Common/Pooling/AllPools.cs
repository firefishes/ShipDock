using System;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 对象池工具静态类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public static class AllPools
    {
        private static Action onResetAllPooling;

        public static void ResetAllPooling()
        {
            onResetAllPooling?.Invoke();
        }

        public static void AddReset(Action onClearPool)
        {
            onResetAllPooling += onClearPool;
        }
    }
}