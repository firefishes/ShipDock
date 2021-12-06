using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace IsKing
{
    [Serializable]
    public class SkillItem : GameItem
    {
#if ODIN_INSPECTOR
        [LabelText("等级"), ShowIf("@this.m_EditEnabled == true"), Indent()]
#endif
        public int level = 1;

#if ODIN_INSPECTOR
        private int mSkillType;
        private void OnSkillTypeChanged()
        {
            mSkillType = skillType;
        }

        [LabelText("@Consts.SkillTypeValues[mSkillType]"), ShowIf("@this.m_EditEnabled == true"), Indent()]
        [OnValueChanged("OnSkillTypeChanged")]
#endif
        public int skillType = 2;

#if ODIN_INSPECTOR
        [LabelText("需要指定目标"), ShowIf("@this.m_EditEnabled == true"), Indent()]
#endif
        public bool needSetTarget;

#if ODIN_INSPECTOR
        [LabelText("目标类型"), ShowIf("@this.m_EditEnabled == true && this.needSetTarget == true"), Indent()]
#endif
        public int targetType;

#if ODIN_INSPECTOR
        [LabelText("技能效果"), ShowIf("@this.m_EditEnabled == true"), Indent()]
#endif
        public int[] effects;

        public override void AutoFill() { }

        public void AfterInitFromJSON()
        {
            OnSkillTypeChanged();
        }

    }

}