using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Modulars;
using ShipDock.Notices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public class PeaceWorldModular : WorldModular
    {
        public PeaceWorldModular() : base(Consts.M_WORLD, Consts.MSG_ADD_UPDATER, Consts.MSG_RM_UPDATER)
        {
            //WorldInteracter
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

            ILogicContext worldContext = contexts.CurrentContext;
            worldContext.Create<PeaceMovementComp>(Consts.COMP_MOVEMENT);
            worldContext.Create<WorldSystem>(Consts.SYSTEM_WORLD, true, Consts.COMP_MOVEMENT);

            ShipDockApp.Instance.StartECS();
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
            base.SettleMessageQueue(message, notice);
        }
    }
}
