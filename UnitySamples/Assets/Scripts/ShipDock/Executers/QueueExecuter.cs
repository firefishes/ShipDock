using System;
using System.Collections.Generic;

namespace ShipDock
{
    public interface IQueueExecuterCollection
    {
        void Executed();
        void QueueCompleted();
    }

    /// <summary>
    /// 队列执行器
    /// 
    /// add by Minghua.ji
    /// 
    /// 若干个队列单元以一个顺序组成队列依次执行
    /// 队列单元支持范围包括：函数、实现了IQueueExecuter接口、继承此类的子类实例
    /// 
    /// </summary>
    public class QueueExecuter : IQueueExecuter, IQueueExecuterCollection
    {
        /// <summary>执行下一个队列单元时触发的回调</summary>
        public QueueNextUnit OnNextUnit { set; get; }
        /// <summary>队列单元已被执行的回调</summary>
        public QueueUnitExecuted OnUnitExecuted { set; get; }
        /// <summary>全部队列执行完毕的回调</summary>
        public QueueUnitCompleted OnUnitCompleted { set; get; }
        /// <summary>此执行项是否可被忽略</summary>
        public bool IgnoreInQueue { get; set; }

        /// <summary>是否自动销毁，开启自动销毁功能会在队列完成时自动销毁各个队列单元</summary>
        private bool mAutoDispose;
        /// <summary>当前执行到的索引</summary>
        private int mCurrentIndex;
        /// <summary>当前正在执行的执行器单元</summary>
        private IQueueExecuter mCurrent;
        /// <summary>流程队列</summary>
        private List<IQueueExecuter> mQueue;
        /// <summary>已执行的流程队列</summary>
        private List<IQueueExecuter> mQueueExecuted;
        /// <summary>方法单元</summary>
        private Queue<Action> mActionUnits;

        /// <summary>获取当前执行队列的执行位置</summary>
        public virtual int CurrentIndex
        {
            get
            {
                return mCurrentIndex;
            }
        }

        public virtual int QueueSize
        {
            get
            {
                return (mActionUnits != null) ? mActionUnits.Count : 0;
            }
        }

        public bool isRunning { get; private set; }
        public bool IsDisposed { get; private set; }
        public bool IsDisposQueueItem { get; set; }
        public Action ActionUnit { get; set; }

        /// <summary>构筑一个执行队列</summary>
        public QueueExecuter(bool autoDispose = true)
        {
            mQueue = new List<IQueueExecuter>();
            mQueueExecuted = new List<IQueueExecuter>();
            mActionUnits = new Queue<Action>();
            mAutoDispose = autoDispose;

            Init();
        }
        
        /// <summary>销毁</summary>
        public virtual void Reclaim()
        {
            if (IsDisposed)
            {
                return;//不重复销毁，否则会引起循环调用
            }
            else { }

            IsDisposed = true;
            ClearQueue(true, true);

#if ILRUNTIME
            ReclaimQueues(IsDisposQueueItem);
            mQueue?.Clear();
            mQueueExecuted?.Clear();
#else
            Utils.Reclaim(ref mQueue, true, IsDisposQueueItem);
            Utils.Reclaim(ref mQueueExecuted, true, IsDisposQueueItem);
#endif

            mQueue = default;
            mQueueExecuted = default;

            CleanEvents();
        }

        private void CleanEvents()
        {
            UnityEngine.Debug.Log("Queue finihed " + this);
            OnNextUnit = default;
            OnUnitExecuted = default;
            OnUnitCompleted = default;
            ActionUnit = default;
        }

#if ILRUNTIME
        private void ReclaimQueues(bool isDisposeItem)
        {
            if (isDisposeItem)
            {
                IDispose item;
                if (mQueue != default)
                {
                    while (mQueue.Count > 0)
                    {
                        item = mQueue[0];
                        item?.Dispose();
                        mQueue.RemoveAt(0);
                    }
                }
                else { }

                if (mQueueExecuted != default)
                {
                    while (mQueueExecuted.Count > 0)
                    {
                        item = mQueueExecuted[0];
                        item?.Dispose();
                        mQueueExecuted.RemoveAt(0);
                    }
                }
                else { }

                mActionUnits?.Clear();
            }
            else { }
        }
#endif

        public void ClearWithoutDispose()
        {
            isRunning = false;

#if ILRUNTIME
            mQueue?.Clear();
            mQueueExecuted?.Clear();
            mActionUnits?.Clear();
#else
            Utils.Reclaim(ref mQueue, false);
            Utils.Reclaim(ref mQueueExecuted, false);
#endif
        }
        
        /// <summary>清理队列</summary>
        public void ClearQueue(bool isDispose = false, bool isDisposeQueue = false)
        {
            isRunning = false;
#if ILRUNTIME
            ReclaimQueues(isDisposeQueue);
            mQueue?.Clear();
            mQueueExecuted?.Clear();
            mActionUnits?.Clear();
#else
            Utils.Reclaim(ref mQueue, isDispose, isDispose);
            Utils.Reclaim(ref mQueueExecuted, isDispose, isDispose);
#endif
            if (isDispose)
            {
                mQueue = default;
                mQueueExecuted = default;

                CleanEvents();
            }
            else { }

            Init();
        }

        /// <summary>初始化</summary>
        private void Init()
        {
            mCurrentIndex = 0;

            mCurrent = default;
            isRunning = false;
            IsDisposed = false;
        }

        /// <summary>销毁后重置队列</summary>
        public void ResetAfterDispose(bool autoDispose = true)
        {
            Reclaim();

            mQueue = new List<IQueueExecuter>();
            mQueueExecuted = new List<IQueueExecuter>();
            mActionUnits = new Queue<Action>();

            mAutoDispose = autoDispose;

            Init();
        }

        /// <summary>将一个流程执行器增加到队列末尾</summary>
        public void Add(IQueueExecuter target)
        {
            mQueue.Add(target);
            mActionUnits.Enqueue(default);
        }

        /// <summary>添加元素</summary>
        public void Add(Action method)
        {
            if (method != default)
            {
                mActionUnits.Enqueue(method);
            }
            else
            {
                mActionUnits.Enqueue(() => { });
            }
        }

        /// <summary>重置</summary>
        public virtual void Reset()
        {
            ClearQueue(false, true);
        }
        
        /// <summary>执行队列中的下一个执行器</summary>
        protected void ExecuteNext()
        {
            if (IgnoreInQueue)
            {
                QueueNext();
            }
            else
            {
                if (HasNext())
                {
                    Executing();
                }
                else
                {
                    if (isRunning)
                    {
                        isRunning = false;
                    }
                    else { }

                    Executed();//本执行单元已执行
                    QueueCompleted();//队列执行完成
                    QueueNext();//继续启动外部队列的执行器

                    if (mAutoDispose)
                    {
                        Reclaim();
                    }
                    else { }
                }
            }
        }

        private void Executing()
        {
            if (mQueueExecuted == default || mActionUnits == default || mQueue == default)
            {
                return;
            }
            else { }

            mCurrentIndex++;
            Action methodUnit = mActionUnits.Dequeue();
            if (methodUnit == default)
            {
                mCurrent = mQueue[0];//设置下一个执行单元
                mQueue.RemoveAt(0);

                if (mCurrent != default)
                {
                    mCurrent.OnNextUnit += OnNext;//衔接上下子项的执行顺序
                    mCurrent.Commit();//执行子项
                }
                else
                {
                    ExecuteNext();
                }

                mQueueExecuted?.Add(mCurrent);
            }
            else
            {
                methodUnit.Invoke();
                Executing();
            }
        }

        private bool HasNext()
        {
            return (mActionUnits != default) && (mActionUnits.Count > 0);
        }

        /// <summary>衔接队列中下一个执行器事件处理函数的执行，构成队列的自动运行结构</summary>
        protected void OnNext(IQueueExecuter param)
        {
            param.OnNextUnit -= OnNext;
            ExecuteNext();
        }

        /// <summary>运行队列执行器的入口</summary>
        public virtual void Commit()
        {
            if (isRunning)
            {
                return;
            }
            else { }

            isRunning = true;
            IsDisposed = false;
            if (mAutoDispose || (QueueSize == 0))
            {
                mCurrentIndex = 0;
            }
            else { }

            ExecuteNext();
        }

        /// <summary>主动调用，执行此对象所在队列的下一个队列元素</summary>
        public virtual void QueueNext()
        {
            OnNextUnit?.Invoke(this);
        }

        public void Executed()
        {
            OnUnitExecuted?.Invoke(this);
        }

        public void QueueCompleted()
        {
            OnUnitCompleted?.Invoke(this);
        }

        public IQueueExecuter Current()
        {
            return mCurrent;
        }

    }
}