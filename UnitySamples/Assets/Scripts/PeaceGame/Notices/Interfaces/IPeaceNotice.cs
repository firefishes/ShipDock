using ShipDock.Notices;

namespace Peace
{
    public interface IPeaceNotice : INotice
    {
        int Message { get; }
    }
}
