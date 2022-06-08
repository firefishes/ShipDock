using ShipDock.Datas;

/// <summary>
/// 玩家账户数据
/// </summary>
public class SampleData : DataProxy
{
    private int mValue;

    public int ActorHp
    {
        set
        {
            mValue = value;
            DataNotify(SampleConsts.DN_SAMPLE_DATA_NOTIFY);
        }
        get
        {
            return mValue;
        }
    }

    public SampleData() : base(SampleConsts.D_SAMPLE)
    {
    }

    public void SomeDataChange(int value)
    {
        mValue = value;
        DataNotify(SampleConsts.DN_SAMPLE_DATA_NOTIFY);
    }
}