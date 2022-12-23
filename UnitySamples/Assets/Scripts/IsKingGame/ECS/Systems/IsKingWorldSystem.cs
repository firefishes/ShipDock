using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Ticks;
using ShipDock.Tools;
using UnityEngine;

namespace IsKing
{
    public class IsKingWorldSystem : LogicSystem
    {
        private HeroMovementComp mHeroMovementComp;
        private MonsterMovementComp mMonsterMovementComp;
        private BehaviourIDsComponent mIDsComp;
        private RolePropertiesComp mRolePropertiesComp;
        private WorldResourceComp mWorldResComp;

        public IsKingWorldSystem()
        {
            Name = "世界交互系统";
        }

        protected override void BeforeUpdateComponents()
        {
            base.BeforeUpdateComponents();

            if (mIDsComp == default)
            {
                mIDsComp = GetRelatedComponent<BehaviourIDsComponent>(Consts.COMP_BEHAVIOUR_IDS);
            }
            else { }

            if (mHeroMovementComp == default)
            {
                mHeroMovementComp = GetRelatedComponent<HeroMovementComp>(Consts.COMP_HERO_MOVEMENT);
            }
            else { }

            if (mMonsterMovementComp == default)
            {
                mMonsterMovementComp = GetRelatedComponent<MonsterMovementComp>(Consts.COMP_MONSTER_MOVEMENT);
            }
            else { }

            if (mRolePropertiesComp == default)
            {
                mRolePropertiesComp = GetRelatedComponent<RolePropertiesComp>(Consts.COMP_ROLE_PROPERTIES);
            }
            else { }

            if (mWorldResComp == default)
            {
                mWorldResComp = GetRelatedComponent<WorldResourceComp>(Consts.COMP_WORLD_RES);
            }
            else { }
            
        }

        public override void Execute(int entitas, int componentName, ILogicData data)
        {
            float deltaTime = Time.deltaTime;

            //mWorldResComp.ActiveMonster(10);

            switch (componentName)
            {
                case Consts.COMP_HERO_MOVEMENT:
                    UpdateHeroMove(entitas, ref data, deltaTime);
                    break;

                case Consts.COMP_MONSTER_MOVEMENT:
                    UpdateMonsterMove(entitas, ref data, deltaTime);
                    break;
            }
        }

        private void UpdateHeroMove(int entitas, ref ILogicData data, float deltaTime)
        {
            UpdateMove(entitas, mHeroMovementComp, deltaTime);
        }

        private void UpdateMonsterMove(int entitas, ref ILogicData data, float deltaTime)
        {
            mMonsterMovementComp.UpdateLockTargetDown(entitas, deltaTime);

            bool flag = mMonsterMovementComp.GetRelockDownTarget(entitas);
            if (flag)
            {
                Vector3 moveToTarget = mMonsterMovementComp.GetMoveToPosition(entitas);
                Vector3 monseterCurrentPos = mMonsterMovementComp.GetLocalPosition(entitas);
                float distance = Vector3.Distance(moveToTarget, monseterCurrentPos);
                if (Mathf.Abs(distance) <= 0.25f)
                {
                    //未达到目标极限, 标记为可继续锁定终点
                    mMonsterMovementComp.RelockDownTarget(entitas, false);
                }
                else { }
            }
            else
            {
                //移动中
                UpdateMove(entitas, mMonsterMovementComp, deltaTime);
            }
        }

        private void UpdateMove(int entitas, WorldMovementComponent comp, float deltaTime)
        {
            bool moveEnabled = comp.ShouldMove(entitas); 
            if (moveEnabled)
            {
                float speed = comp.GetMoveSpeed(entitas);
                Vector3 direction = comp.GetMoveDirection(entitas);
                Vector3 pos = comp.GetLocalPosition(entitas);

                pos += direction * speed * deltaTime;

                comp.MoveSpeed(entitas, speed);
                comp.LocalPosition(entitas, pos);
            }
            else { }
        }
    }

}