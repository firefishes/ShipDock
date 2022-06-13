using ShipDock.Modulars;
using ShipDock.Notices;

public class SampleGameStartModular : ApplicationModular
{
    public SampleGameStartModular()
    {
        ModularName = SampleConsts.M_SAMPLE_GAME_START;
    }

    public override void Purge()
    {
    }

    protected override void InitCustomHandlers()
    {
        base.InitCustomHandlers();

        AddNoticeHandler(OnGameStart);

        AddNotifies(OnGameEnterMission);
    }

    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_START, 1)]
    private void OnGameStart(INoticeBase<int> obj)
    {
        "log".Log("开始游戏");
        NotifyModular(OnGameEnterMission);
    }

    [ModularNotify(SampleConsts.N_SAMPLE_GAME_ENTER_MISSION, NotifyTiming = ModularNotifyTiming.BEFORE)]
    private void OnGameEnterMission(INoticeBase<int> obj)
    {
        IParamNotice<int> notice = obj as IParamNotice<int>;
        int mission = notice.ParamValue;
        "log: 进入第 {0} 关".Log(mission.ToString());
    }
}
