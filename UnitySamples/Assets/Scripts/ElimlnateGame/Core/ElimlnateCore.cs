using ShipDock.Applications;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elimlnate
{
    /// <summary>
    /// 
    /// 三消类玩法核心单例
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
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
                else { }

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

        private bool mKeepsDeactived = true;
        private Queue<float> mAutoActiveTimes;
        private Queue<float> mActiveTimeRemains;

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
        public Action OnUpdate { get; private set; }
        public EliminateData Data { get; private set; }

        private ElimlnateCore()
        {
            Init();
        }

        public void Init()
        {
            GridTypes = new GridTypes();
            GridEffects = new GameEffects();
            Data = new EliminateData(this);
            mAutoActiveTimes = new Queue<float>();
            mActiveTimeRemains = new Queue<float>();
        }

        public void Clean()
        {
            IsClear = true;

            BoardGrids?.Clean();
            GridCreater?.Clean();
            GridOperater?.Clean();
            LineInputer?.Clean();
            GridTypes?.Clean();
            GridEffects?.Clean();
            mAutoActiveTimes?.Clear();
            mActiveTimeRemains?.Clear();

            OnUpdate = default;
            GridsContainer = default;
            DestroyGrid = default;
            ElimlnateCamera = default;
            Inputer = default;
        }

        public void AddUpdate(Action method)
        {
            OnUpdate += method;
        }

        public void RemoveUpdate(Action method)
        {
            OnUpdate -= method;
        }

        public void Update()
        {
            GridEffects?.UpdateEffects();
            LineInputer?.Update();
            GridOperater?.Update();
            OnUpdate?.Invoke();

            if (mKeepsDeactived)
            {
                LineInputer.SetInputEnable(false);
            }
            else
            {
                UpdateAutoActiveTimes();
            }
        }

        private void UpdateAutoActiveTimes()
        {
            if (IsClear)
            {
                return;
            }
            else { }

            float time;
            if (mAutoActiveTimes.Count > 0)
            {
                LineInputer.SetInputEnable(false);
                
                while (mAutoActiveTimes.Count > 0)
                {
                    time = mAutoActiveTimes.Dequeue();
                    time -= Time.deltaTime;
                    if (time > 0f)
                    {
                        mActiveTimeRemains.Enqueue(time);
                    }
                    else { }
                }
                mAutoActiveTimes.Clear();
            }
            else { }

            if (mActiveTimeRemains.Count <= 0)
            {
                LineInputer.SetInputEnable(true);
            }
            else
            {
                while (mActiveTimeRemains.Count > 0)
                {
                    time = mActiveTimeRemains.Dequeue();
                    mAutoActiveTimes.Enqueue(time);
                }
                mActiveTimeRemains.Clear();
            }
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

        private void SetActiveTime(float time)
        {
            mKeepsDeactived = false;
            if (time > 0f)
            {
                mAutoActiveTimes.Enqueue(time);
            }
            else { }
        }

        public void DeactiveInput(float activeTime = 0f)
        {
            mKeepsDeactived = false;
            SetActiveTime(activeTime);
        }

        public void DeactiveInput()
        {
            mKeepsDeactived = true;
        }

        public void ActiveInput()
        {
            if(mAutoActiveTimes.Count <= 0)
            {
                mKeepsDeactived = false;
                LineInputer.SetInputEnable(true);
            }
            else { }
        }
    }
}
