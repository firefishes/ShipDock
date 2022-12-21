using ShipDock.Applications;
using ShipDock.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class IsKingRoleComponent : RoleComponent
    {
        [SerializeField]
        private int m_MovementComponentName = 0;
        [SerializeField]
        protected float m_SpeedMax = 1f;

        protected WorldMovementComponent mMovementComponent;

        protected override void InitRoleComponents()
        {
            base.InitRoleComponents();

            ShipDockApp app = ShipDockApp.Instance;
            ILogicContext context = app.ECSContext.CurrentContext;
            mMovementComponent = context.RefComponentByName(m_MovementComponentName) as WorldMovementComponent;
            mMovementComponent.SyncMovement(m_EntityComponent.Entitas, transform, true);

            mMovementComponent.MoveSpeed(m_EntityComponent.Entitas, 0f);
            mMovementComponent.MoveSpeedMax(m_EntityComponent.Entitas, m_SpeedMax);
        }
    }

}