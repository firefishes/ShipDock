namespace ShipDock.Tools
{
    public class IdentBits
    {
        private int mFlag;

        public IdentBits() { }

        public void Reclaim()
        {
            mFlag = 0;
        }

        public bool Check(int value)
        {
            return (mFlag & (1 << value)) != 0;
        }

        public void Set(int value)
        {
            mFlag |= (1 << value);
        }

        public void Reset(int value)
        {
            mFlag &= ~(1 << value);
        }
    }
}
