using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class MeleeSystem : LogicSystem
    {
        private BehaviourIDsComponent mIDsComp;
        private AttackableComp mAttackableComp;
        private RolePropertiesComp mRolePropertiesComp;
        private HeroMovementComp mHeroMovementComp;
        private MonsterMovementComp mMonsterMovementComp;

        protected override void BeforeUpdateComponents()
        {
            base.BeforeUpdateComponents();

            if (mIDsComp == default)
            {
                mIDsComp = GetRelatedComponent<BehaviourIDsComponent>(Consts.COMP_BEHAVIOUR_IDS);
            }
            else { }

            if (mAttackableComp == default)
            {
                mAttackableComp = GetRelatedComponent<AttackableComp>(Consts.COMP_ATTACKABLE);
            }
            else { }

            if (mRolePropertiesComp == default)
            {
                mRolePropertiesComp = GetRelatedComponent<RolePropertiesComp>(Consts.COMP_ROLE_PROPERTIES);
            }
            else { }

            if (mHeroMovementComp == default)
            {
                mHeroMovementComp = GetRelatedComponent<HeroMovementComp>(Consts.COMP_HERO_MOVEMENT);
            }
            else { }

            if (mMonsterMovementComp == default)
            {
                mMonsterMovementComp = GetRelatedComponent<MonsterMovementComp>(Consts.COMP_MONSTER_MOVEMENT);
            }
            else { }

        }

        private WorldMovementComponent mMovementComp;
        private WorldMovementComponent mEnemyMovementComp;
        private Dictionary<int, int> mIDSMapper;
        private Dictionary<int, int>.Enumerator mIDsEnumer;
        private KeyValuePair<int, int> mIDsCurrent;
        private ScopeChecker mScopeChecker = ScopeChecker.GetScopeChecker(0f, 0f);

        private List<int> mDeads = new List<int>();

        public override void Execute(int entitas, int componentName, ILogicData data)
        {
            switch (componentName)
            {
                case Consts.COMP_ATTACKABLE:
                    int roleEntitas = mAttackableComp.GetRoleOwner(entitas);
                    float range = mAttackableComp.GetAttackRange(entitas);
                    int atk = mAttackableComp.GetAttack(entitas);

                    BehaviourIDs ids = mIDsComp.GetEntitasData(roleEntitas) as BehaviourIDs;
                    ids.isChecking = true;

                    mIDSMapper = mIDsComp.GetAroundIDs(roleEntitas);
                    if (mIDSMapper == default)
                    {
                        return;
                    }
                    else { }

                    mIDsEnumer = mIDSMapper.GetEnumerator();

                    int camp = mRolePropertiesComp.Camp(roleEntitas);
                    int roleAtk = mRolePropertiesComp.Atk(roleEntitas);

                    mMovementComp = camp == 0 ? (WorldMovementComponent)mMonsterMovementComp : mHeroMovementComp;
                    mEnemyMovementComp = mMovementComp == mMonsterMovementComp ? (WorldMovementComponent)mHeroMovementComp : mMonsterMovementComp;

                    //Vector3 enemyPos;
                    Vector3 forward = mMovementComp.GetForward(roleEntitas);

                    //bool volid;
                    int targetEntitas, id, hp;
                    int max = mIDSMapper.Count;
                    List<int> list = new List<int>();
                    for (int i = 0; i < max; i++)
                    {
                        mIDsEnumer.MoveNext();
                        mIDsCurrent = mIDsEnumer.Current;
                        id = mIDsCurrent.Key;

                        targetEntitas = mIDsComp.GetEntitasByGameObjectID(id);

                        if (targetEntitas != default)
                        {
                            hp = mRolePropertiesComp.Hp(targetEntitas);
                            if (hp > 0)
                            {
                                hp -= (atk + roleAtk);
                                mRolePropertiesComp.Hp(targetEntitas, hp);
                            }
                            else { }
                            if (hp <= 0)
                            {
                                mDeads.Add(id);
                            }
                            else { }
                        }
                        else
                        {
                            int a = 0;
                        }

                    }

                    for (int i = 0; i < mDeads.Count; i++)
                    {
                        mIDSMapper.Remove(mDeads[i]);
                    }

                    ids.isChecking = false;
                    mDeads.Clear();
                    break;
            }
        }
    }

    public class HurtSystem : LogicSystem
    {
        public override void Execute(int entitas, int componentName, ILogicData data)
        {
        }
    }

    public class AttackableComp : DataComponent<LogicData>
    {
        private float[] mRange;
        private int[] mAttacks;
        private int[] mRoleOwners;

        protected override void DropData(ref ILogicData target)
        {
            base.DropData(ref target);

            int index = target.DataIndex;
            mRange[index] = default;
            mAttacks[index] = default;
            mRoleOwners[index] = default;
        }

        protected override void OnResetSuccessive(bool clearOnly = false)
        {
            base.OnResetSuccessive(clearOnly);

            Utils.Reclaim(ref mRange, clearOnly);
            Utils.Reclaim(ref mAttacks, clearOnly);
            Utils.Reclaim(ref mRoleOwners, clearOnly);
        }

        protected override void UpdateDataStretch(int dataSize)
        {
            base.UpdateDataStretch(dataSize);

            Utils.Stretch(ref mRange, dataSize);
            Utils.Stretch(ref mAttacks, dataSize);
            Utils.Stretch(ref mRoleOwners, dataSize);
        }

        #region ¹¥»÷·¶Î§
        public void AttackRange(int entitas, float range)
        {
            UpdateValidWithType(entitas, ref mRange, out _, range);
        }

        public float GetAttackRange(int entitas)
        {
            float result = GetDataValueWithType(entitas, ref mRange, out _);
            return result;
        }
        #endregion

        #region ¹¥»÷Á¦
        public void Attack(int entitas, int atk)
        {
            UpdateValidWithType(entitas, ref mAttacks, out _, atk);
        }

        public int GetAttack(int entitas)
        {
            int result = GetDataValueWithType(entitas, ref mAttacks, out _);
            return result;
        }
        #endregion

        #region ËùÊô½ÇÉ«
        public void RoleOwner(int entitas, int roleEntitas)
        {
            UpdateValidWithType(entitas, ref mRoleOwners, out _, roleEntitas);
        }

        public int GetRoleOwner(int entitas)
        {
            int result = GetDataValueWithType(entitas, ref mRoleOwners, out _);
            return result;
        }
        #endregion
    }


    public class RoleProperties : LogicData
    {
        public int hp;
        public int atk;
        public int camp;

        public override string ToString()
        {
            return string.Format("hp = {0}, atk = {1}", hp, atk);
        }
    }

    public class RolePropertiesComp : DataComponent<RoleProperties>
    {

        protected override void DropData(ref ILogicData target)
        {
            base.DropData(ref target);

            RoleProperties roleProperties = target as RoleProperties;
            roleProperties.hp = 0;
            roleProperties.atk = 0;
            roleProperties.camp = 0;
        }

        public void Hp(int entitas, int hp)
        {
            RoleProperties properties = GetEntitasData(entitas) as RoleProperties;
            properties.hp = hp;
        }

        public int Hp(int entitas)
        {
            RoleProperties properties = GetEntitasData(entitas) as RoleProperties;
            return properties.hp;
        }

        public void Atk(int entitas, int atk)
        {
            RoleProperties properties = GetEntitasData(entitas) as RoleProperties;
            properties.atk = atk;
        }

        public int Atk(int entitas)
        {
            RoleProperties properties = GetEntitasData(entitas) as RoleProperties;
            return properties.atk;
        }

        public void Camp(int entitas, int camp)
        {
            RoleProperties properties = GetEntitasData(entitas) as RoleProperties;
            properties.camp = camp;
        }

        public int Camp(int entitas)
        {
            RoleProperties properties = GetEntitasData(entitas) as RoleProperties;
            return properties.camp;
        }
    }

    public class WorldResourceComp : DataComponent<LogicData>
    {
        private Queue<int> mMonstersWillActive;

        public WorldResourceComp()
        {
            mMonstersWillActive = new Queue<int>();
        }

        public override void Reset(bool clearOnly = false)
        {
            base.Reset(clearOnly);

            Utils.Reclaim(ref mMonstersWillActive, clearOnly);
        }

        public void AddMonsterToActive(int gameObjectID)
        {
            mMonstersWillActive.Enqueue(gameObjectID);
        }

        public void ActiveMonster(int count)
        {
            //count = Mathf.Min(count, mMonstersWillActive.Count);
            if (mMonstersWillActive.Count > 0)
            {
                for (int i = 0; i < mMonstersWillActive.Count; i++)
                {
                    int gameObjectID = mMonstersWillActive.Dequeue();
                    gameObjectID.BroadcastWithParam(EntityComponent.ENTITY_SETUP, true);
                }
            }
            else { }
        }
    }


}