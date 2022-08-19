using ShipDock.ECS;
using UnityEngine;

namespace ShipDock.Applications
{

    public class EntityComponent : MonoBehaviour
    {
        [Header("ShipDock ECS")]
        [SerializeField]
        private int m_EntitasID;
        [SerializeField]
        private int[] m_ComponentIDs;
        [SerializeField]
        private int m_OverlayMapperComponentName = int.MaxValue;

        private ComponentBridge mCompBridge;

        public IShipDockEntitas Entity { get; private set; }

        public int OverlayMapperComponentName
        {
            get
            {
                return m_OverlayMapperComponentName;
            }
        }

        public int GetEntitasID
        {
            get
            {
                return m_EntitasID;
            }
        }

        private void OnDestroy()
        {
            Purge();

            Entity?.Reclaim();
            mCompBridge?.Reclaim();
        }

        protected virtual void Purge()
        {
        }

        private void Awake()
        {
            mCompBridge = new ComponentBridge(OnInit);
            mCompBridge.Start();
        }

        private void OnInit()
        {
            mCompBridge?.Reclaim();

            IShipDockEntitas entity = ShipDockEntitas.CreateEntitas();

            int id = GetInstanceID();
            entity.SetEntitasID(id);
            entity.InitComponents();

            FillEntity(entity);
        }

        public void FillEntity(IShipDockEntitas entitas)
        {
            Entity?.Reclaim();
            Entity = entitas;
            m_EntitasID = Entity.ID;

            IShipDockComponentContext context = ShipDockECS.Instance.Context;

            IShipDockComponent component;
            bool hasOverlayMapperComponent = m_OverlayMapperComponentName != int.MaxValue;

            int name;
            int[] names = m_ComponentIDs;
            int max = names.Length;
            for (int i = 0; i < max; i++)
            {
                name = names[i];
                if (hasOverlayMapperComponent && (name != m_OverlayMapperComponentName))
                {
                    component = context.RefComponentByName(name);
                    Entity.AddComponent(component);
                }
                else { }
            }

            if (hasOverlayMapperComponent)
            {
                component = context.RefComponentByName(m_OverlayMapperComponentName);
                Entity.AddComponent(component);
            }
            else { }
        }
    }

}