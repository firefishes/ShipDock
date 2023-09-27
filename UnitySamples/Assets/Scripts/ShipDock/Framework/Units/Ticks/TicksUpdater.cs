using System;
using System.Collections.Generic;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 心跳帧更新器
    /// 
    /// </summary>
    public class TicksUpdater : INotificationSender
    {
        public const int TICKS_FIXED_UPDATE = 0;
        public const int TICKS_UPDATE = 1;
        public const int TICKS_LATE_UPDATE = 2;

        private int mSize;
        private int mIndex;
        private int mUpdateMSec;
        private int mUpdateCountMSec;
        private int mFixedUpdateMSec;
        private int mFixedUpdateCountMSec;
        private bool mEnable;
        private bool mIsUpdateLate;
        private bool mIsFixedUpdate;
        private bool mIsDisposed;
        private IUpdate mItem;
        private IUpdate mItemAdded;
        private IUpdate mItemRemoved;
        private List<IUpdate> mTicksList;
        private List<IUpdate> mListDeleted;
        private SyncUpdater mSyncUpdater;
        private ThreadTicks mThreadTicks;
        private UpdaterNotice mNoticeAdded;
        private UpdaterNotice mNoticeRemoved;

        public bool Enable
        {
            set
            {
                mEnable = value;
                if (mEnable)
                {
                    Enabled();
                }
                else
                {
                    Disabled();
                }
            }
            get
            {
                return mEnable;
            }
        }

        public float DeltaTime
        {
            get
            {
                return LastRunTime - RunTime;
            }
        }

        public float UpdateTime { get; private set; }
        public float FixedUpdateTime { get; private set; }
        public float RunTime { get; private set; }
        public float LastRunTime { get; private set; }

        public TicksUpdater(int tickTime, float fixedUpdateTime = 0.01f)
        {
            mThreadTicks = new ThreadTicks(tickTime);
            mThreadTicks.Add(ThreadUpdating);
            mThreadTicks.Start();

            UpdateTime = mThreadTicks.SleepTime * ThreadTicks.UNIT_MSEC_F;
            mUpdateMSec = mThreadTicks.SleepTime;
            mUpdateCountMSec = mUpdateMSec;

            FixedUpdateTime = fixedUpdateTime;
            mFixedUpdateMSec = (int)(fixedUpdateTime * ThreadTicks.UNIT_SEC);
            mFixedUpdateCountMSec = mFixedUpdateMSec;

            mSyncUpdater = new SyncUpdater();
            mTicksList = new List<IUpdate>();
            mListDeleted = new List<IUpdate>();

            Enabled();
        }

        public void Reclaim()
        {
            mIsDisposed = true;
            ShipDockConsts.NOTICE_REMOVE_UPDATE.Remove(RemoveUpdate);
            ShipDockConsts.NOTICE_ADD_UPDATE.Remove(AddUpdate);
            
            Utils.Reclaim(ref mTicksList);
            Utils.Reclaim(ref mListDeleted);
            Utils.Reclaim(mSyncUpdater);
            Utils.Reclaim(mThreadTicks);

            mItem = null;
            mItemAdded = null;
            mItemRemoved = null;
            mNoticeAdded = null;
            mNoticeRemoved = null;
            mSyncUpdater = null;
            mThreadTicks = null;
        }

        private void Enabled()
        {
            if (mIsDisposed)
            {
                return;
            }
            else { }

            ShipDockConsts.NOTICE_REMOVE_UPDATE.Add(RemoveUpdate);
            ShipDockConsts.NOTICE_ADD_UPDATE.Add(AddUpdate);
            ShipDockConsts.NOTICE_FRAME_UPDATER_COMP_READY.Broadcast();
        }

        private void Disabled()
        {
            if (mIsDisposed)
            {
                return;
            }
            else { }

            ShipDockConsts.NOTICE_REMOVE_UPDATE.Remove(RemoveUpdate);
            ShipDockConsts.NOTICE_ADD_UPDATE.Remove(AddUpdate);
        }

        public void RefreshThreadSleepTime(int newTime)
        {
            mThreadTicks.RefreshThreadSleepTime(newTime);

            UpdateTime = mThreadTicks.SleepTime * ThreadTicks.UNIT_MSEC_F;
            mUpdateMSec = mThreadTicks.SleepTime;
            mUpdateCountMSec = mUpdateMSec;
        }

        private void AddUpdaterItem()
        {
            if (mIsDisposed || mItemAdded == null)
            {
                return;
            }
            else { }

            if ((mTicksList != null) && (mTicksList.IndexOf(mItemAdded) == -1))
            {
                mTicksList.Add(mItemAdded);
                mItemAdded.AfterAddUpdate();
            }
            else { }

            if ((mListDeleted != null) && mListDeleted.Contains(mItemAdded))
            {
                mListDeleted.Remove(mItemAdded);//清除之前添加过的移除刷帧标记，避免最新的队列标记不生效
            }
            else { }

            mItemAdded = null;
        }

        private void RemoveUpdaterItem()
        {
            if (mIsDisposed)
            {
                return;
            }
            else { }

            if ((mItemRemoved != null) && (mListDeleted != null) && (mListDeleted.IndexOf(mItemRemoved) == -1))
            {
                mListDeleted.Add(mItemRemoved);//加入删除列表，下一次帧周期中统一移除
            }
            else { }
        }

        /// <summary>添加一个需要刷帧的对象</summary>
        protected virtual void AddUpdate(INoticeBase<int> param)
        {
            mNoticeAdded = param as UpdaterNotice;
            if ((mNoticeAdded == null) || 
                (mNoticeAdded.ParamValue == null) || 
                (mNoticeAdded.NotifcationSender != null && !mNoticeAdded.CheckSender(this)))
            {
                return;
            }
            else { }

            mItemAdded = mNoticeAdded.ParamValue;
            AddUpdaterItem();
            mNoticeAdded = null;
        }

        /// <summary>移除一个需要刷帧的对象</summary>
        protected virtual void RemoveUpdate(INoticeBase<int> param)
        {
            mNoticeRemoved = param as UpdaterNotice;
            if (mNoticeRemoved != null || 
                mNoticeRemoved.ParamValue == null ||
                (mNoticeAdded.NotifcationSender != null && !mNoticeAdded.CheckSender(this)))
            {
                return;
            }
            else { }

            mItemRemoved = mNoticeRemoved.ParamValue;
            RemoveUpdaterItem();
            mNoticeRemoved = null;
        }

        /// <summary>检测一个刷帧对象是否有效</summary>
        private bool IsValidUpdate(IUpdate target)
        {
            return (target != default) && (mListDeleted != null) && !mListDeleted.Contains(target);
        }
        
        private void ThreadUpdating(int millisecond)
        {
            if (mIsDisposed)
            {
                return;
            }
            else { }

            RunTime += millisecond;

            CheckRemoveUpdate();

            float dTime = millisecond * ThreadTicks.UNIT_MSEC_F;
            mSyncUpdater?.Update(dTime);

            WalkUpdateItems(millisecond, TICKS_FIXED_UPDATE);
            WalkUpdateItems(millisecond, TICKS_UPDATE);
            WalkUpdateItems(millisecond, TICKS_LATE_UPDATE);

            LastRunTime = RunTime;
        }

        private void WalkUpdateItems(int millisecond, int methodType)
        {
            mIndex = 0;
            mSize = (mTicksList != default) ? mTicksList.Count : 0;

            TimeAdvanced(millisecond, methodType);

            for (mIndex = 0; mIndex < mSize; mIndex++)
            {
                if(mTicksList == null)
                {
                    break;
                }
                else { }

                mItem = mTicksList[mIndex];
                if (IsValidUpdate(mItem))
                {
                    CallUpdateMethodByType(millisecond, methodType);
                }
                else { }
            }

            mIsFixedUpdate = false;
            mIsUpdateLate = false;
        }

        private void TimeAdvanced(int millisecond, int methodType)
        {
            switch (methodType)
            {
                case TICKS_FIXED_UPDATE:
                    mFixedUpdateCountMSec -= millisecond;
                    if (mFixedUpdateCountMSec <= 0)
                    {
                        mFixedUpdateCountMSec += mFixedUpdateMSec;
                        mIsFixedUpdate = true;
                    }
                    else { }
                    break;
                case TICKS_UPDATE:
                    mUpdateCountMSec -= millisecond;
                    if (mUpdateCountMSec <= 0)
                    {
                        mUpdateCountMSec += mUpdateMSec;
                        mIsUpdateLate = true;
                    }
                    else { }
                    break;
            }
        }

        private void CallUpdateMethodByType(int millisecond, int methodType)
        {
            float dTime;
            switch(methodType)
            {
                case TICKS_FIXED_UPDATE:
                    if(mIsFixedUpdate && mItem.IsFixedUpdate)
                    {
                        dTime = mFixedUpdateMSec * ThreadTicks.UNIT_MSEC_F;
                        mItem.OnFixedUpdate(dTime);
                        //UnityEngine.Debug.Log(mFixedUpdateCountTime);
                    }
                    else { }
                    break;
                case TICKS_UPDATE:
                    if(mIsUpdateLate && mItem.IsUpdate)
                    {
                        dTime = mUpdateMSec * ThreadTicks.UNIT_MSEC_F;
                        mSyncUpdater?.Update(dTime);
                        mItem?.OnUpdate(dTime);
                    }
                    else { }
                    break;
                case TICKS_LATE_UPDATE:
                    if(mIsUpdateLate && mItem.IsLateUpdate)
                    {
                        mItem.OnLateUpdate();
                    }
                    else { }
                    break;
            }
        }

        public void CallLater(Action<float> method)
        {
            mSyncUpdater.CallLater(method);
        }

        /// <summary>检测已被标记为移除的刷帧对象</summary>
        protected void CheckRemoveUpdate()
        {
            mSize = mListDeleted != default ? mListDeleted.Count : 0;
            if (mSize > 0)
            {
                for (mIndex = 0; mIndex < mSize; mIndex++)
                {
                    mItem = mListDeleted[mIndex];
                    mTicksList.Remove(mItem);
                    mItem.AfterRemoveUpdate();
                }
                mListDeleted.Clear();
            }
            else { }
        }
    }
}
