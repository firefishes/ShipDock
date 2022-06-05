using ShipDock.Applications;
using ShipDock.Notices;
using System;

public class NoticesSample : ShipDockAppComponent
{
    private NoticesObserver mObserver;

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        SampleConsts.N_SAMPLE_NOTICE_BY_DEFAULT.Add(OnSampleNotieHandler);
        SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.Add(OnSampleParamNotieHandler);

        TimeUpdater.New(3f, () =>
        {
            SampleConsts.N_SAMPLE_NOTICE_BY_DEFAULT.Broadcast();

            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<int>(1);
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<string>("ShipDock notice API");
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<float>(100f);
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<SampleParamValue>(new SampleParamValue());
        });
    }

    private void OnSampleNotieHandler(INoticeBase<int> param)
    {
        "log:��Ϣ {0} ����Ӧ������Ϣ�㲥�Ĳ���ΪĬ����Ϣ���� {1}".Log(param.Name.ToString(), param.GetType().Name);

        mObserver = new NoticesObserver();
        mObserver.AddListener(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER, OnEventHandler);
        mObserver.Dispatch(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER);
        mObserver.Dispatch(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER, new ParamNotice<SampleParamValue>()
        {
            ParamValue = new SampleParamValue(),
        });
    }

    private void OnEventHandler(INoticeBase<int> param)
    {
        if (param is IParamNotice<SampleParamValue> sampleParamNotice)
        {
            SampleParamValue paramValue = sampleParamNotice.ParamValue;
            "log:�۲�����Ϣ {0} ����Ӧ������Ϣ�Ĳ���ΪĬ����Ϣ���� {1}, ����ֵ {2}".Log(param.Name.ToString(), param.GetType().Name, paramValue.fieldValue.ToString());
        }
        else
        {
            "log:�۲�����Ϣ {0} ����Ӧ������Ϣ�Ĳ���ΪĬ����Ϣ���� {1}".Log(param.Name.ToString(), param.GetType().Name);
        }
    }

    private void OnSampleParamNotieHandler(INoticeBase<int> param)
    {
        Type type;
        string typeName = default;
        if (param is IParamNotice<int> intParamNotice)
        {
            type = intParamNotice.ParamValue.GetType();
            typeName = type.Name;
        }
        else if (param is IParamNotice<string> strParamNotice)
        {
            type = strParamNotice.ParamValue.GetType();
            typeName = type.Name;
        }
        else if (param is IParamNotice<float> floatParamNotice)
        {
            type = floatParamNotice.ParamValue.GetType();
            typeName = type.Name;
        }
        else { }
        "log:�㲥��Ϣ {0} ����Ӧ������Ϣ�㲥�Ĳ���ΪЯ��һ������ֵ����Ϣ���� {1}, ����ֵ���� {2}".Log(param.Name.ToString(), param.GetType().FullName, typeName);
    }
}

public class SampleParamValue
{
    public int fieldValue = 100;
}

