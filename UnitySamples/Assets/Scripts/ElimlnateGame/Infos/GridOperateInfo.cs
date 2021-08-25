using UnityEngine;

namespace Elimlnate
{
    public class GridOperateInfo
    {
        /// <summary>消除格的UI层操作类型：滑动中进入</summary>
        public const int GRID_OPERATE_TYPE_ENTER = 0;
        /// <summary>消除格的UI层操作类型：按下</summary>
        public const int GRID_OPERATE_TYPE_POINTER_DOWN = 1;
        /// <summary>消除格的UI层操作类型：抬起</summary>
        public const int GRID_OPERATE_TYPE_POINTER_UP = 2;
        /// <summary>消除格的UI层操作类型：点击</summary>
        public const int GRID_OPERATE_TYPE_POINTER_CLICK = 3;
        /// <summary>消除格的UI层操作类型：滑动中退出</summary>
        public const int GRID_OPERATE_TYPE_POINTER_EXIT = 4;

        public int prefabID;
        public int gridIndex;
        public GameObject prefab;
        public Vector2Int gridPos;
        public Vector3 localPos;
    }
}
