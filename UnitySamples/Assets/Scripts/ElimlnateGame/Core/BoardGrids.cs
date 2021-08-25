#define _LOG_GRIDS_GRAPHIC
#define _LOG_GRID_ACCESSIBLE

using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elimlnate
{
    public class BoardGrids
    {

        //private class GridDirty
        //{
        //    private int[] mGrids;
        //    private List<int> mGridIDChanged;
        //    private List<int> mGridIndexChanged;

        //    private Func<int, ElimlnateGrid> OnGetGridByID { get; set; }

        //    public void Init(ref ElimlnateGrid[] grids, Func<int, ElimlnateGrid> onGetGridByID)
        //    {
        //        OnGetGridByID = onGetGridByID;
        //        mGridIDChanged = new List<int>();
        //        mGridIndexChanged = new List<int>();

        //        int max = grids.Length;
        //        mGrids = new int[max];
        //        for (int i = 0; i < max; i++)
        //        {
        //            mGrids[i] = grids[i].GridID;
        //        }
        //    }

        //    public ElimlnateGrid GetGrid(int index)
        //    {
        //        ElimlnateGrid result = mGrids[index];
        //        bool hasChanged = mGridIDChanged.Contains(index);
        //        if (hasChanged)
        //        {
        //            result = 
        //        }
        //        if (index)
        //        {

        //        }
        //        return mGridID;
        //    }

        //    public void Remove(ref ElimlnateGrid grid)
        //    {
        //    }

        //    public void SetGridByIndex(int index, ref ElimlnateGrid grid)
        //    {
        //        int id = mGrids[index];
        //        if (!mGridIDChanged.Contains(id))
        //        {
        //            mGridIDChanged.Add(id);
        //        }
        //        else { }

        //        if (!mGridIndexChanged.Contains(index))
        //        {
        //            mGridIndexChanged.Add(index);
        //        }
        //        else { }

        //        mGrids[index] = grid.GridID;
        //    }
        //}

        /// <summary>当前所有的消除格列表</summary>
        private ElimlnateGrid[] mGrids;
        /// <summary>当前所有消除格列表的脏数据</summary>
        //private GridDirty mGridsDirty;
        /// <summary>当前所有的消除格游戏对象列表</summary>
        private GameObject[] mTileItems;
        /// <summary>消除格的映射</summary>
        private Dictionary<int, ElimlnateGrid> mGridMapper;
        /// <summary>无向图保存消除格的连通性</summary>
        private KeyValueList<int, List<int>> mGridsGraphic;
        /// <summary>无向图保存消除格的连通性</summary>
        private KeyValueList<int, List<int>> mGridsEmpty;

        /// <summary>消除格列数/summary>
        public int ColumnSize { get; private set; }
        /// <summary>消除格行数/summary>
        public int RowSize { get; private set; }
        /// <summary>消除格总数/summary>
        public int GridsSize { get; private set; }
        /// <summary>消除操作结束的回调函数/summary>
        public Action<ElimlnateGrid> AfterGridsDeactived { get; set; }
        /// <summary>消除格重排布的回调函数/summary>
        public Action<List<GridRearrangerInfo>> OnRearrangerGrids { get; set; }
        /// <summary>所有消除格被重置为不可用时的回调函数/summary>
        public Action<ElimlnateGrid> DuringGridDeactvie { get; set; }

        public int GridCount
        {
            get
            {
                return mGrids != default ? mGrids.Length : 0;
            }
        }

        public BoardGrids(int col, int row)
        {
            ColumnSize = col;
            RowSize = row;
            GridsSize = col * row;

            int size = col * row;
            mGrids = new ElimlnateGrid[size];
            //mGridsDirty = new GridDirty();
            mTileItems = new GameObject[size];
            mGridMapper = new Dictionary<int, ElimlnateGrid>();
            mGridsEmpty = new KeyValueList<int, List<int>>();
        }

        public void Clean()
        {
            Array.Clear(mGrids, 0, mGrids.Length);
            Array.Clear(mTileItems, 0, mTileItems.Length);

            int max = mGridMapper.Count;
            var enumer = mGridMapper.GetEnumerator();
            for (int i = 0; i < max; i++)
            {
                enumer.MoveNext();
                enumer.Current.Value.Dispose();
            }
            mGridMapper?.Clear();
            mGridsGraphic?.Clear();

            OnRearrangerGrids = default;
            AfterGridsDeactived = default;
        }

        public List<int> GetEmptyCountInCol(int col)
        {
            return mGridsEmpty.ContainsKey(col) ? mGridsEmpty[col] : new List<int>();
        }

        public int GetGridIndex(int col, int row)
        {
            return row * ColumnSize + col;
        }

        public GameObject GetTileItem(int column, int row)
        {
            int index = GetGridIndex(column, row);
            return mTileItems[index];
        }

        public void CreateGrid(int col, int row, GameObject newTile)
        {
            int index = GetGridIndex(col, row);
            mTileItems[index] = newTile;
        }

        public ElimlnateGrid GetGridByIndex(int index)
        {
            return mGrids[index];
        }

        private void SetGridByIndex(int index, ElimlnateGrid grid, bool isDirty = false)
        {
            if (isDirty)
            {
                //mGridsDirty.SetGridByIndex(index, ref grid);
            }
            else
            {
                mGrids[index] = grid;
            }
        }

        public GameObject GetTileByIndex(int index)
        {
            return mTileItems[index];
        }

        /// <summary>
        /// 获取图形
        /// </summary>
        /// <param name="pos">坐标</param>
        /// <returns></returns>
        public ElimlnateGrid GetBoardGrid(Vector2 pos, bool isGetDirty = false)
        {
            int column = (int)pos.x;
            int row = (int)pos.y;

            ElimlnateGrid result = default;

            if ((column >= 0) && (row >= 0) && (column < ColumnSize) && (row < RowSize))
            {
                int index = GetGridIndex(column, row);
                if(isGetDirty)
                {
                    //result = index < GridsSize ? mGridsDirty.GetGrid(index) : default;
                }
                else
                {
                    result = index < GridsSize ? mGrids[index] : default;
                }
            }
            else { }

            return result;
        }

        /// <summary>
        /// 全部图形初始化
        /// </summary>
        public void ResetAllGridsToDeactive(bool invokeDeactiveEnd = false)
        {
            ElimlnateGrid grid;
            int max = mGrids.Length;
            for (int i = 0; i < max; i++)
            {
                grid = mGrids[i];
                if ((grid != default) && !grid.IsDestroyed)
                {
                    grid.IsActive = false;

                    DuringGridDeactvie?.Invoke(grid);

                    if (invokeDeactiveEnd)
                    {
                        AfterGridsDeactived?.Invoke(grid);
                    }
                    else { }
                }
                else { }
            }
        }

        public ElimlnateGrid GetGridByRowColumn(int col, int row)
        {
            ElimlnateGrid grid = GetBoardGrid(new Vector2(col, row));
            return grid;
        }

        public ElimlnateGrid GetGridFromMapper(int id)
        {
            "error".Log(!mGridMapper.ContainsKey(id), "Grid mapper error");
            return mGridMapper[id];
        }

        private void CheckGridMapper(ref ElimlnateGrid grid)
        {
            if (grid != default)
            {
                int id = grid.GridID;
                if (!mGridMapper.ContainsKey(id))
                {
                    mGridMapper[id] = grid;
                }
                else { }
            }
            else { }
        }

        public bool HasEnoughGrids()
        {
            return mGridMapper != default ? mGridMapper.Count >= GridsSize : false;
        }

        public void UpdateDirty()
        {
            //mGridsDirty.Init(ref mGrids, GetGridFromMapper);
        }

        /// <summary>
        /// 设置消除格行列数据映射
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="grid"></param>
        public void SetGridMapper(Vector2Int pos, ElimlnateGrid grid, bool isDelete, bool isDirty = false)
        {
            int column = pos.x;
            int row = pos.y;
            if ((column >= 0) && (row >= 0) && (column < ColumnSize) && (row < RowSize))
            {
                ElimlnateGrid temp = isDelete ? default : grid;
                if (!isDirty)
                {
                    CheckGridMapper(ref temp);
                }
                else { }

                int index = GetGridIndex(column, row);
                SetGridByIndex(index, temp, isDirty);

                if (isDirty)
                {
                    if (isDelete)
                    {
                        //mGridsDirty.Remove(ref grid);
                    }
                    else { }
                }
                else
                {
                    if (isDelete)
                    {
                        List<int> edges = default;
                        List<int> comparingEdges = default;
                        ResetGridEdges(ref grid, ref edges, ref comparingEdges);
                        mGridsGraphic.Remove(grid.GridID);
                    }
                    else
                    {
                        CheckGridContinuity(ref grid, index);
                    }
                }
            }
            else { }
        }

        private void CheckGridContinuity(ref ElimlnateGrid grid, int gridIndex)
        {
            if (mGridsGraphic == default)
            {
                mGridsGraphic = new KeyValueList<int, List<int>>();
            }
            else { }

            List<int> edges = default;
            List<int> comparingEdges = default;
            ElimlnateGrid comparing = default;

            ResetGridEdges(ref grid, ref edges, ref comparingEdges);
            GraphicCreate(grid.GridID, ref edges);

            if (ElimlnateCore.Instance.GridOperater.IsInclined)
            {
                CheckGridGraphicEdge(gridIndex - ColumnSize - 1, ref grid, ref comparing, ref edges);
                CheckGridGraphicEdge(gridIndex - ColumnSize + 1, ref grid, ref comparing, ref edges);
                CheckGridGraphicEdge(gridIndex + ColumnSize - 1, ref grid, ref comparing, ref edges);
                CheckGridGraphicEdge(gridIndex + ColumnSize + 1, ref grid, ref comparing, ref edges);
            }
            else { }

            CheckGridGraphicEdge(gridIndex - ColumnSize, ref grid, ref comparing, ref edges);
            CheckGridGraphicEdge(gridIndex + ColumnSize, ref grid, ref comparing, ref edges);
            CheckGridGraphicEdge(gridIndex - 1, ref grid, ref comparing, ref edges);
            CheckGridGraphicEdge(gridIndex + 1, ref grid, ref comparing, ref edges);

#if LOG_GRIDS_GRAPHIC
            LogGridGraphic();
#endif

        }

        public void FillAroundsByGridPos(Vector2Int pos, ref List<int> list)
        {
            if (list == default)
            {
                list = new List<int>();
            }
            else { }

            int column = pos.x;
            int row = pos.y;

            int gridIndex = GetGridIndex(column, row), index;

            if (ElimlnateCore.Instance.GridOperater.IsInclined)
            {
                index = gridIndex - ColumnSize - 1;
                if (IsValidGridIndex(index))
                {
                    list.Add(index);
                }
                else { }
                index = gridIndex - ColumnSize + 1;
                if (IsValidGridIndex(index))
                {
                    list.Add(index);
                }
                else { }
                index = gridIndex + ColumnSize - 1;
                if (IsValidGridIndex(index))
                {
                    list.Add(index);
                }
                else { }
                index = gridIndex + ColumnSize + 1;
                if (IsValidGridIndex(index))
                {
                    list.Add(index);
                }
                else { }
            }
            else { }
            index = gridIndex - ColumnSize;
            if (IsValidGridIndex(index))
            {
                list.Add(index);
            }
            else { }
            index = gridIndex + ColumnSize;
            if (IsValidGridIndex(index))
            {
                list.Add(index);
            }
            else { }
            index = gridIndex - 1;
            if (IsValidGridIndex(index))
            {
                list.Add(index);
            }
            else { }
            index = gridIndex + 1;
            if (IsValidGridIndex(index))
            {
                list.Add(index);
            }
            else { }
        }

        private void GraphicCreate(int id, ref List<int> edges)
        {
            if (mGridsGraphic.ContainsKey(id))
            {
                edges = mGridsGraphic[id];
            }
            else
            {
                edges = new List<int>();
                mGridsGraphic[id] = edges;
            }
        }

        private void CheckGridGraphicEdge(int index, ref ElimlnateGrid grid, ref ElimlnateGrid comparing, ref List<int> edges)
        {
            if (IsValidGridIndex(index))
            {
                comparing = GetGridByIndex(index);
                if (comparing != default)
                {
                    UpdateGridsAsseccible(ref grid, ref comparing, ref edges);
                }
                else { }
            }
            else { }
        }

        private void UpdateGridsAsseccible(ref ElimlnateGrid grid, ref ElimlnateGrid comparing, ref List<int> edges)
        {
            int comparingID = comparing.GridID;

            List<int> edgesComparing = default;
            GraphicCreate(comparingID, ref edgesComparing);

            int id = grid.GridID;

            bool isAccessible = comparing.ShouldGridContinuity(ref grid);

#if LOG_GRID_ACCESSIBLE
            "log:{0} 可达 {1}".Log(isAccessible, grid.GridTrans.name, comparing.GridTrans.name);
#endif

            if (isAccessible)
            {
                if (!edges.Contains(comparingID))
                {
                    edges.Add(comparingID);//满足连通条件，设置为可达
                }
                else { }

                if (!edgesComparing.Contains(id))
                {
                    edgesComparing.Add(id);//更新可达点的连通信息
                }
                else { }
            }
            else
            {
                if (edges.Contains(comparingID))
                {
                    edges.Remove(comparingID);//不满足连通条件，设置为不可达
                }
                else { }

                if (edgesComparing.Contains(id))
                {
                    edgesComparing.Remove(id);//更新不可达点的连通信息
                }
                else { }
            }
        }

        public void ResetGridEdges(ref ElimlnateGrid grid, ref List<int> edges, ref List<int> edgesComparinig)
        {
            int item;
            int id = grid.GridID;
            if (mGridsGraphic.ContainsKey(id))
            {
                edges = mGridsGraphic[id];
                int max = edges.Count;
                if (max > 0)
                {
                    for (int i = 0; i < max; i++)
                    {
                        item = edges[i];
                        edgesComparinig = mGridsGraphic[item];
                        edgesComparinig?.Remove(id);
                    }
                    edges.Clear();
                }
            }
            else { }
        }

        public bool IsValidGridIndex(int index)
        {
            return index >= 0 && index < GridsSize;
        }

#if LOG_GRIDS_GRAPHIC
        public void LogGridGraphic()
        {
            string log = string.Empty;
            int max = mGridsGraphic.Keys.Count;
            for (int i = RowSize - 1; i >= 0; i--)
            {
                for (int j = 0; j < ColumnSize; j++)
                {
                    ElimlnateGrid grid = GetGridByRowColumn(j, i);
                    if (grid != default && grid.GridType != 0 && !grid.IsObstacle)
                    {
                        List<int> list = mGridsGraphic[grid.GridID];
                        int n = list.Count;
                        if (n > 0)
                        {
                            for (int m = 0; m < n; m++)
                            {
                                int id = list[m];
                                ElimlnateGrid temp = GetGridFromMapper(id);
                                if (!temp.IsDestroyed)
                                {
                                    log = log.Append(string.Format("{0} 可达 {1}", grid.GridTrans.name, temp.GridTrans.name), "\r\n");
                                }
                            }
                        }
                    }
                    else { }
                }
            }
            Debug.Log(log + "\r\n");
        }
#endif

        private List<GridRearrangerInfo> mGridRearrangerInfos;

        public bool AutoRemovable(out List<ElimlnateGrid> result)
        {
            bool flag = true;
            result = new List<ElimlnateGrid>();

            if (mGridRearrangerInfos == default)
            {
                mGridRearrangerInfos = new List<GridRearrangerInfo>();
            }
            else
            {
                mGridRearrangerInfos.Clear();
            }

            int id;
            ElimlnateGrid grid;
            List<int> list;
            List<int> willRearranger = new List<int>();
            List<List<int>> values = mGridsGraphic.Values;
            int max = values.Count;
            int comboLineMax = ElimlnateCore.Instance.GridOperater.ShouldComboLineMax;
            for (int i = 0; i < max; i++)
            {
                list = values[i];
                if (IsNeedComboVertexs(ref list, comboLineMax - 1, ref result))
                {
                    id = mGridsGraphic.Keys[i];
                    grid = GetGridFromMapper(id);
                    result.Add(grid);
                    break;
                }
                else
                {
                    int n = list.Count;
                    for (int j = 0; j < n; j++)
                    {
                        id = list[j];
                        willRearranger.Add(id);
                    }
                }
            }

            if (result.Count >= comboLineMax)
            {
                max = result.Count;
                bool needRearrangerAgin = false;
                for (int i = 0; i < max; i++)
                {
                    grid = result[i];
                    if (grid.IsDestroyed)
                    {
                        needRearrangerAgin = true;
                        break;
                    }
                    else { }
                }
                if (needRearrangerAgin)
                {
                    flag = false;
                    Rearrangeer();
                }
                else
                {
                    //Debug.LogError("有得消！！！" + result.Count);
                }
            }
            else
            {
                flag = false;
                Rearrangeer();
            }
            return flag;
        }

        private bool IsNeedComboVertexs(ref List<int> list, int comboLineMax, ref List<ElimlnateGrid> grids)
        {
            bool result = list.Count >= comboLineMax;
            if (list.Count >= comboLineMax)
            {
                int id;
                int max = list.Count;
                for (int i = 0; i < max; i++)
                {
                    id = list[i];
                    grids.Add(GetGridFromMapper(id));
                }
            }
            else { }
            return result;
        }

        public void Rearrangeer()
        {
            ElimlnateGrid grid, rearranger;
            List<int> willRearranger = new List<int>();
            int max = GridsSize;
            for (int i = 0; i < max; i++)
            {
                grid = GetGridByIndex(i);
                if (grid != default && grid.ShouldGridContinuity(ref grid))
                {
                    willRearranger.Add(grid.GridID);
                }
                else { }
            }

            int total = willRearranger.Count;
            max = (int)(total * 0.5f);
            int remaints = total - max;

            int id;
            List<int> rearrangered = new List<int>();
            for (int i = 0; i < max; i++)
            {
                id = willRearranger[i];
                grid = GetGridFromMapper(id);

                int index = Utils.UnityRangeRandom(remaints, total);
                id = willRearranger[index];

                if (rearrangered.Contains(id))
                {
                    continue;
                }
                else
                {
                    rearrangered.Add(id);

                    rearranger = GetGridFromMapper(id);

                    //Debug.Log(grid.GridTrans.name + " ->" + rearranger.GridTrans.name);

                    mGridRearrangerInfos.Add(new GridRearrangerInfo()
                    {
                        grid = grid,
                        newGridPos = rearranger.GridPos,
                        moveEnd = rearranger.GridTrans.position
                    });
                    mGridRearrangerInfos.Add(new GridRearrangerInfo()
                    {
                        grid = rearranger,
                        newGridPos = grid.GridPos,
                        moveEnd = grid.GridTrans.position
                    });
                }
            }
            if (OnRearrangerGrids != default)
            {
                OnRearrangerGrids.Invoke(mGridRearrangerInfos);
            }
            else
            {
                ElimlnateCore.Instance.ActiveInput();
            }
            mGridRearrangerInfos.Clear();
        }
    }

    public class GridRearrangerInfo
    {
        public Vector3 moveEnd;
        public Vector2Int newGridPos;
        public ElimlnateGrid grid;

        public void UpdateGridData()
        {
            grid.SetGridPos(newGridPos);
            ElimlnateCore.Instance.BoardGrids.SetGridMapper(newGridPos, grid, false);
        }
    }
}
