using ShipDock.Commons;
using ShipDock.Notices;
using ShipDock.Tools;

namespace ShipDock.Modulars
{
    /// <summary>
    /// 消息队列模块
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class MessageModular : ApplicationModular
    {
        public static void AddMessage(DecorativeModulars modulars, int message, INoticeBase<int> paramNotice)
        {
            modulars?.NotifyModular(ShipDockConsts.NOTICE_MSG_ADD, MessageNotice.Create(message, paramNotice));
        }

        private bool mHasMessageQueue;
        private DoubleBuffers<IMessageNotice> mDoubleBuffers;
        private IMessageNotice mNoticeWillAdd;
        private IUpdate mMessageUpdater;

        public MessageModular(int modularName) : base()
        {
            ModularName = modularName;
        }

        public override void Purge()
        {
            UpdaterNotice.RemoveSceneUpdater(mMessageUpdater);

            mDoubleBuffers?.Dispose();
        }

        public override void InitModular()
        {
            base.InitModular();

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
            mDoubleBuffers.Update(deltaTime);
        }

        private void OnMessageDequeue(int dTime, IMessageNotice param)
        {
            mHasMessageQueue = param != default;

            if (mHasMessageQueue)
            {
                //派发处理消息的模块消息
                NotifyModular(ShipDockConsts.NOTICE_MSG_QUEUE, param);
                param.ToPool();
            }
            else { }

            mHasMessageQueue = false;
        }
    }
}
