using ShipDock.Applications;
#if UNITY_ECS
using Unity.Entities;
#endif
using UnityEngine;

namespace IsKing
{
#if UNITY_ECS
    public class Hero : ECSRes
    {
        class Baker : Baker<Hero>
        {
            public override void Bake(Hero authoring)
            {
                HeroRes comp = new ()
                {
                    resID = authoring.resID,
                    roleRes = GetEntity(authoring.res),
                };
                AddComponent(comp);
            }
        }

        public GameObject res;
    }

    public struct HeroRes : IECSResUnit
    {
        public int resID;
        public Entity roleRes;
        public bool isCreat;

        public Entity GetResEntity()
        {
            return roleRes;
        }

        public int GetResID()
        {
            return resID;
        }
    }
#endif
}