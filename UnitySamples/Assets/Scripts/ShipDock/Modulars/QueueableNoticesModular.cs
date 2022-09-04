using ShipDock.Notices;
using ShipDock.Tools;
using System;

namespace ShipDock.Modulars
{
    /// <summary>
    /// 队列化消息处理模块
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class QueueableNoticesModular : ApplicationModular
    {
        private MessageNotice mMsgNotice;
        private KeyValueList<int, Action<INoticeBase<int>>> mMessageSettles;

        public QueueableNoticesModular(int modularName) : base()
        {
            ModularName = modularName;

            mMessageSettles = new KeyValueList<int, Action<INoticeBase<int>>>();
        }

        public override void Purge()
        {
            mMessageSettles?.Clear();
        }

        protected override void InitCustomHandlers()
        {
            base.InitCustomHandlers();

            //添加消息处理器函数
            AddNoticeHandler(OnMessage);
        }

        /// <summary>
        /// 处理消息队列
        /// </summary>
        /// <param name="param"></param>
        [ModularNoticeListener(ShipDockConsts.NOTICE_MSG_QUEUE)]
        private void OnMessage(INoticeBase<int> param)
        {
            mMsgNotice = param as MessageNotice;
            if (mMsgNotice != default)
            {
                SettleMessageQueue(mMsgNotice.Message, mMsgNotice.MsgNotice);
            }
            else { }
            mMsgNotice = default;
        }

        /// <summary>
        /// 实现此方法以修改处理消息时的逻辑
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notice"></param>
        protected abstract void SettleMessageQueue(int message, INoticeBase<int> notice);

        /// <summary>
        /// 向消息队列中增加消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="param"></param>
        protected void AddMessageToQueue(int message, INoticeBase<int> param)
        {
            MessageNotice notice = MessageNotice.Create(message, param);
            NotifyModular(ShipDockConsts.NOTICE_MSG_ADD, notice);
        }

        protected void SetMessageSettle(int message, Action<INoticeBase<int>> handler)
        {
            mMessageSettles[message] = handler;
        }

        protected void ResetMessageSettle(int message)
        {
            mMessageSettles.Remove(message);
        }

        protected void ExecuteMessageSettle(int message, ref INoticeBase<int> param)
        {
            Action<INoticeBase<int>> method = mMessageSettles[message];
            if (method != default)
            {
                method?.Invoke(param);
            }
            else { }
        }
    }

}