using ShipDock.Interfaces;
using System;

namespace Elimlnate
{
    /// <summary>
    /// 
    /// 消除格补充阶段执行器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class GridSupplementExecuter : IQueueExecuter
    {
        /// <summary>消除格的开场特效</summary>
        private GridEffect EnterEffect { get; set; }
        /// <summary>消除格的创建特效</summary>
        private GridEffect RemainsEffect { get; set; }

        private ElimlnateCore GamePlayCore
        {
            get
            {
                return ElimlnateCore.Instance;
            }
        }

        public GridSupplementExecuter()
        {
            GameEffects effects = ElimlnateCore.Instance.GridEffects;
            EnterEffect = effects.GetEffect(GameEffects.EffectEnter);
            RemainsEffect = effects.GetEffect(GameEffects.EffectRemains);
        }

        private void RemoveUpdate()
        {
            GamePlayCore.RemoveUpdate(OnCheckQueueNext);
        }

        private void OnCheckQueueNext()
        {
            if ((EnterEffect == default) || 
                (EnterEffect.EffectCount <= 0) && (RemainsEffect.EffectCount <= 0))
            {
                RemoveUpdate();
                GamePlayCore.DeactiveInput(0.1f);
                QueueNext();
            }
            else { }
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
            mIsDispose = true;

            RemoveUpdate();

            EnterEffect = default;
            OnNextUnit = default;
            OnUnitExecuted = default;
            OnUnitCompleted = default;
        }

        public virtual void Commit()
        {
            GamePlayCore.AddUpdate(OnCheckQueueNext);
        }

        public void QueueNext()
        {
            OnNextUnit?.Invoke(this);//让所在的队列执行器执行下一个队列单元
        }
        #endregion
    }
}
