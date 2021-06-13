using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Elimlnate
{
    public class GridEnterEffect : GridEffect
    {
        public float EndValueOffset { get; set; } = 2f;

        protected override IEffectInfo<ElimlnateGrid, GridEffectParam> Create(ref ElimlnateGrid target)
        {
            TweenEffect tween = new TweenEffect
            {
                EffectMethod = OnEffect
            };
            tween.Param.DuringTime = 0.5f;
            return tween;
        }

        private void OnEffect(ElimlnateGrid target, TweenEffectBase<GridEffectParam> tw, GridEffectParam param)
        {
            EffectCount++;

            tw.ResetTweenRefs();

            GridCreater gridsCreate = ElimlnateCore.Instance.GridCreater;
            AnimationCurve curve = gridsCreate.EnterEffectCurve;

            float endValue = target.GridTrans.position.y + EndValueOffset;

            //Tween punch = target.GridTrans.DOPunchScale(new Vector3(-0.3f, 0f, 0f), 1f, 8, 0);
            TweenerCore<Vector3, Vector3, VectorOptions> move = target.GridTrans.DOMoveY(endValue, param.DuringTime)
                .From()
                .SetEase(curve)
                .OnKill(OnEffectCompleted);

            tw.TweenRef = new Tween[] { /*punch, */move };
        }

        private void OnEffectCompleted()
        {
            EffectCount--;
            if (EffectCount <= 0)
            {
                EffectCount = 0;
            }
        }

        public void Stop() { }
    }
}
