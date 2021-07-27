using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDOckAssets : ShipDock.Applications.ShipDockAppComponent
{
    protected override string GetLocalsDescription<T>(ref string locals, ref T item)
    {
        return string.Empty;
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();


    }
}
