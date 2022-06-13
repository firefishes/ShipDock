using ShipDock.Applications;
using ShipDock.Modulars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleLogicModulars : ShipDockAppComponent
{
    public override void EnterGameHandler()
    {
        base.EnterGameHandler();
        
        IModular[] modulars = new ApplicationModular[]
        {
            new SampleGameStartModular(),
            new SampleGameMissionModular(),
        };

        ShipDockApp shipDockApp = ShipDockApp.Instance;
        DecorativeModulars appModular = shipDockApp.AppModulars;
        appModular.AddModular(modulars);

        appModular.NotifyModular(SampleConsts.N_SAMPLE_GAME_START);
    }
}
