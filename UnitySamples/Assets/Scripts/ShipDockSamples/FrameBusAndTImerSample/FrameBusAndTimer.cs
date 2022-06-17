using ShipDock.Applications;
using ShipDock.Commons;
using ShipDock.Notices;
using ShipDock.Ticks;

public class FrameBusAndTimer : ShipDockAppComponent
{
    private MethodUpdater mUpdater;
    private int mTimerStartCounts;

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //�½�һ�����������������ڼ��볡��֡���ߵĸ���
        mUpdater = new MethodUpdater()
        {
            Update = OnBusUpdate,
        };
        UpdaterNotice.AddSceneUpdater(mUpdater);

        //�½���ʱ����ÿ 2 ��ִ��һ�Σ����������ȡ����ʱ��
        TimeUpdater timer = TimeUpdater.New(2f, OnTimer, OnTimerCancel, 0);
        timer.IsAutoDispose = true;
    }

    /// <summary>
    /// ��ⶨʱ��ȡ���ķ���
    /// </summary>
    /// <returns></returns>
    private bool OnTimerCancel()
    {
        bool result = mTimerStartCounts >= 4;
        if (result)
        {
            //���� 4 �κ�ȡ����ʱ�������ӳ���֡�����Ƴ�����������
            UpdaterNotice.RemoveSceneUpdater(mUpdater);
            //��һִ֡�з���
            UpdaterNotice.SceneCallLater(OnUpdateNextFrame);
        }
        else { }
        return result;
    }

    /// <summary>
    /// ��һִ֡�еķ���
    /// </summary>
    /// <param name="deltaTime"></param>
    private void OnUpdateNextFrame(int deltaTime)
    {
        "log:Start bus updated by thread next frame...".Log();

        //�������������ĸ��»ص��л�Ϊ���߳�ʹ�õ�֡���·���
        mUpdater.Update = OnBusUpdateByThread;
        //�������������������̵߳�֡���ߵĸ���
        UpdaterNotice.AddUpdater(mUpdater);
    }

    /// <summary>
    /// ��ʱ������
    /// </summary>
    private void OnTimer()
    {
        mTimerStartCounts++;
        "log:Timer completed".Log();
    }

    /// <summary>
    /// ����֡���ߵĸ��·���
    /// </summary>
    /// <param name="deltaTime"></param>
    private void OnBusUpdate(int deltaTime)
    {
        float scaler = UpdatesCacher.UPDATE_CACHER_TIME_SCALE * 1f;
        "log:Bus updated, frame delta time is {0} sec".Log((deltaTime / scaler).ToString());
    }

    /// <summary>
    /// ���̵߳�֡���߸��·���
    /// </summary>
    /// <param name="deltaTime"></param>
    private void OnBusUpdateByThread(int deltaTime)
    {
        float scaler = UpdatesCacher.UPDATE_CACHER_TIME_SCALE * 1f;
        "log:Bus updated by another thread, frame delta time is {0} sec".Log((deltaTime / scaler).ToString());
    }
}
