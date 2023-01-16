#define MANAGED_UPDATE

using ShipDock.Applications;
using ShipDock.ECS;
#if MANAGED_UPDATE
#else
using System.Diagnostics;
#endif
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.Networking.UnityWebRequest;

namespace IsKing
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct MonsterSpanwSystem : ISystem
    {
        //private EntityManager mEMgr;
        //private EntityCommandBuffer mECB;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DefaultWorldExecute>();

            //mEMgr = state.EntityManager;
            //mECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            //mEMgr = default;
            //mECB = default;
        }

        //private Vector3 GetSpawnPosition()
        //{
        //    Vector3 pos = Consts.N_GET_HERO_POS.BroadcastWithParam(new Vector3(), true);

        //    int direction = UnityEngine.Random.Range(0, 4);
        //    Vector3 spawnPos = default;
        //    switch (direction)
        //    {
        //        case 0:
        //            spawnPos = new Vector3(UnityEngine.Random.Range(-20f, -15f), UnityEngine.Random.Range(-30f, 30f), pos.z);
        //            break;
        //        case 1:
        //            spawnPos = new Vector3(UnityEngine.Random.Range(15f, 20f), UnityEngine.Random.Range(-30f, 30f), pos.z);
        //            break;
        //        case 2:
        //            spawnPos = new Vector3(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(30f, 35f), pos.z);
        //            break;
        //        case 3:
        //            spawnPos = new Vector3(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-35f, -30f), pos.z);
        //            break;
        //    }
        //    return pos;
        //}

        private int mCount;

#if MANAGED_UPDATE
#else
        [BurstCompile]
#endif
        public void OnUpdate(ref SystemState state)
        {
            var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

            var mgr = state.EntityManager;
            //var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            //var heroResRaw = SystemAPI.GetSingleton<HeroRes>();
            //if (heroResRaw.isCreat) { }
            //else 
            //{
            //    heroResRaw.isCreat = true;

            //    var prefab = heroResRaw.roleRes;
            //    var hero = ecb.Instantiate(prefab);

            //    ecb.AddComponent<Role>(hero);
            //    ecb.AddComponent<PlayerInputUComp>(hero);
            //    ecb.AddComponent<HeroMovementUComp>(hero);
            //}

#if MANAGED_UPDATE
            foreach (var (resSingleton, entity) in SystemAPI.Query<MonsterResManaged>().WithEntityAccess())
#else
            //foreach (var (resSingleton, entity) in SystemAPI.Query<RefRW<MonsterRes>>().WithEntityAccess())
#endif
            {
#if MANAGED_UPDATE
                if (resSingleton.hasSpanwPoint) { }
                else
                {
                    //产生刷怪点
                    resSingleton.hasSpanwPoint = true;
                    resSingleton.spanwPoint = ecb.CreateEntity();
                    MonsterSpanwUComp comp = new()
                    {

                    };
                    ecb.AddComponent<MonsterSpanwUComp>(resSingleton.spanwPoint, new()
                    {
                        time = 0f,
                        timeGap = 1f,
                        willCreateCount = 3000,
                        resID = resSingleton.resID,
#if MANAGED_UPDATE
#else
                        spanwer = resSingleton.resEntity,
#endif
                    });
                }
#else
                var raw = SystemAPI.GetSingleton<MonsterRes>();
                var monsterResEntity = raw.roleRes;
                if (raw.hasSpanwPoint) { }
                else
                {
                    //产生刷怪点
                    raw.hasSpanwPoint = true;
                    SystemAPI.SetSingleton(raw);

                    Entity spanwPoint = ecb.CreateEntity();
                    ecb.AddComponent<MonsterSpanwUComp>(spanwPoint, new()
                    {
                        time = 0f,
                        timeGap = 1f,
                        willCreateCount = 10,
                        resID = raw.resID,
                        spanwer = monsterResEntity,
                    });

                    ecb.AddComponent<Role>(monsterResEntity);
                    ecb.AddComponent<MonsterMovementUComp>(monsterResEntity);
                }
#endif
                    }

            float deltaTime = SystemAPI.Time.DeltaTime;

#if MANAGED_UPDATE
            foreach (var (spanwComp, resSingleton, entity) in SystemAPI.Query<RefRW<MonsterSpanwUComp>, MonsterResManaged>().WithEntityAccess())
#else
            foreach (var (spanwComp, entity) in SystemAPI.Query<RefRW<MonsterSpanwUComp>>().WithEntityAccess())
#endif
            {
                if (spanwComp.ValueRW.resID == 0)
                {
                    break;
                }
                else { }

                spanwComp.ValueRW.time += deltaTime;

                if (spanwComp.ValueRW.time > spanwComp.ValueRW.timeGap)
                {
                    spanwComp.ValueRW.time -= spanwComp.ValueRW.timeGap;

                    if (spanwComp.ValueRW.spanwReady)
                    {
                        spanwComp.ValueRW.spanwReady = false;
                        spanwComp.ValueRW.willCreateCount--;

                        //刷怪
                        if (spanwComp.ValueRW.willCreateCount <= 0) { }
                        else
                        {
                            //var job = new MonsterCreaterJob()
                            //{
                            //    ecbPW = ecb,
                            //};
                            //state.Dependency = job.Schedule(state.Dependency);
                            //job.Run();
                            var random = Random.CreateFromIndex(mUpdateCounter++);

                            //ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

#if MANAGED_UPDATE
                            Entity newMonster = mgr.CreateEntity();
                            UnityEngine.GameObject newRole = UnityEngine.GameObject.Instantiate(resSingleton.resRef);
                            ecb.AddComponent(newMonster, new RoleManaged()
                            {
                                binded = newMonster,
                                transRef = newRole.transform,
                            });
#else
                            Entity newMonster = ecb.Instantiate(spanwComp.ValueRW.spanwer);
#endif
                            //ecb.AddComponent(newMonster, new MonsterMovementUComp()
                            //{
                            //    moveSpeed = random.NextFloat(),
                            //    direction = float3.zero,
                            //    lockDownGapTime = 1f,
                            //    radiansPerSecond = 15f,
                            //});

                            //ecb.SetComponent(newMonster, new WorldTransform()
                            //{
                            //    Position = new float3(0f, 0f, 0f),
                            //    Scale = 1f,
                            //});

                            //var transform = SystemAPI.GetAspectRW<TransformAspect>(newMonster);
                            //transform.LocalPosition = new float3(12f, 20f, 0f);
                            //transform.LocalScale = 1f;

                            mCount++;
                        }
                    }
                    else { }
                }
                else
                {
                    spanwComp.ValueRW.spanwReady = true;
                }
            }
        }

        public uint mUpdateCounter;

        [WithAll(typeof(MonsterSpanwUComp))]
        [BurstCompile]
        public partial struct MonsterCreaterJob : IJobEntity
        {
            public uint mUpdateCounter;
            public EntityCommandBuffer ecbPW;

            [BurstCompile]
            public void Execute(in MonsterSpanwUComp spanw)
            {
                var random = Random.CreateFromIndex(mUpdateCounter++);
                Entity newMonster = ecbPW.Instantiate(spanw.spanwer);
                ecbPW.SetComponent(newMonster, new MonsterMovementUComp()
                {
                    moveSpeed = random.NextFloat(),
                    direction = float3.zero,
                    lockDownGapTime = 1f,
                    radiansPerSecond = 15f,
                });
            }
        }


    }
}
