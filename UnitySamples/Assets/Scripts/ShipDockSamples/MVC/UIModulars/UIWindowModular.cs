using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.UI;

/// <summary>
/// 
/// 控制层
/// 
/// 根据显示层的输入设置数据，并根据数据变化通知显示层更新
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
    /// 仅做与响应数据修改方面的消息处理，并且只调用显示层的方法
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
                    //数据更新后通知 UI 刷新，执行后续流程
                    UI.ShowByData(mvcData.CurrentData);
                    break;
            }
        }
        else { }
    }

    /// <summary>
    /// 仅做与UI交互方面的消息处理，并且只调用数据层的方法
    /// </summary>
    /// <param name="param"></param>
    protected override void UIModularHandler(INoticeBase<int> param)
    {
        switch (param.Name)
        {
            case UIM_WINDOW_PIC_START_SHOW:
            case UIM_WINDOW_PIC_RESHOW_FINISH:
                //初始化和完成都判断数据是否需要更新
                mReshowWaiting--;
                if (mReshowWaiting <= 0)
                {
                    mReshowWaiting = 0;
                    mMVCData.CurrentDataAdavance();
                }
                else { }
                break;

            case UIM_WINDOW_PIC_RESHOW:
                //有UI的变化需要执行
                mReshowWaiting++;
                break;
        }
    }
}
