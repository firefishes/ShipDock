namespace ShipDock
{
    public interface IMessageNotice : INotice
    {
        int Message { get; }
        INoticeBase<int> MsgNotice { get; }

        void SetMsgNotice(INoticeBase<int> notice);
    }

    public sealed class MessageNotice : Notice, IMessageNotice
    {
        public static MessageNotice Create(int message, INoticeBase<int> notice = default)
        {
            MessageNotice result = Pooling<MessageNotice>.From();
            result.SetMessage(message);
            result.SetMsgNotice(notice);
            return result;
        }

        public int Message { get; private set; }

        public INoticeBase<int> MsgNotice { get; private set; }

        protected override void Purge()
        {
            base.Purge();

            MsgNotice = default;
        }

        public void SetMsgNotice(INoticeBase<int> notice)
        {
            MsgNotice = notice;
        }

        public override void ToPool()
        {
            base.ToPool();

            Pooling<MessageNotice>.To(this);
        }

        public void SetMessage(int message)
        {
            Message = message;
        }
    }
}