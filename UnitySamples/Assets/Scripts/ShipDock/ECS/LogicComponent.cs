
#if G_LOG
#define _DROP_ENTITAS_LOG
#define _DROP_ENTITAS_DATA_INDEX_ERROR
#define _ENTITAS_VALID_LENGTH_ERROR
#define _NEW_INDEX_SET_LOG
#define _PREWARM_DATA_ERROR
#endif

using ECS;
using ShipDock.Tools;
using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.ECS
{
    /// <summary>
    /// ECS 组件基类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class LogicComponent : ECSLogic, ILogicComponent
    {
        /// <summary>组件数据状态：空的数据位</summary>
        public const int DATA_STATE_EMPTY = 1;
        /// <summary>组件数据状态：已废弃</summary>
        public const int DATA_STATE_DROPED = 2;
        /// <summary>组件数据状态：就绪</summary>
        public const int DATA_STATE_READY = 3;
        /// <summary>组件数据状态：修改生效</summary>
        public const int DATA_STATE_VALID = 4;

        /// <summary>初始化时数据的规模</summary>
        protected int mDataSizeStart = 10;
        /// <summary>数据规模伸缩的速率</summary>
        protected float mDataStretchRatio = 2f;
        /// <summary>数据列表</summary>
        protected ILogicData[] mLogicDatas;

        /// <summary>已关联的系统标识位</summary>
        private IdentBitsGroup mRelatedSystems;
        /// <summary>已缓存的数据索引</summary>
        private Dictionary<int, int> mDataIndexCached;
        /// <summary>组件实体集合</summary>
        private Dictionary<int, int> mComponentEntitas;
        /// <summary>组件数据集合</summary>
        private Dictionary<int, ILogicData> mComponentDatas;
        /// <summary>已回收的索引值</summary>
        private Queue<int> mCollectedIndexs;
        /// <summary>实体ID列表</summary>
        private int[] mEntitasValid;
        /// <summary>数据状态列表</summary>
        private int[] mDatasValid;

        private Queue<int> mPoolingWaiting;
        private Queue<ILogicData> mPoolingWaitingDatas;

        /// <summary>当前数据的数量</summary>
        protected int DataSize { private get; set; }

        protected ILogicContext Context { get; private set; }

        public override bool IsSystem
        {
            get
            {
                return false;
            }

            protected set { }
        }

        public bool HasDataChanged { get; private set; }

        /// <summary>当前数据的索引位</summary>
        public int DataPosition { get; private set; }

        public LogicComponent()
        {
            //ArrayPool<int>
            //MemoryPool<int>
            //byte[] vs = new byte[1024];
            //ByteBuffer b = ByteBuffer.Allocate(1024);
            //b.read

            mRelatedSystems = new IdentBitsGroup();
            mDataIndexCached = new Dictionary<int, int>();
            mComponentEntitas = new Dictionary<int, int>();
            mComponentDatas = new Dictionary<int, ILogicData>();
            mCollectedIndexs = new Queue<int>();
            mPoolingWaiting = new Queue<int>();
            mPoolingWaitingDatas = new Queue<ILogicData>();

            Debug.Log(sizeof(bool));
        }

        public virtual Type[] GetEntityDataSizeOf()
        {
            return new Type[] { typeof(int), typeof(int) };
        }

        #region 销毁和重置
        protected override void Purge()
        {
            Reset(true);

            mEntitasValid = default;
            mDatasValid = default;
            mLogicDatas = default;

            mComponentEntitas = default;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset(bool clearOnly = false)
        {
            Utils.Reclaim(ref mEntitasValid, clearOnly);
            Utils.Reclaim(ref mDatasValid, clearOnly);
            Utils.Reclaim(ref mLogicDatas, clearOnly);
            Utils.Reclaim(ref mPoolingWaiting, clearOnly);
            Utils.Reclaim(ref mPoolingWaitingDatas, clearOnly);

            mRelatedSystems.Reset();
            mDataIndexCached.Clear();
            mComponentEntitas.Clear();
            mComponentDatas.Clear();
            mCollectedIndexs.Clear();

            DataPosition = 0;
            DataSize = mDataSizeStart;

            if (clearOnly)
            {
                //仅清理数据时不做其他处理
            }
            else
            {
                OnResetSuccessive(clearOnly);
                UpdateDataStretch(DataSize);
            }
        }

        protected abstract void OnResetSuccessive(bool clearOnly = false);
#endregion

        /// <summary>
        /// 更新所有需要做规模伸缩的数据列表
        /// </summary>
        protected virtual void UpdateDataStretch(int dataSize)
        {
            Utils.Stretch(ref mEntitasValid, dataSize);
            Utils.Stretch(ref mDatasValid, dataSize);
            Utils.Stretch(ref mLogicDatas, dataSize);
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="context"></param>
        public override void Init(ILogicContext context)
        {
            Context = context;

            Reset();
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="entitasID"></param>
        /// <returns></returns>
        public virtual void SetEntitas(int entitasID)
        {
            bool isValid = default;
            bool flag = IsCorrectState(entitasID, DATA_STATE_DROPED, out bool hasEntitas);
            if (flag)
            {
                isValid = true;
            }
            else { }

            if (hasEntitas) { }
            else
            {
                isValid = true;
            }

            if (isValid)
            {
                SetEntitasStateEmpty(entitasID);

                ILogicData data = CreateData(entitasID);
                FillEntitasData(entitasID, data);
            }
            else { }
        }

        private void SetEntitasStateEmpty(int entitasID)
        {
            mComponentEntitas[entitasID] = DATA_STATE_EMPTY;
        }

        private void SetEntitasStateReady(int entitasID)
        {
            mComponentEntitas[entitasID] = DATA_STATE_READY;
        }

        private void SetEntitasStateDroped(int entitasID)
        {
            mComponentEntitas[entitasID] = DATA_STATE_DROPED;
        }

        private void SetDataEmpty(int dataIndex)
        {
            mCollectedIndexs.Enqueue(dataIndex);

            mDatasValid[dataIndex] = DATA_STATE_EMPTY;
            mEntitasValid[dataIndex] = default;
            mLogicDatas[dataIndex] = default;
        }

        private void SetDataValid(int dataIndex)
        {
            mDatasValid[dataIndex] = DATA_STATE_VALID;
            HasDataChanged = true;
        }

        private void SetDataReady(int dataIndex)
        {
            int state = mDatasValid[dataIndex];
            if (state == DATA_STATE_VALID)
            {
                mDatasValid[dataIndex] = DATA_STATE_READY;
            }
            else { }
        }

        private bool IsCorrectState(int entitasID, int stateValue, out bool hasEntitas)
        {
            bool result = default;
            int state = GetEntitasState(entitasID, out hasEntitas);
            if (hasEntitas)
            {
                result = state == stateValue;
            }
            else { }
            return result;
        }

        public bool IsStateRegular(int entitasID, out bool hasEntitas)
        {
            bool result = default;
            int state = GetEntitasState(entitasID, out hasEntitas);
            if (hasEntitas)
            {
                result = state > DATA_STATE_DROPED;
            }
            else { }
            return result;
        }

        /// <summary>
        /// 实现此方法，创建组件数据对象
        /// </summary>
        protected abstract ILogicData CreateData(int entitas);

        /// <summary>
        /// 废弃已添加此组件对应的数据
        /// </summary>
        protected abstract void DropData(ref ILogicData target);

        /// <summary>
        /// 覆盖此方法，填充组件数据
        /// </summary>
        public virtual void FillEntitasData(int entitasID, ILogicData data)
        {
            bool flag = IsCorrectState(entitasID, DATA_STATE_EMPTY, out _);
            if (flag)
            {
                SetEntitasStateReady(entitasID);

                data.BindComponent(entitasID, this);
                mComponentDatas[entitasID] = data;
            }
            else { }
        }

        /// <summary>
        /// 获取已添加此组件的实体相对应的数据
        /// </summary>
        public ILogicData GetEntitasData(int entitasID)
        {
            bool flag = mComponentDatas.TryGetValue(entitasID, out ILogicData data);
            return flag ? data : default;
        }

        /// <summary>
        /// 根据实体 ID 获取组件关联的实体状态
        /// </summary>
        public int GetEntitasState(int entitasID, out bool hasEntitas)
        {
            hasEntitas = mComponentEntitas.TryGetValue(entitasID, out int result);

            result = hasEntitas ? result : DATA_STATE_EMPTY;

            return result;
        }

        /// <summary>
        /// 标记实体为可废弃
        /// </summary>
        public void WillDrop(int entitasID)
        {
            ILogicData data = mComponentDatas[entitasID];
            if (data != default)
            {
                int dataIndex = data.DataIndex;
                if (dataIndex >= 0)
                {
                    if (GetDataByIndex(dataIndex) == data)
                    {
                        SetEntitasStateDroped(entitasID);
                    }
                    else { }
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 废弃实体期间的操作
        /// </summary>
        protected virtual void DuringDropEntitas(int entitasID) { }

        /// <summary>
        /// 标记对应的数据为待更新
        /// </summary>
        public ILogicData UpdateValid(int entitasID, bool ignoreState = false)
        {
            ILogicData result = default;
            if (ignoreState)
            {
                result = GetEntitasData(entitasID);
            }
            else
            {
                bool flag = IsStateRegular(entitasID, out _);
                if (flag)
                {
                    result = GetEntitasData(entitasID);
                }
                else { }
            }

            if (result != default)
            {
                PrewarmData(entitasID, ref result);

                if (DataPosition >= DataSize)
                {
                    DataSize = (int)(DataSize * mDataStretchRatio);
                    UpdateDataStretch(DataSize);
                }
                else { }
            }
            else { }

            return result;
        }

        /// <summary>
        /// 对待更新的数据做预备操作
        /// </summary>
        /// <param name="entitas"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual int PrewarmData(int entitas, ref ILogicData target)
        {
            bool flag = mDataIndexCached.TryGetValue(entitas, out int index);

            if (flag) { }
            else
            {
                if (mCollectedIndexs.Count > 0)
                {
                    index = mCollectedIndexs.Dequeue();
                }
                else
                {
                    index = DataPosition;
                    DataPosition++;
                }
                mDataIndexCached[entitas] = index;
            }

            if (mEntitasValid.Length <= index)
            {
#if ENTITAS_VALID_LENGTH_ERROR
                const string entitasValidLengthError = "error: entitas valid length error, need length {0}, now is {1}";
                entitasValidLengthError.Log((index + 1).ToString(), mEntitasValid.Length.ToString());
#endif
            }
            else { }

            if (mEntitasValid[index] != entitas)
            {
                mEntitasValid[index] = entitas;

                if (target != default)
                {
                    target.SetDataIndex(index);

#if NEW_INDEX_SET_LOG
                    const string newIndexSetLog = "log:[{0}] Logic data index set: [{1}] = entitas {2}";
                    newIndexSetLog.Log(Name, index.ToString(), entitas.ToString());
#endif

                    if (mLogicDatas[index] == default)
                    {
                        mLogicDatas[index] = target;
                    }
                    else
                    {
#if PREWARM_DATA_ERROR
                        const string dataError = "error:[{0}] Logic data [{1}] exsited, entitas is {2}";
                        dataError.Log(Name, index.ToString(), entitas.ToString());
#endif
                    }
                }
                else { }
            }
            else { }

            SetDataValid(index);

            return index;
        }

        public void CheckAllDataValided()
        {
            int max = DataPosition;
            for (int i = 0; i < max; i++)
            {
                SetDataReady(i);
            }

            HasDataChanged = false;
        }

        protected abstract void DuringRecycleEntitasData(int index, ILogicData data);

        public void CheckAllDropedEntitas()
        {
            if (mPoolingWaiting.Count > 0)
            {
                int index;
                ILogicData target;
                while (mPoolingWaiting.Count > 0)
                {
                    index = mPoolingWaiting.Dequeue();
                    target = mPoolingWaitingDatas.Dequeue();

                    DuringRecycleEntitasData(index, target);
                }
            }
            else { }

            int entitasID;
            bool hasEntitas, flag;
            int max = DataPosition;
            for (int i = 0; i < max; i++)
            {
                entitasID = GetEntitasIDByIndex(i);
                flag = IsCorrectState(entitasID, DATA_STATE_DROPED, out hasEntitas);
                if (flag)
                {
                    DropEntitas(entitasID);
                }
                else { }
            }
        }

        /// <summary>
        /// 废弃实体
        /// </summary>
        private void DropEntitas(int entitasID)
        {
            ILogicData data = mComponentDatas[entitasID];
            if (data != default)
            {
                int index = mDataIndexCached[entitasID];

                mComponentDatas.Remove(entitasID);
                mDataIndexCached.Remove(entitasID);

                int dataIndex = data.DataIndex;
                if (dataIndex >= 0 && dataIndex == index)
                {
                    SetDataEmpty(dataIndex);

                    DuringDropEntitas(entitasID);

#if DROP_ENTITAS_LOG
                    const string dropEntitasLog = "log: entitas {0} drop commit, remove from {1}, data index comfirmed {2}";
                    dropEntitasLog.Log(entitasID.ToString(), Name, data.DataIndex.ToString());
#endif
                    data.IsRecycling = true;

                    mPoolingWaiting.Enqueue(index);
                    mPoolingWaitingDatas.Enqueue(data);

                    DropData(ref data);
                }
                else
                {
#if DROP_ENTITAS_DATA_INDEX_ERROR
                    const string dropEntitasDataIndexError = "error: entitas {0} drop error during {1}, data index missing (In data: {2}; In cached {3}; Cached now entitas is {4})";
                    int ce = GetEntitasIDByIndex(index);
                    dropEntitasDataIndexError .Log(entitasID.ToString(), Name, data.DataIndex.ToString(), index.ToString(), ce.ToString());
#endif
                }
            }
            else { }
        }

        public int GetEntitasIDByIndex(int index)
        {
            return mEntitasValid.Length > index ? mEntitasValid[index] : -1;
        }

        public bool IsDatasChanged(int index)
        {
            return mDatasValid[index] == DATA_STATE_VALID;
        }

        public ILogicData GetDataByIndex(int index)
        {
            return mLogicDatas[index];
        }

        protected string CheckOtherComponentDataInfo(int componentName, int entitas)
        {
            string result = Context.RefComponentByName(componentName).GetEntitasData(entitas).ToString();
            return result;
        }
    }
}