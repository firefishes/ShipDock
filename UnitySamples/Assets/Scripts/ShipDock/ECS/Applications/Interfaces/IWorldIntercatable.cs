namespace ShipDock
{
    public interface IWorldIntercatable
    {
        void WorldItemHandler(INoticeBase<int> param);
        void WorldItemDispose();
    }
}