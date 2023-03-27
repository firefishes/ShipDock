using System.Collections.Generic;

namespace ShipDock
{
    /// <summary>
    /// 消息队列模块
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class MessageModular : ApplicationModular
    {
        private static IAppModulars modulars;

        public static void AddMessage(int message, INoticeBase<int> paramNotice)
        {
            if (modulars == default)
            {
                modulars = Framework.Instance.GetUnit<DecorativeModulars>(Framework.UNIT_MODULARS);
            }
            else { }

            MessageNotice msgNotice = MessageNotice.Create(message, paramNotice);
            modulars?.NotifyModular(ShipDockConsts.NOTICE_MSG_ADD, msgNotice);
        }

        private bool mHasMessageNotice;
        private IPoolable mReclaimParam;
        private IPoolable mMessageParam;
        private IUpdate mMessageUpdater;
        private IMessageNotice mNoticeWillAdd;
        private Queue<IPoolable> mMessageParamReclaims;
        private DoubleBuffers<IMessageNotice> mDoubleBuffers;

        public MessageModular(int modularName) : base()
        {
            ModularName = modularName;
        }

        public override void Purge()
        {
            UpdaterNotice.RemoveSceneUpdater(mMessageUpdater);

            mDoubleBuffers?.Reclaim();
            mMessageParamReclaims?.Clear();
        }

        public override void InitModular()
        {
            base.InitModular();

            mMessageParamReclaims = new Queue<IPoolable>();

            mDoubleBuffers = new DoubleBuffers<IMessageNotice>()
            {
                OnDequeue = OnMessageDequeue,
            };

            mMessageUpdater = new MethodUpdater()
            {
                Update = OnModularUpdate,
            };

            UpdaterNotice.AddSceneUpdater(mMessageUpdater);
        }

        protected override void InitCustomHandlers()
        {
            base.InitCustomHandlers();

            AddNoticeHandler(OnMessageAdd);
        }

        [ModularNoticeListener(ShipDockConsts.NOTICE_MSG_ADD)]
        private void OnMessageAdd(INoticeBase<int> param)
        {
            mNoticeWillAdd = param as IMessageNotice;
            if (mNoticeWillAdd != default)
            {
                if (mNoticeWillAdd != default)
                {
                    mDoubleBuffers.Enqueue(mNoticeWillAdd);
                }
                else { }
            }
            else { }

            mNoticeWillAdd = default;
        }

        private void OnModularUpdate(int deltaTime)
        {
            int count = mMessageParamReclaims.Count;
            if (count > 0)
            {
                while(mMessageParamReclaims.Count > 0)
                {
                    mReclaimParam = mMessageParamReclaims.Dequeue();
                    mReclaimParam.ToPool();
                }
                mReclaimParam = default;
            }
            else { }

            mDoubleBuffers.Update(deltaTime);
        }

        private void OnMessageDequeue(int dTime, IMessageNotice param)
        {
            mHasMessageNotice = param != default;

            if (mHasMessageNotice)
            {
                //派发处理消息的模块消息
                NotifyModular(ShipDockConsts.NOTICE_MSG_QUEUE, param);

                mMessageParam = param.MsgNotice as IPoolable;
                if ((mMessageParam != default) && !mMessageParamReclaims.Contains(mMessageParam))
                {
                    mMessageParamReclaims.Enqueue(mMessageParam);
                }
                else { }

                param.ToPool();
            }
            else { }

            mHasMessageNotice = false;
            mMessageParam = default;
        }
    }
}
