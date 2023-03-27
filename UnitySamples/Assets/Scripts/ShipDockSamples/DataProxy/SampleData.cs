using ShipDock;
/// <summary>
/// ����˻�����
/// </summary>
public class SampleData : DataProxy
{
    private int mValue;
    private int mMissionIndex;

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

    public int MissionIndex
    {
        set
        {
            mMissionIndex = value;
        }
        get
        {
            return mMissionIndex;
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