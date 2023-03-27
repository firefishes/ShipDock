using ShipDock;
using System;

public class NoticesSample : ShipDockAppComponent
{
    private NoticesObserver mObserver;

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //������Ϣ�㲥
        SampleConsts.N_SAMPLE_NOTICE_BY_DEFAULT.Add(OnSampleNotieHandler);
        SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.Add(OnSampleParamNotieHandler);

        //�۲�����Ϣ��ʹ��
        mObserver = new NoticesObserver();
        mObserver.AddListener(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER, OnEventHandler);

        // 3 ���㲥��Ϣ
        TimeUpdater.New(3f, () =>
        {
            //�㲥��Ϣ��ʹ��Ĭ����Ϣ�ࣩ
            SampleConsts.N_SAMPLE_NOTICE_BY_DEFAULT.Broadcast();

            //�㲥��Ϣ��ʹ�÷��Ͳ�����Ϣ�ࣩ
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<int>(1);
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<string>("ShipDock notice API");
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<float>(1.2f);

            //�㲥��Ϣ��ʹ�÷��Ͳ�����Ϣ�࣬��ֱ�Ӵ�����Ϣ��Ĳ���ֵ��
            SampleParamValue paramValue = new SampleParamValue()
            {
                fieldValue = 101,
            };
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<SampleParamValue>(paramValue);
        });
    }

    private void OnSampleNotieHandler(INoticeBase<int> param)
    {
        "log:��Ϣ {0} ����Ӧ������Ϣ�㲥�Ĳ���ΪĬ����Ϣ���� {1}".Log(param.Name.ToString(), param.GetType().Name);
        "log".Log(string.Empty);

        //�ɷ��۲�����Ϣ��ʹ��Ĭ����Ϣ�ࣩ
        mObserver.Dispatch(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER);

        //�ɷ��۲�����Ϣ��ʹ�÷��Ͳ�����Ϣ�ࣩ
        SampleParamValue paramValue = new SampleParamValue()
        {
            fieldValue = 102,
        };
        ParamNotice<SampleParamValue> notice = new ParamNotice<SampleParamValue>();
        notice.ParamValue = paramValue;
        mObserver.Dispatch(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER, notice);

        //�ɷ��۲�����Ϣ��ʹ�÷��Ͳ�����Ϣ�࣬��ֱ�Ӵ�����Ϣ��Ĳ���ֵ��
        mObserver.DispatchWithParam(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER, 123);
        mObserver.DispatchWithParam(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER, 1.5f);

        SampleParamValue paramValue2 = new SampleParamValue()
        {
            fieldValue = 103,
        };
        mObserver.DispatchWithParam(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER, paramValue2);
    }

    private void OnEventHandler(INoticeBase<int> param)
    {
        string logContent = default;
        if (param is IParamNotice<SampleParamValue> sampleParamNotice)
        {
            SampleParamValue paramValue = sampleParamNotice.ParamValue;
            "log:�۲�����Ϣ {0} ����Ӧ������Ϣ�Ĳ���ΪĬ����Ϣ���� {1}".Log(param.Name.ToString(), param.GetType().Name);
            "log:�۲�����Ϣ�еĲ���ֵ {0}".Log(paramValue.fieldValue.ToString());
            "log".Log(string.Empty);
        }
        else if (param is IParamNotice<int> intParamNotice)
        {
            logContent = intParamNotice.ParamValue.ToString();
        }
        else if (param is IParamNotice<string> strParamNotice)
        {
            logContent = strParamNotice.ParamValue;
        }
        else if (param is IParamNotice<float> floatParamNotice)
        {
            logContent = floatParamNotice.ParamValue.ToString();
        }
        else { }

        if (string.IsNullOrEmpty(logContent)) { }
        else
        {
            "log:�۲�����Ϣ {0} ����Ӧ������Ϣ�Ĳ���ΪЯ�����Ͳ�������Ϣ���� {1}".Log(param.Name.ToString(), param.GetType().Name);
            "log:�۲�����Ϣ�еĲ���ֵ {0}".Log(logContent);
            "log".Log(string.Empty);
        }
    }

    private void OnSampleParamNotieHandler(INoticeBase<int> param)
    {
        //���ݲ�ͬ��Ϣ�ദ��ҵ��
        Type type;
        string typeName = default;
        string logContent = default;
        if (param is IParamNotice<int> intParamNotice)
        {
            type = intParamNotice.ParamValue.GetType();
            typeName = type.Name;
            logContent = intParamNotice.ParamValue.ToString();
        }
        else if (param is IParamNotice<string> strParamNotice)
        {
            type = strParamNotice.ParamValue.GetType();
            typeName = type.Name;
            logContent = strParamNotice.ParamValue;
        }
        else if (param is IParamNotice<float> floatParamNotice)
        {
            type = floatParamNotice.ParamValue.GetType();
            typeName = type.Name;
            logContent = floatParamNotice.ParamValue.ToString();
        }
        else { }

        "log:�㲥��Ϣ {0} ����Ӧ������Ϣ�㲥�Ĳ���ΪЯ��һ������ֵ����Ϣ���� {1}, ����ֵ���� {2}".Log(param.Name.ToString(), param.GetType().FullName, typeName);

        if (string.IsNullOrEmpty(logContent)) { }
        else
        {
            "log:�㲥��Ϣ�еĲ���ֵ {0}".Log(logContent);
            "log".Log(string.Empty);
        }
    }
}

//�ڷ��Ͳ�����Ϣ����ʹ�õ��Զ�����
public class SampleParamValue
{
    public int fieldValue = 100;
}

