namespace ShipDock.UIControls
{
    public class UIRaw<T> : IUIRaw
    {
        public T raw;

        public UIRaw(T value)
        {
            raw = value;
        }
    }
}
