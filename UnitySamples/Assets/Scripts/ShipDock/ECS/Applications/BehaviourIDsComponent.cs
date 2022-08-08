#define _G_LOG

using ShipDock.ECS;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 行为脚本id数据
    /// </summary>
    [Serializable]
    public struct BehaviourIDs
    {
        public int gameItemID;
        public int animatorID;
        public bool willClear;
    }

    /// <summary>
    /// 行为脚本id映射组件
    /// </summary>
    public class BehaviourIDsComponent : DataComponent<BehaviourIDs>, ICommonOverlayMapper
    {

        private const string LOG_OVERLAY_CHECKED = "log: BehaviourIDs overlay checked, {0} added, gameObject id = {1}";

        /// <summary>周围映射</summary>
        private KeyValueList<int, List<int>> mArounds;
        private KeyValueList<int, List<int>> mAroundColliders;
        private KeyValueList<int, List<int>> mAroundCollisions;
        private KeyValueList<int, bool> mPhysicsCheckable;

        public Action<IShipDockEntitas> AfterAnimatorIDSet { get; set; }
        public Action<IShipDockEntitas> AfterGameObjectIDSet { get; set; }

        public override void Init(IShipDockComponentContext context)
        {
            base.Init(context);

            mArounds = new KeyValueList<int, List<int>>();
            mAroundColliders = new KeyValueList<int, List<int>>();
            mAroundCollisions = new KeyValueList<int, List<int>>();
            mPhysicsCheckable = new KeyValueList<int, bool>();
        }

        protected override BehaviourIDs CreateData()
        {
            return new BehaviourIDs();
        }

        protected override void DropData(ref BehaviourIDs target)
        {
            base.DropData(ref target);

            int id = target.gameItemID;
            mPhysicsCheckable.Remove(id);

            List<int> list = mArounds.Remove(id);
            Utils.Reclaim(ref list);

            list = mAroundColliders.Remove(id);
            Utils.Reclaim(ref list);

            list = mAroundCollisions.Remove(id);
            Utils.Reclaim(ref list);

        }

        public void SetAnimatorID<E>(ref E target, ref Animator animator) where E : IShipDockEntitas
        {
            BehaviourIDs ids = GetEntitasData(ref target);
            ids.animatorID = animator.GetInstanceID();
            FillEntitasData(ref target, ids);

            AfterAnimatorIDSet?.Invoke(target);
        }

        public void SetGameObjectID<E>(ref E target, int id) where E : IShipDockEntitas
        {
            BehaviourIDs ids = GetEntitasData(ref target);
            ids.gameItemID = id;
            FillEntitasData(ref target, ids);

            AfterGameObjectIDSet?.Invoke(target);
        }

        public int GetAnimatorID<E>(ref E target) where E : IShipDockEntitas
        {
            BehaviourIDs ids = GetEntitasData(ref target);
            return ids.animatorID;
        }

        /// <summary>
        /// 检测碰撞的可用性
        /// </summary>
        /// <param name="gameItemID"></param>
        /// <param name="id"></param>
        /// <param name="overlayed"></param>
        /// <param name="isCollision"></param>
        private void CheckAroundsEnabled(int gameItemID, int id, bool overlayed, bool isCollision)
        {
            KeyValueList<int, List<int>> list = isCollision ? mAroundCollisions : mAroundColliders;
            List<int> ids = list[gameItemID];
            if (ids != default)
            {
                if (overlayed)
                {
                    if (ids.IndexOf(id) >= 0) { }
                    else
                    {
                        ids.Add(id);
                    }
                }
                else
                {
                    if (ids.IndexOf(id) < 0) { }
                    else
                    {
                        ids.Remove(id);
                    }
                }
            }
            else { }
        }

        /// <summary>
        /// 处理物理检测
        /// </summary>
        /// <param name="gameItemID">中心id</param>
        /// <param name="id">已检测到的id</param>
        /// <param name="overlayed">是否为触发器</param>
        /// <param name="isCollision">是否为碰撞器</param>
        public void OverlayChecked(int gameItemID, int id, bool overlayed, bool isCollision)
        {
            if ((gameItemID != int.MaxValue) && mPhysicsCheckable[gameItemID])
            {
                List<int> list;
                if (mArounds.ContainsKey(gameItemID))
                {
                    list = mArounds[gameItemID];
                }
                else
                {
                    list = new List<int>();
                    mArounds[gameItemID] = list;

                    mPhysicsCheckable[gameItemID] = true;
                }

                if (list.Contains(id)) { }
                else
                {
                    list.Add(id);

                    LOG_OVERLAY_CHECKED.Log(id.ToString(), gameItemID.ToString());
                }

                CheckAroundsEnabled(gameItemID, id, overlayed, isCollision);
            }
            else { }
        }

        /// <summary>
        /// 获取物理检测到的周围物体的id列表
        /// </summary>
        public List<int> GetAroundIDs<E>(ref E target) where E : IShipDockEntitas
        {
            if (!IsDataValid(ref target))
            {
                return default;
            }
            else { }

            return GetAroundIDsByMapper(ref target, ref mArounds);
        }

        public List<int> GetAroundColliderIDs<E>(ref E target) where E : IShipDockEntitas
        {
            if (!IsDataValid(ref target))
            {
                return default;
            }
            else { }

            return GetAroundIDsByMapper(ref target, ref mAroundColliders);
        }

        public List<int> GetAroundCollisionIDs<E>(ref E target) where E : IShipDockEntitas
        {
            if (!IsDataValid(ref target))
            {
                return default;
            }
            else { }

            return GetAroundIDsByMapper(ref target, ref mAroundCollisions);
        }

        private List<int> GetAroundIDsByMapper<E>(ref E target, ref KeyValueList<int, List<int>> mapper) where E : IShipDockEntitas
        {
            BehaviourIDs ids = GetEntitasData(ref target);
            int id = ids.gameItemID;
            List<int> result = mapper[id];
            return result;
        }

        /// <summary>
        /// 设置物理检测功能为开启
        /// </summary>
        public void PhysicsChecked(int gameItemID, bool isInit = false)
        {
            if (mPhysicsCheckable.ContainsKey(gameItemID) || isInit)
            {
                mPhysicsCheckable[gameItemID] = true;
            }
            else { }
        }

        /// <summary>
        /// 重置物理检测列表，调用 PhysicsChecked 重新开启
        /// </summary>
        public void PhysicsCheckReset(int gameItemID)
        {
            if (mPhysicsCheckable.ContainsKey(gameItemID))
            {
                mPhysicsCheckable[gameItemID] = false;

                List<int> list = mArounds[gameItemID];
                int count = list.Count;
                list.Clear();
            }
            else { }
        }

        /// <summary>
        /// 获取以gameItemID 为中心标记的物理检测是否开启
        /// </summary>
        public bool GetPhysicsChecked(int gameItemID)
        {
            return mPhysicsCheckable[gameItemID];
        }

        public void RemovePhysicsChecker(int subgroupID)
        {
        }
    }
}