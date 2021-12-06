using ShipDock.Notices;

namespace IsKing
{
    public abstract class InfoController<T> : NoticesObserver, IInfoController where T : DataInfo, new()
    {
        public T Info { get; protected set; }

        public InfoController(T info)
        {
            Info = info;
        }
    }
}