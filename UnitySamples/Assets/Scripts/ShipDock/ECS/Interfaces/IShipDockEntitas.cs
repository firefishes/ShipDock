using ShipDock.Interfaces;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public interface IShipDockEntitas : IReclaim
    {
        T GetComponentFromEntitas<T>(int aid) where T : IShipDockComponent;
        void InitComponents();
        bool HasComponent(int componentID);
        void SetEntitasID(int id);
        T GetComponentByName<T>(int name) where T : IShipDockComponent;
        void AddComponent(IShipDockComponent component);
        void RemoveComponent(IShipDockComponent component);
        int FindEntitasInComponent(IShipDockComponent component);
        List<int> ComponentList { get; }
        bool WillDestroy { get; }
        int ID { get; }
    }
}
