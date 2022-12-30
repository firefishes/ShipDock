using Unity.Entities;
using UnityEditor.PackageManager;
using UnityEngine;

namespace ShipDock.Applications
{
    public interface IECSResUnit : IComponentData
    {
        public Entity GetResEntity();
        public int GetResID();
    }

    public abstract class ECSRes : MonoBehaviour
    {
        public int resID;
    }
}
