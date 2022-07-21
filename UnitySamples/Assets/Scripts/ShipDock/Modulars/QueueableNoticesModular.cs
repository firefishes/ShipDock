using ShipDock.Notices;

namespace ShipDock.Modulars
{
    /// <summary>
    /// ���л���Ϣ����ģ��
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

            //�����Ϣ����������
            AddNoticeHandler(OnMessage);
        }

        /// <summary>
        /// ������Ϣ����
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
        /// ʵ�ִ˷������޸Ĵ�����Ϣʱ���߼�
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notice"></param>
        protected abstract void SettleMessageQueue(int message, INoticeBase<int> notice);

        /// <summary>
        /// ����Ϣ������������Ϣ
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