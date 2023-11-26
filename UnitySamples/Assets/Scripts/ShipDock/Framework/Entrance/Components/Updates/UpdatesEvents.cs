using System;
using UnityEngine.Events;

namespace ShipDock
{
    internal class UpdatesEvent : UnityEvent<int, IUpdate> { }
    internal class SyncUpdateEvent : UnityEvent<int, Action<float>> { }
}