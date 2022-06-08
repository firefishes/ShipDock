using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;

public class DataProxySample : ShipDockAppComponent
{
    public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
    {
        base.GetDataProxyHandler(param);

        //定义数据代理
        param.ParamValue = new IDataProxy[]
        {
            new SampleData(),
        };
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //获取数据代理并添加数据侦听器
        SampleData data = SampleConsts.D_SAMPLE.GetData<SampleData>();
        data.AddDataProxyNotify(OnDataNotifyHandler);

        //修改数据
        data.SomeDataChange(100);
        data.ActorHp = 1000;
    }

    private void OnDataNotifyHandler(IDataProxy data, int notyfyName)
    {
        if (data is SampleData sampleData)
        {
            switch (notyfyName)
            {
                case SampleConsts.DN_SAMPLE_DATA_NOTIFY:
                    //数据变更后收到数据变更消息
                    int valueAfterChanged = sampleData.ActorHp;
                    "log".Log("数据已变更: ".Append(valueAfterChanged.ToString()));
                    break;
            }
        }
        else { }
    }
}
