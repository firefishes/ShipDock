using ECS;
using ShipDock.Applications;
using ShipDock.ECS;
using Sirenix.OdinInspector.Demos;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace IsKing
{
    [BurstCompile]
    [UpdateAfter(typeof(MonsterSpanwSystem))]
    public partial struct MovementSystem : ISystem
    {
        private EntityQuery mEntityQuery;
        private ComponentTypeHandle<HeroMovementUComp> mHeroMovementCompHandler;
        private ComponentTypeHandle<MonsterMovementUComp> mMonsterMovementHandler;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DefaultWorldExecute>();

            var builder = new EntityQueryBuilder(Allocator.Temp);
            builder.WithAll<HeroMovementUComp>().WithAll<MonsterMovementUComp>();

            mEntityQuery = state.GetEntityQuery(builder);

            mHeroMovementCompHandler = state.GetComponentTypeHandle<HeroMovementUComp>();
            mMonsterMovementHandler = state.GetComponentTypeHandle<MonsterMovementUComp>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //return;
            EntityManager mgr = state.EntityManager;

            mHeroMovementCompHandler.Update(ref state);
            mMonsterMovementHandler.Update(ref state);

            NativeArray<Unity.Entities.Entity> entites = mEntityQuery.ToEntityArray(Allocator.Temp);
            //var heroMovements = mEntityQuery.ToComponentDataArray<HeroMovementUComp>(Allocator.Temp);
            //var monsterMovements = mEntityQuery.ToComponentDataArray<MonsterMovementUComp>(Allocator.Temp);

            var chunks = mEntityQuery.ToArchetypeChunkArray(Allocator.Temp);
            for (int i = 0; i < chunks.Length; i++)
            {
                var chunk = chunks[i];
                for (int j = 0, entityCount = chunk.Count; j < entityCount; j++)
                {
                    NativeArray<MonsterMovementUComp> mmList = chunk.GetNativeArray(ref mMonsterMovementHandler);
                    //MovementAspect aspect = SystemAPI.GetAspectRW<MovementAspect>(entity);
                    //chunk.
                    var mmListItem = mmList[j];
                    mmListItem.target = default;
                }
            }
        }
    }

}