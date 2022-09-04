using ShipDock.Datas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public class UIData : DataProxy
    {
        public string NextUIModular { get; private set; }

        public UIData() : base(Consts.D_UI)
        {
        }

        public void ActiveNextUIModular(string modularName)
        {
            NextUIModular = modularName;
            DataNotify(Consts.DN_NEXT_UI_MODULAR);
        }
    }

}