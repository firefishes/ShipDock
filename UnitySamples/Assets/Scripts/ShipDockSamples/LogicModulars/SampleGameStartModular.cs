using ShipDock.Modulars;
using ShipDock.Notices;

public class SampleGameStartModular : ApplicationModular
{
    /// <summary>
    /// ��Ϸ����ģ��
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

        //����ģ����Ϣ
        AddNoticeHandler(OnGameStart);
        //����ģ����Ϣ
        AddNoticeHandler(OnGameContinue);

        //��ӿɷ��͵�ģ����Ϣ�ܵ�
        AddPipelineNotifies(OnGameEnterMission);
    }

    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_START, 1)]
    private void OnGameStart(INoticeBase<int> obj)
    {
        "log".Log("��ʼ��Ϸ");
        //�Թܵ���ʽ�㲥ģ����Ϣ
        NotifyModularPipeline(OnGameEnterMission);
    }

    /// <summary>
    /// �ܵ���Ϣ�ɶ�������Ӧ�����ȼ�
    /// </summary>
    /// <param name="obj"></param>
    [ModularNoticeListener(SampleConsts.N_SAMPLE_GAME_MISSION_FINISHED, 1)]
    private void OnGameContinue(INoticeBase<int> obj)
    {
        "log".Log("�ؿ���������Ϸ����");
    }

    [ModularNotify(SampleConsts.N_SAMPLE_GAME_ENTER_MISSION, SampleConsts.N_SAMPLE_GAME_LOAD_MISSION, NotifyTiming = ModularNotifyTiming.BEFORE)]
    private void OnGameEnterMission(INoticeBase<int> param) { }
}
