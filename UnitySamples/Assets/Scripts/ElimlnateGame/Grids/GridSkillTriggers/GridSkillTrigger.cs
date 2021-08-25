using System.Collections.Generic;
using UnityEngine;

namespace Elimlnate
{
    public abstract class GridSkillTrigger : ISkillGridTrigger
    {
        /// <summary>技能有效范围内的消除格索引列表</summary>
        protected List<int> mGridsChecked;

        /// <summary>作为技能中心的消除格数据阵列坐标</summary>
        public Vector2Int CenterGridPos { get; set; }
        /// <summary>技能触发器所属的消除格引用</summary>
        public ElimlnateGrid GridSelf { get; private set; }
        /// <summary>是否为连线结束时触发的技能触发器</summary>
        public virtual bool IsTriggerWhenLineEnd { get; }
        /// <summary>是否为二级技能触发器（连锁反应的后续技能触发器节点）</summary>
        public bool IsSecondary { get; set; }

        protected BoardGrids mBoardGrids;
        protected Queue<ISkillGridTrigger> mSecondarySkillTriggers;

        public GridSkillTrigger(ElimlnateGrid grid)
        {
            GridSelf = grid;
            mBoardGrids = ElimlnateCore.Instance.BoardGrids;
            mGridsChecked = new List<int>();
            mSecondarySkillTriggers = new Queue<ISkillGridTrigger>();
        }

        public virtual void Clean()
        {
            Cancel(default);

            GridSelf = default;
            mGridsChecked = default;
            mBoardGrids = default;
        }

        public abstract Queue<ISkillGridTrigger> Preview(List<int> list);

        public virtual void StopPreview(List<int> list)
        {
            ElimlnateGrid grid;
            int index;
            int max = mGridsChecked.Count;
            for (int i = 0; i < max; i++)
            {
                index = mGridsChecked[i];
                grid = mBoardGrids.GetGridByIndex(index);
                grid?.StopEffect(GameEffects.EffectPreviewGridSkill);
            }
        }

        public virtual bool Trigger(List<ElimlnateGrid> willElimlnates)
        {
            if (GridSelf != default && GridSelf.HasGridSkill)
            {
                GridSelf.TriggerSkill();
                return true;
            }
            else { }

            return false;
        }

        public virtual void Cancel(List<int> list)
        {
            if (list != default && mGridsChecked != default)
            {
                int max = mGridsChecked.Count;
                for (int i = 0; i < max; i++)
                {
                    if (list.Contains(mGridsChecked[i]))
                    {
                        list.Remove(mGridsChecked[i]);
                    }
                    else { }
                }
            }
            else { }

            mSecondarySkillTriggers.Clear();
            mGridsChecked?.Clear();
        }
    }
}
