#define _DO_PUNCH_ROT
#define _DIRECT_FINISH
#define _APPLY_GRID_RANK

using DG.Tweening;
#if DO_PUNCH_ROT
using ShipDock.Tools;
#endif
using UnityEngine;

namespace Elimlnate
{
    public class GridBlastEffect : GridEffect
    {
        protected override IEffectInfo<ElimlnateGrid, GridEffectParam> Create(ref ElimlnateGrid target)
        {
            ElimlnateTweenEffect tw = new ElimlnateTweenEffect
            {
                EffectMethod = OnEffect,
            };
            return tw;
        }

        private void OnEffect(ElimlnateGrid target, TweenEffectBase<ElimlnateEffectParam> tw, ElimlnateEffectParam param)
        {
            Sequence queue = DOTween.Sequence();
#if APPLY_GRID_RANK
            float interval = 0.1f;
            queue.AppendInterval(interval * param.Index);
#endif
#if DO_PUNCH_ROT
            float z = Utils.RangeRandom(45f, 90f);
            Vector3 punch = new Vector3(0f, 0f, z);
            queue.Append(target.GridTrans.DOPunchRotation(punch, 0.4f, 15));//摇晃
            queue.AppendInterval(0.05f * param.Index);//摇晃后间隔时间
#endif
            queue.Append(target.GridTrans.DOScale(Vector3.one * 1.2f, 0.18f));//1.5f, 0.25f
            queue.Append(target.GridTrans.DOScale(Vector3.zero, 0.1f));//zero, 0.15f

            BeforeFinish(ref queue, ref target, ref param);
#if DIRECT_FINISH
            queue.AppendCallback(Finish);//直接结束
#else
            queue.Join(target.GridSprite.DOFade(0f, 0.1f))//褪为透明
                .OnComplete(Finish);
#endif
            void Finish()
            {
                if (UpperStrata.HasFollowUpEffect)
                {
                    UpperStrata.CheckFollowEffect();
                }
                else
                {
                    UpperStrata.AfterGridEffectFinished?.Invoke();
                    target.WillDestroy();
                    queue.Kill();
                }
            }
            OnPlaySound?.Invoke();
        }

        protected virtual void BeforeFinish(ref Sequence queue, ref ElimlnateGrid target, ref ElimlnateEffectParam param) { }
    }
}
