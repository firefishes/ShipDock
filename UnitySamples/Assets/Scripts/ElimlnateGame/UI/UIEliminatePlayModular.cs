using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Elimlnate
{
    public abstract class UIEliminatePlayModular : UIModular<UIEliminatePlay>
    {
        public const int N_UI_BATTLE_GRID_OPERATING = 0;

        private bool mInputEnabled;
        private EliminateData mEliminateData;
        private LayoutGroup mOperateLayout;
        /// <summary>驱动消除格消除操作的UI单元渲染项映射</summary>
        private KeyValueList<int, int> mGridsUIMapper;
        /// <summary>驱动消除格消除操作的UI单元数据映射</summary>
        private KeyValueList<int, GridOperateInfo> mGridOperateMapper;

        public override int[] DataProxyLinks { get; set; }

        private ElimlnateCore ElimCore { get; set; }

        public override void OnDataProxyNotify(IDataProxy data, int keyName)
        {
        }

        protected override void Purge()
        {
        }

        protected override void UIModularHandler(INoticeBase<int> param)
        {
            switch (param.Name)
            {
                case N_UI_BATTLE_GRID_OPERATING:
                    int operateType = (param as IParamNotice<int>).ParamValue;
                    mEliminateData.SetOperatingGrid(operateType);
                    break;
            }
        }

        public override void Init()
        {
            base.Init();

            mGridsUIMapper = new KeyValueList<int, int>();
            mGridOperateMapper = new KeyValueList<int, GridOperateInfo>();

            ElimCore = ElimlnateCore.Instance;
            mEliminateData = ElimCore.Data;
            mEliminateData.AddListener(EliminateData.N_CREATE_GRID_OPERATE_UI, OnCreateGridOperateUI);
            mEliminateData.AddListener(EliminateData.N_UPDATE_PLAY_CORE_INPUT_ENABLED, OnUpdateInputEnabled);
        }

        private void OnCreateGridOperateUI(INoticeBase<int> obj)
        {
            ElimlnateGrid grid = (obj as IParamNotice<ElimlnateGrid>).ParamValue;
            AddGridOperateUI(ref grid);
        }

        private void OnUpdateInputEnabled(INoticeBase<int> obj)
        {
            bool flag = (obj as IParamNotice<bool>).ParamValue;
            mInputEnabled = flag;
        }

        /// <summary>
        /// 添加消除格的UI操作驱动
        /// </summary>
        /// <param name="grid"></param>
        private void AddGridOperateUI(ref ElimlnateGrid grid)
        {
            mOperateLayout = UI.GridOperateLayout;
            GameObject prefab = UnityEngine.Object.Instantiate(UI.OperateCellItemRenderer, mOperateLayout.transform);
            EventTrigger eventTrigger = prefab.GetComponent<EventTrigger>();

            //添加消除格UI驱动层的事件响应
            SetOperateEventTrigger(ref eventTrigger, EventTriggerType.PointerDown, OnGridPointerDown);
            SetOperateEventTrigger(ref eventTrigger, EventTriggerType.PointerUp, OnGridPointerUp);
            SetOperateEventTrigger(ref eventTrigger, EventTriggerType.PointerEnter, OnGridPointerEnter);
            SetOperateEventTrigger(ref eventTrigger, EventTriggerType.PointerClick, OnGridPointerClick);
            SetOperateEventTrigger(ref eventTrigger, EventTriggerType.PointerExit, OnGridPointerExit);

            Vector2Int pos = grid.GridPos;
            int index = ElimCore.BoardGrids.GetGridIndex(pos.y, pos.x);
            int id = prefab.GetInstanceID();
            mGridOperateMapper[id] = new GridOperateInfo()
            {
                gridPos = grid.GridPos,
                prefabID = id,
                prefab = prefab,
                gridIndex = index,
                localPos = (prefab.transform as RectTransform).position,
            };

            mGridsUIMapper[index] = id;
        }

        /// <summary>
        /// 设置UI事件触发器
        /// </summary>
        /// <param name="eventTrigger"></param>
        /// <param name="triggerType"></param>
        /// <param name="handler"></param>
        private void SetOperateEventTrigger(ref EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> handler)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = triggerType,
                callback = new EventTrigger.TriggerEvent()
            };
            entry.callback.AddListener(handler);
            eventTrigger.triggers.Add(entry);
        }

        /// <summary>
        /// 消除格操作：滑动中退出
        /// </summary>
        /// <param name="param"></param>
        private void OnGridPointerExit(BaseEventData param)
        {
            CheckGridPointer(param, GridOperateInfo.GRID_OPERATE_TYPE_POINTER_EXIT);
        }

        /// <summary>
        /// 消除格操作：点击
        /// </summary>
        /// <param name="param"></param>
        private void OnGridPointerClick(BaseEventData param)
        {
            CheckGridPointer(param, GridOperateInfo.GRID_OPERATE_TYPE_POINTER_CLICK);
        }

        /// <summary>
        /// 消除格操作：抬起
        /// </summary>
        /// <param name="param"></param>
        private void OnGridPointerUp(BaseEventData param)
        {
            CheckGridPointer(param, GridOperateInfo.GRID_OPERATE_TYPE_POINTER_UP);
        }

        /// <summary>
        /// 消除格操作：滑动中进入
        /// </summary>
        /// <param name="param"></param>
        private void OnGridPointerEnter(BaseEventData param)
        {
            CheckGridPointer(param, GridOperateInfo.GRID_OPERATE_TYPE_ENTER);
        }

        /// <summary>
        /// 消除格操作：按下
        /// </summary>
        /// <param name="param"></param>
        private void OnGridPointerDown(BaseEventData param)
        {
            CheckGridPointer(param, GridOperateInfo.GRID_OPERATE_TYPE_POINTER_DOWN);
        }

        /// <summary>
        /// 检测消除格的UI层输入
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="gridOperateType"></param>
        private void CheckGridPointer(BaseEventData eventData, int gridOperateType)
        {
            if (mInputEnabled)
            {
                if (mOperateLayout.enabled)
                {
                    mOperateLayout.enabled = false;
                }
                else { }

                PointerEventData inputData = eventData as PointerEventData;

                switch (gridOperateType)
                {
                    case GridOperateInfo.GRID_OPERATE_TYPE_ENTER:
                    case GridOperateInfo.GRID_OPERATE_TYPE_POINTER_EXIT:
                    case GridOperateInfo.GRID_OPERATE_TYPE_POINTER_DOWN:
                        ElimCore.LineInputer.InputEventData = inputData;
                        break;
                }

                GameObject target = inputData.pointerEnter;
                GetGridPointerTarget(ref target);

                UI.Dispatch(N_UI_BATTLE_GRID_OPERATING, gridOperateType);
            }
            else { }
        }

        /// <summary>
        /// 获取正在操作的消除格引用
        /// </summary>
        /// <param name="target"></param>
        private void GetGridPointerTarget(ref GameObject target)
        {
            mEliminateData.OperatingGrid = default;
            if (target != default)
            {
                int id = target != default ? target.GetInstanceID() : int.MaxValue;
                GridOperateInfo info = mGridOperateMapper[id];
                if (info != default)
                {
                    Vector2Int pos = mGridOperateMapper[id].gridPos;
                    mEliminateData.OperatingGrid = ElimCore.BoardGrids.GetGridByRowColumn(pos.x, pos.y);
                }
                else { }
            }
            else { }
        }
    }

}
