using ShipDock.Applications;
using UnityEngine;

public class CreateGameStartUp : ShipDockAppComponent
{
    protected override string GetLocalsDescription<T>(ref string locals, ref T item)
    {
        return string.Empty;
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        GameObject raw = ShipDockApp.Instance.ABs.Get("aa", "a");
        Instantiate(raw);

        TimeUpdater.New(5f, () =>
        {
            ReloadFrameworkScene();
        });
    }
}
