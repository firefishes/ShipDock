using System;
using System.Collections.Generic;

namespace ShipDock
{
    public enum UpdatePipelineType
    {
        UpdatePineline = 0,
        FixedUpdatePineline,
        LateUpdatePineline,
    }

    public class UpdatePipeline : IReclaim
    {
        private int mAddItemNoticeName = 0;
        private int mRemoveItemNoticeName = 1;

        private int mSize;
        private IUpdate mItem;
        private IUpdate[] mCacher;
        private Queue<int> mReusedIndexs;
        private List<IUpdate> mDeleteds;
        private event Action<int, IUpdate> mUpdatesEvent;

        public bool IsReclaimed { get; private set; }
        public UpdatePipelineType PipelineType { get; set; }

        public UpdatePipeline(UpdatePipelineType updatePipelineType, int preInitCount = 8)
        {
            PipelineType = updatePipelineType;

            mSize = preInitCount;
            mCacher = new IUpdate[preInitCount];

            mReusedIndexs = new Queue<int>();
            mDeleteds = new List<IUpdate>();
        }

        public void SetAddNoticeName(int addNoticeName = int.MinValue)
        {
            if (addNoticeName != int.MinValue)
            {
                mAddItemNoticeName = addNoticeName;
                mUpdatesEvent += OnAddItem;
            }
            else { }
        }

        public void SetRemoveNoticeName(int removeNoticeName = int.MinValue)
        {
            if (removeNoticeName != int.MinValue)
            {
                mRemoveItemNoticeName = removeNoticeName;
                mUpdatesEvent += OnRemoveItem;
            }
            else { }
        }

        public void Reclaim()
        {
            IsReclaimed = true;
            
            Utils.Reclaim(ref mReusedIndexs);
            Utils.Reclaim(ref mCacher);
            Utils.Reclaim(ref mDeleteds);

            mReusedIndexs = default;
            mUpdatesEvent = default;
            mItem = default;
        }

        private bool CheckUpdateTypeForTarget(ref IUpdate target)
        {
            bool result = target != default;
            if (result)
            {
                //判断目标是否处于本更新器的管理范围
                switch (PipelineType)
                {
                    case UpdatePipelineType.UpdatePineline:
                        result = target.IsUpdate;
                        break;
                    case UpdatePipelineType.FixedUpdatePineline:
                        result = target.IsFixedUpdate;
                        break;
                    case UpdatePipelineType.LateUpdatePineline:
                        result = target.IsLateUpdate;
                        break;
                    default:
                        throw new Exception("Please set the update pipeline type");
                }
            }
            else 
            {
                throw new Exception("Update target is null");
            }
            return result;
        }

        public void AddUpdate(IUpdate target)
        {
            if (CheckUpdateTypeForTarget(ref target))
            {
                mUpdatesEvent?.Invoke(mAddItemNoticeName, target);
            }
            else { }
        }

        public void RemoveUpdate(IUpdate target)
        {
            if (CheckUpdateTypeForTarget(ref target))
            {
                mUpdatesEvent?.Invoke(mRemoveItemNoticeName, target);
            }
            else { }
        }

        /// <summary>
        /// 根据参数确定目标的可移除标记
        /// </summary>
        /// <param name="target"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private bool MarkTargetForDeletable(ref IUpdate target, bool flag)
        {
            bool result = default;
            if (target != default)
            {
                bool isMarkTrue = flag && !target.WillDelete;
                if (isMarkTrue)
                {
                    result = true;
                    target.WillDelete = flag;
                }
                else
                {
                    bool isMarkFalse = !flag && target.WillDelete;
                    if (isMarkFalse)
                    {
                        result = true;
                        target.WillDelete = flag;
                    }
                    else { }
                }
            }
            else { }
            return result;
        }

        private void OnRemoveItem(int noticeName, IUpdate target)
        {
            if (IsReclaimed || (target == default))
            {
                throw new Exception("Remove update item failed, cacher is reclaimed or target is null");
            }
            else 
            {
                if (mRemoveItemNoticeName == noticeName)
                {
                    if (MarkTargetForDeletable(ref target, true))
                    {
                        //将可标记为删除的目标加入移除列表
                        mDeleteds.Add(target);
                    }
                    else { }
                }
                else { }
            }
        }

        private void OnAddItem(int noticeName, IUpdate target)
        {
            if (IsReclaimed || (target == default))
            {
                throw new Exception("Add update item failed, cacher is reclaimed or target is null");
            }
            else 
            {
                if (noticeName == mAddItemNoticeName)
                {
                    if (MarkTargetForDeletable(ref target, false))
                    {
                        //将可标记为不删除的目标从移除列表删除
                        mDeleteds.Remove(target);
                    }
                    else { }

                    if (target.Index == -1)
                    {
                        int curIndex;
                        if (mReusedIndexs.Count > 0)
                        {
                            //重用现有空位
                            curIndex = mReusedIndexs.Dequeue();
                        }
                        else
                        {
                            if (mCacher.Length == mSize)
                            {
                                //数组扩容
                                mSize *= 2;
                                Array.Resize(ref mCacher, mSize);
                            }
                            else { }

                            curIndex = mCacher.Length;
                        }
                        target.SetIndex(curIndex);
                        mCacher[curIndex] = target;
                    }
                    else { }
                }
                else { }
            }
        }

        public void Update(float time)
        {
            if (IsReclaimed)
            {
                return;
            }
            else { }

            int max = mCacher.Length;
            for (int i = 0; i < max; i++)
            {
                mItem = mCacher[i];
                if ((mItem != default) && mItem.IsUpdate)
                {
                    mItem.OnUpdate(time);
                }
                else { }
            }
            mItem = default;
        }

        public void FixedUpdate(float time)
        {
            if (IsReclaimed)
            {
                return;
            }
            else { }

            int max = mCacher.Length;
            for (int i = 0; i < max; i++)
            {
                mItem = mCacher[i];
                if ((mItem != default) && mItem.IsFixedUpdate)
                {
                    mItem.OnFixedUpdate(time);
                }
                else { }
            }
            mItem = default;
        }

        public void LateUpdate()
        {
            if (IsReclaimed)
            {
                return;
            }
            else { }

            int max = mCacher.Length;
            for (int i = 0; i < max; i++)
            {
                mItem = mCacher[i];
                if ((mItem != default) && mItem.IsLateUpdate)
                {
                    mItem.OnLateUpdate();
                }
                else { }
            }
            mItem = default;
        }

        public void CheckDeleted()
        {
            if (IsReclaimed)
            {
                return;
            }
            else { }

            int max = mDeleteds.Count;
            if (max > 0)
            {
                int index;
                for (int i = max - 1; i >= 0; i--)
                {
                    mItem = mDeleteds[i];
                    if (mItem.WillDelete)
                    {
                        //重新回收空出的位置索引，减少数组的扩容频次
                        index = mItem.Index;
                        mReusedIndexs.Enqueue(index);
                        //移除待更新目标
                        mItem.SetIndex(-1);
                        mCacher[index] = default;
                    }
                    else { }

                    mDeleteds.RemoveAt(i);
                }

                if (max > (mSize * 0.5f))
                {
                    //待删除列表长度大于待更新数组长度的一半时整理列表
                    mDeleteds.TrimExcess();
                }
                else { }

                mItem = default;
            }
            else { }
        }
    }
}
