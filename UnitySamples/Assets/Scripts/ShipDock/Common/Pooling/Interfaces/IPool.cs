using System;

namespace ShipDock
{

    /// <summary>
    /// 
    /// 对象池接口
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public interface IPool<T> : IReclaim, IPoolBase
    {
        T FromPool(Func<T> creater = null);
        void ToPool(T target);
        int UsedCount { get; }
    }
}