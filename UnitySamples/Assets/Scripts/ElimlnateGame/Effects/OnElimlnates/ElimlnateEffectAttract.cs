using ShipDock.Tools;
using System.Collections.Generic;

namespace Elimlnate
{
    /// <summary>
    /// 消除格特效（吸引格子到 EffectCenter 指定的目标点）
    /// </summary>
    public class ElimlnateEffectAttract : BatchEffect
    {
        private ElimlnateCore GamePlayCore { get; set; }

        public ElimlnateEffectAttract(string relateGridEffectName) : base(relateGridEffectName) { }

        public override void Start(ref List<ElimlnateGrid> grids, string triggerGridEffect = null)
        {
            if (GamePlayCore == default)
            {
                GamePlayCore = ElimlnateCore.Instance;
            }
            else { }

            KeyValueList<int, List<int>> gridsEmpty = new KeyValueList<int, List<int>>();

            BoardGrids boardGrids = GamePlayCore.BoardGrids;
            int max = boardGrids.GridsSize;
            for (int i = 0; i < max; i++)
            {
                List<int> emptysInCol;
                ElimlnateGrid grid = boardGrids.GetGridByIndex(i);
                if (grid == default)
                {
                    int col = i % boardGrids.RowSize;
                    if (gridsEmpty[col] == default)
                    {
                        emptysInCol = new List<int>();
                        gridsEmpty[col] = emptysInCol;
                    }
                    else
                    {
                        emptysInCol = gridsEmpty[col];
                    }
                    emptysInCol.Add(i);
                }
                else { }
            }

            base.Start(ref grids, triggerGridEffect);
        }
    }
}
