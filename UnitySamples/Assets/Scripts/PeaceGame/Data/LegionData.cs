using ShipDock.Datas;
using ShipDock.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public class LegionData : DataProxy
    {
        private Legion mPlayerLegion;
        private KeyValueList<int, Legion> mNeutralLegions;

        public LegionData(int dataName) : base(dataName)
        {
            mNeutralLegions = new KeyValueList<int, Legion>();

            LegionData legionData = Consts.D_LEGION.GetData<LegionData>();
        }

        public void InitPlayerLegion()
        {
            if (mPlayerLegion == default)
            {
                mPlayerLegion = new Legion();
            }
            else { }

            mPlayerLegion.Init();
        }
    }
}
