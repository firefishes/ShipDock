#define _G_LOG

using ShipDock.ECS;
using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 物理检测器子组件
    /// </summary>
    [Serializable]
    public class PhysicsCheckerSubgroup : IDispose
    {
#if UNITY_EDITOR
        [Header("测试")]
        [SerializeField]
        private bool m_IsLogTrigger;
        [SerializeField]
        private SphereCollider m_CheckRange;

        private void UpdateInfoForEditor()
        {
            if (m_CheckRange != default)
            {
                m_Radius = m_CheckRange.radius;
                RayAndHit.radius = m_Radius;
                RayAndHit.layerMask = m_ColliderLayer.value;
            }
            else { }

        }
#endif

        [Header("检测半径")]
        [SerializeField]
        private float m_Radius = 2f;

        [Header("是否激活")]
        [SerializeField]
        private bool m_CheckerEnabled;
        [SerializeField]
        private LayerMask m_ColliderLayer;

        [Header("检测频率")]
        [SerializeField]
        private TimeGapper m_CheckGapper = new TimeGapper();

        private int mColliderLayer;
        private Collider mColliderItem;
        private Collider[] mCollidersOverlay;
        private ComponentBridge mBridge;
        private ICommonOverlayMapper mCommonColliderMapper;

        public bool CheckerEnabled
        {
            get
            {
                return m_CheckerEnabled;
            }
            set
            {
                m_CheckerEnabled = value;
            }
        }

        public TimeGapper CheckGapper
        {
            get
            {
                return m_CheckGapper;
            }
        }

        public RayAndHitInfo RayAndHit { get; private set; }
        public Transform CheckerOwner { get; set; }
        public int SubgroupID { get; private set; } = int.MaxValue;

        public void SetCommonOverlayMapper(ICommonOverlayMapper overlayMapper)
        {
            if (SubgroupID != int.MaxValue)
            {
                mCommonColliderMapper = overlayMapper;
                mCommonColliderMapper.PhysicsChecked(SubgroupID, true);
            }
            else { }
        }

        public void Init(Transform trans, BehaviourIDs ids)
        {
            CheckerOwner = trans;
            SubgroupID = ids.gameItemID;

            mBridge = new ComponentBridge(OnInit);
            mBridge.Start();
        }

        private void OnInit()
        {
            mBridge.Dispose();

            mColliderLayer = m_ColliderLayer.value;
            RayAndHit = new RayAndHitInfo
            {
                ray = new Ray(),
                layerMask = mColliderLayer,
                radius = m_Radius
            };

#if UNITY_EDITOR
            if (CheckerOwner != default)
            {
                m_CheckRange = CheckerOwner.gameObject.AddComponent<SphereCollider>();
                m_CheckRange.isTrigger = true;
                m_CheckRange.radius = m_Radius;
            }
            else { }
#endif
        }

        public void Dispose()
        {
            Utils.Reclaim(ref mCollidersOverlay);
            mBridge?.Dispose();

            mCommonColliderMapper?.RemovePhysicsChecker(SubgroupID);

            mCommonColliderMapper = default;
            CheckerOwner = default;
            mBridge = default;
            mColliderItem = default;

            SubgroupID = int.MaxValue;
        }

        private void AddColliding(int id, bool isCollision, out int statu)
        {
            statu = 0;
            if (mCommonColliderMapper != default)
            {
                if (m_CheckerEnabled)
                {
                    mCommonColliderMapper.OverlayChecked(SubgroupID, id, true, isCollision);
                }
                else { }
            }
            else
            {
                statu = 2;
            }
        }

        private void RemoveColliding(int id, bool isCollision, out int statu)
        {
            statu = 0;
            if (mCommonColliderMapper != default)
            {
                if (m_CheckerEnabled)
                {
                    mCommonColliderMapper.OverlayChecked(SubgroupID, id, false, isCollision);
                }
                else { }
            }
            else
            {
                statu = 2;
            }
        }

        public void TriggerEnter(ref Collider other)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            else { }

            int id = other.GetInstanceID();
            AddColliding(id, false, out int statu);
        }

        public void TriggerExit(ref Collider other)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            else { }

            int id = other.GetInstanceID();
            RemoveColliding(id, false, out int statu);
        }

        public void CollisionEnter(ref Collision collision)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            else { }

            int id = collision.collider.GetInstanceID();
            AddColliding(id, true, out _);
        }

        public void CollisionExit(ref Collision collision)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            else { }

            int id = collision.collider.GetInstanceID();
            RemoveColliding(id, true, out _);
        }

        public void UpdatePhysicsCheck(ref Transform transform, bool isCollision)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            else { }

            if (m_CheckGapper.isStart)
            {
                m_CheckGapper.TimeAdvanced(Time.deltaTime);
                return;
            }
            else
            {
                m_CheckGapper.Start();
            }

            mCollidersOverlay = Physics.OverlapSphere(transform.position, RayAndHit.radius, RayAndHit.layerMask);
            int max = mCollidersOverlay != default ? mCollidersOverlay.Length : 0;
            if (max > 0)
            {
                "log: Update physics start check, SubgroupID = {0}".Log(SubgroupID.ToString());
                int id;
                for (int i = 0; i < max; i++)
                {
                    mColliderItem = mCollidersOverlay[i];
                    id = mColliderItem.GetInstanceID();
                    if (id != SubgroupID)
                    {
                        AddColliding(id, isCollision, out _);
                    }
                    else { }
                }
            }
            else { }

            mCommonColliderMapper.PhysicsChecked(SubgroupID);
#if UNITY_EDITOR
            UpdateInfoForEditor();
#endif
        }

        public void SetCheckerGapperTime(float time)
        {
            m_CheckGapper.totalTime = time;
        }
    }
}