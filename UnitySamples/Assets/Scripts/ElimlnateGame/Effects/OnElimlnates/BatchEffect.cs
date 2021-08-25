using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Elimlnate
{
    /// <summary>
    /// 
    /// 批量特效的抽象类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class BatchEffect
    {
        /// <summary>此批量特效作用范围内的消除格数量</summary>
        public int GridCount { get; protected set; }
        /// <summary>当前执行的消除格特效名</summary>
        public string CurGridEffectName { get; protected set; }
        /// <summary>初始化的回调函数</summary>
        public Action OnInit { get; set; }
        /// <summary>批量特效完成的回调函数</summary>
        public Action OnCompleted { get; set; }
        /// <summary>向所有作用的消除格填充特效参数的回调函数</summary>
        public Action<BatchEffect> FillParamBeforeStart { get; set; }
        /// <summary>单个消除格特效完成后的回调函数</summary>
        public UnityAction AfterGridEffectFinished { get; set; }
        /// <summary>后续需要启动的特效名列表</summary>
        public List<string> FollowUpEffect { get; protected set; }

        /// <summary>是否还有后续需要启动的特效</summary>
        public bool HasFollowUpEffect
        {
            get
            {
                return FollowUpEffect.Count > 0 && mWillStartEffectIndex < FollowUpEffect.Count;
            }
        }

        /// <summary>首次启动的消除格特效</summary>
        protected string mStartGridEffectName;
        /// <summary>被关联的消除格特效名，被关联的特效会将前置特效设置为此批量特效</summary>
        protected string[] mRelateEffectName;
        /// <summary>即将启动的特效名</summary>
        protected string mWillStartRelateEffect;

        /// <summary>即将启动的后续特效列表索引</summary>
        private int mWillStartEffectIndex;
        /// <summary>此批量特效作用范围内的消除格列表</summary>
        private List<ElimlnateGrid> mGrids;

        /// <summary>本批次启动的特效数量</summary>
        protected int EffectCount { get; set; }

        public BatchEffect(params string[] relateGridEffectName)
        {
            mWillStartEffectIndex = 0;
            mRelateEffectName = relateGridEffectName;
            FollowUpEffect = GetFollowUpEffectNames();
        }

        /// <summary>清除此批量特效</summary>
        public void Clean()
        {
            AfterGridEffectFinished = default;
        }

        /// <summary>获取后续特效名列表</summary>
        protected virtual List<string> GetFollowUpEffectNames()
        {
            return new List<string>();
        }

        /// <summary>初始化</summary>
        public virtual void Init()
        {
            OnInit?.Invoke();
            AfterGridEffectFinished += BatchEffectCompleted;

            GameEffects effects = ElimlnateCore.Instance.GridEffects;

            string name;
            int max = mRelateEffectName.Length;
            for (int i = 0; i < max; i++)
            {
                name = mRelateEffectName[i];
                if (i == 0)
                {
                    mStartGridEffectName = name;
                    CurGridEffectName = mStartGridEffectName;
                }
                else { }

                if (!string.IsNullOrEmpty(name))
                {
                    effects.GetEffect(name).UpperStrata = this;
                }
                else { }
            }
        }

        /// <summary>
        /// 批特效启动
        /// </summary>
        /// <param name="grids"></param>
        /// <param name="triggerGridEffect"></param>
        public virtual void Start(ref List<ElimlnateGrid> grids, string triggerGridEffect = default)
        {
            if (string.IsNullOrEmpty(triggerGridEffect))
            {
                mWillStartEffectIndex = 0;
                CurGridEffectName = mStartGridEffectName;
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
                else { }
            }
        }

        /// <summary>
        /// 用于外部调用检测是否存在后续特效，如存在则继续启动下一个后续特效
        /// </summary>
        public void CheckFollowEffect()
        {
            BatchEffectCompleted();
        }

        protected virtual void BatchEffectCompleted()
        {
            EffectCount--;
            if (EffectCount <= 0)
            {
                if (mWillStartEffectIndex < FollowUpEffect.Count)
                {
                    mWillStartRelateEffect = FollowUpEffect[mWillStartEffectIndex];
                    mWillStartEffectIndex++;
                    ElimlnateCore.Instance.AddUpdate(StartNext);//下一帧启动下一个后续特效
                }
                else
                {
                    OnCompleted?.Invoke();//批特效已完成
                    mGrids = default;
                }
            }
            else { }
        }

        /// <summary>
        /// 启动下一个后续特效
        /// </summary>
        private void StartNext()
        {
            ElimlnateCore.Instance.RemoveUpdate(StartNext);
            Start(ref mGrids, mWillStartRelateEffect);
        }

        /// <summary>
        /// 根据索引获取特效作用范围内的消除格
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ElimlnateGrid GetGrid(int index)
        {
            return mGrids[index];
        }
    }
}
