﻿using ShipDock.Interfaces;
using System;

namespace ShipDock.Applications
{
    public class ComponentBridge : IReclaim
    {
        private Action mOnStarted;

        public void Reclaim()
        {
            mOnStarted = default;
        }

        public ComponentBridge(Action callback = null)
        {
            mOnStarted = callback;
        }

        public void Start()
        {
            Framework.Instance.AddStart(OnAppStart);
        }

        private void OnAppStart()
        {
            ICustomFramework app = Framework.Instance.App;
            if (app.UpdatesComponent != default)
            {
                mOnStarted?.Invoke();
            }
            else
            {
                app.MergeCallOnMainThread += mOnStarted;
            }
        }
    }

}
