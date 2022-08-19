using ShipDock.Tools;
using System.Collections.Generic;

namespace Elimlnate
{
    public abstract class EliminateResult
    {
        private KeyValueList<int, int> mAllResult;

        public int FirstGridType { get; private set; }
        public int EliminateCount { get; private set; }
        public int FirstGridShapeIndex { get; private set; }

        public EliminateResult()
        {
            mAllResult = new KeyValueList<int, int>();
        }

        public void Reclaim()
        {
            ClearResult();
            Purge();

            mAllResult?.Clear();
            mAllResult = default;
        }

        protected abstract void Purge();

        public void ClearResult()
        {
            EliminateCount = 0;
            mAllResult.Clear();
            FirstGridShapeIndex = int.MaxValue;
        }

        public virtual void SetEliminateCount(ref List<ElimlnateGrid> list)
        {
            int count = list.Count;
            bool flag = count > 0;
            ElimlnateGrid first = list[0];
            int gridType = flag ? first.GridType : -1;
            int shapeIndex = flag ? first.GridShapeIndex : -1;

            FirstGridType = gridType;
            EliminateCount += count;

            if (FirstGridShapeIndex == int.MaxValue)
            {
                FirstGridShapeIndex = shapeIndex;
            }
            else { }

            for (int i = 0; i < count; i++)
            {
                gridType = list[i].NormalGridType;
                if (!mAllResult.ContainsKey(gridType))
                {
                    mAllResult[gridType] = 0;
                }
                else { }
                mAllResult[gridType]++;
            }
        }

        public int GetGridCountByType(int gridType)
        {
            return mAllResult[gridType];
        }

        public int GetGridMainShapeIndex(int index)
        {
            int gridType = mAllResult.Keys[index];
            return gridType;
        }

        public int GetTypesCount()
        {
            return mAllResult.Keys.Count;
        }
    }
}