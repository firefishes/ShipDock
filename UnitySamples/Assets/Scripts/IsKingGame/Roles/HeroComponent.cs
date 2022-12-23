using System;
using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;
using UnityEngine;

namespace IsKing
{
    public class HeroComponent : IsKingRoleComponent
    {
        private int mMeleeWeapont;

        protected override void InitRoleComponents()
        {
            base.InitRoleComponents();

            mMovementComp.Aacceleration(RoleEntitas, 100f);
            mMovementComp.ShouldMove(RoleEntitas, false);

            Consts.N_GET_HERO_POS.Add(OnGetHeroPosition);

            mSpineAnimation.PlayAnim(0, "INTO", false);

            mRolePropertiesComp.Camp(RoleEntitas, 1);
            mRolePropertiesComp.Hp(RoleEntitas, 100);
            mRolePropertiesComp.Atk(RoleEntitas, 40);

            ILogicContext context = ShipDockECS.Instance.Context;
            ILogicEntities allEntitas = context.AllEntitas;
            allEntitas.AddEntitas(out mMeleeWeapont, 3);

            AttackableComp attackableComp = allEntitas.GetComponentFromEntitas<AttackableComp>(mMeleeWeapont, Consts.COMP_ATTACKABLE);
            attackableComp.RoleOwner(mMeleeWeapont, RoleEntitas);
            attackableComp.Attack(mMeleeWeapont, 30);
            attackableComp.AttackRange(mMeleeWeapont, m_PhysicsChecker.OverlapRayAndHit.radius);
        }

        private void OnGetHeroPosition(INoticeBase<int> param)
        {
            IParamNotice<Vector3> notice = param as IParamNotice<Vector3>;
            notice.ParamValue = transform.localPosition;
        }

        protected override float GetHorizontalAxis()
        {
            const string HAxisName = "Horizontal";
            return Input.GetAxis(HAxisName);
        }

        protected override float GetVerticalAxis()
        {
            const string VAxisName = "Vertical";
            return Input.GetAxis(VAxisName);
        }

        protected override void DuringMove()
        {
            base.DuringMove();

            mSpineAnimation.PlayAnim(0, "RUN", true);
        }

        protected override void DuringStopMove()
        {
            base.DuringStopMove();

            int rand = Utils.UnityRangeRandom(0, 2);
            string animName = rand == 0 ? "STAND_1" : "STAND_2";
            mSpineAnimation.PlayAnim(0, animName, true);
        }
    }

}