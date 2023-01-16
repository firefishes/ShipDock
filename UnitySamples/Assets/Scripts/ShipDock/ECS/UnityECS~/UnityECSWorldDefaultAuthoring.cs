#define _UNITY_ECS

#if UNITY_ECS
using Unity.Entities;
#endif
using UnityEngine;

namespace ShipDock.ECS
{
    public class UnityECSWorldDefaultAuthoring : MonoBehaviour
    {
#if UNITY_ECS
        public class Baker : Baker<UnityECSWorldDefaultAuthoring>
        {
            public override void Bake(UnityECSWorldDefaultAuthoring authoring)
            {
                AddComponent<DefaultWorldExecute>();
            }
        }
#endif
    }

#if UNITY_ECS
    public struct DefaultWorldExecute : IComponentData { }
#endif
}