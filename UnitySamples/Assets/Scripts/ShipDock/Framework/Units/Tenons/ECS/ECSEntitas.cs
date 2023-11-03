using System.Collections.Generic;

namespace ShipDock
{
    /// <summary>
    /// ECS 实体管理器
    /// </summary>
    public class ECSEntitas : Tenon
    {
        public readonly static ChunkInfo NULL_ENTITY = new()
        {
            chunkIndex = -1,
            itemIndex = -1,
        };

        private static int entityID = 0;

        private Dictionary<int, int> mEntitas;
        private Dictionary<int, int[]> mEntityTypes;
        private ChunkGroup<Entity> mAllEntitas;
        private Dictionary<int, ChunkInfo> mAllEntitasMap;
        private Queue<int> mIdleEntitasIDs;

        protected override void Purge()
        {
            mIdleEntitasIDs?.Clear();
            mAllEntitasMap?.Clear();
            mEntitas?.Clear();
            mEntityTypes?.Clear();

            mEntitas = default;
        }

        protected override void Reset()
        {
            base.Reset();
            
            if (mEntitas == default)
            {
                mIdleEntitasIDs = new Queue<int>();
                mAllEntitasMap = new Dictionary<int, ChunkInfo>();
                mEntitas = new Dictionary<int, int>();
                mEntityTypes = new Dictionary<int, int[]>();
            }
            else { }

            int preload = 10000;
            ChunkGroup<Entity>.Init(0, 0, preload);
            mAllEntitas = ChunkGroup<Entity>.Instance;
        }

        public override void SetupTenon(Tenons tenons)
        {
            base.SetupTenon(tenons);
        }

        public void BuildEntiy(int entityType, params int[] ids)
        {
            bool flag = mEntityTypes.TryGetValue(entityType, out _);
            if (flag) { }
            else 
            {
                int[] chunkGroupIDs = new int[ids.Length];
                for (int i = 0; i < ids.Length; i++)
                {
                    chunkGroupIDs[i] = ids[i];
                }
                mEntityTypes[entityType] = chunkGroupIDs;
            }
        }

        public int CreateEntiyByType(int entityType)
        {
            int entity = -1;
            bool flag = mEntityTypes.TryGetValue(entityType, out int[] ids);
            if (flag)
            {
                if (mIdleEntitasIDs.Count > 0)
                {
                    entity = mIdleEntitasIDs.Dequeue();
                }
                else
                {
                    entity = entityID;
                    entityID++;
                }

                flag = mAllEntitasMap.TryGetValue(entity, out ChunkInfo info);
                if (flag) { }
                else
                {
                    mAllEntitas.Pop(entity, ref info);
                    mAllEntitasMap[entity] = info;

                    Entity item = mAllEntitas.GetItem(info.chunkIndex, info.itemIndex);
                    item.entityID = entity;
                    item.entityType = entityType;
                }

                int componentID;
                IECSComponentBase componentBase;

                ECS ecs = ECS.Instance;
                int max = ids.Length;
                for (int i = 0; i < max; i++)
                {
                    componentID = ids[i];
                    componentBase = ecs.GetComponentByBase(componentID);
                    if (componentBase != default)
                    {
                        componentBase.BindEntity(entity);
                    }
                    else { }
                }
            }
            return entity;
        }

        public ChunkInfo GetEntityByID(int entity, out bool isValid)
        {
            isValid = mAllEntitasMap.TryGetValue(entity, out ChunkInfo info);
            return isValid ? info : NULL_ENTITY;
        }

        public void DestroyEntity(int entity)
        {
            if (mAllEntitasMap.TryGetValue(entity, out ChunkInfo info))
            {
                Entity item = mAllEntitas.GetItem(info.chunkIndex, info.itemIndex);
                int entityType = item.entityType;
                bool flag = mEntityTypes.TryGetValue(entityType, out int[] ids);
                if (flag)
                {
                    mIdleEntitasIDs.Enqueue(entity);
                    mAllEntitas.Drop(info.chunkIndex, info.itemIndex);
                    mAllEntitasMap.Remove(entity);

                    int componentID;
                    IECSComponentBase componentBase;

                    ECS ecs = ECS.Instance;
                    int max = ids.Length;
                    for (int i = 0; i < max; i++)
                    {
                        componentID = ids[i];
                        componentBase = ecs.GetComponentByBase(componentID);
                        if (componentBase != default)
                        {
                            componentBase.DebindEntity(entity);
                        }
                        else { }
                    }

                    item.entityID = int.MaxValue;
                    item.entityType = int.MaxValue;
                }
                else { }
            }
            else { }
        }
    }

    public static class ECSStatics
    {

    }
}
