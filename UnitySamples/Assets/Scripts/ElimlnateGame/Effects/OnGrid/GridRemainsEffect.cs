using DG.Tweening;
using ShipDock.Tools;
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
                //Tween move = DOTween.To(GetValue, SetValue, EndPosition, duringTime)
                //    .SetEase(curve);
                //seq.Append(move);
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

    public class GridRemainsEffect : GridEffect
    {
        private KeyValueList<int, float> mCurveTime;
        private KeyValueList<int, Vector3> mTargetPos;

        private ElimlnateCore GamePlayCore { get; set; }
        private BoardGrids BoardGrids { get; set; }
        private GridCreater GridCreater { get; set; }

        public bool ApplyEndPosition { get; set; } = true;

        public GridRemainsEffect()
        {
            mCurveTime = new KeyValueList<int, float>();
            mTargetPos = new KeyValueList<int, Vector3>();
        }

        protected override IEffectInfo<ElimlnateGrid, GridEffectParam> Create(ref ElimlnateGrid target)
        {
            TweenEffect tween = new TweenEffect
            {
                EffectMethod = OnEffect,
                ApplyUpdate = true,
            };
            tween.Param.DuringTime = 0.5f;
            GridCreater creater = ElimlnateCore.Instance.GridCreater;
            tween.Param.DuringTime = creater.EnterEffectDuringTime;
            tween.Param.Curve = creater.EnterEffectCurve;
            return tween;
        }

        private int mNextLeftOrRight = 0;

        private bool SetGridTargetPos(ref ElimlnateGrid target, ref GridEffectParam param)
        {
            bool result = false;
            int gridID = target.GridID;

            Vector2Int pos = target.GridPos;
            int ynext = pos.y - 1;
            Vector2Int next = new Vector2Int(pos.x, ynext);
            int nextIndex = BoardGrids.GetGridIndex(next.x, next.y);
            bool nextValidable = BoardGrids.IsValidGridIndex(nextIndex);
            ElimlnateGrid nextGrid = nextValidable ? BoardGrids.GetBoardGrid(next) : default;

            Vector2Int nextLeft = new Vector2Int(pos.x - 1, ynext);
            int nextLeftIndex = BoardGrids.GetGridIndex(next.x, next.y);
            bool nextLeftValidable = BoardGrids.IsValidGridIndex(nextLeftIndex);
            ElimlnateGrid nextLeftGrid = nextValidable ? BoardGrids.GetBoardGrid(nextLeft) : default;

            Vector2Int nextRight = new Vector2Int(pos.x + 1, ynext);
            int nextRightIndex = BoardGrids.GetGridIndex(next.x, next.y);
            bool nextRightValidable = BoardGrids.IsValidGridIndex(nextRightIndex);
            ElimlnateGrid nextRightGrid = nextValidable ? BoardGrids.GetBoardGrid(nextRight) : default;

            if (nextValidable || nextLeftValidable || nextRightValidable)
            {
                if (nextValidable && nextGrid == default)
                {
                    result = true;
                    mTargetPos[gridID] = GridCreater.GetGridPosWithRowCol(next);

                    BoardGrids.SetGridMapper(target.GridPos, target, true);
                    target.SetGridPos(next);

                    Debug.LogError("next");
                    param.Speed = Vector3.down;
                }
                else
                {
                    if (nextLeftValidable || nextRightValidable)
                    {
                        if((mNextLeftOrRight & 1) == 1)
                        {
                            if (nextLeftGrid == default)
                            {
                                result = true;
                                mTargetPos[gridID] = GridCreater.GetGridPosWithRowCol(nextLeft);

                                BoardGrids.SetGridMapper(target.GridPos, target, true);
                                target.SetGridPos(nextLeft);

                                Debug.LogError("next left");
                                param.Speed = new Vector3(-1f, -1f).normalized;
                            }
                            else
                            {
                                if (nextRightGrid == default)
                                {
                                    result = true;
                                    mTargetPos[gridID] = GridCreater.GetGridPosWithRowCol(nextRight);

                                    BoardGrids.SetGridMapper(target.GridPos, target, true);
                                    target.SetGridPos(nextRight);

                                    Debug.LogError("next right");
                                    param.Speed = new Vector3(1f, -1f).normalized;
                                }
                                else { }
                            }
                        }
                        else
                        {
                            if (nextRightGrid == default)
                            {
                                result = true;
                                mTargetPos[gridID] = GridCreater.GetGridPosWithRowCol(nextRight);

                                BoardGrids.SetGridMapper(target.GridPos, target, true);
                                target.SetGridPos(nextRight);

                                Debug.LogError("next right");
                                param.Speed = new Vector3(1f, -1f).normalized;
                            }
                            else
                            {
                                if (nextLeftGrid == default)
                                {
                                    result = true;
                                    mTargetPos[gridID] = GridCreater.GetGridPosWithRowCol(nextLeft);

                                    BoardGrids.SetGridMapper(target.GridPos, target, true);
                                    target.SetGridPos(nextLeft);

                                    Debug.LogError("next left");
                                    param.Speed = new Vector3(-1f, -1f).normalized;
                                }
                                else { }
                            }
                        }
                    }
                    else { }
                }
            }
            else { }

            if (result)
            {
                param.Speed = Vector3.zero;
            }
            else { }

            return result;
        }

        protected virtual void OnEffect(ElimlnateGrid target, TweenEffectBase<GridEffectParam> tw, GridEffectParam param)
        {
            if (GamePlayCore == default)
            {
                GamePlayCore = ElimlnateCore.Instance;
                BoardGrids = GamePlayCore.BoardGrids;
                GridCreater = GamePlayCore.GridCreater;
            }
            else { }

            int gridID = target.GridID;
            if (param.IsInited)
            {
                if (mCurveTime.ContainsKey(gridID))
                {
                    AnimationCurve curve = GridCreater.EnterEffectCurve;
                    float time = mCurveTime[gridID];
                    Vector3 cur = target.GridTrans.position;
                    Vector3 end = mTargetPos[gridID];
                    //float curveValue = curve.Evaluate(time / param.DuringTime);
                    //float lerp = time / param.DuringTime;
                    bool isFinished = Vector3.Distance(cur, end) <= 0.001f;//lerp >= 1f;
                    if (isFinished)
                    {
                        bool flag = SetGridTargetPos(ref target, ref param);
                        if (flag)
                        {
                            mTargetPos.Remove(gridID);
                            mCurveTime.Remove(gridID);

                            BoardGrids.SetGridMapper(target.GridPos, target, false);
                        }
                        else
                        {
                            isFinished = false;
                            mCurveTime[gridID] = 0f;
                        }
                    }
                    else
                    {
                        time += Time.deltaTime;
                        mCurveTime[gridID] = time;
                    }

                    target.GridTrans.position += param.Speed * Time.deltaTime * 10f;//Vector3.Lerp(cur, end, lerp);

                    if (isFinished)
                    {
                        param.IsInited = false;
                        OnEffectCountRevert();
                    }
                    else { }
                }
                else { }

            }
            else
            {
                bool flag = SetGridTargetPos(ref target, ref param);
                if (flag)
                {
                    param.IsInited = true;
                    EffectCount++;
                    mCurveTime[gridID] = 0f;
                }
                else { }
                //GridCreateEffectItem item = new GridCreateEffectItem();
                //TweenCallback callback = GetCompolete(target);
                //item.Play(ApplyEndPosition, ref target, ref tw, callback);
            }
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
            else { }
        }
    }
}
