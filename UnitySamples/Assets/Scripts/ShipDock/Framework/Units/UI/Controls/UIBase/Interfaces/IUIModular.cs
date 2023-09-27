using ShipDock;

namespace ShipDock
{
    public interface IUIModular : INotificationSender, IUIStack, IReclaim
    {
        int[] DataProxyLinks { get; }
    }
}

