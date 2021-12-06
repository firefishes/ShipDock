using ShipDock.Notices;
using ShipDock.Tools;
using ShipDock.UI;
using UnityEngine;
using UnityEngine.UI;

namespace IsKing
{
    public class BattleGeneralOrder : UISubgroup
    {
        [SerializeField]
        private Text m_Text;

        public override string ChangerTaskName { get; protected set; } = "GeneralOrder";
        public override float ChangerTaskerDuring { get; protected set; } = 0f;

        protected override void OnUIHandler(INoticeBase<int> param)
        {
            base.OnUIHandler(param);
        }

        protected override void TaskerChange(UI ui, TimeGapper timeGapper)
        {
            base.TaskerChange(ui, timeGapper);
        }
    }

}