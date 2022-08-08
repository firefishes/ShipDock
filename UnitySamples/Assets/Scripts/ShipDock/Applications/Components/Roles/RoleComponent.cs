using ShipDock.Commons;
using ShipDock.ECS;
using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{
    public class RoleComponent : MonoBehaviour
    {

        [Header("角色")]
        [SerializeField]
        private float m_MoveStepLength;
        [SerializeField]
        private Animator m_Animator;
        [SerializeField]
        private BehaviourIDs m_BehaviourIDs;

        [Header("碰撞信息")]
        [SerializeField]
        private LayerMask m_GroundLayer;
        [SerializeField]
        private CapsuleCollider m_RoleCollider;
        [SerializeField]
        private BoxCollider m_SignCollider;

        [Header("射线检测")]
        [SerializeField]
        private RayAndHitInfo[] m_RayAndHits;

        [Header("物理检测")]
        [SerializeField]
        private PhysicsCheckerSubgroup m_PhysicsChecker = new PhysicsCheckerSubgroup();

        [Header("ECS")]
        [SerializeField]
        private EntityComponent m_EntityComponent;

        private bool mShouldCheckCollsion;
        private Transform mTrans;
        private RayAndHitInfo mRayAndHitMapper;
        private ComponentBridge mCompBridge;

        private BehaviourIDsComponent IDsComp { get; set; }

        public MethodUpdater RoleUpdater { get; private set; }

        protected void OnDestroy()
        {
            Purge();

            mTrans = default;

            RoleUpdater?.Dispose();
            mCompBridge?.Dispose();
        }

        protected virtual void Purge()
        {
        }

        private void Awake()
        {
            RoleUpdater = new MethodUpdater()
            {
                Update = OnUpdate,
                LateUpdate = OnLateUpdate,
                FixedUpdate = OnFixedUpdate,
            };

            mTrans = transform;
            mCompBridge = new ComponentBridge(OnInit);
        }

        private void Start()
        {
            mCompBridge?.Start();
        }

        protected virtual void OnInit()
        {
            mCompBridge?.Dispose();

            InitPhysicsChecker();
        }

        private void InitPhysicsChecker()
        {
            m_PhysicsChecker.Init(transform, m_BehaviourIDs);

            if (m_EntityComponent == default)
            {
                m_EntityComponent = GetComponent<EntityComponent>();
            }
            else { }

            if (m_EntityComponent != default)
            {
                IShipDockComponentContext context = ShipDockECS.Instance.Context;
                int componentName = m_EntityComponent.OverlayMapperComponentName;
                IDataComponent<BehaviourIDs> comp = context.RefComponentByName(componentName) as IDataComponent<BehaviourIDs>;

                if (comp != default)
                {
                    IShipDockEntitas entitas = m_EntityComponent.Entity;
                    bool hasData = comp.IsDataValid(ref entitas);
                    if (hasData)
                    {
                        m_PhysicsChecker.SetCommonOverlayMapper(comp as ICommonOverlayMapper);
                    }
                    else { }
                }
                else { }
            }
            else { }
        }

        protected virtual bool HasCollisionOrTriggerTask()
        {
            return m_RoleCollider != default || m_SignCollider != default;
        }

        public virtual void OnUpdate(int time)
        {
            mShouldCheckCollsion = HasCollisionOrTriggerTask();
            
            if (mShouldCheckCollsion) { }
            else
            {
                m_PhysicsChecker?.UpdatePhysicsCheck(ref mTrans, false);
            }
        }

        public virtual void OnLateUpdate()
        {
        }

        public virtual void OnFixedUpdate(int time)
        {
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            m_PhysicsChecker?.CollisionEnter(ref collision);
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            m_PhysicsChecker?.CollisionExit(ref collision);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            m_PhysicsChecker?.TriggerEnter(ref other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            m_PhysicsChecker?.TriggerExit(ref other);
        }
    }
}
