using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.UI;

/// <summary>
/// 
/// ���Ʋ�
/// 
/// ������ʾ��������������ݣ����������ݱ仯֪ͨ��ʾ�����
/// 
/// </summary>
public class UIWindowModular : UIModular<UIWindow>
{
    public const int UIM_WINDOW_PIC_START_SHOW = 1;
    public const int UIM_WINDOW_PIC_RESHOW = 2;
    public const int UIM_WINDOW_PIC_RESHOW_FINISH = 3;

    public override string Name { get; protected set; } = SampleConsts.U_SAMPLE;
    public override string UIAssetName { get; protected set; } = SampleConsts.U_SAMPLE;
    public override string ABName { get; } = SampleConsts.AB_SAMPLES;
    public override int UILayer { get; protected set; } = UILayerType.WINDOW;
    public override int[] DataProxyLinks { get; set; } = new int[] { SampleConsts.D_SAMPLE_MVC };

    private int mReshowWaiting;
    private SampleMVCData mMVCData;

    protected override void Purge()
    {
        mMVCData = default;
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Enter()
    {
        base.Enter();

        mReshowWaiting = 0;

        mMVCData = SampleConsts.D_SAMPLE_MVC.GetData<SampleMVCData>();
    }

    /// <summary>
    /// ��������Ӧ�����޸ķ������Ϣ��������ֻ������ʾ��ķ���
    /// </summary>
    /// <param name="data"></param>
    /// <param name="keyName"></param>
    public override void OnDataProxyNotify(IDataProxy data, int keyName)
    {
        if (data is SampleMVCData mvcData)
        {
            switch (keyName)
            {
                case SampleConsts.DN_SAMPLE_MVC_DATA_CHANGED:
                    //���ݸ��º�֪ͨ UI ˢ�£�ִ�к�������
                    UI.ShowByData(mvcData.CurrentData);
                    break;
            }
        }
        else { }
    }

    /// <summary>
    /// ������UI�����������Ϣ��������ֻ�������ݲ�ķ���
    /// </summary>
    /// <param name="param"></param>
    protected override void UIModularHandler(INoticeBase<int> param)
    {
        switch (param.Name)
        {
            case UIM_WINDOW_PIC_START_SHOW:
            case UIM_WINDOW_PIC_RESHOW_FINISH:
                //��ʼ������ɶ��ж������Ƿ���Ҫ����
                mReshowWaiting--;
                if (mReshowWaiting <= 0)
                {
                    mReshowWaiting = 0;
                    mMVCData.CurrentDataAdavance();
                }
                else { }
                break;

            case UIM_WINDOW_PIC_RESHOW:
                //��UI�ı仯��Ҫִ��
                mReshowWaiting++;
                break;
        }
    }
}
