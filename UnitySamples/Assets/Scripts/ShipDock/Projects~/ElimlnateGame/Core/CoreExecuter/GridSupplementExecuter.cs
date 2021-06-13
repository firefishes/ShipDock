using ShipDock.Commons;
using ShipDock.Interfaces;
using ShipDock.Notices;
using System;

namespace Elimlnate
{
    public class GridSupplementExecuter : IQueueExecuter
    {
        private MethodUpdater Updater { get; set; }
        private GridEffect EnterEffect { get; set; }
        private GridEffect CraeterEffect { get; set; }

        public GridSupplementExecuter()
        {
            GameEffects effects = ElimlnateCore.Instance.GridEffects;
            EnterEffect = effects.GetEffect(GameEffects.EffectEnter);
            CraeterEffect = effects.GetEffect(GameEffects.EffectCreate);

            Updater = new MethodUpdater()
            {
                Update = OnCheckQueueNext,
            };
        }

        private void OnCheckQueueNext(int time)
        {
            if (EnterEffect.EffectCount <= 0 && CraeterEffect.EffectCount <= 0)
            {
                UpdaterNotice.RemoveSceneUpdater(Updater);

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
        public bool ImmediatelyCommitNext { get; set; }
        public bool IgnoreInQueue { get; set; }

        public virtual void Dispose()
        {
            if (mIsDispose)
            {
                return;
            }
            mIsDispose = true;

            UpdaterNotice.RemoveSceneUpdater(Updater);

            Updater?.Dispose();
            EnterEffect = default;

            OnNextUnit = default;
            OnUnitExecuted = default;
            OnUnitCompleted = default;
        }

        public virtual void Commit()
        {
            UpdaterNotice.AddSceneUpdater(Updater);
        }

        public void QueueNext()
        {
            OnNextUnit?.Invoke(this);//让所在的队列执行器执行下一个队列单元
        }
        #endregion
    }
}
