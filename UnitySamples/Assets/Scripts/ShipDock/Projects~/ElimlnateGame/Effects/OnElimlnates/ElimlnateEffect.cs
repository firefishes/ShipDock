using ShipDock.Notices;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Elimlnate
{
    /// <summary>
    /// 消除格子特效的抽象类
    /// </summary>
    public class ElimlnateEffect
    {
        public int GridCount { get; protected set; }
        public string CurGridEffectName { get; protected set; }
        public Action OnInit { get; set; }
        public Action OnCompleted { get; set; }
        public Action<ElimlnateEffect> FillParamBeforeStart { get; set; }
        public UnityAction AfterGridDestroy { get; set; }
        public List<string> FollowUpEffect { get; protected set; }

        public bool HasFollowUpEffect
        {
            get
            {
                return FollowUpEffect.Count > 0 && mFollowUpEffectIndex < FollowUpEffect.Count;
            }
        }

        protected string mDefaultGridEffect;
        protected string[] mRelateEffectName;
        protected string mRelateEffectNameTemp;

        private int mFollowUpEffectIndex;
        private List<ElimlnateGrid> mGrids;

        protected int EffectCount { get; set; }

        public ElimlnateEffect(params string[] relateGridEffectName)
        {
            mFollowUpEffectIndex = 0;
            mRelateEffectName = relateGridEffectName;
            FollowUpEffect = GetFollowUpEffectNames();
        }

        public void Clean()
        {
            AfterGridDestroy = default;
        }

        protected virtual List<string> GetFollowUpEffectNames()
        {
            return new List<string>();
        }

        public virtual void Init()
        {
            OnInit?.Invoke();
            AfterGridDestroy += EffectCompleted;

            string name;
            int max = mRelateEffectName.Length;
            for (int i = 0; i < max; i++)
            {
                name = mRelateEffectName[i];
                if (i == 0)
                {
                    mDefaultGridEffect = name;
                    CurGridEffectName = mDefaultGridEffect;
                }
                if (!string.IsNullOrEmpty(name))
                {
                    ElimlnateCore.Instance.GridEffects.GetEffect(name).UpperStrata = this;
                }
            }
        }

        public virtual void Start(ref List<ElimlnateGrid> grids, string triggerGridEffect = default)
        {
            if (string.IsNullOrEmpty(triggerGridEffect))
            {
                mFollowUpEffectIndex = 0;
                CurGridEffectName = mDefaultGridEffect;
                mGrids = grids;
                GridCount = grids.Count;
                FillParamBeforeStart?.Invoke(this);
            }
            else
            {
                CurGridEffectName = triggerGridEffect;
            }

            EffectCount = GridCount;

            ElimlnateGrid grid;
            ElimlnateEffectParam param;
            for (int i = 0; i < EffectCount; i++)
            {
                grid = grids[i];
                param = grid.GetEffectParam<ElimlnateEffectParam>(CurGridEffectName);
                if (param != default)
                {
                    param.Index = i;
                    grid.StartEffect(CurGridEffectName);
                }
            }
        }

        public void CheckFollowEffect()
        {
            EffectCompleted();
        }

        protected virtual void EffectCompleted()
        {
            EffectCount--;
            if (EffectCount <= 0)
            {
                if (mFollowUpEffectIndex < FollowUpEffect.Count)
                {
                    mRelateEffectNameTemp = FollowUpEffect[mFollowUpEffectIndex];
                    mFollowUpEffectIndex++;
                    UpdaterNotice.SceneCallLater(PlayNext);
                }
                else
                {
                    OnCompleted?.Invoke();
                    mGrids = default;
                }
            }
        }

        private void PlayNext(int time)
        {
            Start(ref mGrids, mRelateEffectNameTemp);
        }

        public ElimlnateGrid GetGrid(int index)
        {
            return mGrids[index];
        }
    }
}
