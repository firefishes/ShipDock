using ShipDock;
/// <summary>
/// �ؿ�ģ��
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

        //���ģ����Ϣ��������������
        AddNoticeCreater(OnGameStartNoticeCreater);
        //���ģ����Ϣ����װ�δ�����
        AddNoticeDecorater(OnGameStartNoticeDecorater);

        //���ģ����Ϣ������
        AddNoticeHandler(OnGameStart);
        //���ģ����Ϣ������
        AddNoticeHandler(OnGameEnterMission);
        //���ģ����Ϣ������
        AddNoticeHandler(OnLoadMissionNoticeHandler);

        //��ӿɷ��͵�ģ����Ϣ�ܵ�
        AddPipelineNotifies(OnMissionFinished);
    }

    [ModularNoticeCreate(SampleConsts.N_SAMPLE_GAME_ENTER_MISSION)]
    private INoticeBase<int> OnGameStartNoticeCreater(int noticeName)
    {
        "log".Log("������Ϣ");
        return new ParamNotice<int>() { ParamValue = 1 };
    }

    [ModularNoticeDecorater(SampleConsts.N_SAMPLE_GAME_ENTER_MISSION)]
    private void OnGameStartNoticeDecorater(int noticeName, INoticeBase<int> notice)
    {
        switch (noticeName)
        {
            case SampleConsts.N_SAMPLE_GAME_ENTER_MISSION:
                "log".Log("װ����Ϣ");
                (notice as IParamNotice<int>).ParamValue = 10;
                break;
        }
    }

    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_MISSION_FINISHED, 2)]
    private void OnGameStart(INoticeBase<int> obj)
    {
        "log".Log("�˴�������ȼ�������Ϊ 2");
    }

    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_ENTER_MISSION)]
    private void OnGameEnterMission(INoticeBase<int> param)
    {
        IParamNotice<int> notice = param as IParamNotice<int>;
        int mission = notice.ParamValue;
        //���յ�����Ϣ����������ص����ݴ���
        SampleData data = SampleConsts.D_SAMPLE.GetData<SampleData>();
        data.MissionIndex = mission;
        "log: ����� {0} ��".Log(mission.ToString());
    }

    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_LOAD_MISSION)]
    private void OnLoadMissionNoticeHandler(INoticeBase<int> param)
    {
        //����ص����ݴ���ȡ������
        SampleData data = SampleConsts.D_SAMPLE.GetData<SampleData>();
        "log: ���عؿ� {0} ��".Log(data.MissionIndex.ToString());

        NotifyModularPipeline(OnMissionFinished);
    }

    [ModularNotify(SampleConsts.N_SAMPLE_GAME_MISSION_FINISHED, SampleConsts.N_SAMPLE_MODULARS_END)]
    private void OnMissionFinished(INoticeBase<int> param)
    {
    }
}
