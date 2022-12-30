using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Mathematics;
using System.Numerics;
using ShipDock.Applications;

namespace IsKing
{
    public struct MonsterSpanwUComp : IComponentData
    {
        public int willCreateCount;
        public int resID;
        public float time;
        public float timeGap;
        public bool spanwReady;
        public Entity spanwer;
    }

    public struct MonsterMovementUComp : IComponentData, IEnableableComponent
    {
        public float3 direction;
        public float3 target;
        public float lockDownTime;
        public float lockDownGapTime;
        public float moveSpeed;
        public float radiansPerSecond;
    }

    public struct HeroMovementUComp : IComponentData, IEnableableComponent
    {
        public float3 direction;
        public float moveSpeed;
    }

    public struct PlayerInputUComp : IComponentData, IEnableableComponent
    {
        public float2 input;
    }

    public struct Role : IComponentData, IEnableableComponent
    {
        public int noticeName;
    }

    public readonly partial struct MonsterSpanwAspect : IAspect
    {
        readonly RefRO<MonsterRes> resUnit;

        public Entity CreateMonster(EntityCommandBuffer.ParallelWriter ecbPW, int sortKey, Entity e)
        {
            return ecbPW.Instantiate(sortKey, e);
        }
    }

    public readonly partial struct MovementAspect : IAspect
    {
        readonly RefRO<Role> role;
        readonly RefRW<LocalTransform> transform;
        readonly RefRW<MonsterMovementUComp> monsterMovement;
        readonly RefRW<HeroMovementUComp> heroMovement;

        public void MonsterMoveToHero(float deltaTime)
        {
            monsterMovement.ValueRW.lockDownTime += deltaTime;
            
            float3 vec = monsterMovement.ValueRW.target - transform.ValueRW.Position;
            if (monsterMovement.ValueRW.lockDownTime >= monsterMovement.ValueRW.lockDownGapTime)
            {
                //����ʱ������ɣ�����У׼����
                monsterMovement.ValueRW.lockDownTime = 0f;
                vec = math.normalize(vec);
            }
            else { }

            //����ǰ�����ķ���ǰ��
            float t = monsterMovement.ValueRW.radiansPerSecond * deltaTime;
            vec = math.lerp(monsterMovement.ValueRW.direction, vec, t);

            //���·����λ��
            monsterMovement.ValueRW.direction = vec;
            transform.ValueRW.Position += vec * monsterMovement.ValueRW.moveSpeed * deltaTime;

            //����λ��
            var cur = transform.ValueRW.Position;
            Vector3 v = new Vector3(cur.x, cur.y, cur.z);
            role.ValueRO.noticeName.BroadcastWithParam(v, true);
        }
    }
}