using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class IsKingMovement : WorldMovement, IPoolable
    {
        public void Revert()
        {
        }

        public void ToPool()
        {
            Pooling<IsKingMovement>.To(this);
        }
    }

    public class IsKingMovementComp : DataComponent<IsKingMovement>
    {
        protected override IsKingMovement CreateData()
        {
            return Pooling<IsKingMovement>.From();
        }
    }

}