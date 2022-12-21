using System.Collections.Generic;

namespace ShipDock.Tools
{
    /// <summary>
    /// 标记位
    /// </summary>
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

        public void DeMark(int value)
        {
            Value &= ~(1 << value);
        }
    }

    /// <summary>
    /// 多标记位组
    /// </summary>
    public class IdentBitsGroup
    {
        private const int IDENT_BIT_MAX = 32;

        private IdentBits mCurrent;
        private List<int> mAllMarks;
        private List<IdentBits> mIdentBits;

        public IdentBitsGroup()
        {
            Reset();

            AddIdentBits();
        }

        public void Reclaim()
        {
            Reset();

            Utils.Reclaim(ref mAllMarks);
            Utils.Reclaim(ref mIdentBits);
        }

        public void Reset()
        {
            if (mAllMarks == default)
            {
                mAllMarks = new List<int>();
            }
            else
            {
                mAllMarks.Clear();
            }

            if (mIdentBits == default)
            {
                mIdentBits = new List<IdentBits>();
            }
            else
            {
                int max = mIdentBits.Count;
                for (int i = 0; i < max; i++)
                {
                    mCurrent = mIdentBits[i];
                    mCurrent.Reclaim();
                }
            }

            mCurrent = default;
        }

        private void AddIdentBits()
        {
            mIdentBits.Add(new IdentBits());
        }

        private void CheckOrAddNewIdentBits(ref int value, int index = 0)
        {
            if (value <= IDENT_BIT_MAX) { }
            else
            {
                index = value / IDENT_BIT_MAX;

                int count = mIdentBits.Count;
                if (index >= count)
                {
                    count = index - count + 1;
                    if (count == 1)
                    {
                        AddIdentBits();
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            AddIdentBits();
                        }
                    }
                }
                else { }
            }

            mCurrent = mIdentBits[index];
        }

        public bool Check(int value)
        {
            bool result = default;

            CheckOrAddNewIdentBits(ref value);

            result = mCurrent.Check(value);

            return result;
        }

        public void Mark(int value)
        {
            CheckOrAddNewIdentBits(ref value);

            mCurrent.Mark(value);
        }

        public void DeMark(int value)
        {
            CheckOrAddNewIdentBits(ref value);

            mCurrent.DeMark(value);
        }

        public int[] GetAllMarks()
        {
            int[] result;
            mAllMarks.Clear();

            int count = mIdentBits.Count;
            int max = count * IDENT_BIT_MAX;

            for (int i = 0; i < max; i++)
            {
                if (Check(i))
                {
                    mAllMarks.Add(i);
                }
                else { }
            }

            result = mAllMarks.ToArray();

            return result;
        }
    }
}
