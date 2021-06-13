using ShipDock.Applications;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elimlnate
{
    public class ElimlnateCore
    {
        private static ElimlnateCore instance;

        public static ElimlnateCore Instance
        {
            get
            {
                if (instance == default)
                {
                    instance = new ElimlnateCore();
                }
                return instance;
            }
        }

        public static int GetNormalGridType()
        {
            return Instance.GridTypes.NormalGridType;
        }

        public bool ShouldInput
        {
            get
            {
                return LineInputer != default ? LineInputer.ShouldInput : false;
            }
        }

        public bool IsClear { get; private set; }
        public bool IsAutoActiveInput { get; set; } = true;
        public BoardGrids BoardGrids { get; set; }
        public GridCreater GridCreater { get; set; }
        public GridOperater GridOperater { get; set; }
        public PerspectiveInputer Inputer { get; set; }
        public LineInputCore LineInputer { get; set; }
        public GameEffects GridEffects { get; set; }
        public GridTypes GridTypes { get; private set; }
        public Transform GridsContainer { get; set; }
        public Action<GameObject> DestroyGrid { get; set; }
        public Camera ElimlnateCamera { get; set; }

        private ElimlnateCore()
        {
            Init();
        }

        public void Init()
        {
            GridTypes = new GridTypes();
            GridEffects = new GameEffects();
        }

        public void Clear()
        {
            IsClear = true;

            BoardGrids?.Clear();
            GridCreater?.Clear();
            GridOperater?.Clear();
            LineInputer?.Clear();
            GridTypes?.Clear();
            GridEffects?.Clear();

            GridsContainer = default;
            DestroyGrid = default;
            ElimlnateCamera = default;
            Inputer = default;
        }

        public void Update()
        {
            GridEffects?.UpdateEffects();
            LineInputer?.Update();
            GridOperater?.Update();
        }

        public void CreateBoard(int col, int row, Action<ElimlnateGrid> afterGridsDeactived = default, Action<List<GridRearrangerInfo>> rearrangerGrids = default)
        {
            IsClear = false;
            BoardGrids = new BoardGrids(col, row);
            if (afterGridsDeactived != default)
            {
                BoardGrids.AfterGridsDeactived += afterGridsDeactived;
            }
            else { }

            if (rearrangerGrids != default)
            {
                BoardGrids.OnRearrangerGrids += rearrangerGrids;
            }
            else { }
        }

        public void InitGridCreater(Vector2 cellSize, float enterEffectTime, AnimationCurve curve, GameObject tileResRaw, Func<int, int> onGetGridTypeDuringCreate = default)
        {
            GridCreater = new GridCreater()
            {
                CellSize = cellSize,
                EnterEffectDuringTime = enterEffectTime,
                EnterEffectCurve = curve,
                TileResRaw = tileResRaw,
                GetGridTypeDuringCreate = onGetGridTypeDuringCreate,
            };
        }

        public void InitGridCreaterCallback(Func<GameObject, int> beforeGridCreate, Action<ElimlnateGrid> createGridOperateUI, Action<int> gridCreateCompleted)
        {
            GridCreater.BeforeGridCreate = beforeGridCreate;
            GridCreater.CreateGridOperateUI = createGridOperateUI;
            GridCreater.GridCreateCompleted = gridCreateCompleted;
        }

        public void CreateOperater(EliminateResult eliminateResult, bool isInclined, int shouldLineMin, int shouldComboLineMax)
        {
            GridOperater = new GridOperater(eliminateResult)
            {
                LinedRules = new ComboRule[] { },
                IsInclined = isInclined,
                ShouldLineMin = shouldLineMin,
                ShouldComboLineMax = shouldComboLineMax,
            };
        }

        public void DeactiveInput()
        {
            LineInputer?.SetInputEnable(false);
        }

        public void ActiveInput()
        {
            LineInputer?.SetInputEnable(true);
        }
    }
}
