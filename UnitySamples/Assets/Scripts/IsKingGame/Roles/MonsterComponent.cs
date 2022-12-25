using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class MonsterComponent : IsKingRoleComponent
    {
        private Vector3 mDirectionToHero;
        private MonsterMovementComp mMovement;

        protected override void InitRoleComponents()
        {
            base.InitRoleComponents();

            mDirectionToHero = Vector3.zero;

            mMovementComp.Aacceleration(RoleEntitas, 100f);
            mMovementComp.ShouldMove(RoleEntitas, true);

            mMovement = mMovementComp as MonsterMovementComp;
            mMovement.LockTargetDownTimeMax(RoleEntitas, 0.5f, 0.5f);

            mRolePropertiesComp.Camp(RoleEntitas, 0);
            mRolePropertiesComp.Hp(RoleEntitas, 50);
            mRolePropertiesComp.Atk(RoleEntitas, 25);

            const string animNameStand = "STAND";
            //m_SpineController.PlayEmptyAnim(0);
            //m_SpineController.PlayAnim(0, animNameStand, true);
            m_Animation.AnimationState.SetAnimation(0, animNameStand, true);

            //Debug.Log(RoleEntitas.ToString() + " Stand");

            UpdateLockDown();
        }

        protected override void BeforeUpdate()
        {
            base.BeforeUpdate();

            bool shouldMove = mMovement.ShouldMove(RoleEntitas);
            if (shouldMove)
            {
                bool flag = mMovement.GetRelockDownTarget(RoleEntitas);
                if (flag)
                {
                    UpdateLockDown();
                }
                else { }
            }
            else
            {
                mDirectionToHero = Vector3.zero;
            }
        }

        private void UpdateLockDown()
        {
            Vector3 pos = Consts.N_GET_HERO_POS.BroadcastWithParam(new Vector3(), true);
            mMovement.MoveToPosition(RoleEntitas, pos);
            mMovement.RelockDownTarget(RoleEntitas, false);

            mDirectionToHero = (pos - transform.localPosition).normalized;

            //float distance = Vector3.Distance(pos, transform.localPosition);
            //bool flag = Mathf.Abs(distance) <= 5f;
            //mMovement.WillMelee(RoleEntitas, flag);

            //if (flag)
            //{
            //    int hp = mRolePropertiesComp.Hp(RoleEntitas);
            //    hp -= 10;
            //}
            //else { }
        }

        protected override float GetHorizontalAxis()
        {
            return -mDirectionToHero.x;
        }

        protected override float GetVerticalAxis()
        {
            return mDirectionToHero.y;
        }
    }

}