using ShipDock.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class IsKingWorldSystem : SystemComponent
    {
        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

        }
    }

}