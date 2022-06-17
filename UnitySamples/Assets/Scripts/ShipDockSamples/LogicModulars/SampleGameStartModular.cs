using ShipDock.Modulars;
using ShipDock.Notices;

public class SampleGameStartModular : ApplicationModular
{
    /// <summary>
    /// 游戏流程模块
    /// </summary>
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

        //侦听模块消息
        AddNoticeHandler(OnGameStart);
        //侦听模块消息
        AddNoticeHandler(OnGameContinue);

        //添加可发送的模块消息管道
        AddPipelineNotifies(OnGameEnterMission);
    }

    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_START, 1)]
    private void OnGameStart(INoticeBase<int> obj)
    {
        "log".Log("开始游戏");
        //以管道方式广播模块消息
        NotifyModularPipeline(OnGameEnterMission);
    }

    /// <summary>
    /// 管道消息可定义其响应的优先级
    /// </summary>
    /// <param name="obj"></param>
    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_MISSION_FINISHED, 1)]
    private void OnGameContinue(INoticeBase<int> obj)
    {
        "log".Log("关卡结束，游戏继续");
    }

    [ModularNotify(SampleConsts.N_SAMPLE_GAME_ENTER_MISSION, SampleConsts.N_SAMPLE_GAME_LOAD_MISSION, NotifyTiming = ModularNotifyTiming.BEFORE)]
    private void OnGameEnterMission(INoticeBase<int> param) { }
}
