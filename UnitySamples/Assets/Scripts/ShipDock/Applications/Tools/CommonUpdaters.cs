﻿
using ShipDock.Commons;
using ShipDock.Notices;
using ShipDock.Tools;
using System;

namespace ShipDock.Applications
{
    public class CommonUpdaters
    {
        /// <summary>普通帧更新器的映射</summary>
        private KeyValueList<Action<int>, MethodUpdater> mUpdaterMapper;
        private KeyValueList<Action, MethodUpdater> mLateUpdaterMapper;

        public CommonUpdaters()
        {
            mUpdaterMapper = new KeyValueList<Action<int>, MethodUpdater>();
            mLateUpdaterMapper = new KeyValueList<Action, MethodUpdater>();
        }

        public void Clean()
        {
            MethodUpdater updater;
            int max = mUpdaterMapper.Size;
            for (int i = 0; i < max; i++)
            {
                updater = mUpdaterMapper.GetValueByIndex(i);
                UpdaterNotice.RemoveSceneUpdater(updater);
                updater.Dispose();
            }
            max = mUpdaterMapper.Size;
            for (int i = 0; i < max; i++)
            {
                updater = mLateUpdaterMapper.GetValueByIndex(i);
                UpdaterNotice.RemoveSceneUpdater(updater);
                updater.Dispose();
            }
            mUpdaterMapper?.Clear();
            mLateUpdaterMapper?.Clear();
        }

        public void AddUpdate(Action<int> method)
        {
            if (!mUpdaterMapper.ContainsKey(method))
            {
                MethodUpdater updater = new MethodUpdater
                {
                    Update = method
                };
                mUpdaterMapper[method] = updater;
                UpdaterNotice.AddSceneUpdater(updater);
            }
            else { }
        }

        public void AddFixedUpdate(Action<int> method)
        {
            if (!mUpdaterMapper.ContainsKey(method))
            {
                MethodUpdater updater = new MethodUpdater
                {
                    FixedUpdate = method,
                    IsFixedUpdate = true,
                    IsUpdate = false,
                };
                mUpdaterMapper[method] = updater;
                UpdaterNotice.AddSceneUpdater(updater);
            }
            else { }
        }

        public void AddLateUpdate(Action method)
        {
            if (!mLateUpdaterMapper.ContainsKey(method))
            {
                MethodUpdater updater = new MethodUpdater
                {
                    LateUpdate = method,
                    IsLateUpdate = true,
                    IsUpdate = false,
                };
                mLateUpdaterMapper[method] = updater;
                UpdaterNotice.AddSceneUpdater(updater);
            }
            else { }
        }

        public void RemoveUpdate(Action<int> method)
        {
            if (mUpdaterMapper.ContainsKey(method))
            {
                MethodUpdater updater = mUpdaterMapper.GetValue(method, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
                updater.Dispose();
            }
            else { }
        }

        public void RemoveUpdate(Action method)
        {
            if (mLateUpdaterMapper.ContainsKey(method))
            {
                MethodUpdater updater = mLateUpdaterMapper.GetValue(method, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
                updater.Dispose();
            }
            else { }
        }
    }
}