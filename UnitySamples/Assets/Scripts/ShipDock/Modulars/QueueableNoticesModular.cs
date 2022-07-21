using ShipDock.Notices;

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
        public QueueableNoticesModular(int modularName) : base()
        {
            ModularName = modularName;
        }

        public override void Purge()
        {
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
            if (param is MessageNotice notice)
            {
                SettleMessageQueue(notice.Message, notice.MsgNotice);
            }
            else { }
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
    }

}