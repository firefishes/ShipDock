using ShipDock;

public class SampleLogicModulars : ShipDockAppComponent
{
    public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
    {
        base.GetDataProxyHandler(param);

        //添加逻辑模块所需的数据代理
        param.ParamValue = new IDataProxy[]
        {
            new SampleData(),
        };
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //定义功能模块
        IModular[] modulars = new ApplicationModular[]
        {
            new SampleGameStartModular(),
            new SampleGameMissionModular(),
        };

        SampleConsts.N_SAMPLE_MODULARS_END.Add(OnSampleModularsEnd);

        //添加模块
        ShipDockApp shipDockApp = ShipDockApp.Instance;
        DecorativeModulars appModular = shipDockApp.AppModulars;
        appModular.AddModular(modulars);

        //广播模块消息
        appModular.NotifyModular(SampleConsts.N_SAMPLE_GAME_START);
    }

    /// <summary>
    /// 普通的消息广播处理器函数也可以接收模块的消息管道发送的消息
    /// </summary>
    /// <param name="param"></param>
    private void OnSampleModularsEnd(INoticeBase<int> param)
    {
        SampleConsts.N_SAMPLE_MODULARS_END.Remove(OnSampleModularsEnd);
        "log: 案例演示完毕".Log();
    }
}
