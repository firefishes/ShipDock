using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// 实体类型
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class EntityType
    {
        public struct ComponentSizeSection
        {
            public int startPosition;
            public int sectionSize;
            public int sizePerData;
        }

        /// <summary>实体类型值</summary>
        public int Type { get; private set; }
        public Dictionary<int, int[]> CompPropertyDataSize { get; private set; }
        public Dictionary<int, Dictionary<string, int>> CompPropertyKeySize { get; private set; }
        public Dictionary<int, ComponentSizeSection> ComponentSections { get; private set; }
        /// <summary>单个实体所需的数据长度</summary>
        public int SizePerEntity { get; private set; }
        /// <summary>此实体类型当前的实体数量</summary>
        public int EntityCount { get; private set; }
        /// <summary>单个内存块最多能创建的实体数</summary>
        public int CapacityPerChunk { get; private set; }
        /// <summary>单个实体的数据所需的数据长度</summary>
        public int SizePerEntityData { get; set; }

        private int mLastCompSectionSize;
        /// <summary>正在使用的内存块</summary>
        private ChunkUnit mCurret;
        /// <summary>保存此实体类型的所有实体的内存块清单</summary>
        private List<ChunkUnit> mEntityChunk;

        public EntityType(int type)
        {
            Type = type;

            mEntityChunk = new List<ChunkUnit>();

            //单位实体数据长度包括，故默认会需要一个整型数据长度
            SizePerEntity = LogicEntities.sizeOfInt32;

            CompPropertyDataSize = new Dictionary<int, int[]>();
            CompPropertyKeySize = new Dictionary<int, Dictionary<string, int>>();
            ComponentSections = new Dictionary<int, ComponentSizeSection>();
        }

        public void SetCompPropertyDataInfo(int compName, int index, int max)
        {
            bool flag = CompPropertyDataSize.TryGetValue(compName, out int[] info);
            if (flag) { }
            else 
            {
                info = new int[max];
            }
            //info[index] = 
        }

        /// <summary>
        /// 更新单个组件数据所需的数据尺寸
        /// </summary>
        /// <param name="size"></param>
        public void AddCompSizePerData(int compType, int size, int count, int index, string key)
        {
            Dictionary<string, int> keys;
            bool flag = CompPropertyDataSize.TryGetValue(compType, out int[] values);
            if (flag)
            {
                keys = CompPropertyKeySize[compType];
            }
            else
            {
                values = new int[count];
                keys = new Dictionary<string, int>();

                CompPropertyDataSize[compType] = values;
                CompPropertyKeySize[compType] = keys;
            }

            values[index] = SizePerEntityData;
            keys[key] = index;

            //单位组件数据长度包括：组件数据段
            SizePerEntityData += size;
            //单位实体数据长度包括：实体 ID 段、组件数据段
            SizePerEntity += size;
        }

        private int[] mTempValues;
        private Dictionary<string, int> mTemp;

        public int GetKeyPosition(int compType, int start, string key)
        {
            mTemp = CompPropertyKeySize[compType];
            mTempValues = CompPropertyDataSize[compType];

            int index = mTemp[key];
            int size = mTempValues[index];

            return start + size;
        }

        /// <summary>
        /// 记录组件在此类实体中的单位数据长度
        /// </summary>
        /// <param name="compType"></param>
        public void MarkComponentSection(int compType)
        {
            ComponentSizeSection section = new ComponentSizeSection
            {
                sectionSize = SizePerEntityData - mLastCompSectionSize,
                sizePerData = SizePerEntityData,
            };
            ComponentSections[compType] = section;
            mLastCompSectionSize = SizePerEntityData;
        }

        /// <summary>
        /// 设置此实体类型正在使用的内存块
        /// </summary>
        /// <param name="unit"></param>
        public void RebindCurrentChunk(ChunkUnit unit)
        {
            mEntityChunk.Add(unit);

            //重置可用的实体数，直至新的内存块单元用尽
            EntityCount = 0;
            mCurret = unit;
        }

        /// <summary>
        /// 是否可增加实体
        /// </summary>
        /// <returns></returns>
        public bool ShouldAddEntity()
        {
            return mCurret != default ? (EntityCount < CapacityPerChunk) : false;
        }

        /// <summary>
        /// 将实体绑定至内存池对应的块
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="entityType"></param>
        /// <param name="chunks"></param>
        /// <returns></returns>
        public Entity BindEntityToChunk(int entityID, int entityType, Chunks chunks)
        {
            Entity resut;

            if (ShouldAddEntity())
            {
                EntityCount++;
            }
            else
            {
                chunks.CreateChunk(entityID, this);
            }

            //创建实体
            resut = new Entity()
            {
                entityID = entityID,
                entityType = entityType,
                chunkIndex = mCurret.ChunkIndex,
                index = EntityCount,
            };

            mCurret.AddEntityID(EntityCount, entityID);

            return resut;
        }

        public void InitCapacityPerChunk()
        {
            if (SizePerEntity > 0)
            {
                CapacityPerChunk = ChunkUnit.sizeOfBytesPerChunk / SizePerEntity;

                //记录各组件在数据段中的起始位置
                int lastSize = 0;
                int compCount = ComponentSections.Count;
                Dictionary<int, ComponentSizeSection>.Enumerator enumer = ComponentSections.GetEnumerator();
                KeyValuePair<int, ComponentSizeSection> item;
                for (int i = 0; i < compCount; i++)
                {
                    enumer.MoveNext();
                    item = enumer.Current;

                    ComponentSizeSection v = item.Value;
                    v.sectionSize = v.sectionSize * CapacityPerChunk;
                    v.startPosition = lastSize;

                    lastSize = v.sectionSize;

                    int compType = item.Key;
                    ComponentSections[compType] = v;
                }
            }
            else { }
        }
    }

    /// <summary>
    /// 内存块
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class ChunkUnit : IPoolable
    {
        /// <summary>单个内存块的字节数大小（16K）</summary>
        public static int sizeOfBytesPerChunk = 16 * 1000;

        /// <summary>内存块在内存池中的索引号</summary>
        public int ChunkIndex { get; private set; }
        /// <summary>单个实体的数据所需的数据长度（字节数）</summary>
        public int SizePerData { get; private set; }
        /// <summary>此内存块所管理的实体类型</summary>
        public EntityType EntityTypeValue { get; private set; }
        /// <summary>组件数据区在块中的起始位置</summary>
        public int CompDataStartPostiion { get; private set; }

        /// <summary>块数据</summary>
        private byte[] mData;
        /// <summary>块数据缓冲器</summary>
        private ByteBuffer mBuffer;

        public void Revert()
        {
        }

        public void ToPool()
        {
            Pooling<ChunkUnit>.To(this);
        }

        /// <summary>
        /// 根据实体类型初始化内存块
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="chunkIndex"></param>
        public void InitByEntityType(EntityType entityType, int chunkIndex)
        {
            //为块分配内存
            mData = new byte[sizeOfBytesPerChunk];
            mBuffer = ByteBuffer.Allocate(mData);

            //设置块的基础信息
            EntityTypeValue = entityType;
            ChunkIndex = chunkIndex;
            SizePerData = entityType.SizePerEntityData;

            //根据块能容纳的实体数量初始化实体数据段（仅实体 ID 等数据）
            int max = EntityTypeValue.CapacityPerChunk;
            for (int i = 0; i < max; i++)
            {
                mBuffer.WriteInt(0);
            }

            //记录组件数据段在块中的起始位置
            CompDataStartPostiion = mBuffer.ReadPosition();

            //设定该类型后续开辟新实体时应使用的内存块
            entityType.RebindCurrentChunk(this);
        }

        /// <summary>
        /// 将实体 ID 写入内存块
        /// </summary>
        /// <param name="index"></param>
        /// <param name="entityID"></param>
        public void AddEntityID(int index, int entityID)
        {
            int postiion = LogicEntities.sizeOfInt32 * index;

            mBuffer.MarkWriterIndex();
            mBuffer.SetWritePostition(postiion);
            mBuffer.WriteInt(entityID);
            mBuffer.ResetWriterIndex();
        }

        /// <summary>
        /// 填充并初始化实体的组件数据
        /// </summary>
        /// <param name="compName"></param>
        /// <param name="index"></param>
        public void GetComponentDataStart(int compName, int index, out int position)
        {
            EntityType.ComponentSizeSection section = EntityTypeValue.ComponentSections[compName];

            int start = SizePerData * index;
            position = CompDataStartPostiion + section.startPosition + start;
        }

        public int GetDataInt(int start, int compType, string key)
        {
            int position = EntityTypeValue.GetKeyPosition(compType, start, key);

            mBuffer.SetReaderIndex(position);
            int result = mBuffer.ReadInt();
            return result;
        }

        public float GetDataFloat(int start, int compType, string key)
        {
            int position = EntityTypeValue.GetKeyPosition(compType, start, key);

            mBuffer.SetReaderIndex(position);
            float result = mBuffer.ReadFloat();
            return result;
        }

        public void SetDataInt(int start, int compType, string key, int value)
        {
            int position = EntityTypeValue.GetKeyPosition(compType, start, key);

            mBuffer.MarkWriterIndex();
            mBuffer.SetWritePostition(position);
            mBuffer.WriteInt(value);
            mBuffer.ResetWriterIndex();
        }

        public void SetDataFloat(int start, int compType, string key, float value)
        {
            int position = EntityTypeValue.GetKeyPosition(compType, start, key);

            mBuffer.MarkWriterIndex();
            mBuffer.SetWritePostition(position);
            mBuffer.WriteFloat(value);
            mBuffer.ResetWriterIndex();
        }

        public void ReadToBuffs(ref byte[] buff, int x, int y)
        {
            mBuffer.ReadBytes(ref buff, x, y);
        }
    }

    public interface IComponentData
    {
        void Run(byte[] vs);
    }

    /// <summary>
    /// 内存池
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class Chunks
    {
        /// <summary>正在使用的内存块</summary>
        private List<ChunkUnit> ChunksUsing { get; set; }

        public Chunks()
        {
            ChunksUsing = new List<ChunkUnit>();
        }

        /// <summary>
        /// 创建新的内存块
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityTypeResult"></param>
        public void CreateChunk(int entity, EntityType entityTypeResult)
        {
            ChunkUnit chunkUnit = Pooling<ChunkUnit>.From();
            ChunksUsing.Add(chunkUnit);

            int chunkIndex = ChunksUsing.Count - 1;
            chunkUnit.InitByEntityType(entityTypeResult, chunkIndex);
        }

        public void ClearAllUsing()
        {
            int max = ChunksUsing.Count;
            for (int i = 0; i < max; i++)
            {
                ChunksUsing[i].ToPool();
            }
        }

        public ChunkUnit GetChunkUnit(int index)
        {
            return ChunksUsing.Count > index ? ChunksUsing[index] : default;
        }
    }

    /// <summary>
    /// 实体结构
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public struct Entity
    {
        /// <summary>实体 ID</summary>
        public int entityID;
        /// <summary>所属的实体类型</summary>
        public int entityType;
        /// <summary>实体数据所处的内存块索引号</summary>
        public int chunkIndex;
        /// <summary>实体数据位于所处的内存块中的实体索引号</summary>
        public int index;
    }

    public static class ECSUtils
    {
        public static void All(this ILogicSystem system, IComponentFinder finder)
        {
            ILogicContext context = ShipDockECS.Instance.Context;
            Chunks chunks = system.AllEntitas.Chunks;

            ChunkUnit chunk;
            List<int> comps = new List<int>();
            List<Vector3Int> chunkPos = new List<Vector3Int>();
            List<ChunkUnit> list = new List<ChunkUnit>();
            int[] compNames = system.RelateComponents;
            int max = compNames.Length;
            for (int i = 0; i < max; i++)
            {
                ILogicComponent comp = context.RefComponentByName(compNames[i]);

                int n = comp.ChunkUnits.Count;
                for (int j = 0; j < n; j++)
                {
                    comps.Add(comp.ID);

                    int index = comp.ChunkUnits[j];
                    chunk = chunks.GetChunkUnit(index);
                    list.Add(chunk);

                    EntityType.ComponentSizeSection section = chunk.EntityTypeValue.ComponentSections[comp.ID];
                    int start = chunk.CompDataStartPostiion + section.startPosition;
                    int len = section.sectionSize;
                    chunkPos.Add(new Vector3Int(start, len, section.sizePerData));
                }
            }

            finder?.CheckComponents(comps, list, chunkPos);
        }
    }

    public interface IComponentFinder
    {
        void CheckComponents(List<int> comps, List<ChunkUnit> chunkUnits, List<Vector3Int> chunkUnitsPos);
    }

    public abstract class ECSSearcher : IComponentFinder
    {
        private int mComponentChecking;
        private ChunkUnit mChunkChecking;
        private ByteBuffer mChunkBuffer;
        private ByteBuffer mComponentDataBuffer;

        public void Search<T>(ILogicSystem system, T component) where T : ILogicComponent
        {
            ILogicContext context = ShipDockECS.Instance.Context;
            Chunks chunks = system.AllEntitas.Chunks;

            ChunkUnit chunk;
            List<int> comps = new List<int>();
            List<Vector3Int> chunkPos = new List<Vector3Int>();
            List<ChunkUnit> list = new List<ChunkUnit>();

            int index, start, len;
            int n = component.ChunkUnits.Count;
            for (int j = 0; j < n; j++)
            {
                comps.Add(component.ID);

                index = component.ChunkUnits[j];
                chunk = chunks.GetChunkUnit(index);
                list.Add(chunk);

                EntityType.ComponentSizeSection section = chunk.EntityTypeValue.ComponentSections[component.ID];
                start = chunk.CompDataStartPostiion + section.startPosition;
                len = section.sectionSize;
                chunkPos.Add(new Vector3Int(start, len, section.sizePerData));
            }

            CheckComponents(comps, list, chunkPos);
        }

        public void CheckComponents(List<int> comps, List<ChunkUnit> chunkUnits, List<Vector3Int> chunkUnitsPos)
        {
            Vector3Int pos;
            byte[] vs = default, vs2 = default;

            int max = chunkUnits.Count;
            for (int i = 0; i < max; i++)
            {
                mComponentChecking = comps[i];

                mChunkChecking = chunkUnits[i];
                pos = chunkUnitsPos[i];
                mChunkChecking.ReadToBuffs(ref vs, pos.x, pos.y);

                if (mChunkBuffer == default)
                {
                    mChunkBuffer = ByteBuffer.Allocate(vs);
                }
                else
                {
                    mChunkBuffer.Reset(vs);
                }

                int n = vs.Length;
                for (int j = 0; j < n; j += pos.z)
                {
                    mChunkBuffer.ReadBytes(ref vs2, j, pos.z);
                    OnChunkBuffer(ref vs2);
                }

                mChunkBuffer.ClearRef();
            }
        }

        protected virtual void OnChunkBuffer(ref byte[] vs)
        {
            if (mComponentDataBuffer == default)
            {
                mComponentDataBuffer = ByteBuffer.Allocate(vs);
            }
            else
            {
                mComponentDataBuffer.Reset(vs);
            }

            int state = mComponentDataBuffer.ReadInt();
            int entityID = mComponentDataBuffer.ReadInt();
            if (state == LogicComponent.DATA_STATE_VALID)
            {
                SettleComponentData(mComponentChecking, entityID, ref mComponentDataBuffer);
            }
            else { }

            mComponentDataBuffer.ClearRef();
        }

        public abstract void SettleComponentData(int componentName, int entityID, ref ByteBuffer componentDataBuffer);
    }

}