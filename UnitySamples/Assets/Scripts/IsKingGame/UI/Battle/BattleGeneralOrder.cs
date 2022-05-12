using ShipDock.Notices;
using ShipDock.Tools;
using ShipDock.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IsKing
{
    public class BattleGeneralOrder : UISubgroup
    {
        [SerializeField]
        private RectTransform m_HandCards;
        [SerializeField]
        private GameObject m_CardItemRenderer;

        private Queue<CardInfoController> mCardsGained;

        public override string ChangerTaskName { get; protected set; } = "GeneralOrder";
        public override float ChangerTaskerDuring { get; protected set; } = 0f;

        protected override void OnUIHandler(INoticeBase<int> param)
        {
            base.OnUIHandler(param);

            switch (param.Name)
            {
                case UIBattleModular.UI_UPDATE_BATTLE_CARDS:
                    mCardsGained = (param as IParamNotice<Queue<CardInfoController>>).ParamValue;
                    break;
            }
        }

        protected override void TaskerChange(UI ui, TimeGapper timeGapper)
        {
            base.TaskerChange(ui, timeGapper);
        }
    }

}