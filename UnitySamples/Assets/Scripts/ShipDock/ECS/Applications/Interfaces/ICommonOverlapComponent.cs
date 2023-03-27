using System;
using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// 通用覆盖检测映射器接口
    /// </summary>
    public interface ICommonOverlapComponent : ICommonBehavioursComponent
    {
        void OverlapChecked(int entitas, int targetID, bool overlayed, bool isCollision);
        void RemovePhysicsChecker(int entitas, int subgroupID);
    }

    public interface ICommonBehavioursComponent : ILogicComponent
    {
        Action<int> AfterAnimatorIDSet { get; set; }
        Action<int> AfterGameObjectIDSet { get; set; }

        int GetGameObjectID(int entitas);
        void SetGameObjectID(int entitas, int gbjInstanceID);
        int GetEntitasByGameObjectID(int gbjInstanceID);
        void SetAnimator(int entitas, ref Animator animator, int animatorName = -1);
        Animator GetAnimator(int entitas, int animatorName = -1);

    }
}