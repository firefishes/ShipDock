using System;

namespace ShipDock
{
    public class ComponentBridge : IReclaim
    {
        private Action mOnStarted;

        public void Reclaim()
        {
            mOnStarted = default;
        }

        public ComponentBridge(Action callback = default)
        {
            OnStarted(callback);
        }

        public void OnStarted(Action callback)
        {
            if (callback != default)
            {
                mOnStarted += callback;
            }
            else { }
        }

        public void Start()
        {
            Framework.Instance.DuringCurrentFrame(mOnStarted);
        }

        //private void OnAppStart()
        //{
            //ICustomFramework app = Framework.Instance.App;
            //if (app.UpdatesComponent != default)
            //{
            //    mOnStarted?.Invoke();
            //}
            //else
            //{
            //    app.MergeCallOnMainThread += mOnStarted;
            //}
        //    mOnStarted?.Invoke();
        //}
    }

}
