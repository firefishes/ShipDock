using ShipDock.Tools;
using System;
using Unity.Jobs;

namespace ShipDock.ECS
{
    public abstract class LogicSystem : ECSLogic, ILogicSystem
    {
        private ILogicData mData;
        private ILogicComponent mComponent;
        private IdentBitsGroup mRelatedComponents;

        protected int mDeltaTime;

        public bool IsSceneUpdate { get; private set; }
        
        #region 帧末更新相关的回调
        public Action<int> OnFinalUpdateForEntitas { set; private get; }
        public Action<int> OnFinalUpdateForComp { private get; set; }
        public Action<ILogicData> OnFinalUpdateForData { private get; set; }
        public Action<Action<int, int, ILogicData>> OnFinalUpdateForExecute { set; private get; }
        #endregion

        public int[] RelateComponents { get; private set; } = new int[] { };

        protected ILogicContext Context { get; private set; }

        public override bool IsSystem
        {
            get
            {
                return true;
            }

            protected set { }
        }

        public LogicSystem()
        {
            mRelatedComponents = new IdentBitsGroup();
        }

        protected override void Purge()
        {
            Reset(true);

            Context = default;
        }

        public override void Reset(bool clearOnly = false)
        {
            mRelatedComponents.Reset();
        }

        public override void Init(ILogicContext context)
        {
            Context = context;

            Reset();

        }

        public void RelateComponent(int componentName)
        {
            if (mRelatedComponents.Check(componentName)) { }
            else
            {
                mRelatedComponents.Mark(componentName);
            }

            RelateComponents = mRelatedComponents.GetAllMarks();
        }

        /// <summary>
        /// 获取已关联到系统的组件
        /// </summary>
        /// <typeparam name="T">组件泛型</typeparam>
        /// <param name="componentName">组件名</param>
        /// <returns></returns>
        public T GetRelatedComponent<T>(int componentName) where T : ILogicComponent
        {
            return (T)Context.RefComponentByName(componentName);
        }

        public void SetSceneUpdate(bool value)
        {
            IsSceneUpdate = value;
        }

        /// <summary>
        /// 组件更新后的处理
        /// </summary>
        protected virtual void AfterComponentExecuted() { }

        protected virtual void BeforeUpdateComponents()
        {
        }

        protected virtual JobHandle MakeJobs(int compName, int max)
        {
            return mJobHandlers;
        }

        private JobHandle mJobHandlers;

        /// <summary>
        /// 组件的帧更新方法
        /// </summary>
        /// <param name="time"></param>
        public void UpdateComponents(int time)
        {
            BeforeUpdateComponents();

            mDeltaTime = time;

            bool hasDataChanged;
            int j, m, componentName, entitasID;

            int max = RelateComponents.Length;
            for (int i = 0; i < max; i++)
            {
                componentName = RelateComponents[i];
                mComponent = Context.RefComponentByName(componentName);

                if (mComponent != default)
                {
                    hasDataChanged = mComponent.HasDataChanged;
                    if (hasDataChanged)
                    {
                        m = mComponent.DataPosition;

                        mJobHandlers = MakeJobs(componentName, m);

                        for (j = 0; j < m; j++)
                        {
                            hasDataChanged = mComponent.IsDatasChanged(j);
                            if (hasDataChanged)
                            {
                                entitasID = mComponent.GetEntitasIDByIndex(j);
                                if (mComponent.IsStateRegular(entitasID, out _))
                                {
                                    mData = mComponent.GetDataByIndex(j);
                                    Execute(entitasID, componentName, mData);
                                }
                                else { }
                            }
                            else { }
                        }
                    }
                    else { }
                }
                else { }
            }

            mJobHandlers.Complete();
            AfterComponentExecuted();

            mData = default;
            mComponent = default;
        }

        /// <summary>
        /// 组件的帧更新执行函数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="entitasID"></param>
        public abstract void Execute(int entitas, int componentName, ILogicData data);

        /// <summary>
        /// 将实体及对应的方法加入帧末执行队列
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="entitas"></param>
        /// <param name="method"></param>
        protected void ExecuteInFinal(int entitas, int componentName, Action<int, int, ILogicData> method)
        {
            if (IsSceneUpdate) { }
            else
            {
                OnFinalUpdateForEntitas.Invoke(entitas);
                OnFinalUpdateForComp.Invoke(componentName);
                OnFinalUpdateForExecute(method);
            }
        }
    }
}