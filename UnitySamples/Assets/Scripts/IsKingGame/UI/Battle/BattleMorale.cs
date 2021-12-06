using ShipDock.Notices;
using ShipDock.Tools;
using ShipDock.UI;
using UnityEngine;
using UnityEngine.UI;

namespace IsKing
{
    public class BattleMorale : UISubgroup
    {
        [SerializeField]
        private Text m_Text;

        private Vector2 mTarget;

        public override string ChangerTaskName { get; protected set; } = "battleMorale";
        public override float ChangerTaskerDuring { get; protected set; } = 0.1f;

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
                case UIBattleModular.UI_UPDATE_MORALE:
                    mTarget = (param as IParamNotice<Vector2>).ParamValue;
                    break;
            }
        }

        protected override void TaskerChange(UI ui, TimeGapper timeGapper)
        {
            base.TaskerChange(ui, timeGapper);

            m_Text.text = "士气:".Append(((int)mTarget.x).ToString(), "/", ((int)mTarget.y).ToString());
        }
    }
}
