using System;

namespace ShipDock
{
    public interface ICustomFramework
    {
        bool IsStarted { get; }
        IFrameworkUnit [] FrameworkUnits { get; }
        IUpdatesComponent UpdatesComponent { get; }
        Action MergeCallOnMainThread { get; set; }

        void Run(int ticks);
        void SetUpdatesComponent(IUpdatesComponent component);
        void SetStarted(bool value);
        void SyncToUpdatesComponent(Action method);
        void Clean();
    }
}
