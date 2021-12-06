using System;
using System.Collections;
using System.Collections.Generic;
using ShipDock.Notices;
using ShipDock.Tools;
using ShipDock.UI;
using UnityEngine;
using UnityEngine.UI;

namespace IsKing
{
    public class BattleIntelligence : UISubgroup
    {
        [SerializeField]
        private Text m_Text;

        private Vector2 mValue = Vector2.zero;
        private Vector2 mTarget;

        public override string ChangerTaskName { get; protected set; } = "battleIntellige";
        public override float ChangerTaskerDuring { get; protected set; } = 1f;

        protected override void OnUIHandler(INoticeBase<int> param)
        {
            base.OnUIHandler(param);

            switch (param.Name)
            {
                case UIBattleModular.UI_UPDATE_INTELLIGENCE:
                    mTarget = (param as IParamNotice<Vector2>).ParamValue;
                    break;
            }
        }

        protected override void TaskerChange(UI ui, TimeGapper timeGapper)
        {
            base.TaskerChange(ui, timeGapper);

            float x = mValue.x;
            x = Mathf.Lerp(x, mTarget.x, timeGapper.Progress);

            mValue.x = x;
            mValue.y = mTarget.y;

            m_Text.text = "情报:".Append(((int)mTarget.x).ToString(), "/", ((int)mTarget.y).ToString());
        }
    }

}