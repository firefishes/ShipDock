using DG.Tweening;
using UnityEngine;

namespace Elimlnate
{
    public class GridCreateEffectItem
    {
        private TweenEffectBase<GridEffectParam> mTw;

        public ElimlnateGrid Target { get; set; }

        private Vector3 EndPosition { get; set; } = Vector3.zero;

        private TweenCallback CompleteCallback { get; set; }

        public void AddCompleteCallback(TweenCallback callback)
        {
            CompleteCallback += callback;
        }

        public void Play(bool applyMoveToEndPos, ref ElimlnateGrid target, ref TweenEffectBase<GridEffectParam> tw, TweenCallback completed)
        {
            Target = target;
            if (completed != default)
            {
                AddCompleteCallback(completed);//外部的完成回调
            }
            else { }
            
            mTw = tw;
            GridEffectParam param = mTw.Param;
            float duringTime = param.DuringTime;
            AnimationCurve curve = param.Curve;
            EndPosition = param.EndPosition;

            Tween punch = Target.GridTrans.DOPunchScale(new Vector3(0.15f, -0.1f, 0f), 1f, 7, 1)
                .SetEase(Ease.InCubic);

            if (applyMoveToEndPos)
            {
                Sequence seq = DOTween.Sequence();
                Tween move = DOTween.To(GetValue, SetValue, EndPosition, duringTime)
                    .SetEase(curve);
                seq.Append(move);
                seq.Append(punch);
                seq.OnKill(TweenFinished);
                mTw.TweenRef = new Tween[] { punch, seq };
            }
            else
            {
                punch.OnKill(TweenFinished);
                mTw.TweenRef = new Tween[] { punch };
            }
        }

        private void TweenFinished()
        {
            mTw.TweenRef = default;
            mTw = default;

            CompleteCallback?.Invoke();
            CompleteCallback = default;

            Target.GridTrans.localScale = Vector3.one;
            Target = default;

        }

        private void SetValue(Vector3 v)
        {
            if (EndPosition == v)
            {
                EndPosition = Vector3.zero;
                CompleteCallback?.Invoke();
                Target.GridTrans.localScale = Vector3.one;
                return;
            }
            Target.GridTrans.position = v;
        }

        private Vector3 GetValue()
        {
            return Target.GridTrans.position;
        }
    }

    public class GridCreateEffect : GridEffect
    {
        public bool ApplyEndPosition { get; set; } = true;

        protected override IEffectInfo<ElimlnateGrid, GridEffectParam> Create(ref ElimlnateGrid target)
        {
            TweenEffect tween = new TweenEffect
            {
                EffectMethod = OnEffect,
            };
            GridCreater creater = ElimlnateCore.Instance.GridCreater;
            tween.Param.DuringTime = creater.EnterEffectDuringTime;
            tween.Param.Curve = creater.EnterEffectCurve;
            return tween;
        }

        protected virtual void OnEffect(ElimlnateGrid target, TweenEffectBase<GridEffectParam> tw, GridEffectParam param)
        {
            EffectCount++;
            GridCreateEffectItem item = new GridCreateEffectItem();
            TweenCallback callback = GetCompolete(target);
            item.Play(ApplyEndPosition, ref target, ref tw, callback);
        }

        protected virtual TweenCallback GetCompolete(ElimlnateGrid param)
        {
            return OnEffectCountRevert;
        }

        private void OnEffectCountRevert()
        {
            EffectCount--;
            if (EffectCount <= 0)
            {
                EffectCount = 0;
            }
        }
    }
}
