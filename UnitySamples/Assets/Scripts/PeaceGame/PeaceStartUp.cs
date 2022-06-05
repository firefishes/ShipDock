using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public class PeaceStartUp : ShipDockAppComponent
    {
        public override void ApplicationCloseHandler()
        {
            base.ApplicationCloseHandler();
        }

        public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
        {
            base.GetDataProxyHandler(param);

            param.ParamValue = new IDataProxy[]
            {
                new PlayerData(),
            };
        }

        public override void EnterGameHandler()
        {
            base.EnterGameHandler();


        }
    }
}