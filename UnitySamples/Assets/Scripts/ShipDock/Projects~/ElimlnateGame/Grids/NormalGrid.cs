#define _LOG_GRIDS_MARTRIX


namespace Elimlnate
{
    public class NormalGrid : ElimlnateGrid
    {
        public override void SetCreateInfo(GridCreateInfo info)
        {
            base.SetCreateInfo(info);

            if (info.customShapeID > 0)
            {
                int index = info.customShapeID - 1;
                ChangeSprite(index);
            }
            else
            {
                SetRandomShapeIndex();
            }

#if LOG_GRIDS_MARTRIX
            mCore.GridCreater.gridsMartrixLog = mCore.GridCreater.gridsMartrixLog.Append(GridSprite.sprite.name.Replace("grid_", ""), ",");
#endif
        }

        public override bool ShouldGridContinuity(ref ElimlnateGrid grid)
        {
            bool result = false;

            if (GridType != 0 && !IsObstacle)
            {
                result = grid != default &&
                    !grid.IsObstacle && //非障碍
                    grid.GridType != 0 && //非空消除格
                    grid.GridShapeIndex == GridShapeIndex;//图形索引一致
            }
            else { }

            return result;
        }
    }
}
