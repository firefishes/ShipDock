using System.Collections.Generic;
using UnityEngine;

namespace Elimlnate
{
    /// <summary>
    /// 消除格类型库
    /// </summary>
    public class GridTypes
    {
        /// <summary>消除格资源池/summary>
        private Dictionary<int, GridType> mGridResItems;
        private List<int> mGridTypeWeights;
        private List<int> mInvalidWegihts;

        public int NormalGridType { get; set; }
        public int WeightTotal { get; private set; }
        public string NormalGridABName { get; set; }
        public string ShapesABName { get; set; }

        public GridTypes()
        {
            mGridResItems = new Dictionary<int, GridType>();
            mGridTypeWeights = new List<int>();
            mInvalidWegihts = new List<int>();
        }

        public void Clean()
        {
            var eumer = mGridResItems.GetEnumerator();
            int max = mGridResItems.Count;
            for (int i = 0; i < max; i++)
            {
                eumer.MoveNext();
                eumer.Current.Value.creater = default;
                eumer.Current.Value.gridRes = default;
            }
            mGridResItems.Clear();
            mInvalidWegihts.Clear();
        }

        public void SetGridCreater(int typeValue, bool isStaticAsset, System.Func<GameObject> creater, int weight = 1, int mainGridType = int.MaxValue)
        {
            if (!mGridResItems.ContainsKey(typeValue))
            {
                GridType gridType = new GridType
                {
                    isStaticAsset = isStaticAsset,
                    creater = creater,
                    gridTypeKey = typeValue,
                    mainGridType = mainGridType != int.MaxValue ? mainGridType : typeValue,
                    weight = weight,
                };
                WeightTotal += weight;

                for (int i = 0; i < weight; i++)
                {
                    mGridTypeWeights.Add(typeValue);
                }

                mGridResItems[typeValue] = gridType;
            }
            else { }
        }

        public void AddInvalidWeightGrid(int type)
        {
            mInvalidWegihts.Add(type);
        }

        public GridType GetGridPool(int typeValue)
        {
            mGridResItems.TryGetValue(typeValue, out GridType result);
            return result;
        }

        public bool HasGridPool(int typeValue)
        {
            return mGridResItems.ContainsKey(typeValue);
        }

        public int GetRandomGrid()
        {
            int index = Random.Range(0, WeightTotal);
            int result = mGridTypeWeights[index];
            if (mInvalidWegihts.Contains(result))
            {
                mGridTypeWeights.Remove(result);
                GetRandomGrid();
            }
            else { }
            return result;
        }
    }
}
