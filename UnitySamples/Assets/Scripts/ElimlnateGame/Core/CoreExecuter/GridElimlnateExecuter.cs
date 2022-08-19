using ShipDock.Interfaces;
using System;
using System.Collections.Generic;

namespace Elimlnate
{
    public class GridElimlnateExecuter : IQueueExecuter
    {

        public int GridCount
        {
            get
            {
                return mGrids != default ? mGrids.Count : 0;
            }
        }

        /// <summary>需要销毁的消除格个数/summary>
        public int GridNeedDestroyCount { get; set; }

        private bool IsCommited { get; set; }
        private GridOperater GridOperater { get; set; }
        private BoardGrids BoardGrids { get; set; }

        private List<ElimlnateGrid> mGrids;

        public GridElimlnateExecuter(GridOperater operater, BoardGrids boardGrids, ref List<ElimlnateGrid> list)
        {
            GridOperater = operater;
            BoardGrids = boardGrids;

            ElimlnateGrid grid;
            mGrids = new List<ElimlnateGrid>();
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                grid = list[i];
                grid.AddDestroyCallback(CountNeedDestroyGrid);
                mGrids.Add(grid);
            }
            GridNeedDestroyCount = mGrids.Count;
        }

        public ElimlnateGrid GetLastRemovingGrid()
        {
            return mGrids[mGrids.Count - 1];
        }

        private void CountNeedDestroyGrid(ElimlnateGrid grid)
        {
            GridNeedDestroyCount--;
            if (GridNeedDestroyCount <= 0)
            {
                QueueNext();
                Reclaim();
            }
            else { }
        }

        #region 执行队列单元的实现代码
        private bool mIsDispose;

        public int QueueSize
        {
            get
            {
                return 1;
            }
        }

        public QueueNextUnit OnNextUnit { get; set; }
        public QueueUnitCompleted OnUnitCompleted { get; set; }
        public QueueUnitExecuted OnUnitExecuted { get; set; }
        public Action ActionUnit { get; set; }
        public bool IgnoreInQueue { get; set; }

        public virtual void Reclaim()
        {
            if (mIsDispose)
            {
                return;
            }
            else { }

            mIsDispose = true;

#if !ILRUNTIME
            OnNextUnit = default;
            OnUnitExecuted = default;
            OnUnitCompleted = default;

            GridOperater = default;
            BoardGrids = default;

            mGrids.Clear();
#endif
            mGrids = default;
        }

        public virtual void Commit()
        {
            IsCommited = true;
            GridOperater.SetGridElimlnateCounts(this);
            GridOperater.EliminateResult.SetEliminateCount(ref mGrids);

            Queue<ElimlnateGrid> hasDestroyeds = new Queue<ElimlnateGrid>();

            ElimlnateGrid grid;
            int max = mGrids.Count;
            for (int i = 0; i < max; i++)
            {
                grid = mGrids[i];
                if ((grid == default) || grid.IsDestroyed)
                {
                    hasDestroyeds.Enqueue(grid);
                }
                else
                {
                    grid.GridTrans?.SetParent(default);
                }
            }

            max = hasDestroyeds.Count;
            while (max > 0)
            {
                grid = hasDestroyeds.Dequeue();
                mGrids.Remove(grid);
                max--;
            }

            GridOperater.ElimlnateEffect.Start(ref mGrids);
        }

        public void QueueNext()
        {
            OnNextUnit?.Invoke(this);//让所在的队列执行器执行下一个队列单元
        }
#endregion
    }
}
