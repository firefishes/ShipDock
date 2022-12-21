using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.ECS
{
    public class WorldMovementSystem : LogicSystem
    {
        public int WorldComponentName { get; set; }

        public override void Execute(int entitas, int componentName, ILogicData data)
        {
            if (componentName == WorldComponentName)
            {

            }
            else { }
        }
    }
}
