using System;

namespace ShipDock
{
    public interface ILogicSystem : IECSLogic
    {
        bool IsSceneUpdate { get; }
        int[] RelateComponents { get; }
        Action<int> OnFinalUpdateForEntitas { set; }
        Action<int> OnFinalUpdateForComp { set; }
        Action<ILogicData> OnFinalUpdateForData { set; }
        Action<Action<int, int, ILogicData>> OnFinalUpdateForExecute { set; }

        void RelateComponent(int componentName);
        void SetSceneUpdate(bool value);
        void Execute(int entitas, int componentName, ILogicData data);
        T GetRelatedComponent<T>(int aid) where T : ILogicComponent;
        void UpdateSystem(int time);
    }
}