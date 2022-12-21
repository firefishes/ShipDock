#define _G_LOG

using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public struct AroundsCheckedInfo
    {
        public int checkingAroundID;
        public float distanceBetween;
        public float distanceCrowding;
        public BehaviourIDs ids;
    }

    public abstract class WorldSceneSystem : LogicSystem
    {
        protected WorldMovement mMovement;
        protected WorldInteracter mWorldItem;
        protected Queue<INotice> mWorldEventNotices;
        protected Queue<WorldInteracter> mEventItems;

        private bool mHasBehaviourIDs;
        private bool mBehaviourIdsCompChecked;
        private INotice mItemNotice;
        private WorldInteracter mEventItem;
        private ClusteringData mClusteringData;
        protected KeyValueList<int, WorldInteracter> mWorldItemMapper;
        private KeyValueList<int, ClusteringData> mGroupsMapper;
        private KeyValueList<int, WorldMovement> mAroundMapper;

        protected ClusteringComponent ClusteringComp { get; private set; }
        protected BehaviourIDsComponent BehaviourIDsComp { get; private set; }
        protected WorldComponent WorldComp { get; set; }
        protected abstract int WorldComponentName { get; }

        public bool ShouldWorldGroupable { get; private set; }

        public override void Init(ILogicContext context)
        {
            base.Init(context);

            mWorldEventNotices = new Queue<INotice>();
            mEventItems = new Queue<WorldInteracter>();

            mWorldItemMapper = new KeyValueList<int, WorldInteracter>();
            mGroupsMapper = new KeyValueList<int, ClusteringData>();
            mAroundMapper = new KeyValueList<int, WorldMovement>();

            WorldComp = GetRelatedComponent<WorldComponent>(WorldComponentName);
            BehaviourIDsComp = context.RefComponentByName(WorldComp.BehaviaourIDsComponentName) as BehaviourIDsComponent;
            ClusteringComp = context.RefComponentByName(WorldComp.WorldGroupComponentName) as ClusteringComponent;

            ShouldWorldGroupable = ClusteringComp != default;
        }

        //public override int DropEntitas(int entitasID)
        //{
        //    mWorldItem = WorldComp.GetEntitasData(ref entitasID);

        //    if ((mWorldItem != default) && (mWorldItem.worldItemID != int.MaxValue))
        //    {
        //        if (mWorldItemMapper.ContainsKey(mWorldItem.worldItemID))
        //        {
        //            DropWorldItem(ref entitasID);
        //        }
        //        else { }
        //    }
        //    else { }

        //    return base.DropEntitas(entitasID);
        //}

        protected virtual void DropWorldItem(int entitas)
        {
            int worldItemID = mWorldItem.worldItemID;
            mWorldItemMapper.Remove(worldItemID);

            mGroupsMapper.Remove(mWorldItem.groupID);
            mAroundMapper.Remove(mWorldItem.aroundID);

            List<int> list = BehaviourIDsComp.GetAroundIDs(entitas);
            list?.Clear();

            BehaviourIDs ids = (BehaviourIDs)BehaviourIDsComp.GetEntitasData(entitas);
            ids.willClear = true;
            BehaviourIDsComp.FillEntitasData(entitas, ids);

            mWorldItem.WorldItemDispose?.Invoke();
            mWorldItem.WorldItemDispose = default;
            mWorldItem.isDroped = true;
        }

        private bool ShouldAddToWorldItems()
        {
            if (mWorldItem == default)
            {
                return false;
            }
            else { }

            int id = mWorldItem.worldItemID;
            return IsWorldItemValid(ref mWorldItem) && !mWorldItemMapper.ContainsKey(id);
        }

        private bool IsWorldItemValid(ref WorldInteracter item)
        {
            return (item != default) && !item.isDroped;
        }

        public override void Execute(int entitasID, int componentName, ILogicData data)
        {
            mMovement = GetMovmentData(ref entitasID);

            CheckWorldItemCaches(entitasID);
            if (ShouldWorldGroupable)
            {
                SetClusteringPosition(ref entitasID);
                CheckClustering(ref entitasID);
            }
            else { }

            CheckAround(ref entitasID);
            CheckWorldEvents();
        }

        /// <summary>
        /// 检测物体四周目标
        /// </summary>
        private void CheckAround(ref int target)
        {
            if (!mBehaviourIdsCompChecked)
            {
                mBehaviourIdsCompChecked = true;
                mHasBehaviourIDs = BehaviourIDsComp != default;
            }
            else { }

            if (mHasBehaviourIDs && IsWorldItemValid(ref mWorldItem))
            {
                BehaviourIDs ids = (BehaviourIDs)BehaviourIDsComp.GetEntitasData(target);
                int aroundID = ids.gameItemID;
                if (mAroundMapper.ContainsKey(aroundID))
                {
                    WalkEachAroundItem(ref target, ids);
                }
                else
                {
                    mWorldItem.aroundID = aroundID;
                    mAroundMapper[aroundID] = mMovement;
                }
            }
            else { }
        }

        private void WalkEachAroundItem(ref int target, BehaviourIDs ids)
        {
            bool flag;
            int id, aroundID = ids.gameItemID;
            float distance;
            AroundsCheckedInfo info;
            WorldMovement itemMovement;
            List<int> list = BehaviourIDsComp.GetAroundIDs(target);
            int max = (list != default) ? list.Count : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    id = list[i];
                    itemMovement = GetAroundTargetMovement(id);
                    if (itemMovement != default && !itemMovement.Invalid)
                    {
                        distance = mMovement.DistanceBetween(itemMovement.Position);
                        info = new AroundsCheckedInfo
                        {
                            ids = ids,
                            checkingAroundID = id,
                            distanceBetween = distance,
                            distanceCrowding = GetCrowdingDistance(),
                        };
                        flag = CheckingAround(ref target, aroundID, info, ref itemMovement);
                        if (!flag)
                        {
                            break;
                        }
                        else { }
                    }
                    else { }
                }
                BehaviourIDsComp.PhysicsCheckReset(aroundID);
            }
            else { }

            AroundsChecked(ref target, aroundID);
        }

        protected virtual float GetCrowdingDistance()
        {
            return 1f;
        }

        protected virtual void AroundsChecked(ref int target, int aroundID)
        {
        }

        protected WorldMovement GetAroundTargetMovement(int aroundID)
        {
            return mAroundMapper?.GetValue(aroundID);
        }

        /// <summary>
        /// 检测世界交互物体的缓存
        /// </summary>
        private void CheckWorldItemCaches(int entitasID)
        {
            if (WorldComp.IsStateRegular(entitasID, out _))
            {
                if (mWorldItemMapper.ContainsKey(mWorldItem.worldItemID))
                {
                    return;
                }
                else { }

                mWorldItem = (WorldInteracter)WorldComp.GetEntitasData(entitasID);
                if (ShouldAddToWorldItems())
                {
                    mWorldItemMapper.Put(mWorldItem.worldItemID, mWorldItem);
                    AfterWorldItemCached(ref entitasID);
                }
                else { }
            }
            else { }
        }

        protected WorldInteracter GetWorldItemFromCache(int worldItemID)
        {
            return mWorldItemMapper[worldItemID];
        }

        /// <summary>
        /// 检测群聚
        /// </summary>
        private void CheckClustering(ref int target)
        {
            if ((mMovement != default) && IsWorldItemValid(ref mWorldItem))
            {
                int id = mWorldItem.worldItemID;
                
                mClusteringData = (ClusteringData)ClusteringComp.GetEntitasData(target);
                if ((id != int.MaxValue) && (mClusteringData != default))
                {
                    if (mClusteringData.IsGroupCached)
                    {
                        mClusteringData.ClusteringMag = mMovement.ClusteringDirection.magnitude;
                        "todo".Log("开发群聚功能");
                    }
                    else
                    {
                        if (!mGroupsMapper.ContainsKey(id))
                        {
                            mWorldItem.groupID = id;
                            mClusteringData.IsGroupCached = true;
                            mGroupsMapper[id] = mClusteringData;
                        }
                        else { }
                    }
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 检测世界交换物体的事件
        /// </summary>
        private void CheckWorldEvents()
        {
            if (mWorldEventNotices.Count > 0)
            {
                mItemNotice = mWorldEventNotices.Dequeue();
                if (mEventItems.Count > 0)
                {
                    mEventItem = mEventItems.Dequeue();
                    if (IsEventItemValid())
                    {
                        mEventItem.Dispatch(mItemNotice);//派发世界物体消息
                        mItemNotice.ToPool();
                    }
                    else { }
                }
                else { }
            }
            else { }
        }

        private bool IsEventItemValid()
        {
            return IsWorldItemValid(ref mEventItem) && 
                !mEventItem.isDroped && 
                (mItemNotice != default);
        }

        protected abstract WorldMovement GetMovmentData(ref int target);
        protected abstract void AfterWorldItemCached(ref int target);
        protected abstract void SetClusteringPosition(ref int target);
        protected abstract bool CheckingAround(ref int target, int aroundID, AroundsCheckedInfo aroundInfo, ref WorldMovement movement);

    }
}