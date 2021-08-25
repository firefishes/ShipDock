#define _LOG_GRID_CREATED
#define _LOG_GRIDS_MARTRIX

using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elimlnate
{
    public class GridCreater
    {
#if LOG_GRIDS_MARTRIX
        public string gridsMartrixLog = string.Empty;
#endif

        private string mPosXTemp = string.Empty;
        private string mPosYTemp = string.Empty;
        private string mNameTemp = string.Empty;
        /// <summary>变换组件引用</summary>
        private Transform mBoardParent, mGridParent;
        /// <summary>新创建的消除格信息映射/summary>
        private Dictionary<int, GridCreateInfo> mGridCreatingInfo;
        /// <summary>消除游戏核心对象/summary>
        private ElimlnateCore mElimiGamePlay;

        public int GridCreatingSize
        {
            get
            {
                return mGridCreatingInfo != default ? mGridCreatingInfo.Count : 0;
            }
        }

        /// <summary>即将初始化的消除格数量</summary>
        public int InitedGridCount { get; private set; }
        /// <summary>消除格排布的起始坐标</summary>
        public Vector3 StartPos { get; private set; }
        /// <summary>操作面板的整体坐标偏移</summary>
        public Vector3 PosOffset { get; private set; }
        /// <summary>所有消除格的偏移</summary>
        public Vector3 GridsOffset { get; private set; }
        /// <summary>消除格入场特效的持续时间/summary>
        public float EnterEffectDuringTime { get; set; } = 0.2f;
        /// <summary>单元格下落的动画曲线/summary>
        public AnimationCurve EnterEffectCurve { get; set; }
        /// <summary>底板尺寸</summary>
        public Vector2 CellSize { get; set; }

        /// <summary>底板预设/summary>
        public GameObject TileResRaw { get; set; }
        /// <summary>新建消除格后的消息处理回调函数/summary>
        public Func<GameObject, int> BeforeGridCreate { get; set; }
        /// <summary>新建消除格后的消息处理回调函数/summary>
        public Action<ElimlnateGrid> CreateGridOperateUI { get; set; }
        /// <summary>消除格创建完成的回调函数/summary>
        public Action<int> GridCreateCompleted { get; set; }
        /// <summary>在创建消除格时读取单个消除格的类型/summary>
        public Func<int, int> GetGridTypeDuringCreate { get; set; }

        public GridCreater()
        {
            mElimiGamePlay = ElimlnateCore.Instance;
            mGridCreatingInfo = new Dictionary<int, GridCreateInfo>();
        }

        public void Clean()
        {
            var enumer = mGridCreatingInfo.GetEnumerator();
            int max = mGridCreatingInfo.Count;
            for (int i = 0; i < max; i++)
            {
                enumer.MoveNext();
                enumer.Current.Value.Clear();
            }
            mGridCreatingInfo.Clear();

            BeforeGridCreate = default;
            CreateGridOperateUI = default;
            GridCreateCompleted = default;
            GetGridTypeDuringCreate = default;

            EnterEffectCurve = default;
            mBoardParent = default;
            mGridParent = default;
            TileResRaw = default;
            mElimiGamePlay = default;
        }

        public void InitAndCreate(BoardGrids boardGrids, Vector3 gridsOffset)
        {
            GameObject boardParent = new GameObject("boardParent");
            mBoardParent = boardParent.transform;
            mBoardParent.SetParent(mElimiGamePlay.GridsContainer.transform);
            GameObject shapeParent = new GameObject("shapeParent");
            mGridParent = shapeParent.transform;
            mGridParent.SetParent(mElimiGamePlay.GridsContainer.transform);

            Transform trans = mElimiGamePlay.GridsContainer.transform;
            Vector3 pos = trans.position;
            StartPos = new Vector3(pos.x, pos.y, 0f);//消除格开始摆放的起点

            float halfToCenter = -0.5f;
            int colCount = boardGrids.ColumnSize;
            int rowCount = boardGrids.RowSize;
            InitedGridCount = rowCount * colCount;

            GridsOffset = gridsOffset;
            PosOffset = new Vector3(CellSize.x * colCount, CellSize.y * rowCount) * halfToCenter + GridsOffset;

            InitGrids(rowCount, colCount, ref boardGrids);
        }

        private void InitGrids(int rowCount, int colCount, ref BoardGrids boardGrids)
        {
            Vector2Int pos;
            Vector3 curPos;// tileOffset, ;
            GameObject newTile = default;
            Transform newTileTF = default;
            for (int row = rowCount - 1; row >= 0; row--)//预制体为非镜像的方式摆放，故使用倒序遍历
            {
                for (int col = 0; col < colCount; col++)
                {
                    //tileOffset = new Vector3(CellSize.x * col, CellSize.y * row, 0f);
                    //curPos = StartPos + PosOffset + tileOffset;
                    pos = new Vector2Int(col, row);
                    curPos = GetGridPosWithRowCol(pos);

                    if (TileResRaw != default)
                    {
                        newTile = UnityEngine.Object.Instantiate(TileResRaw, curPos, Quaternion.identity);
                        newTileTF = newTile.transform;
                        newTileTF.SetParent(mBoardParent);
                    }
                    else { }

                    boardGrids.CreateGrid(col, row, newTile);

                    int rowValue = rowCount - row - 1;//需要倒序遍历，使用差运算
                    int index = (rowValue * colCount) + col;
                    int gridType = GetGridTypeDuringCreate != default ? GetGridTypeDuringCreate.Invoke(index) : mElimiGamePlay.GridTypes.NormalGridType;
                    CreateGrid(pos, curPos, out GridCreateInfo info, string.Empty, true, gridType);
                }
            }
        }

        /// <summary>创建可供消除的单元格</summary>
        public void CreateGrid(Vector2Int gridRowColPos, Vector3 targetPos, out GridCreateInfo info, string enterEffectName, bool isDirectShow = false, int gridType = 0)
        {
            info = GetNewGridInfo(gridType, targetPos, enterEffectName, isDirectShow);
            int column = gridRowColPos.x;
            int row = gridRowColPos.y;
#if LOG_GRID_CREATED
            "error: GridCrateInfo is null, pos = {0}, {1}".Log(info == default, column.ToString(), row.ToString());
#endif
            info.row = row;
            info.column = column;
            info.AddCreatedCallback(OnGridInied);

            mPosXTemp = column.ToString();
            mPosYTemp = row.ToString();
            mNameTemp = info.name;

            info.name = mNameTemp.Append(mPosXTemp, mPosYTemp);
            GridCreateCompleted?.Invoke(info.id);
        }

        private void OnGridInied(ElimlnateGrid grid, GridCreateInfo info)
        {
            InitedGridCount--;
            if (InitedGridCount <= 0)
            {
                InitedGridCount = 0;
                "log".Log("Grids Inied...");
                bool hasEnterAnim = !info.isDirectShow;
                if (hasEnterAnim)
                {
                    ElimlnateGrid item;
                    int max = mElimiGamePlay.BoardGrids.GridCount;
                    for (int i = 0; i < max; i++)
                    {
                        item = mElimiGamePlay.BoardGrids.GetGridByIndex(i);
                        item.StartEffect(GameEffects.EffectInited);
                    }
                }
                else { }

#if LOG_GRIDS_MARTRIX
                Debug.Log(gridsMartrixLog);
#endif
            }
            else { }
        }

        /// <summary>
        /// 获取消除格的创建信息
        /// </summary>
        /// <param name="gridType">类型</param>
        /// <param name="targetPos">位置</param>
        /// <param name="enterEffectName">是否需要展示出场动画</param>
        /// <param name="isDirectShow">是否跳过消除格的初始化特效直接显示</param>
        private GridCreateInfo GetNewGridInfo(int gridType, Vector3 targetPos, string enterEffectName, bool isDirectShow = true)
        {
            int gridTypeValue = gridType;
            bool willSetShapeIndex = gridType < 0;//类型值小于 0 则将定制具体的图形索引
            GridType gridPool = mElimiGamePlay.GridTypes.GetGridPool(gridType);
            gridType = gridPool != default ? gridPool.mainGridType : ElimlnateCore.GetNormalGridType();

            GameObject ins = gridPool.creater.Invoke();
            Transform trans = ins.transform;
            trans.position = targetPos;
            trans.rotation = Quaternion.identity;
            trans.SetParent(mGridParent);

            int noticeName;
            GridCreateInfo info = new GridCreateInfo
            {
                isDirectShow = isDirectShow,
                enterEffectName = enterEffectName,
                customShapeID = willSetShapeIndex ? Mathf.Abs(gridTypeValue) : 0,//这里取整有可能超出图形库的长度
            };
            if (BeforeGridCreate != default)
            {
                noticeName = BeforeGridCreate(ins);
                noticeName.Add(OnNewGridCreated);
#if LOG_GRID_CREATE_NOTICE
                "Grid created, id is {0}".Log(id.ToString());
#endif
            }
            else
            {
                noticeName = ins.GetInstanceID();
            }
            info.id = noticeName;
            mGridCreatingInfo[noticeName] = info;
            return info;
        }

        private void OnNewGridCreated(INoticeBase<int> param)
        {
            int noticeName = param.Name;
            noticeName.Remove(OnNewGridCreated);

            IParamNotice<ElimlnateGrid> notice = param as IParamNotice<ElimlnateGrid>;
            ElimlnateGrid grid = notice.ParamValue;

            GridCreateInfo info = mGridCreatingInfo[noticeName];
            int col = info.column, row = info.row;
#if LOG_GRID_CREATED
            "OnNewGridCreated, pos = {0},{1}".Log(col.ToString(), row.ToString());
#endif

            grid.SetGridPos(info.column, info.row);
            grid.GridTrans.SetParent(mGridParent);
            grid.SetColliderSize(CellSize);
            grid.SetCreateInfo(info);
            grid.AddDestroyCallback((g) => 
            {
                info.Clear();
                mGridCreatingInfo.Remove(noticeName);
            });

            string name = grid.GridTrans.name;
            grid.GridTrans.name = name.Append("@[", row.ToString(), "_", col.ToString(), "] #", grid.GridShapeIndex.ToString());//Sample: @[行，列] #图案索引

            if (!mElimiGamePlay.BoardGrids.HasEnoughGrids())
            {
                CreateGridOperateUI?.Invoke(grid);
            }
            else { }
            mElimiGamePlay.BoardGrids.SetGridMapper(new Vector2Int(col, row), grid, false);

            info.GridCommit(grid);
        }

        /// <summary>
        /// 从全局的消除类型库中随机选择一种类型
        /// </summary>
        /// <returns></returns>
        public virtual int GetNextCreatedGridType()
        {
            return mElimiGamePlay.GridTypes.GetRandomGrid();
        }

        public Vector3 GetGridPosWithRowCol(int col, int row)
        {
            Vector3 tileOffset = new Vector3(CellSize.x * col, CellSize.y * row);
            Vector3 result = StartPos + PosOffset + tileOffset;
            return result;
        }

        public Vector3 GetGridPosWithRowCol(Vector2Int pos)
        {
            return GetGridPosWithRowCol(pos.x, pos.y);
        }

        public void FillGridByRandom(out ElimlnateGrid result, Func<ElimlnateGrid, bool> randomAginFilters = default)
        {
            BoardGrids boardGrids = mElimiGamePlay.BoardGrids;
            int index = Utils.UnityRangeRandom(0, boardGrids.GridsSize);
            result = boardGrids.GetGridByIndex(index);

            bool flag = randomAginFilters != default ? randomAginFilters.Invoke(result) : true;

            if ((result == default) || result.IsDestroyed || flag)
            {
                FillGridByRandom(out result, randomAginFilters);
            }
            else { }
        }

        public bool ReplaceGird(int gridTypeWillCreate, Action<ElimlnateGrid, GridCreateInfo> gridCreated = default)
        {
            FillGridByRandom(out ElimlnateGrid grid);
            bool result = ReplaceGird(grid.GridPos.x, grid.GridPos.y, gridTypeWillCreate, gridCreated);
            return result;
        }

        /// <summary>
        /// 使用其他类型的消除格替换棋盘上现有的消除格
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="gridTypeWillCreate"></param>
        /// <param name="gridCreated"></param>
        /// <returns></returns>
        public bool ReplaceGird(int col, int row, int gridTypeWillCreate, Action<ElimlnateGrid, GridCreateInfo> gridCreated = default)
        {
            ElimlnateGrid grid = mElimiGamePlay.BoardGrids.GetBoardGrid(new Vector2(col, row));

            grid.WillDestroy();

            Vector3 targetPos = GetGridPosWithRowCol(col, row);
            CreateGrid(new Vector2Int(col, row), targetPos, out GridCreateInfo info, string.Empty, true, gridTypeWillCreate);
            if (gridCreated != default)
            {
                info.AddCreatedCallback(gridCreated);//在这里的回调函数重新定义图形索引
            }
            else { }

            //mElimiGamePlay.LineInputer.CancelAllInput();
            return true;
        }
    }
}
