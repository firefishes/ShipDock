using ShipDock;

public class SampleLogicModulars : ShipDockAppComponent
{
    public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
    {
        base.GetDataProxyHandler(param);

        //����߼�ģ����������ݴ���
        param.ParamValue = new IDataProxy[]
        {
            new SampleData(),
        };
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //���幦��ģ��
        IModular[] modulars = new ApplicationModular[]
        {
            new SampleGameStartModular(),
            new SampleGameMissionModular(),
        };

        SampleConsts.N_SAMPLE_MODULARS_END.Add(OnSampleModularsEnd);

        //���ģ��
        ShipDockApp shipDockApp = ShipDockApp.Instance;
        DecorativeModulars appModular = shipDockApp.AppModulars;
        appModular.AddModular(modulars);

        //�㲥ģ����Ϣ
        appModular.NotifyModular(SampleConsts.N_SAMPLE_GAME_START);
    }

    /// <summary>
    /// ��ͨ����Ϣ�㲥����������Ҳ���Խ���ģ�����Ϣ�ܵ����͵���Ϣ
    /// </summary>
    /// <param name="param"></param>
    private void OnSampleModularsEnd(INoticeBase<int> param)
    {
        SampleConsts.N_SAMPLE_MODULARS_END.Remove(OnSampleModularsEnd);
        "log: ������ʾ���".Log();
    }
}
