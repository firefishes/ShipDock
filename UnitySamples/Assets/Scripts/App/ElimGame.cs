using ShipDock.Applications;
using ShipDock.Game;
using ShipDock.Loader;
using UnityEngine;

public class ElimGame : ShipDockAppComponent
{
    protected override string GetLocalsDescription<T>(ref string locals, ref T item)
    {
        throw new System.NotImplementedException();
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        "log".Log("Game Entered");

        IAppILRuntime app = ShipDockApp.Instance;
        app.SetHotFixSetting(new ILRuntimeHotFix(app), new AppHotFixConfigBase());

        ElimConsts.UIM_ELIM_UI.LoadAndOpenUI<UIElimModular>(OnCreateBoard, ElimConsts.AB_ELIM_UI);
    }

    private void OnCreateBoard(UIElimModular ui)
    {
        AssetBundles abs = ShipDockApp.Instance.ABs;
        GameObject board = abs.GetAndQuote<GameObject>("elim_game_res/prefabs", "EliminatePlay", out _);
        GameObject map = abs.GetAndQuote<GameObject>("elim_game_map", "MissionMap", out _);

        EliminatePlayComponent comp = board.GetComponent<EliminatePlayComponent>();
        comp.StartEliminatePlay();
    }
}
