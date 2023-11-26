using System;
using UnityEngine.Events;

namespace ShipDock
{
    [Serializable]
    public class UIRootAwaked : UnityEvent<IUIRoot> { };
}