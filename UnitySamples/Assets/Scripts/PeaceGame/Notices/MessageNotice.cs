using ShipDock.Notices;
using ShipDock.Pooling;

namespace Peace
{
    public sealed class MessageNotice : PeaceNotice
    {
        public INoticeBase<int> MsgNotice { get; private set; }

        public static MessageNotice Create(int message, INoticeBase<int> notice = default)
        {
            MessageNotice result = Pooling<MessageNotice>.From();
            result.SetMessage(message);
            result.SetMsgNotice(notice);
            return result;
        }

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
    }
}