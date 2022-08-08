using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public class PeaceMovement : WorldMovement, IPoolable
    {
        public void Revert()
        {
        }

        public void ToPool()
        {
            Pooling<PeaceMovement>.To(this);
        }
    }

    public class PeaceMovementComp : DataComponent<PeaceMovement>
    {
        protected override PeaceMovement CreateData()
        {
            return Pooling<PeaceMovement>.From();
        }
    }
}
