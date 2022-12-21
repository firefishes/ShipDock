using ShipDock.Interfaces;

namespace ShipDock.ECS
{
    /// <summary>
    /// ECS 逻辑接口
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public interface IECSLogic : IReclaim
    {
        int ID { get; }
        bool IsSystem { get; }
        ILogicEntitas AllEntitas { get; }

        void Init(ILogicContext context);
        void SetAllEntitas(ILogicEntitas allEntitas);
        void SetUnitID(int id);
    }
}