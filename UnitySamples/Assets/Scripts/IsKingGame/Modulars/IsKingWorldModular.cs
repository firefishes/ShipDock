using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Notices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class IsKingWorldModular : WorldModular
    {
        public IsKingWorldModular() : base(Consts.M_WORLD, Consts.MSG_ADD_UPDATER, Consts.MSG_RM_UPDATER)
        {
        }

        public override void InitModular()
        {
            base.InitModular();

            InitECS();

            //IParamNotice<string> notice = Pooling<ParamNotice<string>>.From();
            //notice.ParamValue = "£¡£¡£¡";

            //AddMessageToQueue(Consts.MSG_GAME_READY, notice);
        }

        private void InitECS()
        {
            ECSContext contexts = ShipDockApp.Instance.ECSContext;
            contexts.CreateContext(Consts.ECS_CONTEXT_PEACE);
            contexts.ActiveECSContext();

            IShipDockComponentContext worldContext = contexts.CurrentContext;

            worldContext.Create<IsKingMovementComp>(Consts.COMP_MOVEMENT);

            worldContext.Create<IsKingWorldSystem>(Consts.SYSTEM_WORLD, true, Consts.COMP_MOVEMENT);

            ShipDockApp.Instance.StartECS();
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
            base.SettleMessageQueue(message, notice);
        }
    }

}