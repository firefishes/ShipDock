using System.Collections.Generic;
using UnityEngine;

namespace Elimlnate
{
    public interface ISkillGridTrigger
    {
        /// <summary>是否在连线结束时触发</summary>
        bool IsTriggerWhenLineEnd { get; }
        bool IsSecondary { get; set; }
        Vector2Int CenterGridPos { get; set; }
        ElimlnateGrid GridSelf { get; }
        bool Trigger(List<ElimlnateGrid> willElimlnates);
        Queue<ISkillGridTrigger> Preview(List<int> list);
        void Cancel(List<int> list);
        void Clean();
        void StopPreview(List<int> mSkillGridRanges);
    }
}
