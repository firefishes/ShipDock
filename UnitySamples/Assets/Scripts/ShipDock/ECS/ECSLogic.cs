namespace ShipDock.ECS
{
    public abstract class ECSLogic : IECSLogic
    {
        public virtual bool IsSystem { get; protected set; }

        public ILogicEntitas AllEntitas { get; private set; }

        public int ID { get; private set; } = int.MaxValue;

        public abstract void Init(ILogicContext context);

        public virtual void Reclaim()
        {
            Purge();

            ID = int.MaxValue;
            AllEntitas = default;
        }

        public abstract void Reset(bool clearOnly = false);

        protected abstract void Purge();

        public void SetAllEntitas(ILogicEntitas entitas)
        {
            AllEntitas = entitas;
        }

        /// <summary>
        /// 设置组件名
        /// </summary>
        /// <param name="id"></param>
        public void SetUnitID(int id)
        {
            if (ID == int.MaxValue)
            {
                ID = id;
            }
            else { }
        }
    }
}
