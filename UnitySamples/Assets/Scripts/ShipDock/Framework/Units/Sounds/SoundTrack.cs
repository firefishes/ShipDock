using System;
using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// 音轨
    /// </summary>
    public class SoundTrack
    {
        public float clipLength;
        public string soundInfoName;
        public AudioSource source;
        public Action onPlayCompleted;
    }
}
