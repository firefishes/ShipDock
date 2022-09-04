using ShipDock.Datas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public interface IServiceData : IDataProxy
    {
        int IDAdvanced { get; }

        void SyncIDAdvanced(int value);
    }

    public class ServiceData : DataProxy, IServiceData
    {
        public int IDAdvanced { get; private set; }

        public ServiceData() : base(Consts.D_SERVICE)
        {

        }

        public void SyncIDAdvanced(int value)
        {
            IDAdvanced = value;
        }
    }
}
