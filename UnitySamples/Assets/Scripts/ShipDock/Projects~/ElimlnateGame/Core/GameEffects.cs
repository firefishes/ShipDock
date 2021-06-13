#define _LOG_GAME_EFFECTS

using ShipDock.Tools;
using System;

namespace Elimlnate
{
    public class GameEffects
    {
        public const int EFFECT_CHECK_STATE_IDLE = 0;
        public const int EFFECT_CHECK_STATE_SUPP = 1;
        public const int EFFECT_CHECK_STATE_TIDY = 2;

        /// <summary>消除格初始化时（游戏开始时）的特效名</summary>
        public static string EffectInited { get; set; } = "Inited";
        /// <summary>消除格被创建时的特效名</summary>
        public static string EffectCreate { get; set; } = "Create";
        /// <summary>消除格入场时（被创建之后，如自由下落等）的特效名</summary>
        public static string EffectEnter { get; set; } = "Enter";
        /// <summary>消除格被选中时的特效名</summary>
        public static string EffectChoosen { get; set; } = "Choosen";
        /// <summary>消除格被消除时的特效名</summary>
        public static string EffectElimlnate { get; set; } = "Elimlnate";
        /// <summary>消除格被消除后的后续特效名</summary>
        public static string EffectAfterElimlnate { get; set; } = "AfterElimlnate";
        /// <summary>消除格技能预览时的特效名</summary>
        public static string EffectPreviewGridSkill { get; set; } = "PreviewGridSkill";

        /// <summary>当消除格被创建或移动时的更新函数</summary>
        public Action OnGridCreateAndEnters { get; set; }
        /// <summary>当消除格特效稳定时的更新函数</summary>
        public Action OnGridEffectsIdleState { get; set; }

        public int EffectCheckState { get; private set; } = EFFECT_CHECK_STATE_IDLE;

        private GridEffect mEnterEffect;
        private GridEffect mCreateEffect;

        private KeyValueList<string, GridEffect> Effects { get; set; } = new KeyValueList<string, GridEffect>
        {
            [EffectEnter] = new GridEnterEffect(),
            [EffectCreate] = new GridCreateEffect(),
            [EffectChoosen] = new GridChoosenEffect(),
        };

        public void Clear()
        {
            int max = Effects.Size;
            for (int i = 0; i < max; i++)
            {
                Effects.Values[i].Clear();
            }

            Effects.Clear();
            Effects = default;

            OnGridCreateAndEnters = default;
            OnGridEffectsIdleState = default;
            mEnterEffect = default;
            mCreateEffect = default;
        }

        public void SetEffectState(int state)
        {
#if LOG_GAME_EFFECTS
            "log:Effects state set to {0}".Log(state.ToString());
#endif
            EffectCheckState = state;
        }

        public void SetEffect(string name, GridEffect target)
        {
            Effects[name] = target;
        }

        public GridEffect GetEffect(string name)
        {
            return Effects[name];
        }

        public KeyValueList<string, GridEffect> GetEffectsMapper()
        {
            return Effects;
        }

        private bool CheckCreateOrEnterEffectExistable(bool conditionWithFinished, int remainsEnter = 0, int remainsCreate = 0)
        {
            if (mEnterEffect == default)
            {
                mEnterEffect = Effects[EffectEnter];
            }
            else { }

            if (mCreateEffect == default)
            {
                mCreateEffect = Effects[EffectCreate];
            }
            else { }

            int enterCount = mEnterEffect.EffectCount;
            int createCount = mCreateEffect.EffectCount;
            bool result = conditionWithFinished ?
                (enterCount == remainsEnter) && (createCount == remainsCreate) :
                (enterCount > remainsEnter) || (createCount > remainsCreate);
#if LOG_GAME_EFFECTS
            if (result)
            {
                "log:Grid enter effect count {0}".Log(enterCount.ToString());
                "log:Grid create effect count {0}".Log(createCount.ToString());
                "log:Grid effect {0}".Log(isFinished ? "all played" : "still play");
            }
            else { }
#endif
            return result;
        }

        public void UpdateEffects()
        {
            switch (EffectCheckState)
            {
                case EFFECT_CHECK_STATE_IDLE:
                    if (CheckCreateOrEnterEffectExistable(false))
                    {
                        EffectCheckState = EFFECT_CHECK_STATE_SUPP;//正在补充
                    }
                    else
                    {
                        OnGridEffectsIdleState?.Invoke();
                    }
                    break;
                case EFFECT_CHECK_STATE_SUPP:
                    OnGridCreateAndEnters?.Invoke();
                    break;
            }
        }

    }
}
