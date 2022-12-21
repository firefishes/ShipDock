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
    public class BehaviourIDs : LogicData
    {
        public int gameItemID = default;
        public int animatorID = -1;
    }

    public class BehaviourIDsComponent : DataComponent<BehaviourIDs>, ICommonOverlapComponent
    {
        public Action<int> AfterAnimatorIDSet { get; set; }
        public Action<int> AfterGameObjectIDSet { get; set; }

        private int[] mGameItemIDs;
        private Dictionary<int, int> mGameItemMapper;
        private Dictionary<int, Dictionary<int, int>> mArounds;
        private Dictionary<int, Dictionary<int, int>> mAroundColliders;
        private Dictionary<int, Dictionary<int, int>> mAroundCollisions;
        private Dictionary<int, Dictionary<int, Animator>> mCachedAnimator;

        public override void Init(ILogicContext context)
        {
            base.Init(context);

            mGameItemMapper = new Dictionary<int, int>();
            mArounds = new Dictionary<int, Dictionary<int, int>>();
            mAroundColliders = new Dictionary<int, Dictionary<int, int>>();
            mAroundCollisions = new Dictionary<int, Dictionary<int, int>>();

            mCachedAnimator = new Dictionary<int, Dictionary<int, Animator>>();
        }

        public override void Reset(bool clearOnly = false)
        {
            base.Reset(clearOnly);

            Utils.Reclaim(ref mArounds, clearOnly);
            Utils.Reclaim(ref mAroundColliders, clearOnly);
            Utils.Reclaim(ref mAroundCollisions, clearOnly);

            Utils.Reclaim(ref mGameItemMapper, clearOnly);
            Utils.Reclaim(ref mCachedAnimator, clearOnly);
        }

        protected override void DropData(ref ILogicData data)
        {
            base.DropData(ref data);

            int key = data.DataIndex;

            mArounds.TryGetValue(key, out Dictionary<int, int> mapper);
            Utils.Reclaim(ref mapper);

            mAroundColliders.TryGetValue(key, out mapper);
            Utils.Reclaim(ref mapper);

            mAroundCollisions.TryGetValue(key, out mapper);
            Utils.Reclaim(ref mapper);

            mCachedAnimator.TryGetValue(key, out Dictionary<int, Animator> animators);
            Utils.Reclaim(ref animators);

            mGameItemIDs[key] = default;

            BehaviourIDs ids = data as BehaviourIDs;
            mGameItemMapper[ids.gameItemID] = default;
        }

        protected override void OnResetSuccessive(bool clearOnly = false)
        {
            base.OnResetSuccessive(clearOnly);

            Utils.Reclaim(ref mGameItemIDs, clearOnly);
        }

        protected override void UpdateDataStretch(int dataSize)
        {
            base.UpdateDataStretch(dataSize);

            Utils.Stretch(ref mGameItemIDs, dataSize);
        }

        /// <summary>
        /// 处理物理检测
        /// </summary>
        /// <param name="key">中心id</param>
        /// <param name="targetID">已检测到的id</param>
        /// <param name="overlayed">是否在检测触发</param>
        /// <param name="isCollision">是否为碰撞器</param>
        public void OverlapChecked(int entitas, int targetID, bool overlayed, bool isCollision)
        {
            ILogicData data = UpdateValid(entitas);

            if (data != default)
            {
                int dataIndex = data.DataIndex;
                mArounds.TryGetValue(dataIndex, out Dictionary<int, int> mapper);

                if (mapper != default) { }
                else
                {
                    mapper = new Dictionary<int, int>();
                    mArounds[dataIndex] = mapper;
                }

                mapper.TryGetValue(targetID, out int statu);
                if (statu != default) { }
                else
                {
                    mapper[targetID] = 1;

                    //LOG_OVERLAY_CHECKED.Log(id.ToString(), gameItemID.ToString());
                }

                CheckAroundsEnabled(dataIndex, targetID, overlayed, isCollision);
            }
            else { }
        }

        private void CheckAroundsEnabled(int dataIndex, int targetID, bool overlayed, bool isCollision)
        {
            Dictionary<int, Dictionary<int, int>> source = isCollision ? mAroundCollisions : mAroundColliders;

            source.TryGetValue(dataIndex, out Dictionary<int, int> ids);

            if (ids != default)
            {
                if (overlayed)
                {
                    if (ids.ContainsKey(targetID)) { }
                    else
                    {
                        ids[targetID] = 1;
                    }
                }
                else
                {
                    if (ids.ContainsKey(targetID))
                    {
                        ids.Remove(targetID);
                    }
                    else { }
                }
            }
            else { }
        }

        private void RemoveTargetIDFromMapper(ref Dictionary<int, int> mapper, int targetID = default)
        {
            if (targetID != default)
            {
                mapper?.Clear();
            }
            else
            {
                mapper?.Remove(targetID);
            }
        }

        public void RemovePhysicsChecker(int entitas, int targetID = default)
        {
            ILogicData data = GetEntitasData(entitas);

            if (data != default)
            {
                int key = data.DataIndex;

                mArounds.TryGetValue(key, out Dictionary<int, int> mapper);
                RemoveTargetIDFromMapper(ref mapper, targetID);

                mAroundCollisions.TryGetValue(key, out mapper);
                RemoveTargetIDFromMapper(ref mapper, targetID);

                mAroundColliders.TryGetValue(key, out mapper);
                RemoveTargetIDFromMapper(ref mapper, targetID);
            }
            else { }
        }

        public bool SetGameObjectID(int entitas, int gbjInstanceID)
        {
            bool isInit = default;

            ILogicData data = UpdateValid(entitas);

            if (data is BehaviourIDs ids)
            {
                if (ids.gameItemID == default)
                {
                    isInit = true;

                    ids.gameItemID = gbjInstanceID;
                    mGameItemIDs[data.DataIndex] = gbjInstanceID;
                    mGameItemMapper[gbjInstanceID] = entitas;

                    AfterGameObjectIDSet?.Invoke(entitas);
                }
                else { }
            }
            else { }

            return isInit;
        }

        public int GetGameObjectID(int entitas)
        {
            int result = default;

            if (IsStateRegular(entitas, out _))
            {
                ILogicData data = GetEntitasData(entitas);
                result = mGameItemIDs[data.DataIndex];
            }
            else { }

            return result;
        }

        public int GetEntitasByGameObjectID(int gbjInstanceID)
        {
            int result = default;
            bool flag = mGameItemMapper != default ? mGameItemMapper.TryGetValue(gbjInstanceID, out result) : false;
            return flag ? result : default;
        }

        private BehaviourIDs CacheBehaviour<T>(int entitas, ref T value, ref Dictionary<int, Dictionary<int, T>> collections, out int instanceID, out Dictionary<int, T> mapper) where T : UnityEngine.Object
        {
            BehaviourIDs result = default;

            instanceID = 0;
            mapper = default;

            ILogicData data = UpdateValid(entitas);

            if (data is BehaviourIDs ids)
            {
                int gameItemID = ids.gameItemID;
                if (gameItemID != -1)
                {
                    collections.TryGetValue(entitas, out mapper);

                    if (mapper == default)
                    {
                        mapper = new Dictionary<int, T>();
                        collections[gameItemID] = mapper;
                    }
                    else { }

                    instanceID = value.GetInstanceID();
                    result = ids;
                }
                else { }
            }
            else { }

            return result;
        }

        private BehaviourIDs GetCachedBehaviour<T>(int entitas, ref Dictionary<int, Dictionary<int, T>> collections, out Dictionary<int, T> mapper)
        {
            BehaviourIDs result = default;

            mapper = default;

            if (IsStateRegular(entitas, out _))
            {
                ILogicData data = GetEntitasData(entitas);
                if (data is BehaviourIDs ids)
                {
                    collections.TryGetValue(entitas, out mapper);
                    if (mapper != default)
                    {
                        result = ids;
                    }
                    else { }
                }
                else { }
            }
            else { }

            return result;
        }

        public void SetAnimator(int entitas, ref Animator animator, int animatorName = -1)
        {
            BehaviourIDs ids = CacheBehaviour(entitas, ref animator, ref mCachedAnimator, out int instanceID, out Dictionary<int, Animator> mapper);
            if (ids != default)
            {
                bool flag = animatorName != -1 && ids.animatorID == -1;
                if (flag)
                {
                    ids.animatorID = instanceID;
                }
                else { }

                int key = flag ? instanceID : animatorName;
                mapper[key] = animator;

                AfterAnimatorIDSet?.Invoke(entitas);
            }
            else { }
        }

        public Animator GetAnimator(int entitas, int animatorName = -1)
        {
            Animator result = default;

            BehaviourIDs ids = GetCachedBehaviour(entitas, ref mCachedAnimator, out Dictionary<int, Animator> mapper);

            if (ids != default)
            {
                int animatorID = ids.animatorID;
                bool flag = animatorName != -1 && animatorID != -1;
                int key = flag ? animatorID : animatorName;
                result = mapper[key];
            }
            else { }

            return result;
        }

        private Dictionary<int, int> GetOverlayIDsByMapper(int entitas, ref Dictionary<int, Dictionary<int, int>> mapper)
        {
            ILogicData data = GetEntitasData(entitas);

            int key = data.DataIndex;
            mapper.TryGetValue(key, out Dictionary<int, int> result);

            return result;
        }

        /// <summary>
        /// 获取物理检测到的周围物体的id列表
        /// </summary>
        public Dictionary<int, int> GetAroundIDs(int entitasID)
        {
            if (!IsStateRegular(entitasID, out _))
            {
                return default;
            }
            else { }

            return GetOverlayIDsByMapper(entitasID, ref mArounds);
        }

        public Dictionary<int, int> GetAroundColliderIDs(int entitasID)
        {
            if (!IsStateRegular(entitasID, out _))
            {
                return default;
            }
            else { }

            return GetOverlayIDsByMapper(entitasID, ref mAroundColliders);
        }

        public Dictionary<int, int> GetAroundCollisionIDs(ref int entitasID)
        {
            if (!IsStateRegular(entitasID, out _))
            {
                return default;
            }
            else { }

            return GetOverlayIDsByMapper(entitasID, ref mAroundCollisions);
        }
    }
}