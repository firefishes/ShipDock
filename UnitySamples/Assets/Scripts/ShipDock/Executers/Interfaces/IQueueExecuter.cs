using System;

namespace ShipDock
{
    public delegate void QueueNextUnit(IQueueExecuter param);
    public delegate void QueueUnitExecuted(IQueueExecuter param);
    public delegate void QueueUnitCompleted(IQueueExecuter param);

    /// <summary>队列执行器接口</summary>
    public interface IQueueExecuter : IReclaim
    {
        QueueNextUnit OnNextUnit { get; set; }
        QueueUnitExecuted OnUnitExecuted { get; set; }
        QueueUnitCompleted OnUnitCompleted { get; set; }
        Action ActionUnit { get; set; }
        bool IgnoreInQueue { get; set; }

        int QueueSize { get; }
        void QueueNext();
        void Commit();
    }
}

#region Sample
/*
 * Copy Code
 * 
    #region 执行队列单元的实现代码
    private bool mIsReclaimed;

    public int QueueSize { get { return 1; } }
    public QueueNextUnit OnNextUnit { get; set; }
    public QueueUnitCompleted OnUnitCompleted { get; set; }
    public QueueUnitExecuted OnUnitExecuted { get; set; }
    public Action ActionUnit { get; set; }
    public bool ShouldNextAfterCommit { get; }
    public bool IgnoreInQueue { get; set; }

    public virtual void Reclaim()
    {
        if (mIsReclaimed)
        {
            return;
        }
        else { }

        mIsReclaimed = true;

        OnNextUnit = default;
        OnUnitExecuted = default;
        OnUnitCompleted = default;
    }

    public virtual void Commit() { }

    public void QueueNext()
    {
        OnNextUnit?.Invoke(this);//让所在的队列执行器执行下一个队列单元
    }
    #endregion
*/
#endregion