namespace ShipDock.Notices
{
    public interface INoticesHandler
    {
        void ListenerHandler(INoticeBase<int> param);
    }
}