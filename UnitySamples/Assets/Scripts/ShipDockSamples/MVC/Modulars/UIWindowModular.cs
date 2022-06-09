using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowModular : UIModular<UIWindow>
{
    public const int UIM_WINDOW_PIC_RESHOW = 1;
    public const int UIM_WINDOW_PIC_RESHOW_FINISH = 2;

    public override string Name { get; protected set; } = SampleConsts.U_SAMPLE;
    public override string UIAssetName { get; protected set; } = SampleConsts.U_SAMPLE;
    public override string ABName { get; } = SampleConsts.AB_SAMPLES;
    public override int UILayer { get; protected set; } = UILayerType.WINDOW;
    public override int[] DataProxyLinks { get; set; } = new int[] { SampleConsts.D_SAMPLE_MODEL };

    protected override void Purge()
    {
    }

    public override void Init()
    {
        base.Init();

    }

    public override void OnDataProxyNotify(IDataProxy data, int keyName)
    {
    }

    protected override void UIModularHandler(INoticeBase<int> param)
    {
        switch (param.Name)
        {
            case UIM_WINDOW_PIC_RESHOW_FINISH:
                Debug.Log("UIModularHandler");
                break;
        }
    }
}
