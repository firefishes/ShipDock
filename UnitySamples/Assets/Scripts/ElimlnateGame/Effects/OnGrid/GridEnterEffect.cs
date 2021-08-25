using ShipDock.Tools;
using UnityEngine;

namespace Elimlnate
{
    /// <summary>
    /// 
    /// 消除格被创建时的下落特效
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class GridEnterEffect : GridEffect
    {
        private KeyValueList<int, float> mEndDistance;
        private KeyValueList<int, float> mCurveTime;
        private KeyValueList<int, float> mStartPos;

        public float EndValueOffset { get; set; } = 2f;

        public GridEnterEffect()
        {
            mEndDistance = new KeyValueList<int, float>();
            mStartPos = new KeyValueList<int, float>();
            mCurveTime = new KeyValueList<int, float>();
        }

        public override void Clear()
        {
            base.Clear();

            mEndDistance?.Clear();
        }

        protected override IEffectInfo<ElimlnateGrid, GridEffectParam> Create(ref ElimlnateGrid target)
        {
            TweenEffect tween = new TweenEffect
            {
                EffectMethod = OnEffect,
                ApplyUpdate = true,
            };
            tween.Param.DuringTime = 0.5f;
            return tween;
        }

        private void OnEffect(ElimlnateGrid target, TweenEffectBase<GridEffectParam> tw, GridEffectParam param)
        {
            if (param.IsInited)
            {
                GridCreater gridsCreate = ElimlnateCore.Instance.GridCreater;
                AnimationCurve curve = gridsCreate.EnterEffectCurve;

                int gridID = target.GridID;
                float start = mStartPos[gridID];
                float end = mEndDistance[gridID];
                float time = mCurveTime[gridID];
                float curveValue = curve.Evaluate(time / param.DuringTime);

                Vector3 pos = target.GridTrans.position;
                bool isFinished = curveValue >= 1f;
                if (isFinished)
                {
                    mStartPos.Remove(gridID);
                    mEndDistance.Remove(gridID);
                    mCurveTime.Remove(gridID);

                    pos.Set(pos.x, end, pos.z);
                }
                else
                {
                    pos.Set(pos.x, end * curveValue, pos.z);

                    time += Time.deltaTime;
                    mCurveTime[gridID] = time;
                }

                target.GridTrans.position = pos;

                if (isFinished)
                {
                    OnEffectCompleted();
                }
                else { }
            }
            else
            {
                EffectCount++;

                tw.ResetTweenRefs();

                float start = target.GridTrans.position.y;
                float endValue = start + EndValueOffset;
                mStartPos[target.GridID] = start;
                mEndDistance[target.GridID] = endValue;
                mCurveTime[target.GridID] = 0f;
            }
        }

        private void OnEffectCompleted()
        {
            EffectCount--;
            if (EffectCount <= 0)
            {
                EffectCount = 0;
            }
            else { }
        }

        public void Stop() { }
    }
}
