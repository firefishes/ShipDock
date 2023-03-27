using ShipDock;
using System;

public class NoticesSample : ShipDockAppComponent
{
    private NoticesObserver mObserver;

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //侦听消息广播
        SampleConsts.N_SAMPLE_NOTICE_BY_DEFAULT.Add(OnSampleNotieHandler);
        SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.Add(OnSampleParamNotieHandler);

        //观察者消息的使用
        mObserver = new NoticesObserver();
        mObserver.AddListener(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER, OnEventHandler);

        // 3 秒后广播消息
        TimeUpdater.New(3f, () =>
        {
            //广播消息（使用默认消息类）
            SampleConsts.N_SAMPLE_NOTICE_BY_DEFAULT.Broadcast();

            //广播消息（使用泛型参数消息类）
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<int>(1);
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<string>("ShipDock notice API");
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<float>(1.2f);

            //广播消息（使用泛型参数消息类，并直接传入消息类的参数值）
            SampleParamValue paramValue = new SampleParamValue()
            {
                fieldValue = 101,
            };
            SampleConsts.N_SAMPLE_NOTICE_BY_PARAM.BroadcastWithParam<SampleParamValue>(paramValue);
        });
    }

    private void OnSampleNotieHandler(INoticeBase<int> param)
    {
        "log:消息 {0} 已响应。此消息广播的参数为默认消息类型 {1}".Log(param.Name.ToString(), param.GetType().Name);
        "log".Log(string.Empty);

        //派发观察者消息（使用默认消息类）
        mObserver.Dispatch(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER);

        //派发观察者消息（使用泛型参数消息类）
        SampleParamValue paramValue = new SampleParamValue()
        {
            fieldValue = 102,
        };
        ParamNotice<SampleParamValue> notice = new ParamNotice<SampleParamValue>();
        notice.ParamValue = paramValue;
        mObserver.Dispatch(SampleConsts.N_SAMPLE_NOTICE_BY_OBSERVER, notice);

        //派发观察者消息（使用泛型参数消息类，并直接传入消息类的参数值）
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
            "log:观察者消息 {0} 已响应。此消息的参数为默认消息类型 {1}".Log(param.Name.ToString(), param.GetType().Name);
            "log:观察者消息中的参数值 {0}".Log(paramValue.fieldValue.ToString());
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
            "log:观察者消息 {0} 已响应。此消息的参数为携带泛型参数的消息类型 {1}".Log(param.Name.ToString(), param.GetType().Name);
            "log:观察者消息中的参数值 {0}".Log(logContent);
            "log".Log(string.Empty);
        }
    }

    private void OnSampleParamNotieHandler(INoticeBase<int> param)
    {
        //根据不同消息类处理业务
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

        "log:广播消息 {0} 已响应。此消息广播的参数为携带一个泛型值的消息类型 {1}, 参数值类型 {2}".Log(param.Name.ToString(), param.GetType().FullName, typeName);

        if (string.IsNullOrEmpty(logContent)) { }
        else
        {
            "log:广播消息中的参数值 {0}".Log(logContent);
            "log".Log(string.Empty);
        }
    }
}

//在泛型参数消息类中使用的自定义类
public class SampleParamValue
{
    public int fieldValue = 100;
}

