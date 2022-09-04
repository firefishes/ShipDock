using ShipDock.Datas;
using ShipDock.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public class TroopsData : DataProxy
    {
        private KeyValueList<int, TroopFields> mPlayerTroops;

        public TroopsData() : base(Consts.D_TROOPS)
        {
            mPlayerTroops = new KeyValueList<int, TroopFields>();
        }
    }
}
