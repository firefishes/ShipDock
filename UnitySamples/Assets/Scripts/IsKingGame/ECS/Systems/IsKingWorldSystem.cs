using ShipDock.Applications;
using ShipDock.ECS;
using UnityEngine;

namespace IsKing
{
    public class IsKingWorldSystem : LogicSystem
    {
        private IsKingHeroMovementComp mHeroMovementComp;
        private BehaviourIDsComponent mIDs;

        protected override void BeforeUpdateComponents()
        {
            base.BeforeUpdateComponents();

            if (mIDs == default)
            {
                mIDs = GetRelatedComponent<BehaviourIDsComponent>(Consts.COMP_BEHAVIOUR_IDS);
            }
            else { }

            if (mHeroMovementComp == default)
            {
                mHeroMovementComp = GetRelatedComponent<IsKingHeroMovementComp>(Consts.COMP_HERO_MOVEMENT);
            }
            else { }
        }

        public override void Execute(int entitas, int componentName, ILogicData data)
        {
            switch (componentName)
            {
                case Consts.COMP_HERO_MOVEMENT:

                    //int heroInstanceID = mIDs.GetGameObjectID(entitas);
                    //heroInstanceID.BroadcastWithParam(1, true);

                    float speed = mHeroMovementComp.GetMoveSpeed(entitas);
                    //float acce = mHeroMovementComp.GetAacceleration(entitas);
                    //float time = mHeroMovementComp.GetAaccelerationTimes(entitas);

                    //time += Time.deltaTime;
                    //speed += acce * time;

                    Vector3 direction = mHeroMovementComp.GetMoveDirection(entitas);
                    Vector3 pos = mHeroMovementComp.GetLocalPosition(entitas);
                    pos += direction * speed * Time.deltaTime;

                    //mHeroMovementComp.AaccelerationTime(entitas, time);
                    mHeroMovementComp.MoveSpeed(entitas, speed);
                    mHeroMovementComp.LocalPosition(entitas, pos);
                    break;
            }
        }
    }

}