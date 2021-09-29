using System;

namespace ShipDock.FSM
{
    public class AutomataMachine : StateMachine
    {
        private IAutomateState mFAState;

        public AutomataMachine(int name, Action<IStateMachine> fsmRegister = null) : base(name, fsmRegister) { }

        public override void Dispose()
        {
            base.Dispose();

            mFAState = default;
        }

        public override void Run(IStateParam param = null, int initState = int.MaxValue)
        {
            IsRun = true;

            FSMFrameUpdater?.Invoke(this, true);

            if (initState != int.MaxValue)
            {
                base.ChangeState(initState, param);
            }
            else if (DefaultState != int.MaxValue)
            {
                ChangeToDefaultState(param);
            }
            else { }
        }

        public sealed override void ChangeState(int name, IStateParam param = default)
        {
            throw new Exception("AutomateMachine can not be call ChangeState(int, IStateParam) method.");
        }

        public sealed override void ChangeStateByIndex(int index, IStateParam param = default)
        {
            throw new Exception("AutomateMachine can not be call ChangeStateByIndex(int, IStateParam) method.");
        }

        public sealed override void ChangeToNextState(IStateParam param = default)
        {
            throw new Exception("AutomateMachine can not be call ChangeToNextState(IStateParam) method.");
        }

        public override void ChangeToPreviousState(IStateParam param = default)
        {
            if (Previous != default)
            {
                if (Previous is IAutomateState state)
                {
                    ChangeAutomateState(state);
                }
                else { }
            }
            else { }
        }

        protected virtual void ChangeAutomateState(IAutomateState autoState)
        {
            if (autoState != default)
            {
                base.ChangeState(autoState.StateName);
            }
            else { }
        }

        protected override void AfterStateChanged()
        {
            mFAState = Current as IAutomateState;

            base.AfterStateChanged();
        }

        public override void UpdateState(int dTime)
        {
            if (!IsStateChanging)
            {
                bool flag = mFAState != default ? mFAState.StateExecuting() : false;
                if (flag)
                {
                    base.ChangeState(mFAState.NextState);
                }
                else { }
            }
            else { }

            base.UpdateState(dTime);
        }
    }

    public interface IAutomateCondition
    {
        bool CommitCondition();
    }

    public interface IAutomateState : IState
    {
        IAutomateCondition[] TransitionConditions { get; }
        bool StateExecuting();
        void UpdateStateParam();

    }

    public abstract class FAutomateState : FState, IAutomateState
    {
        public abstract IAutomateCondition[] TransitionConditions { get; }

        public FAutomateState(int name) : base(name) { }

        public abstract void UpdateStateParam();

        public virtual bool StateExecuting()
        {
            return false;
        }

    }
}
