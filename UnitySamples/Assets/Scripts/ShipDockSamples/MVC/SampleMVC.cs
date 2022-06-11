using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleMVC : ShipDockAppComponent
{
    private UIFromResource mUI;

    public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
    {
        base.GetDataProxyHandler(param);

        param.ParamValue = new IDataProxy[]
        {
            new SampleMVCData(),
        };
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        UIManager UIMgr = ShipDockApp.Instance.UIs;
        mUI = UIMgr.OpenResourceUI<UIFromResource>("ResourceUISample");

        TimeUpdater.New(3f, () =>
        {
            UIMgr.CloseResourceUI("ResourceUISample");

            UIMgr.Open<UIWindowModular>(SampleConsts.U_SAMPLE);
        });
    }
}
