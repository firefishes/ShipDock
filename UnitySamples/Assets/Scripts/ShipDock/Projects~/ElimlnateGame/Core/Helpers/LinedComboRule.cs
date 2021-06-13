using UnityEngine;

namespace Elimlnate
{
    public class LinedComboRule : ComboRule
    {
        public LinedComboRule()
        {
            linedCombo = 5;
            ruleName = "普通连消道具";
        }

        public override void CreateSkillGrid(ref ElimlnateGrid rootGrid)
        {
            CreateDyeShape(ref rootGrid);
        }

        private void CreateDyeShape(ref ElimlnateGrid rootGrid)
        {
            Vector2 pos = rootGrid.GridPos;
            int column = (int)pos.x;
            int row = (int)pos.y;

            GameObject target = ElimlnateCore.Instance.BoardGrids.GetTileItem(column, row);
            Transform tf = target.transform;
            Vector3 posTF = tf.position;
            Vector2Int v = new Vector2Int(column, row);
            //ElimlnateCore.Instance.GridCreater.CreateGrid(v, rootGrid.GridTrans.position, out GridCreateInfo info, string.Empty, true, GridType.Dye);
            GridCreateInfo info = default;
            info.gridInRawPos = rootGrid;
            //info.OnGridCreated = GridCreated;
        }

        private void GridCreated(ElimlnateGrid target, GridCreateInfo info)
        {
            //(target as DyeGridHotFix).dyeIndex = info.gridInRawPos.shapeIndex;
        }
    }
}
