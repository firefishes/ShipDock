using System;
using System.Collections.Generic;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 双缓冲队列更新器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleBuffers<T> : IReclaim
    {
        private bool mIsDisposed;
        private bool mIsFront;
        private Queue<T> mCacheFront;
        private Queue<T> mCacheBack;
        private Queue<T> mCache;
        private Queue<T> mEnqueueCache;

        public T Current { get; private set; }
        public Action<float, T> OnDequeue { get; set; }

        public bool HasQueueItem
        {
            get
            {
                return mCache != default ? (mCache.Count > 0) : false;
            }
        }

        public DoubleBuffers()
        {
            mCacheFront = new Queue<T>();
            mCacheBack = new Queue<T>();
            mCache = mCacheFront;
            mEnqueueCache = mCacheBack;
            mIsFront = true;
        }

        public virtual void Reclaim()
        {
            mIsDisposed = true;
            Current = default;
            OnDequeue = default;
            Utils.Reclaim(ref mCacheFront);
            Utils.Reclaim(ref mCacheBack);
            Utils.Reclaim(ref mCache);
        }

        public void Advance(float dTime)
        {
            if (mCache != default)
            {
                int max = mCache.Count;
                if (max > 0)
                {
                    while (HasQueueItem)
                    {
                        Current = mCache.Dequeue();
                        OnDequeue?.Invoke(dTime, Current);
                    }
                }
                else { }
            }
            else { }

            Current = default;
        }

        public void UpdateBuffer(float dTime)
        {
            if (mIsDisposed) { }
            else
            {
                mCache = mIsFront ? mCacheBack : mCacheFront;//切换到需要处理的队列
                mEnqueueCache = mIsFront ? mCacheFront : mCacheBack;

                Advance(dTime);

                mIsFront = !mIsFront;
            }
        }

        /// <summary>
        /// 添加在下一帧需要执行的队列项
        /// </summary>
        public void Enqueue(T target, bool isCheckContains = true)
        {
            if (mIsDisposed) { }
            else
            {
                if (mEnqueueCache != default)
                {
                    if (isCheckContains)
                    {
                        if (!mEnqueueCache.Contains(target))
                        {
                            mEnqueueCache.Enqueue(target);
                        }
                        else { }
                    }
                    else
                    {
                        mEnqueueCache.Enqueue(target);
                    }
                }
                else { }
            }
        }
    }
}
