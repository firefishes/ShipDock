using ShipDock.Notices;
using ShipDock.Tools;
using ShipDock.UI;
using UnityEngine;
using UnityEngine.UI;

namespace IsKing
{
    public class BattleTroops : UISubgroup
    {
        [SerializeField]
        private Text m_Text;

        private Vector2Int mTarget = Vector2Int.zero;

        public override string ChangerTaskName { get; protected set; } = "battleTroops";
        public override float ChangerTaskerDuring { get; protected set; } = 1f;

        protected override void Start()
        {
            base.Start();

            Consts.D_BATTLE.DataNotify(Consts.DN_BATTLE_DATA_UPDATE);
        }

        protected override void OnUIHandler(INoticeBase<int> param)
        {
            base.OnUIHandler(param);

            switch (param.Name)
            {
                case UIBattleModular.UI_UPDATE_TROOPS:
                    mTarget = (param as IParamNotice<Vector2Int>).ParamValue;
                    break;
            }
        }

        protected override void TaskerChange(UI ui, TimeGapper timeGapper)
        {
            base.TaskerChange(ui, timeGapper);

            m_Text.text = "兵力:".Append(mTarget.x.ToString(), "/", mTarget.y.ToString());
        }
    }

}