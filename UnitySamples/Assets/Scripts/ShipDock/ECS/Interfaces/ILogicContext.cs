using System;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public interface ILogicContext
    {
        ILogicEntitas AllEntitas { get; }
        Action<List<int>, bool> PreUpdate { get; set; }
        int CountTime { get; }
        int FrameTimeInScene { get; set; }

        int Create<T>(int aid, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IECSLogic, new();
        ILogicComponent RefComponentByName(int componentName);
        void UpdateECSUnits(int time, Action<Action<int>> method = default);
        void UpdateECSUnitsInScene(int time, Action<Action<int>> method = default);
    }
}