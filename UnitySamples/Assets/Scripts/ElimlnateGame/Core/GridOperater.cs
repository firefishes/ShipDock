#define _LOG_LINED_RULES
#define _LOG_SUPPLYEMENT

using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elimlnate
{
    /// <summary>
    /// 三消玩法消除格操作器
    /// </summary>
    public class GridOperater
    {
        public const int PROCESS_PHASE_IDLE = 0;
        public const int PROCESS_PHASE_ELIMLNATE = 1;
        public const int PROCESS_PHASE_SUPP = 2;
        public const int PROCESS_PHASE_TIDY = 3;

        /// <summary>已连线的消除格个数/summary>
        private int mGridLinedCount;
        /// <summary>需要销毁的消除格个数/summary>
        private float mRearrangerTime;
        /// <summary>上一个操作的消除格/summary>
        private ElimlnateGrid mPreRemovingGrid;
        /// <summary>当前操作的消除格/summary>
        private ElimlnateGrid mRemovingGrid;
        /// <summary>正在操作中的消除格列表/summary>
        private List<ElimlnateGrid> mOperatingGrids;
        /// <summary>连消规则映射/summary>
        private Dictionary<int, ComboRule> mLinedRulesMapper;
        /// <summary>消除游戏核心对象/summary>
        private ElimlnateCore mCore;
        /// <summary>棋盘中消除格的数据/summary>
        private BoardGrids mBoardGrids;
        /// <summary>消除格重组器/summary>
        private GridsRearranger mRearranger;
        /// <summary>带技能的消除格的</summary>
        public ElimlnateGrid mLastRemovingGrid;
        /// <summary>达成特殊消除的条件/summary>
        private ComboRule[] mLineRules;

        /// <summary>达成特殊消除的条件/summary>
        private List<int> mObstacleIndes;
        /// <summary>临时存储/summary>
        private ElimlnateGrid mGridTemp;
        /// <summary>每列需要补充的消除格数量/summary>
        private KeyValueList<int, int> mSupplementSizeData;
        /// <summary>每列消除格为空的最高行数/summary>
        private KeyValueList<int, int> mHigestInCols;
        /// <summary>每列的障碍地形映射/summary>
        private KeyValueList<int, int> mObstacleCols;
        /// <summary>需要调整位置的消除格队列/summary>
        private Queue<GridUpdateInfo> mUpdateCols;
        /// <summary>消除格流程处理器/summary>
        private QueueExecuter mGridProcesser;
        /// <summary>当前在执行的消除格补充执行器/summary>
        GridSupplementExecuter mSupplmentExecuter = new GridSupplementExecuter();

        private ElimlnateCore GamePlay
        {
            get
            {
                if (mCore == default)
                {
                    mCore = ElimlnateCore.Instance;
                }
                else { }
                return mCore;
            }
        }

        public ComboRule[] LinedRules
        {
            get
            {
                return mLineRules;
            }
            set
            {
                mLineRules = value;

                int max = value.Length;
                for (int i = 0; i < max; i++)
                {
                    int combo = value[i].linedCombo;
#if LOG_LINED_RULES
                    "log: Lined rules added, linedCombo = {0}".Log(combo.ToString());
#endif
                    mLinedRulesMapper[combo] = value[i];
                }
            }
        }

        /// <summary>是否正在处理消除逻辑</summary>
        public bool IsProcessing
        {
            get
            {
                return mGridProcesser.QueueSize > 0;
            }
        }

        protected BoardGrids BoardGrids
        {
            get
            {
                if (mBoardGrids == default)
                {
                    mBoardGrids = GamePlay.BoardGrids;
                }
                else { }

                return mBoardGrids;
            }
        }

        /// <summary>是否在流程结束后重置为可输入</summary>
        public bool ActiveInputAfterProcess { get; set; } = true;
        /// <summary>是否可以斜着消</summary>
        public bool IsInclined { get; set; }
        /// <summary>是否可以斜着消</summary>
        public bool CancelAutoRemovable { get; set; }
        /// <summary>可连续消除的最小单元格数</summary>
        public int ShouldComboLineMax { get; set; } = 3;
        /// <summary>可显示连接的单元格数</summary>
        public int ShouldLineMin { get; set; } = 2;
        /// <summary>消除效果（目前只是吸引消除格的效果，可进一步抽象）</summary>
        public BatchEffect ElimlnateEffect { get; set; }
        /// <summary>消除特效是否已初始化</summary>
        public bool IsElimlnateEffectInited { get; private set; }
        /// <summary>当前的处理阶段</summary>
        public int ProcessPhase { get; private set; } = PROCESS_PHASE_IDLE;
        /// <summary>消除结果事件</summary>
        public EliminateResult EliminateResult { get; private set; }

        public float RearrangerTotalTime { get; set; } = 0.3f;

        public GridOperater(EliminateResult eliminateResult)
        {
            EliminateResult = eliminateResult;
            mLinedRulesMapper = new Dictionary<int, ComboRule>();
            mRearranger = new GridsRearranger();

            mOperatingGrids = new List<ElimlnateGrid>();//TODO 这里保存的自动消除的功能数据需要再看看如何实现

            mSupplementSizeData = new KeyValueList<int, int>();
            mHigestInCols = new KeyValueList<int, int>();
            mObstacleCols = new KeyValueList<int, int>();
            mUpdateCols = new Queue<GridUpdateInfo>();
            mObstacleIndes = new List<int>();

            mGridProcesser = new QueueExecuter(false);
            mGridProcesser.OnUnitCompleted += OnGridProcessComplete;
        }

        public void Clean()
        {
            mGridProcesser?.Dispose();
            EliminateResult?.Dispose();
            mLinedRulesMapper?.Clear();
            mRearranger?.Clear();
            mOperatingGrids?.Clear();

            mGridProcesser = default;
            EliminateResult = default;
            mLinedRulesMapper = default;
            mRearranger = default;
            mOperatingGrids = default;
        }

        private void OnGridProcessComplete(IQueueExecuter param)
        {
            if (ActiveInputAfterProcess)
            {
                GamePlay.ActiveInput();
            }
            else { }

            CheckAutoRemovable();
        }

        /// <summary>
        /// 本次操作结束
        /// </summary>
        /// <param name="willRemoveGrids"></param>
        public bool LineEnd(List<ElimlnateGrid> willRemoveGrids, bool willSupplementGrids = true)
        {
            CancelAutoRemovable = false;

            if (!IsElimlnateEffectInited)
            {
                IsElimlnateEffectInited = true;
                ElimlnateEffect.Init();
            }
            else { }

            if (willRemoveGrids != default)
            {
                if (willRemoveGrids.Count > 0)
                {
                    mGridProcesser.Add(GamePlay.DeactiveInput);
                    mGridProcesser.Add(new GridElimlnateExecuter(this, mCore.BoardGrids, ref willRemoveGrids));

                    if (willSupplementGrids)
                    {
                        //mGridProcesser.Add(CheckGridComboRules);
                        mGridProcesser.Add(SupplementGrids);
                        mGridProcesser.Add(new GridSupplementExecuter());
                        mGridProcesser.Commit();
                    }
                    else { }
                }
                else { }
            }
            else { }

            return IsProcessing;
        }

        public void ClearGridProcesser()
        {
            mGridProcesser?.Reset();
        }

        public void AddGridsProcess(IQueueExecuter process)
        {
            mGridProcesser.Add(process);
            mGridProcesser.Commit();
        }

        public void AddGridsProcessCompleted(QueueUnitCompleted completion)
        {
            if (completion != default)
            {
                mGridProcesser.OnUnitCompleted += completion;
            }
            else { }
        }

        public void RemoveGridsProcessCompleted(QueueUnitCompleted completion)
        {
            if (completion != default)
            {
                mGridProcesser.OnUnitCompleted -= completion;
            }
            else { }
        }

        public void SetGridElimlnateCounts(GridElimlnateExecuter executer)
        {
            mLastRemovingGrid = executer.GetLastRemovingGrid();
            mGridLinedCount += executer.GridCount;
        }

        /// <summary>
        /// 检查是否还有可消除的图形
        /// </summary>
        public bool CheckAutoRemovable()
        {
            //mRearrangerTime = RearrangerTotalTime;
            return mCore.BoardGrids.AutoRemovable(out _);
        }

        /// <summary>
        /// 获取可以被选中的图形
        /// </summary>
        /// <param name="gridPos">单元格坐标</param>
        public List<ElimlnateGrid> CollectAllowOperateGrids(Vector2 gridPos)
        {
            GamePlay.BoardGrids.ResetAllGridsToDeactive();

            List<ElimlnateGrid> result = new List<ElimlnateGrid>();
            Vector2 left = new Vector2(gridPos.x - 1, gridPos.y);
            Vector2 right = new Vector2(gridPos.x + 1, gridPos.y);
            Vector2 up = new Vector2(gridPos.x, gridPos.y + 1);
            Vector2 down = new Vector2(gridPos.x, gridPos.y - 1);

            ElimlnateGrid allowComboGrid = default;
            AddAllowOperateGrid(ref allowComboGrid, left, ref result);
            AddAllowOperateGrid(ref allowComboGrid, right, ref result);
            AddAllowOperateGrid(ref allowComboGrid, up, ref result);
            AddAllowOperateGrid(ref allowComboGrid, down, ref result);

            if (IsInclined)
            {
                Vector2 upLeft = new Vector2(gridPos.x - 1, gridPos.y + 1);
                Vector2 upRight = new Vector2(gridPos.x + 1, gridPos.y + 1);
                Vector2 downLeft = new Vector2(gridPos.x - 1, gridPos.y - 1);
                Vector2 downRight = new Vector2(gridPos.x + 1, gridPos.y - 1);

                AddAllowOperateGrid(ref allowComboGrid, upLeft, ref result);
                AddAllowOperateGrid(ref allowComboGrid, upRight, ref result);
                AddAllowOperateGrid(ref allowComboGrid, downLeft, ref result);
                AddAllowOperateGrid(ref allowComboGrid, downRight, ref result);
            }
            else { }
            return result;
        }

        private void AddAllowOperateGrid(ref ElimlnateGrid allowComboGrid, Vector2 gridPos, ref List<ElimlnateGrid> allowOperates)
        {
            allowComboGrid = GamePlay.BoardGrids.GetBoardGrid(gridPos);
            if (allowComboGrid != default)
            {
                allowOperates.Add(allowComboGrid);
                allowComboGrid.IsActive = true;
            }
            else { }
        }

        /// <summary>
        /// 提示当前可消除的一组图形
        /// </summary>
        public void HintRemoveShape()
        {
            if (GamePlay.ShouldInput)
            {
                //curCanShape列表保存当前可消除的一组图形,可用作提示
                for (int i = 0; i < mOperatingGrids.Count; i++)
                {
                    //curCanShape[i].SetColor(Color.red);
                }
            }
            else { }
        }

        /// <summary>
        /// 检测连消规则
        /// </summary>
        private void CheckGridComboRules()
        {
            ComboRule rule, validRule = default, preRule = default;
            for (int i = 0; i < mGridLinedCount; i++)
            {
                rule = mLinedRulesMapper.ContainsKey(i) ? mLinedRulesMapper[i] : default;
                bool isRuleEmpty = validRule == default;
                if ((rule == default) && isRuleEmpty)
                {
                    validRule = rule;//首次设置生效的连消规则
                }
                else
                {
                    if (isRuleEmpty || ((rule != default) && rule.ShouldCoverRule(ref preRule)))
                    {
                        validRule = rule;//连消规则覆盖
                    }
                    else { }
                }
                preRule = rule;
            }
            ElimlnateGrid skillGridRoot = mLastRemovingGrid;
            validRule?.CreateSkillGrid(ref skillGridRoot);//连消规则启用
        }

        /// <summary>
        /// 补充消除格
        /// </summary>
        public void SupplementGrids()
        {
            CheckDataInit();
            InitSupplementsData();
            //UpdateSupplementGrids();
            UpdateRemainsGrid();

            mObstacleCols.Clear();
            mUpdateCols.Clear();
            mSupplementSizeData.Clear();
            mObstacleIndes.Clear();
            mGridTemp = default;
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        private void CheckDataInit()
        {
            if (mHigestInCols != default)
            {
                mHigestInCols.Clear();
            }
            else { }

            GameEffects effects = GamePlay.GridEffects;
            if (effects.OnGridCreateAndEnters == default)
            {
                effects.OnGridCreateAndEnters += OnGridCreateAndEnters;
            }
            else { }
        }

        private void OnGridCreateAndEnters()
        {
            int state = GamePlay.GridEffects.EffectCheckState;
            switch (state)
            {
                case GameEffects.EFFECT_CHECK_STATE_SUPP:
                    //case GameEffects.EFFECT_CHECK_STATE_TIDY:
                    FillOtherEmptyGrids();
                    break;
            }
        }

        private void FillOtherEmptyGrids()
        {
            BoardGrids boardGrids = GamePlay.BoardGrids;
            int colCount = boardGrids.ColumnSize;
            int rowCount = boardGrids.RowSize;

            int max = colCount;
            ElimlnateGrid highterLeft, highterRight, willMove;
            for (int i = 0; i < max; i++)
            {
                int col = i;
                if (mHigestInCols.ContainsKey(col))
                {
                    int fromCol = 0, fromRow = 0;
                    int lacking = mHigestInCols[col];
                    //Debug.Log("列" + col + ", 缺少：" + lacking);
                    willMove = default;
                    if (lacking > 0)
                    {
                        for (int j = 0; j < rowCount; j++)
                        {
                            mGridTemp = boardGrids.GetGridByRowColumn(col, j);
                            if (mGridTemp == default)
                            {
                                fromRow = j + 1;
                                int nearLeft = col - 1;
                                int nearRight = col + 1;
                                highterLeft = boardGrids.GetGridByRowColumn(nearLeft, fromRow);
                                highterRight = boardGrids.GetGridByRowColumn(nearRight, fromRow);

                                if (highterLeft != default)
                                {
                                    willMove = highterLeft;
                                    fromCol = nearLeft;
                                }
                                else
                                {
                                    willMove = highterRight;
                                    fromCol = nearRight;
                                }

                                if (willMove != default && !willMove.IsObstacle)
                                {
                                    mUpdateCols.Enqueue(new GridUpdateInfo()
                                    {
                                        grid = willMove,
                                        targetPos = new Vector2Int(col, j),
                                    });
                                    boardGrids.SetGridMapper(willMove.GridPos, willMove, true);

                                }
                                else { }
                            }
                            else { }
                        }
                    }
                    else { }
                }
                else { }
            }
            mHigestInCols.Clear();

            if (HasUpdateCols())
            {
                SupplementGrids();

            }
            else { }

            if (!HasUpdateCols())
            {
                GamePlay.GridEffects.SetEffectState(GameEffects.EFFECT_CHECK_STATE_IDLE);
            }
            else { }
        }

        private bool HasUpdateCols()
        {
            return mUpdateCols.Count > 0;
        }

        /// <summary>
        /// 初始化需要补充的消除格映射数据
        /// </summary>
        private void InitSupplementsData()
        {
            BoardGrids boardGrids = GamePlay.BoardGrids;
            int colCount = boardGrids.ColumnSize;
            int rowCount = boardGrids.RowSize;

            int max = colCount * rowCount;
            for (int i = 0; i < max; i++)
            {
                int row = Mathf.CeilToInt(i / colCount);
                int col = i % rowCount;

                ResetColsEmptyData(col);

                mGridTemp = boardGrids.GetBoardGrid(new Vector2Int(col, row));
                if (mGridTemp == default)
                {
#if LOG_SUPPLYEMENT
                    "log:Supplement grid x={0},y={1}".Log(col.ToString(), row.ToString());
#endif
                    mSupplementSizeData[col]++;
                    mHigestInCols[col]++;
                }
                else
                {
                    CheckAndResetGridCells(ref boardGrids, ref mGridTemp, col, row);//检测障碍地形对消除格补充数量的影响，并将需要移动到新位置的消除格加入更新队列
                }
            }
        }

        private int mMoveToNextLeftOrRight = 0;

        /// <summary>
        /// 检测并创建需要填充的消除格信息
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void CheckAndResetGridCells(ref BoardGrids boardGrids, ref ElimlnateGrid grid, int col, int row)
        {
            if (grid.IsObstacle)
            {
                mSupplementSizeData[col] = 0;//遇到障碍地形，重置此列需要补充的消除格个数

                int index = boardGrids.GetGridIndex(col, row);
                if (!mObstacleIndes.Contains(index))
                {
                    mObstacleIndes.Add(index);//记录障碍地形的消除格索引
                }
                else { }
            }
            else
            {
                if (mSupplementSizeData.ContainsKey(col))
                {
                    int count = mSupplementSizeData[col];
                    if (count > 0)
                    {
                        Vector2Int endPos = new Vector2Int(col, row - count);
                        mUpdateCols.Enqueue(new GridUpdateInfo()
                        {
                            grid = grid,
                            targetPos = endPos,
                        });
                        boardGrids.SetGridMapper(grid.GridPos, grid, true);//将当前消除格的数据置空，为消除格调整到新位置做准备
                    }
                    else { }
                }
                else { }
            }
        }

        /// <summary>
        /// 更新剩下的消除格
        /// </summary>
        private void UpdateRemainsGrid()
        {
            //BoardGrids.UpdateDirty();

            //int max = BoardGrids.GridsSize - 1;
            //while (max > 0)
            //{
            //    //updateInfo = mUpdateCols.Dequeue();
            //    mGridTemp = BoardGrids.GetGridByIndex(max);//updateInfo.grid;
            //    //pos = updateInfo.targetPos;
            //    //mGridTemp.SetGridPos(pos);
            //    if (mGridTemp != default && !mGridTemp.IsObstacle)
            //    {
            //        //targetPos = creater.GetGridPosWithRowCol(pos.x, pos.y);
            //        //effectParam = mGridTemp.GetEffectParam<GridEffectParam>(GameEffects.EffectRemains);
            //        //effectParam.EndPosition = targetPos;
            //        mGridTemp.StartEffect(GameEffects.EffectRemains);
            //    }
            //    else { }

            //    max--;
            //}

            ElimlnateGrid grid, next;
            int col = BoardGrids.ColumnSize;
            int row = BoardGrids.RowSize;
            for (int n = 0; n < col; n++)
            {
                for (int m = 0; m < row; m++)
                {
                    if (m >= 1)
                    {
                        grid = BoardGrids.GetGridByRowColumn(n, m);
                        int indexNext = BoardGrids.GetGridIndex(n, m = 1);
                        //bool indexNextValidable = BoardGrids.IsValidGridIndex(indexNext);
                        next = BoardGrids.GetGridByRowColumn(n, m - 1);
                        if (grid != default && !grid.IsObstacle)
                        {
                            //Debug.Log("index " + indexNext);
                            //Debug.Log("index " + indexNextValidable);
                            if (/*indexNextValidable && */next == default)
                            {
                                grid.GridTrans.localScale = Vector3.one * 1.5f;
                                Debug.Log("grid !!! " + n + ", " + m);
                                //mGridTemp.StartEffect(GameEffects.EffectRemains);
                            }
                            else { }
                        }
                        else { }
                    }
                    else { }
                }
            }
        }

        /// <summary>
        /// 补充消除格
        /// </summary>
        private void UpdateSupplementGrids()
        {
            BoardGrids boardGrids = GamePlay.BoardGrids;
            GridCreater gridCreater = GamePlay.GridCreater;

            Vector3 curPos;
            Vector2Int cellPos;
            int colCount = boardGrids.ColumnSize;
            int rowCount = boardGrids.RowSize;
            int max = colCount, gridType;
            for (int i = 0; i < max; i++)
            {
                int col = i, start;
                int supplement = mSupplementSizeData[col];
                if (supplement > 0)
                {
                    start = rowCount - supplement;

                    for (int row = start; row < rowCount; row++)
                    {
                        cellPos = new Vector2Int(col, row);
                        curPos = gridCreater.GetGridPosWithRowCol(cellPos.x, cellPos.y);
                        gridType = gridCreater.GetNextCreatedGridType();
                        gridCreater.CreateGrid(cellPos, curPos, out GridCreateInfo info, GameEffects.EffectEnter, true, gridType);

                        mHigestInCols[col]--;
                    }
                }
                else
                {
                    start = rowCount - 1;
                }
            }
        }

        /// <summary>
        /// 初始化空的消除格补充的数据映射
        /// </summary>
        /// <param name="col"></param>
        private void ResetColsEmptyData(int col)
        {
            if (!mSupplementSizeData.ContainsKey(col))
            {
                mSupplementSizeData[col] = 0;
            }
            else { }
            //if (!mObstacleCols.ContainsKey(col))
            //{
            //    mObstacleCols[col] = 1;
            //}
            //else { }
        }

        public void Update()
        {
            //if (mCore != default)
            //{
            //    if (mRearrangerTime > 0f)
            //    {
            //        mRearrangerTime -= Time.deltaTime;

            //        if (mRearrangerTime <= 0f)
            //        {
            //            mRearrangerTime = 0f;
            //            mCore.BoardGrids.AutoRemovable(out _);
            //        }
            //        else { }
            //    }
            //    else { }
            //}
            //else { }
        }
    }
}
