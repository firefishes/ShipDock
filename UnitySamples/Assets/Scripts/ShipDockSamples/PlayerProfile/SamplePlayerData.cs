using ShipDock;

/// <summary>
/// ����˻�����
/// </summary>
public class SamplePlayerData : DataProxy
{
    private SampleLocalClient mLocalClient;

    public SamplePlayerData() : base(SampleConsts.D_PLAYER)
    {
    }
}