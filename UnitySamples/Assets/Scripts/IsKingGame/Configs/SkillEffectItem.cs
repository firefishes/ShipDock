#define ODIN

using System;
using ShipDock.Scriptables;
#if ODIN_INSPECTOR && ODIN
using Sirenix.OdinInspector;
#endif

namespace IsKing
{
    [Serializable]
    public class SkillEffectItem : ScriptableItem
    {

#if ODIN_INSPECTOR
        private int mEffectField;
        private void OnEffectFieldChanged()
        {
            mEffectField = effectField;
        }

        [LabelText("@Consts.FieldNames[mEffectField]"), ShowIf("@this.m_EditEnabled == true"), Indent()]
        [OnValueChanged("OnEffectFieldChanged")]
#endif
        public int effectField;

#if ODIN_INSPECTOR
        private int mEffectType;
        private void OnEffectTypeChanged()
        {
            mEffectType = effectType;
        }

        [LabelText("@Consts.SkillEffectTypeValues[mEffectType]"), ShowIf("@this.m_EditEnabled == true"), Indent()]
        [OnValueChanged("OnEffectTypeChanged")]
#endif
        public int effectType;

#if ODIN_INSPECTOR
        [LabelText("Int"), ShowIf("@this.m_EditEnabled == true"), Indent()]
#endif
        public int effectCount;

#if ODIN_INSPECTOR
        [LabelText("Float"), ShowIf("@this.m_EditEnabled == true"), Indent()]
#endif
        public double effectValue;

        public override void AutoFill() { }

        public void AfterInitFromJSON()
        {
            OnEffectFieldChanged();
            OnEffectTypeChanged();
        }
    }
}