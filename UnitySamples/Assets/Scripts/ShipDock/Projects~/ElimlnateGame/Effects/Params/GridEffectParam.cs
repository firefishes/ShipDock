using System;
using UnityEngine;

namespace Elimlnate
{
    /// <summary>
    /// 消除格特效参数
    /// </summary>
    public class GridEffectParam
    {
        public int GridIndex { get; set; }
        public bool IsInited { get; set; }
        public float UpdateTime { get; set; }
        public float DuringTime { get; set; }
        public Action Callabck { get; set; }
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public Vector3 Speed { get; set; }
        public AnimationCurve Curve { get; set; }

        public void Clean()
        {
            Curve = default;
        }

        public float GetUpdatingTime()
        {
            return UpdateTime;
        }

        public void SetUpdatingTime(float time)
        {
            UpdateTime = time;
        }
    }
}
