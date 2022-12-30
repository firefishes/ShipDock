using ShipDock.Applications;
using Unity.Entities;
using UnityEngine;

namespace IsKing
{
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
}