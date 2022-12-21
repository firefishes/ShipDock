using ShipDock.ECS;
using System;

namespace ShipDock.Applications
{
    public interface IProcessingComponent : IECSLogic
    {
        bool AddProcess(Action<IProcessing> method);
    }

}