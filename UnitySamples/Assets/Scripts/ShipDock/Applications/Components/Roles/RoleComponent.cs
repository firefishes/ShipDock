using UnityEngine;

namespace ShipDock
{
    [DisallowMultipleComponent]
    public class RoleComponent : MonoBehaviour, INotificationSender
    {
        public struct BehaviourIDs
        {
            public int gameItemID;
        }

        [Header("½ÇÉ«")]
        [SerializeField]
        private float m_MoveStepLength;
        [SerializeField]
        private Animator m_Animator;
        [SerializeField]
        private BehaviourIDs m_BehaviourIDs;

        [Header("Åö×²ÐÅÏ¢")]
        [SerializeField]
        private LayerMask m_GroundLayer;
        [SerializeField]
        private CapsuleCollider m_RoleCollider;
        /// <summary>ÐÅºÅ¼ì²â´¥·¢Æ÷</summary>
        [SerializeField]
        private BoxCollider m_OverlapSigner;

        [Header("ÉäÏß¼ì²â")]
        [SerializeField]
        private RayAndHitInfo[] m_RayAndHits;

        [Header("ÎïÀí¼ì²â")]
        [SerializeField]
        protected PhysicsCheckerSubgroup m_PhysicsChecker = new PhysicsCheckerSubgroup();

        [Header("ECS")]
        [SerializeField]
        protected EntityComponent m_EntityComponent;

        private bool mShouldCheckCollsion;
        private Transform mTrans;
        private RayAndHitInfo mRayAndHitMapper;
        private ComponentBridge mCompBridge;

        public MethodUpdater RoleUpdater { get; private set; }

        protected void OnDestroy()
        {
            Purge();

            gameObject.GetInstanceID().Remove(OnNoticeHandler);

            mTrans = default;
            m_EntityComponent = default;
            m_Animator = default;
            m_RoleCollider = default;
            m_OverlapSigner = default;

            RoleUpdater?.Reclaim();
            m_PhysicsChecker?.Reclaim();
            mCompBridge?.Reclaim();
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
            //mCompBridge = new ComponentBridge(OnInit);

            gameObject.GetInstanceID().Add(OnNoticeHandler);
        }

        private void Start()
        {
            //mCompBridge?.Start();
        }

        protected virtual void OnInit()
        {
        }

        private void FillRole()
        {
            if (m_EntityComponent == default)
            {
                //m_EntityComponent = GetComponent<EntityComponent>();
            }
            else { }

            if (m_EntityComponent != default)
            {
                m_EntityComponent.FillEntity();

                ILogicContext context = ShipDockECS.Instance.Context;
                int componentName = m_EntityComponent.OverlayMapperComponentName;
                ICommonOverlapComponent comp = context.RefComponentByName(componentName) as ICommonOverlapComponent;

                InitRole(ref comp);
            }
            else { }
        }

        private void OnNoticeHandler(INoticeBase<int> param)
        {
            if (param is IParamNotice<int> notice)
            {
                int noticeName = gameObject.GetInstanceID();
                if (param.Name == noticeName)
                {
                    ILogicContext context = ShipDockECS.Instance.Context;
                    ILogicEntities allEntitas = context.AllEntitas;

                    int msg = notice.ParamValue;
                    switch (msg)
                    {
                        case EntityComponent.ENTITY_SETUP:
                            FillRole();
                            break;
                    }
                }
                else { }
            }
            else { }
        }

        private void InitRole(ref ICommonOverlapComponent comp)
        {
            if (comp != default)
            {
                int entitas = m_EntityComponent.Entitas;
                int instanceID = m_OverlapSigner.GetInstanceID();

                comp.SetGameObjectID(entitas, instanceID);

                BehaviourIDs ids = default;// = comp.GetEntitasData(entitas) as BehaviourIDs;
                m_BehaviourIDs = ids;

                if (m_OverlapSigner != default)
                {
                    m_OverlapSigner.isTrigger = true;

                    m_PhysicsChecker.Init(transform, m_BehaviourIDs);
                    m_PhysicsChecker.SetCommonOverlayMapper(entitas, comp);
                }
                else { }

                if (m_Animator == default)
                {
                    //m_Animator = GetComponent<Animator>();
                }
                else { }

                if (m_Animator != default)
                {
                    comp.SetAnimator(entitas, ref m_Animator, 0);
                }
                else { }

                InitRoleComponents();
            }
            else { }
        }

        protected virtual void InitRoleComponents()
        {
        }

        protected virtual bool HasCollisionOrTriggerTask()
        {
            return m_RoleCollider != default || m_OverlapSigner != default;
        }

        public virtual void OnUpdate(int time)
        {
        }

        public virtual void OnLateUpdate()
        {
        }

        public virtual void OnFixedUpdate(int time)
        {
            mShouldCheckCollsion = HasCollisionOrTriggerTask();

            if (mShouldCheckCollsion)
            {
                m_PhysicsChecker?.UpdatePhysicsCheck(ref mTrans, false);
            }
            else { }
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
