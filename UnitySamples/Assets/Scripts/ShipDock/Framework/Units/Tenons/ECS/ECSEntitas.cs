using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock
{
    public class ECSEntitas : Tenon
    {
        private static int entityID = 0;

        private Dictionary<int, int> mEntitas;
        private Dictionary<int, int[]> mEntityTypes;

        protected override void Purge()
        {
            mEntitas?.Clear();
            mEntityTypes?.Clear();
        }

        protected override void Reset()
        {
            base.Reset();

            if (mEntitas == default)
            {
                mEntitas = new Dictionary<int, int>();
                mEntityTypes = new Dictionary<int, int[]>();
            }
            else { }
        }

        public override void SetupTenon(Tenons tenons)
        {
            base.SetupTenon(tenons);
        }

        public void BuildEntiy<T>(int entityType, params int[] ids)
        {
            bool flag = mEntityTypes.TryGetValue(entityType, out int[] chunkGroupIDs);
            if (flag) { }
            else 
            {
                chunkGroupIDs = new int[ids.Length];
                for (int i = 0; i < ids.Length; i++)
                {
                    chunkGroupIDs[i] = ids[i];
                }
            }
        }

        public int CreateEntiyByType(int entityType)
        {
            bool flag = mEntityTypes.TryGetValue(entityType, out int[] ids);
            if (flag)
            {
                //IChunkGroup chunkGroup;
                for (int i = 0; i < ids.Length; i++)
                {
                    //ShipDockApp.Instance.Tenons.AddTenonByType<>();
                }
            }
            return entityID;
        }
    }

    public class ECS : Singletons<ECS>
    {
        private Dictionary<int, IChunkGroup> mAllChunkGroups;

        public ECSEntitas AllEntitas { get; private set; }

        public void InitECS()
        {
            mAllChunkGroups = new Dictionary<int, IChunkGroup>();

            Tenons tenons = ShipDockApp.Instance.Tenons;
            AllEntitas = tenons.AddTenonByType<ECSEntitas>(ShipDockConsts.TENON_ECS_ALL_ENTITAS);
        }

        public void InitChunkGroup<T>(int groupID, int sizePerInstance = 0, int preloadSize = 0, int totalBytesPerChunk = 14) where T : IECSData, new()
        {
            ChunkGroup<T>.Init(sizePerInstance, preloadSize, totalBytesPerChunk);
            ChunkGroup<T>.Instance.SetGroupID(groupID);

            mAllChunkGroups[groupID] = ChunkGroup<T>.Instance;
        }

        public IChunkGroup GetChunkGroup(int groupID)
        {
            mAllChunkGroups.TryGetValue(groupID, out IChunkGroup result);
            return result;
        }
    }
}
