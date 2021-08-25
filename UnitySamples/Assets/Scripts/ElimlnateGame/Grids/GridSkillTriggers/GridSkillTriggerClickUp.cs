using System.Collections.Generic;

namespace Elimlnate
{
    /// <summary>
    /// 点击式触发消除格技能
    /// </summary>
    public class GridSkillTriggerClickUp : GridSkillTrigger
    {
        public GridSkillTriggerClickUp(ElimlnateGrid grid) : base(grid) { }

        public override Queue<ISkillGridTrigger> Preview(List<int> list)
        {
            return default;
        }

        public override bool Trigger(List<ElimlnateGrid> willElimlnates)
        {
            if (GridSelf != default && GridSelf.HasGridSkill)
            {
                GridSelf.TriggerSkill();
                return true;
            }
            return false;
        }
    }
}
