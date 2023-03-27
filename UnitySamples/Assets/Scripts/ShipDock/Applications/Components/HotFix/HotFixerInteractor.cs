namespace ShipDock
{
    /// <summary>
    /// 
    /// 热更端 UI 交互器抽象类，用于容纳热更端的 UI 逻辑
    /// 
    /// 对标于纯主工程下开发所使用的 UIModular<T> 中的泛型类 T 
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class HotFixerInteractor
    {
        public HotFixerUI UI { get; private set; }
        public HotFixerUIAgent Agent { get; private set; }
        public UIModularHotFixer UIModular { get; private set; }

        public UIChangingTasker UIChangingTask { get; protected set; }

        public virtual void Release()
        {
            UI = default;
            Agent = default;
            UIModular = default;

            UIChangingTask?.Clean();
        }

        public void SetUIModular(UIModularHotFixer modular)
        {
            UIModular = modular;
        }

        public virtual void InitInteractor(HotFixerUI UIOwner, HotFixerUIAgent agent)
        {
            UI = UIOwner;
            Agent = agent;

            UIChangingTask = new UIChangingTasker(UIOwner);
        }

        public virtual void Dispatch(int name, INoticeBase<int> param = default)
        {
            Agent.Dispatch(name, param);
        }

        public virtual T Dispatch<T>(int name, T vs = default)
        {
            return Agent.Dispatch(name, vs);
        }

        public virtual void UpdateInteractor() { }
    }
}