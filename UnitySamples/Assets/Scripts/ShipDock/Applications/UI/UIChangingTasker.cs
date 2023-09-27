using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock
{
    public class UIChangingTasker
    {

        private string mCurrentTaskName;
        private Action<TimeGapper> mTaskCallback;
        private List<string> mStartedChangeTask;
        private List<int> mFinishTaskIndex;
        private List<TimeGapper> mChangeTimes;
        private KeyValueList<string, Action<TimeGapper>> mChangeHandlers;

        private UI Owner { get; set; }

        public UIChangingTasker(UI target)
        {
            Owner = target;

            mStartedChangeTask = new List<string>();
            mFinishTaskIndex = new List<int>();
            mChangeTimes = new List<TimeGapper>();
            mChangeHandlers = new KeyValueList<string, Action<TimeGapper>>();
        }

        public void Clean()
        {
            mStartedChangeTask?.Clear();
            mFinishTaskIndex?.Clear();
            mChangeTimes = default;

            string key;
            int max = mChangeHandlers.Keys.Count;
            for (int i = 0; i < max; i++)
            {
                key = mChangeHandlers.Keys[i];
                mChangeHandlers[key] = default;
            }

            Owner = default;
        }

        public bool HasTask(string taskName)
        {
            return mStartedChangeTask.Contains(taskName);
        }

        public bool IsTaskStoped(string taskName)
        {
            bool result = false;
            if (mStartedChangeTask.Contains(taskName))
            {
                int index = mStartedChangeTask.IndexOf(taskName);
                if (index != -1)
                {
                    if (!mFinishTaskIndex.Contains(index))
                    {
                        mFinishTaskIndex.Add(index);
                        TimeGapper timeGapper = mChangeTimes[index];
                        result = timeGapper.IsFinised;
                    }
                    else { }
                }
                else { }
            }
            else { }
            return result;
        }

        public void StopChangeTask(string taskName)
        {
            if (mStartedChangeTask.Contains(taskName))
            {
                int index = mStartedChangeTask.IndexOf(taskName);
                if (index != -1)
                {
                    if (!mFinishTaskIndex.Contains(index))
                    {
                        mFinishTaskIndex.Add(index);
                        TimeGapper timeGapper = mChangeTimes[index];
                        timeGapper.Stop();

                        mChangeTimes[index] = timeGapper;
                    }
                    else { }
                }
                else { }
            }
            else { }
        }

        public void AddChangeTask(string taskName, float duringTime, Action<TimeGapper> handler)
        {
            TimeGapper timeGapper;
            if (mStartedChangeTask.Contains(taskName))
            {
                int index = mStartedChangeTask.IndexOf(taskName);
                mFinishTaskIndex.Remove(index);

                timeGapper = mChangeTimes[index];
                timeGapper.Start(duringTime);
                mChangeTimes[index] = timeGapper;
            }
            else
            {
                timeGapper = new TimeGapper();
                timeGapper.Start(duringTime);
                mChangeTimes.Add(timeGapper);

                mStartedChangeTask.Add(taskName);
                mChangeHandlers[taskName] = handler;
            }
            Owner.UIChanged = true;
        }

        public void UpdateUITasks()
        {
            if (Owner != default)
            {
                bool isChangeFinish = false;
                int max = mStartedChangeTask.Count;

                TimeGapper gapper;
                for (int i = 0; i < max; i++)
                {
                    if (mFinishTaskIndex.IndexOf(i) == -1)
                    {
                        gapper = mChangeTimes[i];
                        isChangeFinish = gapper.TimeAdvanced(Time.deltaTime);
                        mChangeTimes[i] = gapper;
                        if (isChangeFinish)
                        {
                            mFinishTaskIndex.Add(i);
                        }
                        else
                        {
                            mCurrentTaskName = mStartedChangeTask[i];
                        }
                        mTaskCallback = mChangeHandlers[mCurrentTaskName];
                        mTaskCallback?.Invoke(gapper);
                        mTaskCallback = default;
                    }
                    else { }
                }

                Owner.UIChanged = !isChangeFinish;
            }
            else { }
        }

    }
}