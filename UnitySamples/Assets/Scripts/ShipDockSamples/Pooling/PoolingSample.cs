using System;
using ShipDock.Applications;
using ShipDock.Commons;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using UnityEngine;

public class PoolingSample : ShipDockAppComponent
{
    //帧函数更新器
    private MethodUpdater mUpdater;

    /// <summary>
    /// 应用关闭的回调函数
    /// </summary>
    public override void ApplicationCloseHandler()
    {
        base.ApplicationCloseHandler();

        //移除游戏开始消息
        SampleConsts.N_SAMPLE_GAME_START.Remove(OnGameStarted);
        //移除进入关卡消息
        SampleConsts.N_SAMPLE_GAME_LOAD_MISSION.Remove(OnEnterMission);
        //移除关卡加载消息
        SampleConsts.N_SAMPLE_GAME_ENTER_MISSION.Remove(OnLoadMission);
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //从默认类型的消息池中获取消息对象
        Notice notice = Pooling<Notice>.From();
        //侦听并广播消息
        SampleConsts.N_SAMPLE_GAME_START.Add(OnGameStarted);
        SampleConsts.N_SAMPLE_GAME_START.Broadcast(notice);

        //从携带一个泛型参数类型的消息池中获取消息对象
        ParamNotice<int> paramNotice = Pooling<ParamNotice<int>>.From();
        paramNotice.ParamValue = 100;
        //侦听并广播消息
        SampleConsts.N_SAMPLE_GAME_ENTER_MISSION.Add(OnEnterMission);
        SampleConsts.N_SAMPLE_GAME_ENTER_MISSION.Broadcast(paramNotice);

        //初始化帧函数更新器
        mUpdater = new MethodUpdater()
        {
            Update = OnUpdate,
        };
        //添加场景帧更新器
        UpdaterNotice.AddSceneUpdater(mUpdater);
        //侦听需要每帧广播的消息
        SampleConsts.N_SAMPLE_GAME_LOAD_MISSION.Add(OnLoadMission);
    }

    private void OnEnterMission(INoticeBase<int> param)
    {
        Debug.Log(param);
        IParamNotice<int> notice = param as IParamNotice<int>;
        "log:Param notice is revert to pooling, param value is {0}".Log(notice.ParamValue.ToString());
        notice.ToPool();
    }

    private void OnGameStarted(INoticeBase<int> param)
    {
        Notice notice = param as Notice;
        notice.ToPool();

        "log:Notice is revert to pooling".Log();
    }

    private void OnUpdate(int deltaTime)
    {
        //从一个自定义类型的对象池获取对象
        CustomUpdateNotice notice = Pooling<CustomUpdateNotice>.From();
        notice.deltaTime += (deltaTime * 1f) / 10000f;
        //广播消息
        SampleConsts.N_SAMPLE_GAME_LOAD_MISSION.Broadcast(notice);
        //归还对象到池子
        notice.ToPool();

        string abName = "sample_res";
        string assetName = "Cube";

        AssetBundles abs = ShipDockApp.Instance.ABs;
        //从资源包管理器模组获取资源母本
        GameObject raw = abs.Get(abName, assetName);
        //传入母本，通过资源对象池获取实例
        GameObject model = ShipDockApp.Instance.AssetsPooling.FromPool(1, ref raw, OnModelInit);

        TimeUpdater.New(1f, () =>
        {
            ShipDockApp.Instance.AssetsPooling.ToPool(1, model);
        });
    }

    private void OnModelInit(GameObject target)
    {
        float x = Utils.UnityRangeRandom(-300f, 300f);
        float y = Utils.UnityRangeRandom(-100f, 100f);
        float z = 0f;
        target.transform.position = new Vector3(x, y, z);
        
        x = Utils.UnityRangeRandom(1f, 3f);
        y = Utils.UnityRangeRandom(1f, 3f);
        z = Utils.UnityRangeRandom(1f, 3f);
        target.transform.localScale = new Vector3(x, y, z);
    }

    private void OnLoadMission(INoticeBase<int> param)
    {
        CustomUpdateNotice notice = param as CustomUpdateNotice;
        "log: Updating delta time is {0}".Log(notice.deltaTime.ToString());
    }
}

/// <summary>
/// 自定义类型的消息
/// </summary>
public class CustomUpdateNotice : Notice
{
    //实例数
    public static int instanceCount;

    public float deltaTime;

    public CustomUpdateNotice()
    {
        instanceCount++;
    }

    /// <summary>
    /// 析构时打印总实例数
    /// </summary>
    ~CustomUpdateNotice()
    {
        instanceCount--;

        "log: ~CustomUpdateNotice invoked, instance count is {0}".Log(instanceCount.ToString());
    }

    /// <summary>
    /// 覆盖池对象接口中的归还方法，定义归还时需要使用的池子
    /// </summary>
    public override void ToPool()
    {
        base.ToPool();

        Pooling<CustomUpdateNotice>.To(this);

        "log: CustomUpdateNotice instance count is {0}".Log(instanceCount.ToString());
    }

    /// <summary>
    /// 覆盖池对象接口中的还原方法，定义归还时对象的复原逻辑
    /// </summary>
    public override void Revert()
    {
        base.Revert();

        deltaTime = default;
    }
}
