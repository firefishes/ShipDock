using Elimlnate;
using ShipDock.Notices;
using ShipDock.Pooling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Game
{
    [Serializable]
    public class GridItem
    {
        public int id;
        public string abName;
        public string resName;
        public string aniName;
    }

    public class GridComponent : MonoBehaviour
    {
        [SerializeField]
        private bool m_ShouldLine;
        [SerializeField]
        private bool m_IsObstacle;
        [SerializeField]
        private int m_GridTypeValue = 0;
        [SerializeField]
        private SpriteRenderer m_Grid;
        [SerializeField]
        private SpriteRenderer m_GridBg;
        [SerializeField]
        private Vector3 m_GridScale = Vector3.one;
        [SerializeField]
        private Vector3 m_GridLocalPosOffset;
        [SerializeField]
        private BoxCollider m_GridCollider;
        [SerializeField]
        private LineRenderer m_GridLineRenderer;
        [SerializeField]
        private List<GridItem> m_GridItems = new List<GridItem>();

        public NormalGrid Grid { get; private set; }

        private void OnDestroy()
        {
            Grid = default;
        }

        public virtual void Init()
        {
            Grid = CreateGrid();
            Grid.OnGetGridShapeAssets = GetGridAssets;

            Grid.GridID = transform.GetInstanceID();
            Grid.ShouldLine = m_ShouldLine;
            Grid.GridTrans = transform;
            Grid.GridBg = m_GridBg;
            Grid.IsObstacle = m_IsObstacle;

            Grid.GridType = m_GridTypeValue;
            Grid.GridSprite = m_Grid;
            Grid.GridLocalScale = m_GridScale;
            Grid.GridLocalPosition = m_GridLocalPosOffset;
            Grid.GridCollider = m_GridCollider;
            Grid.LineRenderer = m_GridLineRenderer;

            GetGridAssets(Grid, Grid.GridType);

            IParamNotice<ElimlnateGrid> notice = Pooling<ParamNotice<ElimlnateGrid>>.From();
            notice.ParamValue = Grid;

            int noticeName = GetInstanceID();
            noticeName.Broadcast(notice);
            notice.ToPool();
        }

        protected virtual void GetGridAssets(ElimlnateGrid grid, int gridType)
        {
            GridTypes gridTypes = ElimlnateCore.Instance.GridTypes;

            string key = string.Empty;
            int max = m_GridItems.Count, id;

            GridItem item;
            GridShapeType[] result = new GridShapeType[max];
            for (int i = 0; i < max; i++)
            {
                item = m_GridItems[i];
                id = item.id;
                result[i] = new GridShapeType()
                {
                    abName = item.abName,
                    assetName = item.resName,
                    aniName = item.aniName,
                };
            }
#if LOG_GRID_SHAPE_RES_NULL
            "log:Grid asset names length is {0}".Log(result.Length.ToString());
#endif
            grid.GridShapeTypes = result;
        }

        protected virtual NormalGrid CreateGrid()
        {
            return new NormalGrid();
        }
    }

}