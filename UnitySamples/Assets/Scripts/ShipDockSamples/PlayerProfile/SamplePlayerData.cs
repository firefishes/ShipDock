using ShipDock;

/// <summary>
/// 玩家账户数据
/// </summary>
public class SamplePlayerData : DataProxy
{
    private SampleLocalClient mLocalClient;

    public SamplePlayerData() : base(SampleConsts.D_PLAYER)
    {
    }
}