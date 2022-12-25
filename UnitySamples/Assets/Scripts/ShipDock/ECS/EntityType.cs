using ShipDock.ECS;
using ShipDock.Pooling;
using ShipDock.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public class EntityType
    {

        public int Type { get; private set; }

        public int SizePerEntity { get; private set; }
        public int EntityCount { get; private set; }
        public int CapacityPerChunk { get; set; }
        public int SizePerData { get; set; }

        private ChunkUnit mCurret;
        private List<ChunkUnit> mEntityChunk;

        public EntityType(int type)
        {
            Type = type;

            mEntityChunk = new List<ChunkUnit>();

            SizePerEntity = LogicEntities.sizeOfInt32;
        }

        public void AddComponentSizePerData(int size)
        {
            SizePerData = size;
            SizePerEntity += size;
        }

        public void AddChunk(ChunkUnit unit)
        {
            mEntityChunk.Add(unit);

            EntityCount = 0;
            mCurret = unit;
        }

        public bool ShouldAddEntity()
        {
            return mCurret != default ? (EntityCount < CapacityPerChunk) : false;
        }

        public Entity BindEntity(int entity, int entityType, ref Chunks chunks)
        {
            Entity resut;

            if (ShouldAddEntity())
            {
                EntityCount++;
            }
            else
            {
                EntityCount = 0;

                chunks.CreateChunk(entity, this);
                mEntityChunk.Add(mCurret);
            }

            resut = new Entity()
            {
                entity = entity,
                entityType = entityType,
                chunkIndex = mCurret.ChunkIndex,
                index = EntityCount,
            };

            mCurret.SetEntityID(EntityCount, entity);

            return resut;
        }

        public void SetCurrentChunk(ChunkUnit unit)
        {
            mCurret = unit;
        }

    }

    public class ChunkUnit : IPoolable
    {
        public static int sizeOfBytesPerChunk = 16 * 1000;

        public int ChunkIndex { get; private set; }
        public int SizePerData { get; private set; }
        public EntityType EntityTypeValue { get; private set; }

        private byte[] mData;
        private int mCompDataStartPostiion;
        private ByteBuffer mBuffer;

        public void Revert()
        {
        }

        public void ToPool() { }

        public void InitByEntityType(EntityType entityType, int chunkIndex)
        {
            mData = new byte[sizeOfBytesPerChunk];
            mBuffer = ByteBuffer.Allocate(mData);

            EntityTypeValue = entityType;
            ChunkIndex = chunkIndex;
            SizePerData = entityType.SizePerData;

            var max = EntityTypeValue.CapacityPerChunk;
            for (int i = 0; i < max; i++)
            {
                mBuffer.WriteInt(0);
            }

            mCompDataStartPostiion = mBuffer.ReadPosition();
        }

        public void SetEntityID(int index, int entity)
        {
            mBuffer.MarkWriterIndex();
            mBuffer.SetWritePostition(LogicEntities.sizeOfInt32 * index);
            mBuffer.WriteInt(entity);
            mBuffer.ResetWriterIndex();
        }

        public void FillEntityCompData(int index, ref byte[] data)
        {
            int position = mCompDataStartPostiion + (SizePerData * index);
            mBuffer.ReadBytes(ref data, position, SizePerData);
        }
    }

    public class Chunks
    {

        private List<ChunkUnit> mAllChunks { get; set; }

        public Chunks()
        {
            mAllChunks = new List<ChunkUnit>();
        }

        public void CreateChunk(int entity, EntityType entityTypeResult)
        {
            ChunkUnit chunkUnit = Pooling<ChunkUnit>.From();
            mAllChunks.Add(chunkUnit);

            chunkUnit.InitByEntityType(entityTypeResult, mAllChunks.Count);

            entityTypeResult.SetCurrentChunk(chunkUnit);
        }
    }

    public struct Entity
    {
        public int entity;
        public int entityType;
        public int chunkIndex;
        public int index;
    }

}