using ShipDock;
/// <summary>
/// ShipDock ����: ����˻�����
/// </summary>
public class PlayerProfileDataSample : ShipDockAppComponent
{
    public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
    {
        base.GetDataProxyHandler(param);

        param.ParamValue = new IDataProxy[]
        {
            new SamplePlayerData(),
        };

    }
}