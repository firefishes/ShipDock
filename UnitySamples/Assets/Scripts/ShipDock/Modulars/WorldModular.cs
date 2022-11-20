using ShipDock.Commons;
using ShipDock.Modulars;
using ShipDock.Notices;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public abstract class WorldModular : QueueableNoticesModular
    {
        private int mAddToWorldMessage;
        private int mRemoveFromWorldMessage;
        private MethodUpdater mWorldUpdater;
        private IParamNotice<IUpdate> mUpdaterParamNotice;
        private IUpdate mElement;
        private List<IUpdate> mElementDeleteds;
        private List<IUpdate> mElementUpdaters;

        public WorldModular(int modularName, int addToMessage, int removeFromMessage) : base(modularName)
        {
            mAddToWorldMessage = addToMessage;
            mRemoveFromWorldMessage = removeFromMessage;

            mElementDeleteds = new List<IUpdate>();
            mElementUpdaters = new List<IUpdate>();
            mWorldUpdater = new MethodUpdater();
        }

        public override void InitModular()
        {
            base.InitModular();

            InitMessageSettles();

            mWorldUpdater = new MethodUpdater()
            {
                Update = OnWorldUpdate,
                LateUpdate = OnWorldLateUpdate,
                FixedUpdate = OnWorldFixedUpdate,
            };

            UpdaterNotice.AddSceneUpdater(mWorldUpdater);
        }

        protected virtual void InitMessageSettles()
        {
            SetMessageSettle(mAddToWorldMessage, OnAddUpdater);
            SetMessageSettle(mRemoveFromWorldMessage, OnRemoveUpdater);
        }

        private void OnWorldUpdate(int dTime)
        {
            int max = mElementDeleteds.Count;
            if (max > 0)
            {
                for (int i = max; i >= 0; i--)
                {
                    mElement = mElementDeleteds[i];
                    mElementUpdaters.Remove(mElement);
                }
                mElementDeleteds.Clear();
            }
            else { }

            max = mElementUpdaters.Count;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mElement = mElementUpdaters[i];
                    if (mElement.IsUpdate)
                    {
                        mElement.OnUpdate(dTime);
                    }
                    else { }
                }
            }
            else { }

            mElement = default;
        }

        private void OnWorldLateUpdate()
        {
            int max = mElementUpdaters.Count;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mElement = mElementUpdaters[i];
                    if (mElement.IsUpdate)
                    {
                        mElement.OnLateUpdate();
                    }
                    else { }
                }
            }
            else { }

            mElement = default;
        }

        private void OnWorldFixedUpdate(int dTime)
        {
            int max = mElementUpdaters.Count;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mElement = mElementUpdaters[i];
                    if (mElement.IsUpdate)
                    {
                        mElement.OnFixedUpdate(dTime);
                    }
                    else { }
                }
            }
            else { }

            mElement = default;
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
            ExecuteMessageSettle(message, ref notice);
        }

        private void OnRemoveUpdater(INoticeBase<int> param)
        {
            mUpdaterParamNotice = param as IParamNotice<IUpdate>;
            IUpdate willDelete = mUpdaterParamNotice.ParamValue;
            int index = mElementDeleteds.IndexOf(willDelete);
            if (index < 0)
            {
                mElementDeleteds.Add(willDelete);
            }
            else { }

            mUpdaterParamNotice.ToPool();
            mUpdaterParamNotice = default;
        }

        private void OnAddUpdater(INoticeBase<int> param)
        {
            mUpdaterParamNotice = param as IParamNotice<IUpdate>;
            IUpdate updater = mUpdaterParamNotice.ParamValue;
            int index = mElementUpdaters.IndexOf(updater);
            if (index >= 0)
            {
                mElementUpdaters.Add(updater);

                index = mElementDeleteds.IndexOf(updater);
                if (index >= 0)
                {
                    mElementDeleteds.Remove(updater);
                }
                else { }
            }
            else { }

            mUpdaterParamNotice.ToPool();
            mUpdaterParamNotice = default;
        }
    }

}