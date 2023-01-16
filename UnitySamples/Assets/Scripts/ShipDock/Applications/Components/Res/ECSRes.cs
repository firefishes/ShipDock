
#if UNITY_ECS
using Unity.Entities;
#endif
using UnityEditor.PackageManager;
using UnityEngine;

namespace ShipDock.Applications
{
#if UNITY_ECS
    public interface IECSResUnit : IComponentData
    {
        public Entity GetResEntity();
        public int GetResID();
    }
#endif

    public abstract class ECSRes : MonoBehaviour
    {
        public int resID;
    }
}
