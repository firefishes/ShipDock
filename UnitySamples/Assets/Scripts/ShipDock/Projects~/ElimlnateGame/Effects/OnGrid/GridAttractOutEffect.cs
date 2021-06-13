using DG.Tweening;
using UnityEngine;

namespace Elimlnate
{
    public class GridAttractOutEffect : GridEffect
    {

        protected override IEffectInfo<ElimlnateGrid, GridEffectParam> Create(ref ElimlnateGrid target)
        {
            ElimlnateTweenEffect tw = new ElimlnateTweenEffect
            {
                EffectMethod = OnEffect,
            };
            return tw;
        }

        public static Tween AttrackOutTween(ref Transform tf, ElimlnateEffectParam effectParam, out Vector3 center, float multiplying = 1.5f, float during = 0.2f)
        {
            center = effectParam.EndPosition;
            Vector2 direction = center - tf.position;
            direction.Normalize();

            Vector3 end = tf.position + new Vector3(-direction.x * multiplying, -direction.y * multiplying, 0f);
            Tween tw = tf.DOMove(end, during);
            return tw;
        }

        private void OnEffect(ElimlnateGrid target, TweenEffectBase<ElimlnateEffectParam> tw, GridEffectParam param)
        {
            //target.StopEffect(GameEffects.EffectChoosen);//, "Glow");

            Transform tf = target.GridTrans;
            ElimlnateEffectParam effectParam = param as ElimlnateEffectParam;
            //Vector3 end = GetdAttrackOutTween(ref tf, effectParam, out Vector3 center);
            Tween attracktOut = AttrackOutTween(ref tf, effectParam, out Vector3 center)
                .SetEase(Ease.OutSine);

            Sequence queue = DOTween.Sequence();
            queue.AppendInterval(0.2f * effectParam.Index);
            queue.Append(attracktOut);
            queue.Append(
                tf.DOMove(center, 0.4f)
                .SetEase(Ease.InSine))
            .OnComplete(() =>
            {
                target.WillDestroy();
                UpperStrata.AfterGridDestroy?.Invoke();
                queue.Kill();
            });
        }

        public ElimlnateGrid Target { get; private set; }
    }
}
