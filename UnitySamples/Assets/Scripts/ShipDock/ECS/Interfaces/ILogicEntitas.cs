using ShipDock.Interfaces;
using System;

namespace ShipDock.ECS
{
    /// <summary>
    /// ECS 实体生成器接口
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public interface ILogicEntities : IReclaim
    {
        void BuildEntitasTemplate(int entitasType, params int[] componentNames);
        void AddEntitas(out int newEntitasID, int entitasType = 0);
        bool HasEntitas(int entitasID);
        void RemoveEntitas(int entitasID);
        void AddComponent(int entitasID, int componentName);
        void AddComponent(int entitasID, ILogicComponent component);
        bool HasComponent(int entitasID, int componentID);
        int[] GetComponentList(int entitasID);
        T GetComponentFromEntitas<T>(int entitasID, int componentID) where T : ILogicComponent;
        void RemoveComponent(int entitasID, ILogicComponent component);

        void AddTypeSizeOf(Type type, int byteSize);

        void MakeChunks();
    }
}
