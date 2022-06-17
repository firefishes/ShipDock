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

        //新建一个方法更新器，用于加入场景帧总线的更新
        mUpdater = new MethodUpdater()
        {
            Update = OnBusUpdate,
        };
        UpdaterNotice.AddSceneUpdater(mUpdater);

        //新建定时器，每 2 秒执行一次，并根据情况取消定时器
        TimeUpdater timer = TimeUpdater.New(2f, OnTimer, OnTimerCancel, 0);
        timer.IsAutoDispose = true;
    }

    /// <summary>
    /// 检测定时器取消的方法
    /// </summary>
    /// <returns></returns>
    private bool OnTimerCancel()
    {
        bool result = mTimerStartCounts >= 4;
        if (result)
        {
            //运行 4 次后取消定时器，并从场景帧总线移除方法更新器
            UpdaterNotice.RemoveSceneUpdater(mUpdater);
            //下一帧执行方法
            UpdaterNotice.SceneCallLater(OnUpdateNextFrame);
        }
        else { }
        return result;
    }

    /// <summary>
    /// 下一帧执行的方法
    /// </summary>
    /// <param name="deltaTime"></param>
    private void OnUpdateNextFrame(int deltaTime)
    {
        "log:Start bus updated by thread next frame...".Log();

        //将方法更新器的更新回调切换为子线程使用的帧更新方法
        mUpdater.Update = OnBusUpdateByThread;
        //将方法更新器加入子线程的帧总线的更新
        UpdaterNotice.AddUpdater(mUpdater);
    }

    /// <summary>
    /// 定时器方法
    /// </summary>
    private void OnTimer()
    {
        mTimerStartCounts++;
        "log:Timer completed".Log();
    }

    /// <summary>
    /// 场景帧总线的更新方法
    /// </summary>
    /// <param name="deltaTime"></param>
    private void OnBusUpdate(int deltaTime)
    {
        float scaler = UpdatesCacher.UPDATE_CACHER_TIME_SCALE * 1f;
        "log:Bus updated, frame delta time is {0} sec".Log((deltaTime / scaler).ToString());
    }

    /// <summary>
    /// 子线程的帧总线更新方法
    /// </summary>
    /// <param name="deltaTime"></param>
    private void OnBusUpdateByThread(int deltaTime)
    {
        float scaler = UpdatesCacher.UPDATE_CACHER_TIME_SCALE * 1f;
        "log:Bus updated by another thread, frame delta time is {0} sec".Log((deltaTime / scaler).ToString());
    }
}
