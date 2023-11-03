using ShipDock;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : ECSComponent<Movement>
{

}

public class MovementSystem : ECSSystem
{
    public override int SystemID { get; } = 1;

    public override void Execute()
    {
        ECS.Instance.Search<Movement>(this, OnSearchedResult);
    }

    protected void OnSearchedResult(int componentID, int entity)
    {
        if (componentID == Consts.TENON_TYPE_MOVEMENT)
        {
            Movement movement = GetComponentDataByEntity<Movement>(componentID, entity, out bool isValid);
            if (isValid)
            {
                Debug.Log(entity);
            }
            else { }
        }
        else { }
    }
}
