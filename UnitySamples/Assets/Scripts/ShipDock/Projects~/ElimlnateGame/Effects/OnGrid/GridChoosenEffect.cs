
using DG.Tweening;
using UnityEngine;

namespace Elimlnate
{
    public class GridChoosenEffect : GridEffect
    {
        private Vector3 mScale_1 = new Vector3(2f, 2f, 1f);
        private Vector3 mScale_2 = new Vector3(1.2f, 1.2f, 1f);

        protected override IEffectInfo<ElimlnateGrid, GridEffectParam> Create(ref ElimlnateGrid target)
        {
            TweenEffect tween = new TweenEffect
            {
                EffectMethod = OnEffect,
            };
            tween.Param.DuringTime = 0.1f;
            tween.Param.UpdateTime = 0f;
            return tween;
        }

        protected virtual void OnEffect(ElimlnateGrid target, TweenEffectBase<GridEffectParam> tw, GridEffectParam param)
        {
            tw.ResetTweenRefs();

            Sequence seq = DOTween.Sequence();
            seq.Append(target.GridTrans.DOScale(mScale_1, 0.1f));
            seq.Append(target.GridTrans.DOScale(mScale_2, 0.1f));

            tw.TweenRef = new Tween[] { seq };
        }

        protected override void AfterStop(ElimlnateGrid target)
        {
            base.AfterStop(target);

            target.GridTrans.localScale = Vector3.one;
        }
    }
}
