using ShipDock.Notices;

namespace Peace
{
    public abstract class PeaceNotice : Notice, IPeaceNotice
    {
        public int Message { get; private set; }

        public void SetMessage(int message)
        {
            Message = message;
        }
    }
}
