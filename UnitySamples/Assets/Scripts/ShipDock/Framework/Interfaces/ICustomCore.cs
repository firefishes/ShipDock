using System;

namespace ShipDock
{
    public interface ICustomCore
    {
        bool IsStarted { get; }
        IFrameworkUnit [] FrameworkUnits { get; }
        IUpdatesComponent UpdatesComponent { get; }
        Action MergeCallOnMainThread { get; set; }

        void Run(int ticks);
        void SetUpdatesComponent(IUpdatesComponent component);
        void SetStarted(bool value);
        void InitUIRoot(IUIRoot root);
        void SyncToUpdates(Action method);
        void Clean();
    }
}
