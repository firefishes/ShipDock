using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using ShipDock.Interfaces;
using ShipDock.Tools;
using ShipDock.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public class AwardsMoveToUIPos : IQueueExecuter
    {
        private int mPoolName;
        private int mFXCount;
        private Vector3 mStartPos;
        private List<GameObject> mRes;

        public int UIPos { get; set; }
        public float ToLocalScale { get; set; } = 1f;
        public float LocalScaleDuring { get; set; } = 1f;
        public float During { get; set; } = 0.5f;
        public float EachRandomDelay { get; set; } = 0.5f;
        public Vector3Int ApplyRotate { get; set; } = Vector3Int.zero;
        public Vector2 RotateRandomSpeed { get; set; } = Vector2.zero;
        public Vector2 EndPos { get; set; } = new Vector2(-340f, 700f);
        public Rect MoveRange { get; set; } = new Rect(-300f, -100f, 150f, 240f);
        public TweenCallback OnUpdate { get; set; }
        public TweenCallback OnCompleted { get; set; }

        public AwardsMoveToUIPos(int count, Vector3 startPos, int poolName)
        {
            mPoolName = poolName;
            mStartPos = startPos;
            mRes = new List<GameObject>();
            Effects effects = ShipDockApp.Instance.Effects;
            Transform trans;
            GameObject result = default;
            UIManager ui = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
            RectTransform layer = ui.UIRoot.Popups;
            for (int i = 0; i < count; i++)
            {
                effects.CreateEffect(poolName, out result);
                if (result != default)
                {
                    trans = result.transform;
                    trans.SetParent(layer);
                    trans.localPosition = mStartPos;
                    trans.localScale = Vector3.one;
                    mRes.Add(result);
                }
                else { }
            }
        }

        #region 执行队列单元的实现代码
        private bool mIsDispose;

        public int QueueSize
        {
            get
            {
                return 1;
            }
        }

        public QueueNextUnit OnNextUnit { get; set; }
        public QueueUnitCompleted OnUnitCompleted { get; set; }
        public QueueUnitExecuted OnUnitExecuted { get; set; }
        public Action ActionUnit { get; set; }
        public bool IgnoreInQueue { get; set; }

        public virtual void Dispose()
        {
            if (mIsDispose)
            {
                return;
            }
            else { }

            mIsDispose = true;

            OnNextUnit = null;
            OnUnitExecuted = null;
            OnUnitCompleted = null;
        }

        private Vector3 GetRandomPos(Vector3 pos)
        {
            float x = Utils.UnityRangeRandom(MoveRange.xMin, MoveRange.xMax);
            float y = Utils.UnityRangeRandom(MoveRange.yMin, MoveRange.yMax);
            return new Vector3(pos.x + x, pos.y + y, pos.z);
        }

        private float GetRotateValue()
        {
            return Utils.UnityRangeRandom(RotateRandomSpeed.x, RotateRandomSpeed.y);
        }

        public virtual void Commit()
        {
            float rX, rY, rZ;
            Transform trans;
            mFXCount = mRes.Count;
            int max = mFXCount;
            TweenerCore<Vector3, Path, PathOptions> tween;
            for (int i = 0; i < max; i++)
            {
                GameObject item = mRes[i];
                item.SetActive(false);

                trans = item.transform;

                Vector3 pos = GetRandomPos(trans.localPosition);
                Vector3[] path = new Vector3[]
                {
                    pos,
                    new Vector3(EndPos.x, EndPos.y, pos.z),
                };

                TweenCallback itemCompleted = default;
                if (OnCompleted != default)
                {
                    itemCompleted += OnCompleted;
                }
                else { }

                itemCompleted += () =>
                {
                    item.SetActive(false);
                    mFXCount--;

                    if (mFXCount <= 0)
                    {
                        QueueNext();
                    }
                    else { }
                };

                trans.DOScale(Vector3.one * ToLocalScale, LocalScaleDuring);

                float delay = Utils.UnityRangeRandom(0f, EachRandomDelay);
                tween = trans.DOLocalPath(path, During, PathType.CatmullRom)
                    .SetEase(Ease.Linear)
                    .OnStart(() => { item.SetActive(true); })
                    .OnComplete(itemCompleted)
                    .SetDelay(delay);

                TweenCallback itemUpdate = default;
                if (OnUpdate != default)
                {
                    itemUpdate += OnUpdate;
                }
                else { }

                if (RotateRandomSpeed.y > 0f)
                {
                    rX = ApplyRotate.x > 0 ? GetRotateValue() : 0f;
                    rY = ApplyRotate.y > 0 ? GetRotateValue() : 0f;
                    rZ = ApplyRotate.z > 0 ? GetRotateValue() : 0f;

                    itemUpdate += () => { item.transform.Rotate(rX, rY, rZ); };
                }
                else { }

                if (itemUpdate != default)
                {
                    tween.OnUpdate(itemUpdate);
                }
                else { }
            }
        }

        public void QueueNext()
        {
            mRes?.Clear();
            mRes = default;

            OnUpdate = default;
            OnCompleted = default;

            OnNextUnit?.Invoke(this);//让所在的队列执行器执行下一个队列单元
        }
        #endregion
    }
}
