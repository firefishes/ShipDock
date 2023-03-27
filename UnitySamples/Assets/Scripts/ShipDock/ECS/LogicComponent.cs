
#if G_LOG
#define _DROP_ENTITAS_LOG
#define _DROP_ENTITAS_DATA_INDEX_ERROR
#define _ENTITAS_VALID_LENGTH_ERROR
#define _NEW_INDEX_SET_LOG
#define _PREWARM_DATA_ERROR
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// ECS 组件基类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class LogicComponent : ECSLogic, ILogicComponent
    {

        /// <summary>数据状态值, 数据实体 ID 值</summary>
        private static Type[] entityDataSizeOf = new Type[] { typeof(int), typeof(int) };

        /// <summary>组件数据状态：没有数据</summary>
        public const int DATA_STATE_NONE = 0;
        /// <summary>组件数据状态：空的数据位</summary>
        public const int DATA_STATE_EMPTY = 1;
        /// <summary>组件数据状态：已废弃</summary>
        public const int DATA_STATE_DROPED = 2;
        /// <summary>组件数据状态：就绪</summary>
        public const int DATA_STATE_READY = 3;
        /// <summary>组件数据状态：修改生效</summary>
        public const int DATA_STATE_VALID = 4;

        /// <summary>单个组件数据的尺寸</summary>
        private int mSizePerData;
        /// <summary>已关联的系统标识位</summary>
        private IdentBitsGroup mRelatedSystems;

        public byte[] DataBuffs { get; set; }

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

        public LogicComponent()
        {
            //ArrayPool<int>
            //MemoryPool<int>
            //byte[] vs = new byte[1024];
            //ByteBuffer b = ByteBuffer.Allocate(1024);
            //ByteBuffer b = ByteBuffer.Allocate(1024);
            //b.read

            Debug.Log(sizeof(bool));

            mRelatedSystems = new IdentBitsGroup();
        }

        public virtual Type[] GetEntityDataSizeOf()
        {
            return entityDataSizeOf;
        }

        #region 销毁和重置
        protected override void Purge()
        {
            Reset(true);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset(bool clearOnly = false)
        {
            mRelatedSystems.Reset();
        }
        #endregion

        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="context"></param>
        public override void Init(ILogicContext context)
        {
            Context = context;

            Reset();
        }

        public List<int> ChunkUnits { get; private set; } = new List<int>();

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public virtual void SetEntitas(int entityID)
        {
            ChunkDataInfo(entityID, out int dataPosition, out int dataIndex, out ChunkUnit chunkUnit);
            if (chunkUnit != default)
            {
                ChunkUnits.Add(chunkUnit.ChunkIndex);
                chunkUnit.GetComponentDataStart(ID, dataIndex, out dataPosition);

                bool isValid = default;
                int state = chunkUnit.GetDataInt(dataPosition, ID, "DataState");
                switch (state)
                {
                    case DATA_STATE_NONE:
                    case DATA_STATE_EMPTY:
                    case DATA_STATE_DROPED:
                        isValid = true;
                        break;
                }

                if (isValid)
                {
                    chunkUnit.SetDataInt(dataPosition, ID, "DataState", DATA_STATE_READY);
                    chunkUnit.SetDataInt(dataPosition, ID, "EntityID", entityID);

                    CreateData(ref chunkUnit, entityID, dataPosition);
                }
                else { }
            }
            else { }
        }

        protected void ChunkDataInfo(int entityID, out int dataPosition, out int dataIndex, out ChunkUnit chunkUnit)
        {
            dataPosition = -1;
            chunkUnit = default;
            ILogicEntities logicEntities = Context.AllEntitas;
            EntityType entityType = logicEntities.GetEntityType(entityID, out dataIndex, out int chunkIndex);
            if (entityType != default)
            {
                chunkUnit = logicEntities.Chunks.GetChunkUnit(chunkIndex);
                chunkUnit.GetComponentDataStart(ID, dataIndex, out dataPosition);
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
        protected abstract void CreateData(ref ChunkUnit chunkUnit, int entityID, int dataPosition);

        /// <summary>
        /// 废弃已添加此组件对应的数据
        /// </summary>
        protected abstract void DropData(int entityID);

        /// <summary>
        /// 根据实体 ID 获取组件关联的实体状态
        /// </summary>
        public int GetEntitasState(int entityID, out bool hasEntity)
        {
            ChunkDataInfo(entityID, out int dataPosition, out int dataIndex, out ChunkUnit chunkUnit);
            int result = chunkUnit.GetDataInt(dataPosition, ID, "DataState");
            hasEntity = dataIndex >= 0;
            return result;
        }

        /// <summary>
        /// 标记实体为可废弃
        /// </summary>
        public void WillDrop(int entityID)
        {
            ChunkDataInfo(entityID, out int dataPosition, out int dataIndex, out ChunkUnit chunkUnit);
            chunkUnit.SetDataInt(dataPosition, ID, "DataState", DATA_STATE_DROPED);
        }

        public void CheckAllDataValided()
        {
            HasDataChanged = false;
        }

        public void CheckAllDropedEntitas()
        {
        }

        /// <summary>
        /// 废弃实体
        /// </summary>
        private void DropEntity(int entityID)
        {
            ChunkDataInfo(entityID, out int dataPosition, out int dataIndexValue, out ChunkUnit chunkUnit);
            chunkUnit.SetDataInt(dataPosition, ID, "DataState", DATA_STATE_EMPTY);
        }

        public void SetSizePerData(int size)
        {
            if (DataBuffs == default)
            {
                mSizePerData = size;
                DataBuffs = new byte[mSizePerData];
            }
            else { }
        }

        public int GetSizePerData()
        {
            return mSizePerData;
        }

        public virtual string[] GetEntityDataKeys()
        {
            return new string[]
            {
                "DataState",
                "EntityID",
            };
        }
    }
}