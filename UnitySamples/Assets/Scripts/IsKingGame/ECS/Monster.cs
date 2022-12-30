using ShipDock.Applications;
using Unity.Entities;
using UnityEngine;

namespace IsKing
{
    public class Monster : ECSRes
    {
        class Baker : Baker<Monster>
        {
            public override void Bake(Monster authoring)
            {
                MonsterRes comp = new ()
                {
                    resID = authoring.resID,
                    roleRes = GetEntity(authoring.res),
                };
                AddComponent(comp);
                AddComponentObject(new MonsterResManaged()
                {
                    resID = authoring.resID,
                    resRef = authoring.gameObject,
                });
            }
        }

        public GameObject res;
    }

    public struct MonsterRes : IECSResUnit
    {
        public int resID;
        public Entity roleRes;
        public bool hasSpanwPoint;

        public Entity GetResEntity()
        {
            return roleRes;
        }

        public int GetResID()
        {
            return resID;
        }
    }

    public class MonsterResManaged : IComponentData
    {
        public GameObject resRef;
        public bool hasSpanwPoint;
        public Entity spanwPoint;
        public int resID;
    }

    public class RoleManaged : IComponentData
    {
        public Entity binded;
        public Transform transRef;
    }
}