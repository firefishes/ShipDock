using ShipDock.Modulars;
using ShipDock.Notices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleGameMissionModular : ApplicationModular
{
    public SampleGameMissionModular()
    {
        ModularName = SampleConsts.M_SAMPLE_GAME_MISSIONS;
    }

    public override void Purge()
    {
    }

    protected override void InitCustomHandlers()
    {
        base.InitCustomHandlers();

        AddNoticeCreater(OnGameStartNoticeCreater);
        AddNoticeDecorater(OnGameStartNoticeDecorater);
    }

    [ModularNoticeCreate(SampleConsts.N_SAMPLE_GAME_ENTER_MISSION)]
    private INoticeBase<int> OnGameStartNoticeCreater(int noticeName)
    {
        "log".Log("创建消息");
        return new ParamNotice<int>() { ParamValue = 1 };
    }

    [ModularNoticeDecorater(SampleConsts.N_SAMPLE_GAME_ENTER_MISSION)]
    private void OnGameStartNoticeDecorater(int noticeName, INoticeBase<int> notice)
    {
        switch (noticeName)
        {
            case SampleConsts.N_SAMPLE_GAME_ENTER_MISSION:
                "log".Log("响应消息");
                (notice as IParamNotice<int>).ParamValue = 10;
                break;
        }
    }
}
