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
    //֡����������
    private MethodUpdater mUpdater;

    /// <summary>
    /// Ӧ�ùرյĻص�����
    /// </summary>
    public override void ApplicationCloseHandler()
    {
        base.ApplicationCloseHandler();

        //�Ƴ���Ϸ��ʼ��Ϣ
        SampleConsts.N_SAMPLE_GAME_START.Remove(OnGameStarted);
        //�Ƴ�����ؿ���Ϣ
        SampleConsts.N_SAMPLE_GAME_LOAD_MISSION.Remove(OnEnterMission);
        //�Ƴ��ؿ�������Ϣ
        SampleConsts.N_SAMPLE_GAME_ENTER_MISSION.Remove(OnLoadMission);
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //��Ĭ�����͵���Ϣ���л�ȡ��Ϣ����
        Notice notice = Pooling<Notice>.From();
        //�������㲥��Ϣ
        SampleConsts.N_SAMPLE_GAME_START.Add(OnGameStarted);
        SampleConsts.N_SAMPLE_GAME_START.Broadcast(notice);

        //��Я��һ�����Ͳ������͵���Ϣ���л�ȡ��Ϣ����
        ParamNotice<int> paramNotice = Pooling<ParamNotice<int>>.From();
        paramNotice.ParamValue = 100;
        //�������㲥��Ϣ
        SampleConsts.N_SAMPLE_GAME_ENTER_MISSION.Add(OnEnterMission);
        SampleConsts.N_SAMPLE_GAME_ENTER_MISSION.Broadcast(paramNotice);

        //��ʼ��֡����������
        mUpdater = new MethodUpdater()
        {
            Update = OnUpdate,
        };
        //��ӳ���֡������
        UpdaterNotice.AddSceneUpdater(mUpdater);
        //������Ҫÿ֡�㲥����Ϣ
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
        //��һ���Զ������͵Ķ���ػ�ȡ����
        CustomUpdateNotice notice = Pooling<CustomUpdateNotice>.From();
        notice.deltaTime += (deltaTime * 1f) / 10000f;
        //�㲥��Ϣ
        SampleConsts.N_SAMPLE_GAME_LOAD_MISSION.Broadcast(notice);
        //�黹���󵽳���
        notice.ToPool();

        string abName = "sample_res";
        string assetName = "Cube";

        AssetBundles abs = ShipDockApp.Instance.ABs;
        //����Դ��������ģ���ȡ��Դĸ��
        GameObject raw = abs.Get(abName, assetName);
        //����ĸ����ͨ����Դ����ػ�ȡʵ��
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
/// �Զ������͵���Ϣ
/// </summary>
public class CustomUpdateNotice : Notice
{
    //ʵ����
    public static int instanceCount;

    public float deltaTime;

    public CustomUpdateNotice()
    {
        instanceCount++;
    }

    /// <summary>
    /// ����ʱ��ӡ��ʵ����
    /// </summary>
    ~CustomUpdateNotice()
    {
        instanceCount--;

        "log: ~CustomUpdateNotice invoked, instance count is {0}".Log(instanceCount.ToString());
    }

    /// <summary>
    /// ���ǳض���ӿ��еĹ黹����������黹ʱ��Ҫʹ�õĳ���
    /// </summary>
    public override void ToPool()
    {
        base.ToPool();

        Pooling<CustomUpdateNotice>.To(this);

        "log: CustomUpdateNotice instance count is {0}".Log(instanceCount.ToString());
    }

    /// <summary>
    /// ���ǳض���ӿ��еĻ�ԭ����������黹ʱ����ĸ�ԭ�߼�
    /// </summary>
    public override void Revert()
    {
        base.Revert();

        deltaTime = default;
    }
}
