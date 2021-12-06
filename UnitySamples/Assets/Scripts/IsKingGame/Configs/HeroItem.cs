using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace IsKing
{
    [Serializable]
    public class HeroItem : GameItem
    {
#if ODIN_INSPECTOR
        [LabelText("等级"), ShowIf("@this.m_EditEnabled == true"), ProgressBar(1, 100), Indent()]
#endif
        public int level = 1;

#if ODIN_INSPECTOR
        [LabelText("体力"), ShowIf("@this.m_EditEnabled == true"), ProgressBar(1f, 300f), Indent()]
#endif
        public double hp;

#if ODIN_INSPECTOR
        [LabelText("武力"), ShowIf("@this.m_EditEnabled == true"), ProgressBar(0f, 300f), Indent()]
#endif
        public double atk;

#if ODIN_INSPECTOR
        [LabelText("防御"), ShowIf("@this.m_EditEnabled == true"), ProgressBar(0f, 300f), Indent()]
#endif
        public double def;

#if ODIN_INSPECTOR
        [LabelText("智力"), ShowIf("@this.m_EditEnabled == true"), ProgressBar(1f, 300f), Indent()]
#endif
        public double intellect;

#if ODIN_INSPECTOR
        [LabelText("兵法"), ShowIf("@this.m_EditEnabled == true"), ProgressBar(1f, 300f), Indent()]
#endif
        public double aow;

#if ODIN_INSPECTOR
        [LabelText("兵力"), ShowIf("@this.m_EditEnabled == true"), ProgressBar(0, 3000), Indent()]
#endif
        public int troops;

#if ODIN_INSPECTOR
        [LabelText("主帅技"), ShowIf("@this.m_EditEnabled == true"), Indent()]
#endif
        public int skillCIC;

#if ODIN_INSPECTOR
        [LabelText("军师计"), ShowIf("@this.m_EditEnabled == true"), Indent()]
#endif
        public int skillCounsellor;

#if ODIN_INSPECTOR
        [LabelText("武将技"), ShowIf("@this.m_EditEnabled == true"), Indent()]
#endif
        public int[] skillGeneral;

        public override void AutoFill() { }
    }
}