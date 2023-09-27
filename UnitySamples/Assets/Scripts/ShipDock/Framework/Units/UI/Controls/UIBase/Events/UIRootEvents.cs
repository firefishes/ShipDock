using System;
using UnityEngine.Events;

namespace ShipDock
{
    [Serializable]
    public class OnUIRootAwaked : UnityEvent<IUIRoot> { };
}