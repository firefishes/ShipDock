using ShipDock;
using UnityEngine;

public class ILRuntimeSample : ShipDockAppComponent
{
    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        Entered();
    }

    private void Entered()
    {
        Debug.Log("Game entered..");

        ShipDockApp.Instance.Tester.AddAsserter("start", false, new string[]
        {
            "game core loaded",
            "game core input loaded",
        });

        IAppILRuntime app = ShipDockApp.Instance;

#if APPLY_HOTFIX_IN_STARTUP
        app.ILRuntimeHotFix.Clear();
#endif
        app.SetHotFixSetting(new ILRuntimeHotFix(app), new AppHotFixConfig());

        //ShipDockApp.Instance.Tester.LogEnabled("loader deps", false);
        //ShipDockApp.Instance.Tester.LogEnabled("walk deps", false);
        //ShipDockApp.Instance.Tester.LogEnabled("deps", false);

        ShipDockApp shipDockApp = ShipDockApp.Instance;
        GameObject mainBridge = shipDockApp.ABs.Get<GameObject>("main", "GameMainBridge");
        //Instantiate(mainBridge);
    }
}
