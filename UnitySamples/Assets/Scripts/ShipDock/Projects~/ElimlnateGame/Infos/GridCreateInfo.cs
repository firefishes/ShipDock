using System;

namespace Elimlnate
{
    public class GridCreateInfo
    {
        public int id;
        public int row;
        public int column;
        public int customShapeID;
        public bool isDirectShow;
        public string name;
        public string enterEffectName = GameEffects.EffectEnter;
        public ElimlnateGrid gridInRawPos;

        public bool IsGridCreated { get; private set; }
        public ElimlnateGrid Grid { get; private set; }
        public Action<ElimlnateGrid, GridCreateInfo> OnGridCreated { get; private set; }

        public void GridCommit(ElimlnateGrid target)
        {
            Grid = target;
            OnGridCreated?.Invoke(target, this);
            IsGridCreated = true;
        }

        public void Clear()
        {
            Grid = default;
            gridInRawPos = default;
            OnGridCreated = default;
        }

        public void AddCreatedCallback(Action<ElimlnateGrid, GridCreateInfo> method)
        {
            if (IsGridCreated)
            {
                method?.Invoke(Grid, this);
            }
            else
            {
                OnGridCreated += method;
            }
        }
    }
}
