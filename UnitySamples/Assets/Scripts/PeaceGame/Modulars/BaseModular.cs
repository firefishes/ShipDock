using System;
using ShipDock.Modulars;
using ShipDock.Notices;

namespace Peace
{
    public abstract class BaseModular : ApplicationModular
    {
        public BaseModular(int modularName) : base()
        {
            ModularName = modularName;
        }

        public override void Purge()
        {
        }

        protected override void InitCustomHandlers()
        {
            base.InitCustomHandlers();

            AddNoticeHandler(OnMessage);
        }

        [ModularNoticeListener(Consts.N_MSG_QUEUE)]
        private void OnMessage(INoticeBase<int> param)
        {
            if (param is MessageNotice notice)
            {
                SettleMessageQueue(notice.Message, notice.MsgNotice);
            }
            else { }
        }

        protected abstract void SettleMessageQueue(int message, INoticeBase<int> notice);

        protected void AddMessageToQueue(int message, INoticeBase<int> param)
        {
            MessageNotice notice = MessageNotice.Create(message, param);
            NotifyModular(Consts.N_MSG_ADD, notice);
        }
    }

}