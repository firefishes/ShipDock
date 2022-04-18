namespace ShipDock.Tools
{
    public class IdentBits
    {
        public int Value { get; private set; }

        public IdentBits() { }

        public void Reclaim()
        {
            Value = 0;
        }

        public void SetIdentValue(int value)
        {
            Value = value;
        }

        public bool Check(int value)
        {
            return (Value & (1 << value)) != 0;
        }

        public void Mark(int value)
        {
            Value |= (1 << value);
        }

        public void ResetMark(int value)
        {
            Value &= ~(1 << value);
        }
    }
}
