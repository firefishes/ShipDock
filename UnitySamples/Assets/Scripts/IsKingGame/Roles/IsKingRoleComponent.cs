using Joypie;
using ShipDock.Applications;
using ShipDock.Commons;
using ShipDock.ECS;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class IsKingRoleComponent : RoleComponent
    {
        [SerializeField]
        private int m_PoolName = 0;
        [SerializeField]
        private int m_MovementComponentName = 0;
        [SerializeField]
        protected float m_SpeedMax = 1f;
        [SerializeField]
        protected USpineController m_SpineController;
        [SerializeField]
        protected SkeletonAnimation m_Animation;

        protected Vector2 mDirection;
        protected WorldMovementComponent mMovementComp;
        protected RolePropertiesComp mRolePropertiesComp;

        protected int mRoleState;

        protected int RoleEntitas
        {
            get
            {
                return m_EntityComponent != default ? m_EntityComponent.Entitas : default;
            }
        }

        protected override void InitRoleComponents()
        {
            base.InitRoleComponents();

            mIsDead = false;
            gameObject.SetActive(true);
            m_Animation.enabled = true;

            ShipDockApp app = ShipDockApp.Instance;
            ILogicContext context = app.ECSContext.CurrentContext;
            mMovementComp = context.RefComponentByName(m_MovementComponentName) as WorldMovementComponent;
            mMovementComp.SyncMovement(RoleEntitas, transform, true);

            mMovementComp.MoveSpeed(RoleEntitas, 0f);
            mMovementComp.MoveSpeedMax(RoleEntitas, m_SpeedMax);

            mRolePropertiesComp = context.RefComponentByName(Consts.COMP_ROLE_PROPERTIES) as RolePropertiesComp;

            AddToWorld();
        }

        public void AddToWorld()
        {
            IParamNotice<IUpdate> notice = Pooling<ParamNotice<IUpdate>>.From();
            notice.ParamValue = RoleUpdater;
            MessageModular.AddMessage(Consts.MSG_ADD_UPDATER, notice);
        }

        public void RemoveFromWorld()
        {
            IParamNotice<IUpdate> notice = Pooling<ParamNotice<IUpdate>>.From();
            notice.ParamValue = RoleUpdater;
            MessageModular.AddMessage(Consts.MSG_RM_UPDATER, notice);
        }

        protected virtual float GetHorizontalAxis()
        {
            return 0f;
        }

        protected virtual float GetVerticalAxis()
        {
            return 0f;
        }

        protected void UpdateMoveDirection()
        {
            mDirection = Vector2.zero;

            float h = GetHorizontalAxis();
            float v = GetVerticalAxis();

            if (h != 0f)
            {
                mDirection.x = -h;
            }
            else { }


            if (v != 0f)
            {
                mDirection.y = v;
            }
            else { }
        }

        protected void UpdatePositionByInput()
        {
            if (IsUpdateValid() && mMovementComp != default)
            {
                UpdateMoveDirection();

                bool flag = mDirection != Vector2.zero;
                if (flag)
                {
                    //Vector3 pos = mMovementComp.GetLocalPosition(RoleEntitas);
                    //transform.localPosition = pos;

                    float speedMax = mMovementComp.GetMoveSpeedMax(RoleEntitas);

                    float h = mDirection.x;
                    float v = mDirection.y;
                    h = Mathf.Abs(h);
                    v = Mathf.Abs(v);

                    mMovementComp.MoveSpeed(RoleEntitas, Mathf.Max(h, v) * speedMax);

                    if (mRoleState != 1)
                    {
                        mRoleState = 1;
                        DuringMove();
                    }
                    else { }
                }
                else
                {
                    mMovementComp.MoveSpeed(RoleEntitas, 0f);
                    if (mRoleState != 0)
                    {
                        mRoleState = 0;
                        DuringStopMove();
                    }
                    else { }
                }

                mMovementComp.ShouldMove(RoleEntitas, flag);
                mMovementComp.MoveDirection(RoleEntitas, mDirection);
                mMovementComp.Forward(RoleEntitas, mDirection);
            }
            else { }
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();

            if (mRoleState == 1)
            {
                Vector3 pos = mMovementComp.GetLocalPosition(RoleEntitas);
                transform.localPosition = pos;
            }
            else { }
        }

        protected virtual void DuringMove()
        {
        }

        protected virtual void DuringStopMove()
        {
        }

        protected bool IsUpdateValid()
        {
            return m_EntityComponent != default;
        }

        protected virtual void BeforeUpdate()
        {
        }

        public override void OnUpdate(int time)
        {
            base.OnUpdate(time);

            if (mIsDead || mRolePropertiesComp == default)
            {
                return;
            }
            else { }

            int hp = mRolePropertiesComp.Hp(RoleEntitas);
            if (hp <= 0)
            {
                mIsDead = true;

                RemoveFromWorld();

                const string animNameEnd = "END";
                //m_SpineController.PlayAnim(0, animNameEnd, false);
                m_Animation.AnimationState.SetAnimation(0, animNameEnd, false);

                if (mTimer == default)
                {
                    mTimer = TimeUpdater.GetTimeUpdater(2f, OnAnimDeadCallback, default, 1);
                    mTimer.Start();
                }
                else
                {
                    mTimer.Restart();
                }
                return;
            }
            else { }

            BeforeUpdate();
            UpdatePositionByInput();
        }

        private bool mIsDead;
        private TimeUpdater mTimer;

        private void OnAnimDeadCallback()
        {
            m_Animation.enabled = false;
            gameObject.SetActive(false);
            gameObject.GetInstanceID().BroadcastWithParam(EntityComponent.ENTITY_DROPED, true);
            ShipDockApp.Instance.AssetsPooling.ToPool(m_PoolName, gameObject);

            Consts.N_MONSTER_COLLECTED.BroadcastWithParam(UnityEngine.Random.Range(0, 4), true);
            MonsterSpawner.allMonster--;
        }
    }

}