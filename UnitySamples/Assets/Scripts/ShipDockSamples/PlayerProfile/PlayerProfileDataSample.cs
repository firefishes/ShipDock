using ShipDock;
/// <summary>
/// ShipDock 案例: 玩家账户数据
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