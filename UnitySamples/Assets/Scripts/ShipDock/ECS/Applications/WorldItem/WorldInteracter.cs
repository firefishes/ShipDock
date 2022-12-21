using ShipDock.ECS;
using ShipDock.Notices;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    //public class WorldInteracter : LogicData, INotificationSender
    //{
    //    public static void Init<T>(int componentName, WorldInteracter item, int entitas, GameObject gameObject, IWorldIntercatable target = default) where T : StaticWorldComponent
    //    {
    //        ILogicContext context = ShipDockECS.Instance.Context;
    //        ILogicEntitas allEntitas = context.AllEntitas;

    //        T comp = allEntitas.GetComponentFromEntitas<T>(entitas, componentName);
    //        item = (WorldInteracter)comp.GetEntitasData(entitas);
    //        item.worldItemID = gameObject.GetInstanceID();

    //        if (target != default)
    //        {
    //            item.Add(target.WorldItemHandler);
    //            item.WorldItemDispose += target.WorldItemDispose;
    //        }
    //        else { }

    //        item.DataValid();
    //    }

    //    public Action WorldItemDispose { get; set; }

    //    public int worldItemID;
    //    public int groupID;
    //    public int aroundID;
    //    public bool isDroped;
    //}
}