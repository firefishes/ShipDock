using System.Collections.Generic;

namespace ShipDock
{
    /// <summary>
    /// 世界模块
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class WorldModular : QueueableNoticesModular
    {
        /// <summary>加入世界项的消息名</summary>
        private int mAddToWorldMessage;
        /// <summary>从世界移除项的消息名</summary>
        private int mRemoveFromWorldMessage;
        /// <summary>帧更新器</summary>
        private MethodUpdater mWorldUpdater;
        /// <summary>帧更新消息</summary>
        private IParamNotice<IUpdate> mUpdaterParamNotice;
        /// <summary>临时存储的项元素</summary>
        private IUpdate mElement;
        /// <summary>被移除的项元素</summary>
        private List<IUpdate> mElementDeleteds;
        /// <summary>处于更新中的项元素</summary>
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

        /// <summary>
        /// 初始化消息处理流程
        /// </summary>
        protected virtual void InitMessageSettles()
        {
            SetMessageSettle(mAddToWorldMessage, OnAddUpdater);
            SetMessageSettle(mRemoveFromWorldMessage, OnRemoveUpdater);
        }

        /// <summary>
        /// 每帧更新的回调方法
        /// </summary>
        /// <param name="dTime"></param>
        private void OnWorldUpdate(int dTime)
        {
            int max = mElementDeleteds.Count;
            if (max > 0)
            {
                while (mElementDeleteds.Count > 0)
                {
                    int i = mElementDeleteds.Count - 1;
                    mElement = mElementDeleteds[i];
                    mElementDeleteds.RemoveAt(i);

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
                    if (mElement != default && mElement.IsUpdate)
                    {
                        mElement.OnUpdate(dTime);
                    }
                    else { }
                }
            }
            else { }

            mElement = default;
        }

        /// <summary>
        /// 每帧更新之后的回调方法
        /// </summary>
        private void OnWorldLateUpdate()
        {
            int max = mElementUpdaters.Count;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mElement = mElementUpdaters[i];
                    if (mElement != default && mElement.IsUpdate)
                    {
                        mElement.OnLateUpdate();
                    }
                    else { }
                }
            }
            else { }

            mElement = default;
        }

        /// <summary>
        /// 固定更新的回调方法
        /// </summary>
        /// <param name="dTime"></param>
        private void OnWorldFixedUpdate(int dTime)
        {
            int max = mElementUpdaters.Count;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mElement = mElementUpdaters[i];
                    if (mElement != default && mElement.IsUpdate)
                    {
                        mElement.OnFixedUpdate(dTime);
                    }
                    else { }
                }
            }
            else { }

            mElement = default;
        }

        /// <summary>
        /// 消息队列处理器函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notice"></param>
        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
            ExecuteMessageSettle(message, ref notice);
        }

        /// <summary>
        /// 移除世界项消息处理器函数
        /// </summary>
        /// <param name="param"></param>
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

        /// <summary>
        /// 添加世界项消息处理器函数
        /// </summary>
        /// <param name="param"></param>
        private void OnAddUpdater(INoticeBase<int> param)
        {
            mUpdaterParamNotice = param as IParamNotice<IUpdate>;
            IUpdate updater = mUpdaterParamNotice.ParamValue;
            int index = mElementUpdaters.IndexOf(updater);
            if (index <= 0)
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