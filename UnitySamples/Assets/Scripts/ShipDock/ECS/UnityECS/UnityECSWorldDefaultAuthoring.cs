using Unity.Entities;
using UnityEngine;

namespace ShipDock.ECS
{
    public class UnityECSWorldDefaultAuthoring : MonoBehaviour
    {
        public class Baker : Baker<UnityECSWorldDefaultAuthoring>
        {
            public override void Bake(UnityECSWorldDefaultAuthoring authoring)
            {
                AddComponent<DefaultWorldExecute>();
            }
        }
    }

    public struct DefaultWorldExecute : IComponentData { }
}