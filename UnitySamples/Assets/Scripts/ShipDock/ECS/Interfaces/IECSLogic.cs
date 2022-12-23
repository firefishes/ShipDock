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
        string Name { get; set; }
        int ID { get; }
        bool IsSystem { get; }
        ILogicEntities AllEntitas { get; }

        void Init(ILogicContext context);
        void SetAllEntitas(ILogicEntities allEntitas);
        void SetUnitID(int id);
    }
}