using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elimlnate
{
    /// <summary>
    /// 三消玩法核心输入器
    /// </summary>
    public class LineInputCore
    {
        /// <summary>连线确认</summary>
        private const int LINING_STATU_CONFIRM = 0;
        /// <summary>连线取消</summary>
        private const int LINING_STATU_CANCEL = 1;
        /// <summary>连线无效</summary>
        private const int LINING_STATU_INVALID = 2;

        public const int SORT_LAYER_INDEX_DEFAULT_GRID = 0;
        public const int SORT_LAYER_INDEX_LINE_SHOW = 1;

        /// <summary>线特效颜色</summary>
        private Color mLineColor = Color.blue;
        /// <summary>最小可消除个数</summary>
        private int mRemoveMinNum = 2;
        /// <summary>首次选中的消除格</summary>
        private ElimlnateGrid mHeaderGrid;
        /// <summary>当前连线的消除格</summary>
        private ElimlnateGrid mLining;
        /// <summary>本次选中的起始消除格变换组件</summary>
        private Transform mHeaderGridTrans;
        /// <summary>当前可以划的图形列表</summary>
        private List<ElimlnateGrid> mAllowLineGrids;
        /// <summary>是否正在连线</summary>
        private bool mIsLining;
        /// <summary>当前已经划到的图形列表</summary>
        private Stack<ElimlnateGrid> mLinedGridStack;
        /// <summary>当前已经划到的图形列表</summary>
        private List<ElimlnateGrid> mLinedGridList;
        /// <summary>当前已经划到的图形列表</summary>
        //private List<ElimlnateGrid> mSecondarySkillList;
        private Queue<ISkillGridTrigger> mSecondarySkillListDels;
        /// <summary>技能范围列表</summary>
        private List<int> mSkillGridRanges;

        /// <summary>变换变量</summary>
        private Transform mTransTemp;
        /// <summary>消除核心</summary>
        private ElimlnateCore mCore;
        /// <summary>上一个加入操作列表的消除格</summary>
        private ElimlnateGrid mPrevAddGrid;
        /// <summary>即将触发的消除格技能列表</summary>
        private List<ISkillGridTrigger> mSkillTriggers;
        /// <summary>是否启动自动取消当前输入状态的机制（例如：上一帧正常，下一帧不在棋盘内的操作）</summary>
        private bool mIsCheckAutoCancelInput;
        /// <summary>自动取消目前输入的计时</summary>
        private float mAutoCancelInputTime;
        /// <summary>核心玩法操作器</summary>
        private GridOperater mGridOperate;
        /// <summary>是否已触发指针按下的输入操作</summary>
        private bool mIsPointerDown;
        /// <summary>连接列表中最后一个消除格的变换对象</summary>
        private Transform mLastGridTrans;
        /// <summary>触发指针抬起的消除格变换对象</summary>
        private Transform mPointerUpTrans;
        /// <summary>上一次的输入事件数据</summary>
        private PointerEventData mLastInputData;

        protected GridOperater GridOperate
        {
            get
            {
                if (mGridOperate == default)
                {
                    mGridOperate = mCore.GridOperater;
                }
                else { }

                return mGridOperate;
            }
        }

        /// <summary>是否允许取消当前选择的消除格</summary>
        public bool IsAllowChooseBack { get; set; } = true;
        /// <summary>线特效宽度</summary>
        public float LineWidth { get; set; } = 0.5f;
        /// <summary>绘线颜色</summary>
        public Color[] LineColors { get; set; }
        /// <summary>初始化完成后的回调函数</summary>
        public Action AfterInit { get; set; }
        /// <summary>输入的可用性被修改的回调函数</summary>
        public Action<bool> InputEnabledChanged { get; set; }
        /// <summary>出现第一个消除格被作为输入值后的回调函数</summary>
        public Action<ElimlnateGrid> AfterHasInput { get; set; }
        /// <summary>消除格被选中的回调函数</summary>
        public Action<ElimlnateGrid> OnGridChoosen { get; set; }
        /// <summary>消除格被连接后的回调函数</summary>
        public Action<ElimlnateGrid> OnGridLinedCancel { get; set; }
        /// <summary>渲染排序层索引列表</summary>
        public int[] SortingLayers { get; set; } = new int[] { 5, 4 };
        /// <summary>是否接收输入</summary>
        public bool ShouldInput { get; private set; }
        /// <summary>可以识别的层级</summary>
        public LayerMask mLayerMask { get; set; }
        /// <summary>输入事件数据</summary>
        public PointerEventData InputEventData { get; set; }

        public LineInputCore()
        {
            mCore = ElimlnateCore.Instance;
            mLinedGridList = new List<ElimlnateGrid>();
            mLinedGridStack = new Stack<ElimlnateGrid>();
            mSecondarySkillListDels = new Queue<ISkillGridTrigger>();
            mSkillGridRanges = new List<int>();
            mSkillTriggers = new List<ISkillGridTrigger>();
        }

        public void Clear()
        {
            mCore = default;
            mGridOperate = default;

            mAllowLineGrids?.Clear();
            mLinedGridList?.Clear();
            mLinedGridStack?.Clear();
            mSkillTriggers?.Clear();
            mSecondarySkillListDels?.Clear();
            mSkillGridRanges?.Clear();

            AfterInit = default;
            AfterHasInput = default;
            InputEnabledChanged = default;
            OnGridChoosen = default;
            OnGridLinedCancel = default;

            InputEventData = default;
            mHeaderGrid = default;
            mLastGridTrans = default;
            mPointerUpTrans = default;
            mTransTemp = default;
            mPrevAddGrid = default;
            mLining = default;
            mHeaderGridTrans = default;
        }

        public void Init()
        {
            mRemoveMinNum = GridOperate.ShouldComboLineMax;

            AfterInit?.Invoke();
        }

        public void SetInputEnable(bool value)
        {
            ShouldInput = value;
            InputEnabledChanged?.Invoke(value);
        }

        private bool IsContainsInLined(ref ElimlnateGrid target)
        {
            return mLinedGridList.Contains(target);
        }

        public int GetLiningGridCount()
        {
            return mLinedGridStack.Count;
        }

        public void UpdateLiningGridLine(Material material)
        {
            ElimlnateGrid grid;
            int max = mLinedGridList.Count;
            for (int i = 0; i < max; i++)
            {
                grid = mLinedGridList[i];
                grid.SetLineMaterial(ref material);
            }
        }

        /// <summary>
        /// 检测正在连中的图形
        /// </summary>
        /// <param name="target"></param>
        private void CheckLiningGrid(ref ElimlnateGrid target)
        {
            if ((mHeaderGrid == default) || (target.GridTrans == mHeaderGridTrans) && (mPrevAddGrid == default))//如果与第一个消除格相同且发生过结束连接操作的情况则返回
            {
                "log".Log("Checking lining grid is cancel.");
            }
            else
            {
                if (target.ShouldLine)
                {
                    if (mLining == default)
                    {
                        mTransTemp = target.GridTrans;
                        mLining = mCore.BoardGrids.GetGridFromMapper(mTransTemp.GetInstanceID());
                        "error: Judge shape error, id = {0}".Log(mLining == default, mTransTemp.GetInstanceID().ToString());
                    }
                    else { }

                    LiningGridValidable();
                }
                else
                {
                    "log".Log("Header grid do not allow to conect.");
                }
            }
            mLining = default;
        }

        /// <summary>
        /// 连线中的消除格是否有效
        /// </summary>
        private void LiningGridValidable()
        {
            int statu = LINING_STATU_CONFIRM;
            bool isContained = IsContainsInLined(ref mLining);
            bool shouldLine = mLining.ShouldLine;
            bool willCancel = isContained || !shouldLine;
            if (mLining.IsActive)
            {
                if (willCancel)
                {
                    statu = LINING_STATU_CANCEL;
                }
                else { }
            }
            else
            {
                if (willCancel)
                {
                    if (isContained && mPrevAddGrid != default)
                    {
                        statu = LINING_STATU_CANCEL;
                    }
                    else
                    {
                        statu = LINING_STATU_INVALID;
                        mLining = default;//排除未列入可选中范围（IsActive 为 false）、首次添加到连接列表中、不可连线的消除格等情况
                    }
                }
                else { }
            }
            LineToGrids(statu);
        }

        private void LineToGrids(int statu)
        {
            switch (statu)
            {
                case LINING_STATU_CONFIRM:
                case LINING_STATU_CANCEL:
                    bool allowLine = mAllowLineGrids.Contains(mLining);
                    "log:消除格 {0} 不在可选范围内".Log(!allowLine, mLining.GridTrans.name);

                    if (allowLine)
                    {
                        if (statu == LINING_STATU_CANCEL)
                        {
                            RemovePrevLinedGrid();
                        }
                        else
                        {
                            OnGridChoosen?.Invoke(mLining);
                            AddLinedGrid();
                        }
                        mAllowLineGrids = GridOperate.CollectAllowOperateGrids(mLining.GridPos);
                    }
                    else { }
                    break;
            }
        }

        public void InputPointerEnter(ElimlnateGrid grid)
        {
            if (IsValidPointerEnterOrExit())
            {
                mIsCheckAutoCancelInput = false;

                if (mIsLining)
                {
                    ElimlnateGrid target = grid;
                    CorrectGridEnter(ref target, MissGridEnter);
                }
                else
                {
                    mLining = default;
                }
            }
            else { }
        }

        private void MissGridEnter()
        {
            if (!mIsLining)
            {
                return;
            }
            else { }

            Vector2 delta = InputEventData.delta;
            Vector2 pos = mPrevAddGrid.GridPos;

            pos += delta.normalized;

            ElimlnateGrid target = mCore.BoardGrids.GetGridByRowColumn(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y));

            mLining = default;
            CorrectGridEnter(ref target);
        }

        private void CorrectGridEnter(ref ElimlnateGrid target, Action onMissCallback = default)
        {
            if (target != default && target.GridTrans != default)
            {
                if (mHeaderGrid != default && mPrevAddGrid != default)
                {
                    Vector2 direct = target.GridPos - mPrevAddGrid.GridPos;
                    Vector2 delta = InputEventData.delta;
                    float dot = Vector2.Dot(direct.normalized, delta.normalized);

                    if (mAllowLineGrids.Contains(target) && mHeaderGrid.ShouldLineToTarget(ref target))
                    {
                        CheckLiningGrid(ref target);
                    }
                    else if (dot >= 0.6f)
                    {
                        onMissCallback?.Invoke();
                    }
                    else
                    {
                        mLining = default;
                    }
                }
                else
                {
                    mLining = default;
                }
            }
            else
            {
                mLining = default;
            }
        }

        private bool IsValidPointerEnterOrExit()
        {
            return mLining == default;
        }

        public void InputGridPointerDown(ElimlnateGrid grid)
        {
            if (mIsPointerDown || grid == default)
            {
                return;
            }
            else
            {
                mIsPointerDown = true;
            }

            mIsLining = true;
            mHeaderGrid = grid;

            if (mHeaderGrid != default)
            {
                CheckHeaderGrid();
            }
            else { }
        }

        public void InputPointerExit(ElimlnateGrid grid)
        {
            if (IsValidPointerEnterOrExit())
            {
                mAutoCancelInputTime = 0.5f;
                mIsCheckAutoCancelInput = true;
            }
            else { }
        }

        /// <summary>
        /// 是否未无效的指针抬起输入操作，防止多个消除格输入操作出现的问题
        /// </summary>
        /// <returns></returns>
        private bool IsInvalidPointerUp()
        {
            bool result = false;
            int count = mLinedGridStack.Count;
            if (count > 0)
            {
                mLastGridTrans = mLinedGridList[count - 1].GridTrans;
                bool isInvalidTrans = (mPointerUpTrans != default) && (mLastGridTrans != mPointerUpTrans);//判断触发抬起操作的对象是否未无效对象
                if (isInvalidTrans)
                {
                    if (Input.touchCount > 1)
                    {
                        TouchPhase phase = Input.GetTouch(0).phase;//检测第一个触控输入
                        if (phase == TouchPhase.Moved || phase == TouchPhase.Stationary)
                        {
                            mPointerUpTrans = default;
                            result = true;//如果第一个触控发输入处于移动或悬停则本次抬起操作无效
                        }
                        else { }
                    }
                    else { }
                }
                else { }
            }
            else { }
            return result;
        }

        public void InputGridPointerUp(ElimlnateGrid grid)
        {
            if (mCore != default)
            {
                mPointerUpTrans = grid != default ? grid.GridTrans : default;

                if (!IsInvalidPointerUp())
                {
                    mHeaderGridTrans = mPointerUpTrans;

                    LineEnd();

                    StopPreviewSkillRanges(false);
                    ClearLastInput();
                }
                else { }

            }
            else { }
        }

        public void ClearLastInput()
        {
            mLinedGridList?.Clear();
            mLinedGridStack?.Clear();
            mAllowLineGrids?.Clear();
            mSkillTriggers?.Clear();

            mHeaderGrid = default;
            mHeaderGridTrans = default;
            mPointerUpTrans = default;
            mIsPointerDown = false;
            mIsLining = false;
        }

        public void InputGridPointerClick(ElimlnateGrid grid)
        {
            if (mCore == default || grid == default || mLinedGridStack.Count > 1)
            {
                return;
            }
            else { }

            bool isIdentical = grid == mHeaderGrid;
            bool isNormalGrid = grid.GridType == mCore.GridTypes.NormalGridType;
            if (!isNormalGrid)
            {
                CheckSkillTriggerByClick(ref grid, isIdentical);
            }
            else { }

            StopPreviewSkillRanges(false);
            mLinedGridStack?.Clear();
            mAllowLineGrids?.Clear();
        }

        private void CheckSkillTriggerByClick(ref ElimlnateGrid grid, bool isIdentical)
        {
            bool isTrigger = false;
            if (isIdentical)
            {
                ISkillGridTrigger trigger = grid.ShapeSkillTrigger;
                if ((trigger != default) && !trigger.IsTriggerWhenLineEnd)
                {
                    isTrigger = trigger.Trigger(mLinedGridList);//TODO 增加连接后触发技能的触发器
                }
                else { }
            }
            else { }

            if (isTrigger)
            {
                mLinedGridList.Add(grid);
                mLinedGridStack.Push(grid);

                GridOperate.LineEnd(mLinedGridList);
            }
            else { }
        }

        private void CheckHeaderGrid()
        {
            if (mHeaderGrid.ShouldLine)
            {
                mHeaderGridTrans = mHeaderGrid.GridTrans;
                OnGridChoosen?.Invoke(mHeaderGrid);//此处的回调函数中需要处理消除格被选中的逻辑

                if (mLinedGridStack.Count == 0)
                {
                    mCore.GridOperater.ClearGridProcesser();
                    mCore.GridOperater.EliminateResult.ClearResult();
                    AfterHasInput?.Invoke(mHeaderGrid);//发生首次输入时的回调
                }
                else { }

                if (!mLinedGridStack.Contains(mHeaderGrid))
                {
                    mHeaderGrid.IsActive = true;
                    mLinedGridList.Add(mHeaderGrid);
                    mLinedGridStack.Push(mHeaderGrid);//添加打头的消除格
                    mAllowLineGrids = GridOperate.CollectAllowOperateGrids(mHeaderGrid.GridPos);
                }
                else { }

                StopPreviewSkillRanges(false);
                CheckGridSkills(ref mHeaderGrid, false);

                mPrevAddGrid = mHeaderGrid;
                mLining = default;
            }
            else
            {
                mHeaderGrid = default;
                mHeaderGridTrans = default;
            }
        }

        /// <summary>
        /// 添加到图形列表中
        /// </summary>
        private void AddLinedGrid()
        {
            StopPreviewSkillRanges(false);
            if (!IsContainsInLined(ref mLining))
            {
                if ((mPrevAddGrid == default) && !mLining.ShouldLineAsFirst)
                {
                    return;//去除无法作为连线起点的消除格
                }
                else { }

                int lastIndex = mLinedGridStack.Count - 1;//获取已连接的消除格列表中最后一个索引位置
                ElimlnateGrid last = mLinedGridList[lastIndex];
                Vector3 lineStartPos = last.GridTrans.position;

                int sortingLayer = SortingLayers[SORT_LAYER_INDEX_DEFAULT_GRID];
                mLining.SetItemSortingLayer(sortingLayer + 1);
                mLinedGridList.Add(mLining);
                mLinedGridStack.Push(mLining);
                mAllowLineGrids = GridOperate.CollectAllowOperateGrids(mLining.GridPos);

                int shapeIndex = last.GridShapeIndex;
                mLineColor = LineColors[shapeIndex];
                mLining.SetLineShow(lineStartPos, LineWidth, mLineColor);

                mPrevAddGrid = mLining;

                CheckGridSkills(ref mLining, false);
            }
            else { }
        }

        private void CheckGridSkills(ref ElimlnateGrid grid, bool isRemovePrev)
        {
            GridSkillValidable(ref grid, isRemovePrev);

            if (mLinedGridList.Count >= mRemoveMinNum)
            {
                ISkillGridTrigger trigger;
                int max = mSkillTriggers.Count;
                for (int i = 0; i < max; i++)
                {
                    trigger = mSkillTriggers[i];
                    GridSkillPreviwing(ref grid, ref trigger);
                }
            }
            else { }

        }

        private void GridSkillPreviwing(ref ElimlnateGrid grid, ref ISkillGridTrigger trigger)
        {
            trigger.CenterGridPos = grid.GridPos;
            Queue<ISkillGridTrigger> queue = trigger.Preview(mSkillGridRanges);
            if (queue.Count > 0)
            {
                while (queue.Count > 0)
                {
                    mSkillTriggers.Add(queue.Dequeue());
                }
            }
            else { }
        }

        private void GridSkillValidable(ref ElimlnateGrid grid, bool isRemovePrev)
        {
            ISkillGridTrigger trigger;
            if (grid.HasGridSkill)
            {
                trigger = grid.ShapeSkillTrigger;
                if (trigger != default)
                {
                    if (trigger.IsTriggerWhenLineEnd)
                    {
                        if (isRemovePrev)
                        {
                            RemoveExistedSkill(ref trigger);
                        }
                        else
                        {
                            AddValidSkill(ref trigger);
                        }
                    }
                    else { }
                }
                else { }
            }
            else { }
        }

        private void AddValidSkill(ref ISkillGridTrigger trigger)
        {
            if (!mSkillTriggers.Contains(trigger))
            {
                mSkillTriggers.Add(trigger);
            }
            else { }
        }

        private void RemoveExistedSkill(ref ISkillGridTrigger trigger)
        {
            if (mSkillTriggers.Contains(trigger))
            {
                trigger.Cancel(mSkillGridRanges);
                mSkillTriggers.Remove(trigger);
            }
            else { }
        }

        /// <summary>
        /// 取消上一个选择的消格
        /// </summary>
        private void RemovePrevLinedGrid()
        {
            StopPreviewSkillRanges(false);
            if (mPrevAddGrid != default)
            {
                if (mPrevAddGrid == mLinedGridStack.Peek())
                {
                    mLinedGridList.Remove(mPrevAddGrid);
                    mPrevAddGrid = mLinedGridStack.Pop();

                    int sortingLayer = SortingLayers[SORT_LAYER_INDEX_DEFAULT_GRID];
                    mPrevAddGrid.SetItemSortingLayer(sortingLayer);
                    mPrevAddGrid.CancelLineShow();
                    OnGridLinedCancel?.Invoke(mPrevAddGrid);

                    CheckGridSkills(ref mPrevAddGrid, true);
                    mLining = mLinedGridStack.Peek();
                    mPrevAddGrid = mLining;

                    StopPreviewSkillRanges(false);
                    CheckGridSkills(ref mLining, false);
                }
                else { }
            }
            else { }
        }

        public ElimlnateGrid GetLinedGrid()
        {
            return mLining;
        }

        /// <summary>
        /// 结束本次操作
        /// </summary>
        private void LineEnd()
        {
            mLining = default;
            mPrevAddGrid = default;

            if (mIsLining)
            {
                mCore.BoardGrids.ResetAllGridsToDeactive(true);

                ElimlnateGrid grid;
                Queue<ElimlnateGrid> invalids = new Queue<ElimlnateGrid>();
                int count = mLinedGridList.Count;
                for (int i = 0; i < count; i++)
                {
                    grid = mLinedGridList[i];

                    if (grid != default)
                    {
                        CancelGridOperate(ref grid);
                    }
                    else
                    {
                        invalids.Enqueue(grid);
                    }
                }

                int max = invalids.Count;
                while(max > 0)
                {
                    grid = invalids.Dequeue();
                    mLinedGridList.Remove(grid);
                    max--;
                }
                bool willCancel = max > 0;
                SetResult(willCancel);
            }
            else { }
        }

        private void SetResult(bool willCancel)
        {
            int count = mLinedGridList.Count;
            if (willCancel || (count < mRemoveMinNum))
            {
                mCore.ActiveInput();
            }
            else
            {
                bool flag = mSkillTriggers.Count <= 0;
                GridOperate.LineEnd(mLinedGridList, flag);
                TriggerGridSkill();
            }
            ElimlnateGrid item;
            for (int i = 0; i < count; i++)
            {
                item = mLinedGridList[i];
                CancelGridOperate(ref item);
            }
        }

        private void CancelGridOperate(ref ElimlnateGrid grid)
        {
            grid.CancelLineShow();
            grid.ResetShapeItem();
            OnGridLinedCancel?.Invoke(grid);
        }

        private void TriggerGridSkill()
        {
            int max = mSkillGridRanges.Count;
            if (max > 0)
            {
                StopPreviewSkillRanges(true);
            }
            else { }
        }

        private void StopPreviewSkillRanges(bool isCommitSkill)
        {
            ISkillGridTrigger trigger;
            int max = mSkillTriggers.Count;
            for (int i = 0; i < max; i++)
            {
                trigger = mSkillTriggers[i];
                trigger.StopPreview(mSkillGridRanges);

                if (isCommitSkill)
                {
                    trigger.Trigger(mLinedGridList);

                    bool flag = i >= max - 1;
                    if (flag)
                    {
                        GridOperate.LineEnd(mLinedGridList, flag);
                    }
                    else { }
                }
                else { }

                if (trigger.IsSecondary)
                {
                    trigger.IsSecondary = false;
                    mSecondarySkillListDels.Enqueue(trigger);
                }
                else { }
            }

            max = mSecondarySkillListDels.Count;
            while (max > 0)
            {
                trigger = mSecondarySkillListDels.Dequeue();
                mSkillTriggers.Remove(trigger);
                max--;
            }

            mSkillGridRanges.Clear();
        }

        public void ResetInput()
        {
            mLinedGridStack?.Clear();
            mAllowLineGrids?.Clear();
        }

        public void SetSortingOrder(int index, int sortingLayer)
        {
            SortingLayers[index] = sortingLayer;
        }

        public int GetSortingOrder(int index)
        {
            return SortingLayers[index];
        }

        public void Update()
        {
            //bool flag = GridOperate != default ? GridOperate.IsProcessing : false;
            //SetInputEnable(!flag);

            if (mIsCheckAutoCancelInput && mAutoCancelInputTime > 0f)
            {
                mAutoCancelInputTime -= Time.deltaTime;
                if (mAutoCancelInputTime <= 0f && mLining != default)
                {
                    mIsCheckAutoCancelInput = false;
                    InputGridPointerUp(default);
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 是否已被连线
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public bool IsGridLining(ref ElimlnateGrid grid)
        {
            return mLinedGridList.Contains(grid);
        }
    }
}
