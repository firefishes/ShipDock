using ShipDock;
/// <summary>
/// 关卡模块
/// </summary>
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

        //添加模块消息对象容器处理器
        AddNoticeCreater(OnGameStartNoticeCreater);
        //添加模块消息对象装饰处理器
        AddNoticeDecorater(OnGameStartNoticeDecorater);

        //添加模块消息处理器
        AddNoticeHandler(OnGameStart);
        //添加模块消息处理器
        AddNoticeHandler(OnGameEnterMission);
        //添加模块消息处理器
        AddNoticeHandler(OnLoadMissionNoticeHandler);

        //添加可发送的模块消息管道
        AddPipelineNotifies(OnMissionFinished);
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
                "log".Log("装饰消息");
                (notice as IParamNotice<int>).ParamValue = 10;
                break;
        }
    }

    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_MISSION_FINISHED, 2)]
    private void OnGameStart(INoticeBase<int> obj)
    {
        "log".Log("此处理的优先级被设置为 2");
    }

    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_ENTER_MISSION)]
    private void OnGameEnterMission(INoticeBase<int> param)
    {
        IParamNotice<int> notice = param as IParamNotice<int>;
        int mission = notice.ParamValue;
        //将收到的消息参数存入相关的数据代理
        SampleData data = SampleConsts.D_SAMPLE.GetData<SampleData>();
        data.MissionIndex = mission;
        "log: 进入第 {0} 关".Log(mission.ToString());
    }

    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_LOAD_MISSION)]
    private void OnLoadMissionNoticeHandler(INoticeBase<int> param)
    {
        //从相关的数据代理取出参数
        SampleData data = SampleConsts.D_SAMPLE.GetData<SampleData>();
        "log: 加载关卡 {0} 关".Log(data.MissionIndex.ToString());

        NotifyModularPipeline(OnMissionFinished);
    }

    [ModularNotify(SampleConsts.N_SAMPLE_GAME_MISSION_FINISHED, SampleConsts.N_SAMPLE_MODULARS_END)]
    private void OnMissionFinished(INoticeBase<int> param)
    {
    }
}
