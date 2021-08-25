#if !ILRUNTIME
using ShipDock.Notices;
#endif

namespace Elimlnate
{
#if ILRUNTIME
    public class EliminateData
#else
    public class EliminateData : NoticesObserver
#endif
    {
        public const int N_UPDATE_PLAY_CORE_INPUT_ENABLED = 0;
        public const int N_CREATE_GRID_OPERATE_UI = 1;

        private ElimlnateCore mElimiGamePlay;

        /// <summary>正在操作的消除格引用</summary>
        public ElimlnateGrid OperatingGrid { get; set; }

        public EliminateData(ElimlnateCore gamePlay)
        {
            mElimiGamePlay = gamePlay;
        }

        /// <summary>
        /// 设置正在被操作的消除格
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="operateType"></param>
        public void SetOperatingGrid(int operateType)
        {
            LineInputCore inputCore = mElimiGamePlay.LineInputer;
            switch (operateType)
            {
                case GridOperateInfo.GRID_OPERATE_TYPE_POINTER_DOWN:
                    inputCore.InputGridPointerDown(OperatingGrid);
                    break;
                case GridOperateInfo.GRID_OPERATE_TYPE_ENTER:
                    inputCore.InputPointerEnter(OperatingGrid);
                    break;
                case GridOperateInfo.GRID_OPERATE_TYPE_POINTER_EXIT:
                    inputCore.InputPointerExit(OperatingGrid);
                    break;
                case GridOperateInfo.GRID_OPERATE_TYPE_POINTER_UP:
                    inputCore.InputGridPointerUp(OperatingGrid);
                    break;
                case GridOperateInfo.GRID_OPERATE_TYPE_POINTER_CLICK:
                    inputCore.InputGridPointerClick(OperatingGrid);
                    break;
            }
            OperatingGrid = default;
        }
    }
}
