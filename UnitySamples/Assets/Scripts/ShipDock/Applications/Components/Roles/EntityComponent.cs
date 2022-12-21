using ShipDock.ECS;
using UnityEngine;

namespace ShipDock.Applications
{
    [DisallowMultipleComponent]
    public class EntityComponent : MonoBehaviour
    {
        public static int COMP_NAME_OVERLAY_MAPPER = int.MaxValue;

        [Header("ShipDock ECS")]
        [SerializeField]
        private int m_EntitasType;
        [SerializeField]
        private int m_EntitasID;
        [SerializeField]
        private int[] m_ComponentNames;
        [SerializeField]
        private int m_OverlayMapperComponentName = default;

        private ComponentBridge mCompBridge;

        public int Entitas { get; private set; }

        public int OverlayMapperComponentName
        {
            get
            {
                return m_OverlayMapperComponentName;
            }
        }

        private void OnDestroy()
        {
            Purge();

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

            FillEntity();
        }

        public void FillEntity()
        {
            ILogicContext context = ShipDockECS.Instance.Context;
            ILogicEntitas allEntitas = context.AllEntitas;
            allEntitas.AddEntitas(out int entitas, m_EntitasType);

            Entitas = entitas;
            m_EntitasID = Entitas;

            if (m_OverlayMapperComponentName == default)
            {
                m_OverlayMapperComponentName = COMP_NAME_OVERLAY_MAPPER;
            }
            else { }

            bool hasOverlayMapperComponent = m_OverlayMapperComponentName != int.MaxValue;

            int name;
            ILogicComponent component;
            int[] names = m_ComponentNames;
            int max = names.Length;
            for (int i = 0; i < max; i++)
            {
                name = names[i];
                if (hasOverlayMapperComponent && (name != m_OverlayMapperComponentName))
                {
                    component = context.RefComponentByName(name);

                    allEntitas.AddComponent(Entitas, component);
                }
                else { }
            }

            if (hasOverlayMapperComponent)
            {
                component = context.RefComponentByName(m_OverlayMapperComponentName);
                allEntitas.AddComponent(Entitas, component);
            }
            else { }
        }
    }

}