using ShipDock;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTenon : ECSComponent<Movement>
{

}

public class MovementSystem : ECSSystem
{
    public override int SystemID { get; } = 1;

    public override void Execute()
    {
        ChunkGroup<Movement>.Instance.TraverseAll(DuringExecute);
    }

    protected override void DuringExecute<T>(T data)
    {
        base.DuringExecute(data);


    }
}
