using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Ticks;
using ShipDock.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
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
                //UpdateMove(entitas, mMonsterMovementComp, deltaTime);
            }
        }

        //[Unity.Burst.BurstDiscard()]
        struct t : IJobParallelFor
        {
            public NativeArray<Vector3> result;

            [ReadOnly]
            public NativeArray<Vector3> direction;
            [ReadOnly]
            public NativeArray<float> speed;

            [ReadOnly]
            public float deltaTime;

            [ReadOnly]
            public NativeArray<int> entity;

            public void Execute(int index)
            {
                //Debug.Log(entity[index] + " Jobing");
                result[index] += direction[index] * (speed[index] * deltaTime);
            }
        }

        private NativeArray<Vector3> mDirectionJobs;
        private NativeArray<Vector3> mPosResultJobs;
        private NativeArray<float> mSpeedJobs;
        private NativeArray<int> mEIDs;

        private int mJobsComplete;

        protected override JobHandle MakeJobs(int compName, int max)
        {
            //return default;
            JobHandle handler = base.MakeJobs(compName, max);

            if (Consts.COMP_MONSTER_MOVEMENT == compName && mJobsComplete == 0)
            {
                int[] entities = mMonsterMovementComp.GetEntitasValid();

                mPosResultJobs = new NativeArray<Vector3>(max, Allocator.TempJob);
                mDirectionJobs = new NativeArray<Vector3>(max, Allocator.TempJob);
                mSpeedJobs = new NativeArray<float>(max, Allocator.TempJob);
                mEIDs = new NativeArray<int>(max, Allocator.TempJob);

                for (int i = 0; i < max; i++)
                {
                    mPosResultJobs[i] = mMonsterMovementComp.GetLocalPosition(entities[i]);
                    mSpeedJobs[i] = mMonsterMovementComp.GetMoveSpeed(entities[i]);
                    mDirectionJobs[i] = mMonsterMovementComp.GetMoveDirection(entities[i]);
                    mEIDs[i] = entities[i];
                }

                mJobsComplete = max;

                t jobs = new t
                {
                    deltaTime = Time.deltaTime,
                    result = mPosResultJobs,
                    direction = mDirectionJobs,
                    entity = mEIDs,
                    speed = mSpeedJobs
                };

                handler = jobs.Schedule(max, max / 2, handler);

            }
            else { }

            return handler;
        }

        protected override void AfterComponentExecuted()
        {
            base.AfterComponentExecuted();

            //return;
            if (mJobsComplete > 0)
            {
                Vector3 pos;
                for (int i = 0; i < mJobsComplete; i++)
                {
                    pos = mPosResultJobs[i];

                    mMonsterMovementComp.MoveSpeed(mEIDs[i], mSpeedJobs[i]);
                    mMonsterMovementComp.LocalPosition(mEIDs[i], pos);
                }

                mJobsComplete = 0;

                mPosResultJobs.Dispose();
                mSpeedJobs.Dispose();
                mDirectionJobs.Dispose();
                mEIDs.Dispose();
            }
            else { }
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