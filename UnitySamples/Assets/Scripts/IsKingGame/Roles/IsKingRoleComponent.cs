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

        protected Vector2 mDirection;
        protected WorldMovementComponent mMovementComp;
        protected RolePropertiesComp mRolePropertiesComp;
        protected USpineController mSpineAnimation;

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

            if (mSpineAnimation == default)
            {
                mSpineAnimation = GetComponent<USpineController>();
            }
            else { }

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

        private void LateUpdate()
        {
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

        private bool mIsDead;
        private TimeUpdater mTimer;

        protected virtual void Update()
        {
            if (mIsDead || mRolePropertiesComp == default)
            {
                return;
            }
            else { }

            int hp = mRolePropertiesComp.Hp(RoleEntitas);
            if (hp <= 0)
            {
                mIsDead = true;
                //mSpineAnimation.AddOnEnd(OnAnimDeadCallback);
                mSpineAnimation.PlayAnim(0, "END", false);
                mTimer = TimeUpdater.New(2f, OnAnimDeadCallback);
                mTimer.IsAutoDispose = true;
                return;
            }
            else { }

            BeforeUpdate();
            UpdatePositionByInput();
        }

        private void OnAnimDeadCallback()
        {
            RemoveFromWorld();
            gameObject.GetInstanceID().BroadcastWithParam(EntityComponent.ENTITY_DROPED, true);
            ShipDockApp.Instance.AssetsPooling.ToPool(m_PoolName, gameObject);
        }
    }

}