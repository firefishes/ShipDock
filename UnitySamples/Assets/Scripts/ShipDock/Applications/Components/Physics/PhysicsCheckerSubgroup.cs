#define _G_LOG

using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 物理检测器子组
    /// </summary>
    [Serializable]
    public class PhysicsCheckerSubgroup : IReclaim
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
                OverlapRayAndHit.radius = m_Radius;
                OverlapRayAndHit.layerMask = m_OverlapLayer.value;
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
        private LayerMask m_OverlapLayer;

        [Header("检测频率")]
        [SerializeField]
        private TimeGapper m_CheckGapper = new TimeGapper();

        private int mOverlapLayer;
        private Collider mOverlapItem;
        private Collider[] mOverlaps;
        private ComponentBridge mBridge;
        private ICommonOverlapComponent mCommonOverlapCacher;

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

        public RayAndHitInfo OverlapRayAndHit { get; private set; }
        public Transform CheckerOwner { get; set; }
        public int SubgroupID { get; private set; } = -1;

        private int Entitas { get; set; }

        public void SetCommonOverlayMapper(int entitas, ICommonOverlapComponent overlapComp)
        {
            Entitas = entitas;

            if (SubgroupID != -1)
            {
                mCommonOverlapCacher = overlapComp;
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
            mBridge.Reclaim();

            mOverlapLayer = m_OverlapLayer.value;
            OverlapRayAndHit = new RayAndHitInfo
            {
                ray = new Ray(),
                layerMask = mOverlapLayer,
                radius = m_Radius
            };

#if UNITY_EDITOR
            if (CheckerOwner != default)
            {
                GameObject gbj = CheckerOwner.gameObject;
                m_CheckRange = gbj.AddComponent<SphereCollider>();
                m_CheckRange.isTrigger = true;
                m_CheckRange.radius = m_Radius;
            }
            else { }
#endif
        }

        public void Reclaim()
        {
            Utils.Reclaim(ref mOverlaps);
            mBridge?.Reclaim();

            mCommonOverlapCacher?.RemovePhysicsChecker(Entitas, SubgroupID);

            mCommonOverlapCacher = default;
            CheckerOwner = default;
            mBridge = default;
            mOverlapItem = default;

            SubgroupID = -1;
        }

        private void AddColliding(int id, bool isCollision, out int statu)
        {
            statu = 0;
            if (mCommonOverlapCacher != default)
            {
                if (m_CheckerEnabled)
                {
                    mCommonOverlapCacher.OverlapChecked(Entitas, id, true, isCollision);
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
            if (mCommonOverlapCacher != default)
            {
                if (m_CheckerEnabled)
                {
                    mCommonOverlapCacher.OverlapChecked(Entitas, id, false, isCollision);
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
            if (SubgroupID == -1)
            {
                return;
            }
            else { }

            int id = other.GetInstanceID();
            AddColliding(id, false, out int statu);
        }

        public void TriggerExit(ref Collider other)
        {
            if (SubgroupID == -1)
            {
                return;
            }
            else { }

            int id = other.GetInstanceID();
            RemoveColliding(id, false, out int statu);
        }

        public void CollisionEnter(ref Collision collision)
        {
            if (SubgroupID == -1)
            {
                return;
            }
            else { }

            int id = collision.collider.GetInstanceID();
            AddColliding(id, true, out _);
        }

        public void CollisionExit(ref Collision collision)
        {
            if (SubgroupID == -1)
            {
                return;
            }
            else { }

            int id = collision.collider.GetInstanceID();
            RemoveColliding(id, true, out _);
        }

        public void UpdatePhysicsCheck(ref Transform transform, bool isCollision)
        {
            if (SubgroupID == -1)
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

            mOverlaps = Physics.OverlapSphere(transform.position, OverlapRayAndHit.radius, OverlapRayAndHit.layerMask);
            int max = mOverlaps != default ? mOverlaps.Length : 0;
            if (max > 0)
            {
                const string physicsStartCheckLog = "log: Update physics start check, SubgroupID = {0}";
                physicsStartCheckLog.Log(SubgroupID.ToString());

                int id;
                for (int i = 0; i < max; i++)
                {
                    mOverlapItem = mOverlaps[i];
                    id = mOverlapItem.GetInstanceID();
                    if (id != SubgroupID)
                    {
                        AddColliding(id, isCollision, out _);
                    }
                    else { }
                }
            }
            else { }

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