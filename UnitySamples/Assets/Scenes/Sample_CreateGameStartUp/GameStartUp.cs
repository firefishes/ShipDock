using ShipDock.Applications;

public class GameStartUp : ShipDockAppComponent
{
    protected override string GetLocalsDescription<T>(ref string locals, ref T item)
    {
        return string.Empty;
    }
}
