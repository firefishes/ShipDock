using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;

public class DataProxySample : ShipDockAppComponent
{
    public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
    {
        base.GetDataProxyHandler(param);

        //�������ݴ���
        param.ParamValue = new IDataProxy[]
        {
            new SampleData(),
        };
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //��ȡ���ݴ����������������
        SampleData data = SampleConsts.D_SAMPLE.GetData<SampleData>();
        data.AddDataProxyNotify(OnDataNotifyHandler);

        //�޸�����
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
                    //���ݱ�����յ����ݱ����Ϣ
                    int valueAfterChanged = sampleData.ActorHp;
                    "log".Log("�����ѱ��: ".Append(valueAfterChanged.ToString()));
                    break;
            }
        }
        else { }
    }
}
