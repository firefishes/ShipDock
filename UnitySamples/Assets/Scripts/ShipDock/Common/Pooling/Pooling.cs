#define _G_LOG

using System;
using System.Collections.Generic;

namespace ShipDock
{

    /// <summary>
    /// 
    /// 泛型对象池
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class Pooling<T> : IPool<T> where T : class, IPoolable, new()
    {
        #region 静态成员
        private static Pooling<T> instance;

        public static Pooling<T> Instance
        {
            get
            {
                CheckPoolNull(null);
                return instance;
            }
        }

        public static Pooling<T> InitByCustom(Pooling<T> customPool, Func<T> func)
        {
            CheckPoolNull(customPool, func);
            return instance;
        }

        private static void CheckPoolNull(Pooling<T> customPool, Func<T> func = default)
        {
            if (instance == null)
            {
                if (customPool != null)
                {
                    instance = customPool;
                }
                else
                {
                    instance = new Pooling<T>(func ?? (() => { return new T(); }));
                }
            }
        }

        public static T From(Pooling<T> customPool = default)
        {
            CheckPoolNull(customPool);
            return instance.FromPool();
        }

        public static void To(T target, Pooling<T> customPool = default)
        {
            //"pool type error".Log((target != default) && (typeof(T).FullName != target.GetType().FullName), target.GetType().FullName);
            CheckPoolNull(customPool);
            instance.ToPool(target);
        }

#if UNITY_EDITOR
        public static bool IsUsed(T target)
        {
            CheckPoolNull(default);
            return instance.CheckUsed(target);
        }
#endif
#endregion

        private readonly string mPoolTypeName = typeof(T).FullName;

        private object mLock;
        private Stack<T> mPool;
        private Func<T> mCreater;
        private bool mIsAddResetCallback = true;

        /// <summary>获取当前对象池中对象的数量</summary>
        public int UsedCount { get; private set; }

        /// <summary>对象池构造函数</summary>
        public Pooling(Func<T> customCreater = default, Stack<T> pool = default)
        {
            if (instance == default)
            {
                instance = this;
            }
            else { }

            if (pool != default)
            {
                mPool = pool;
            }
            else
            {
                mPool = new Stack<T>();
            }

            mCreater = customCreater;

            if (mIsAddResetCallback)
            {
                AllPools.AddReset(ClearPool);
            }
            else { }

            mLock = new object();

#if UNITY_EDITOR
            const string format = "Pooling {0} created";
            string log = string.Format(format, mPoolTypeName);
            "log".Log(log);
#endif
        }

        /// <summary>销毁对象池</summary>
        public virtual void Reclaim()
        {
            ClearPool();
            mCreater = default;
            mPool = default;
            mLock = default;
            instance = default;
        }

        private int mInstanceCount = 0;

        /// <summary>获取一个对象</summary>
        public virtual T FromPool(Func<T> creater = default)
        {
            lock (mLock)
            {
                T result = default;
                if (mInstanceCount > 0 && mPool.Count > 0)
                {
                    mInstanceCount--;
                    result = mPool.Pop();
                }
                else
                {
                    if (creater != default)
                    {
                        result = creater();
                    }
                    else
                    {
                        result = (mCreater != default) ? mCreater() : new T();
                    }
                }
                UsedCount++;

                if (result == default)
                {
                    result = new T();
                }
                else { }

                return result;
            }
        }

        /// <summary>重置并归还一个对象</summary>
        public virtual void ToPool(T target)
        {
            if (mPool == default)
            {
                return;
            }
            else { }

            target.Revert();

#if UNITY_EDITOR
            if (mPool.Contains(target))
            {
                const string format = "Pooling {0} try revert an instance one more time.";
                string log = string.Format(format, mPoolTypeName);
                "error".Log(log);
            }
            else
            {
#endif
                mPool.Push(target);
                mInstanceCount++;
                UsedCount--;
#if UNITY_EDITOR
            }
#endif
        }

#if UNITY_EDITOR
        /// <summary>检测一个对象是否正在使用</summary>
        public bool CheckUsed(T target)
        {
            if (target == default)
            {
                return false;
            }
            else { }

            bool result = true;
            if (mPool != default)
            {
                if (mPool.Contains(target))
                {
                    result = false;
                }
                else { }
            }
            else { }
            return result;
        }
#endif

        /// <summary>重置池中所有对象</summary>
        public void ClearPool()
        {
            if (mPool != default)
            {
                int max = mPool.Count;
                for (int i = 0; i < max; i++)
                {
                    T item = mPool.Pop();
                    item.Revert();
                    if (item is IDisposable)
                    {
                        (item as IDisposable).Dispose();
                    }
                    else { }
                }
                mPool.Clear();

#if UNITY_EDITOR
                const string format = "Pooling {0} clear";
                string log = string.Format(format, mPoolTypeName);
                "log".Log(log);
#endif
            }
            else { }
            UsedCount = 0;
        }

        public IPoolable Create()
        {
            return FromPool();
        }

        public void Reserve(ref IPoolable item)
        {
            ToPool((T)item);
        }
    }
}