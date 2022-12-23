using ShipDock.ECS;
using ShipDock.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public class EntityType
    {

        public int Type { get; private set; }

        public long SizePerData { get; private set; }
        public int EntityMax { get; private set; }
        public long CapacityPerChunk { get; set; }

        public EntityType(int type)
        {
            Type = type;

            SizePerData += LogicEntities.sizeOfInt32;
        }

        public void AddComponentSizePerData(int size)
        {
            SizePerData += size;
        }

        public void SetEntityMax(int count)
        {
            EntityMax = count;
        }
    }

    public class ChunkUnit : IPoolable
    {

        public void Revert() { }

        public void ToPool() { }
    }

    public class Chunks
    {
        private ChunkUnit mCurret;

        private List<ChunkUnit> mAllChunks { get; set; }

        public Chunks()
        {
            mAllChunks = new List<ChunkUnit>();
        }

        public void Alloc()
        {

        }
    }

}