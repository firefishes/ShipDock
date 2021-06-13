using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using UnityEngine;

namespace Elimlnate
{
    public class CreateSkillGridExecuter : IQueueExecuter
    {
        public int GridType { get; private set; }
        public Action<ElimlnateGrid, GridCreateInfo> CreateGridCallback { get; private set; }

        public CreateSkillGridExecuter(int gridType, Action<ElimlnateGrid, GridCreateInfo> onCreawteGridCallback)
        {
            GridType = gridType;
            CreateGridCallback = onCreawteGridCallback;
        }

        private Vector2Int GetRandomGridPos()
        {
            Vector2Int result;
            ElimlnateCore core = ElimlnateCore.Instance;
            int index = Utils.UnityRangeRandom(0, core.BoardGrids.GridsSize);
            ElimlnateGrid grid = core.BoardGrids.GetGridByIndex(index);
            if (grid.IsObstacle || grid.HasGridSkill)
            {
                result = GetRandomGridPos();
            }
            else
            {
                result = grid.GridPos;
            }
            return result;
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
        public bool ImmediatelyCommitNext { get; set; }
        public bool IgnoreInQueue { get; set; }

        public virtual void Dispose()
        {
            if (mIsDispose)
            {
                return;
            }
            mIsDispose = true;

            OnNextUnit = default;
            OnUnitExecuted = default;
            OnUnitCompleted = default;

            GridType = int.MaxValue;
            CreateGridCallback = default;
        }

        public virtual void Commit()
        {
        }

        public void QueueNext()
        {
            OnNextUnit?.Invoke(this);//让所在的队列执行器执行下一个队列单元
        }

        #endregion
    }
}
