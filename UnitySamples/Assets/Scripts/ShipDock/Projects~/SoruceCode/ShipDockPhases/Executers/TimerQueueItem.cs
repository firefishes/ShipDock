﻿using ShipDock.Interfaces;
using System;

namespace ShipDock.Tools
{
    /// <summary>
    /// 
    /// 时间队列项
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class TimerQueueItem : IQueueExecuter
    {

        public Action mOnComplete;
        
        //private TimeUpdater mTimeUpdater;

        public Action ActionUnit { get; set; }
        public QueueNextUnit OnNextUnit { get; set; }
        public QueueUnitCompleted OnUnitCompleted { get; set; }
        public QueueUnitExecuted OnUnitExecuted { get; set; }
        public bool ImmediatelyCommitNext { get; set; }
        public bool IgnoreInQueue { get; set; }

        public TimerQueueItem() { }

        public TimerQueueItem(float time, Action complete = null)
        {
            Reinit(time, complete);
        }

        public void Reinit(float time, Action complete = null)
        {
            if (complete != default)
            {
                mOnComplete += complete;
            }
            //mTimeUpdater = TimeUpdater.GetTimUpdater(time, TimerComplete);
        }

        public void Revert()
        {
            //Utils.Reclaim(mTimeUpdater);
            //mTimeUpdater = default;

            OnNextUnit = null;
            OnUnitExecuted = null;
            OnUnitCompleted = null;
            mOnComplete = null;
        }

        #region 执行队列单元的实现代码
        private bool mIsDispose;

        public void Dispose()
        {
            if (mIsDispose)
            {
                return;
            }

            mIsDispose = true;

            Revert();
        }

        public void Commit()
        {
            //mTimeUpdater.Start();
        }

        private void TimerComplete()
        {
            mOnComplete?.Invoke();
            QueueNext();
        }

        public void QueueNext()
        {
            //Utils.Reclaim(mTimeUpdater);
            //mTimeUpdater = default;
            mOnComplete = default;
            OnNextUnit?.Invoke(this);//让所在的队列执行器执行下一个队列单元
        }

        public int QueueSize
        {
            get
            {
                return 1;
            }
        }
        #endregion
    }

}