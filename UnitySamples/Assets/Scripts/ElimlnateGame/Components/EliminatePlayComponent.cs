#define _ATTRACT_EFFECT
#define BLAST_EFFECT

using DG.Tweening;
using Elimlnate;
using ShipDock.Config;
using ShipDock.Loader;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Game
{
    public class GamingResult : EliminateResult
    {
        protected override void Purge()
        {
        }
    }

    [Serializable]
    public class GridTypeItem
    {
        public int id;
        public string prefabABName;
        public string prefabResName;
        public bool isSpriteAsset;
        public bool isInvalidRandom;
    }

    public class EliminatePlayComponent : MonoBehaviour
    {

        [SerializeField]
        private bool m_IsInclined = true;
        [SerializeField]
        private int m_ShouldLineMin = 2;
        [SerializeField]
        private int m_ShouldComboLineMax = 3;
        [SerializeField]
        private float m_GridEnterOffset = 0.5f;
        [SerializeField]
        private float m_EnterEffectDuringTime = 0.5f;
        [SerializeField]
        private float m_LineWidth = 0.3f;
        [SerializeField]
        private LayerMask m_LineLayer;
        [SerializeField]
        private Color[] m_LineColors;
        [SerializeField]
        private AnimationCurve m_EffectCurve;
        [SerializeField]
        private Vector2 m_CellSize = Vector2.one * 1.3f;
        [SerializeField]
        private Vector3 m_GridsOffset = new Vector3(0.077f, 0f);
        [SerializeField]
        private Vector2Int m_GridsRowAndCol;
        [SerializeField]
        private List<Transform> m_RolePosInBoard = new List<Transform>();
        [SerializeField]
        private SpriteRenderer m_StageBgRenderer;
        [SerializeField]
        private SpriteRenderer m_BoardBgRenderer;
        [SerializeField]
        private GameObject m_TileResRaw;
        [SerializeField]
        private Transform m_GridsContainer;
        [SerializeField]
        private Transform m_CameraPos;
        [SerializeField]
        private int m_NormalGridType = 1;
        [SerializeField]
        private List<GridTypeItem> m_GridTypeItems = new List<GridTypeItem>();

        private LineInputCore mInputCore;
        /// <summary>消除格棋盘中的数据类型</summary>
        private List<int> mGridTypesInBoard;
        private ElimlnateCore mCore;
        /// <summary>消除格的热更脚本组件映射</summary>
        private KeyValueList<int, GridComponent> mGridCompMapper;

        public Action OnInitUIGridOperateLayout { get; set; }

        private void OnDestroy()
        {
            mGridCompMapper?.Clear();

            ElimlnateCore.Instance.Clean();

            OnInitUIGridOperateLayout = default;
            mInputCore = default;
            mCore = default;
        }

        public void StartEliminatePlay()
        {
            mGridCompMapper = new KeyValueList<int, GridComponent>();

            OnInitUIGridOperateLayout?.Invoke();

            InitGridsData();
            InitGameCore();
            InitGameEffect();
            InitLineInputCore();
            InitBroad();
        }

        private void InitLineInputCore()
        {
            mInputCore = new LineInputCore
            {
                LineWidth = m_LineWidth,
                mLayerMask = m_LineLayer,
                LineColors = m_LineColors,
            };
            mCore.IsAutoActiveInput = false;
            mCore.LineInputer = mInputCore;
            mInputCore.AfterInit = OnAfterInit;
            mInputCore.Init();
            mInputCore.AfterHasInput += AfterInputFirstGrid;
            mInputCore.OnGridChoosen += GridChoosen;
            mInputCore.OnGridLinedCancel += GridLinedCancel;
            mInputCore.InputEnabledChanged += OnInputEnabled;
        }

        private void InitBroad()
        {
            #region TODO 棋盘视野适配
            //m_CameraPos = HotFixer.GetSceneNode("cameraPos").value.transform;

            //const float devW = 8.28f;//9.6f;
            //const float devH = 17.92f;//12.8f;

            //Camera.main.orthographic = true;
            //float orthographicSize = Camera.main.orthographicSize;
            //float asceptRatio = Screen.width * 1f / Screen.height;
            //float cameraW = orthographicSize * asceptRatio * 2f;

            //Debug.Log("cameraW < devW " + devW + " " + devH);
            //Debug.Log("cameraW < devW " + Screen.width + " " + Screen.height);
            //Debug.Log("cameraW < devW " + cameraW + " " + devW);
            ////if (cameraW < devW)
            ////{
            //    orthographicSize = Screen.width * 0.01f / (asceptRatio * 2f);
            //    Camera.main.orthographicSize = orthographicSize;
            //    Debug.Log("orthographicSize = " + orthographicSize);
            ////}
            #endregion

            Transform cameraTrans = Camera.main.transform;
            Vector3 pos = m_CameraPos.position;
            cameraTrans.position = new Vector3(pos.x, pos.y, pos.z);// * ratio);

            ElimlnateCore.Instance.DeactiveInput();
            mCore.GridCreater.InitAndCreate(mCore.BoardGrids, m_GridsOffset);
            mCore.ElimlnateCamera = cameraTrans.GetComponent<Camera>();
        }

        private void OnInputEnabled(bool value)
        {
            mCore.Data.DispatchWithParam(EliminateData.N_UPDATE_PLAY_CORE_INPUT_ENABLED, value);

            if (value)
            {
                mCore.BoardGrids.ResetAllGridsToDeactive(true);
            }
            else { }
        }

        private void GridLinedCancel(ElimlnateGrid grid)
        {
            grid.StopEffect(GameEffects.EffectChoosen);
            grid.StartEffect("Consts.GRID_EFFECT_CANCEL");

            grid = mCore.LineInputer.GetLinedGrid();
            //mUIData.SetShowEffectGrid(grid);
            //mBattleData.DataNotify(Consts.DN_GRID_OPERATE_UI_INPUT);
            //mUIData.SetShowEffectGrid(default);
        }

        private void GridChoosen(ElimlnateGrid grid)
        {
            //Consts.N_PLAY_SOUND.BroadcastWithParam(Consts.SOUND_GRID_CHOOSEN);

            grid.StartEffect(GameEffects.EffectChoosen, true);

            //mUIData.SetShowEffectGrid(grid);
            //mBattleData.DataNotify(Consts.DN_GRID_OPERATE_UI_INPUT);
            //mUIData.SetShowEffectGrid(default);

            //mGridChoosenShapeIndex = grid.GridShapeIndex;

            //int index = mGridLineMatIndex;
            //int min = mLineMaterialLevels.GetIntValue(0, 0);
            //int combo = mCore.LineInputer.GetLiningGridCount();
            //if (combo <= min)
            //{
            //    index = 0;
            //}
            //else
            //{
            //    int max = mLineMaterialLevels.Source.Count;
            //    for (int i = 0; i < max; i++)
            //    {
            //        int count = mLineMaterialLevels.GetIntValue(i, 0);
            //        if (combo > count)
            //        {
            //            index = mLineMaterialLevels.GetIntValue(i, 1);
            //        }
            //        else { }
            //    }
            //}

            //int poolName = Consts.PoolNameStar[index];
            //mUIData.SetElimEffectPoolName(poolName);

            //Material mat;
            //if (mGridLineMatIndex != index)
            //{
            //    mGridLineMatIndex = index;
            //    mat = mLineMaterials[mGridLineMatIndex];
            //    mCore.LineInputer.UpdateLiningGridLine(mat);
            //}
            //else
            //{
            //    mat = mLineMaterials[mGridLineMatIndex];
            //}
            //grid.SetLineMaterial(ref mat);

            //VibratorAfterChoosen();
        }

        private void AfterInputFirstGrid(ElimlnateGrid grid)
        {
            //if (grid.IsNormalGrid())
            //{
            //    mBattleData.SetFirstGridShapeIndex(grid.GridShapeIndex);
            //}
            //else { }

            //mUIData.SetShowEffectGrid(grid);
            //mBattleData.DataNotify(Consts.DN_GRID_OPERATE_UI_INPUT);
            //mUIData.SetShowEffectGrid(default);
            //Modulars.NotifyModularAndRelease(Consts.N_START_OPERATING_GRID);
        }

        private void OnAfterInit()
        {
        }

        private void InitGameEffect()
        {
            string elimlnateName = GameEffects.EffectElimlnate;
            mCore.GridEffects.SetEffect(GameEffects.EffectPreviewGridSkill, new GridSkillPreviewEffect()
            {
                //RangeSignResPoolName = Consts.POOL_EL_GRID_SKILL_RANGE_SIGNS
            });
            mCore.GridEffects.SetEffect(GameEffects.EffectChoosen, new GridChoosenEffect()
            {
                //ApplyEndPosition = false,
                OnPlaySound = () =>
                {
                    //Consts.N_PLAY_SOUND.BroadcastWithParam(Consts.SOUND_GRID_CHOOSEN);
                },
            });
            mCore.GridEffects.SetEffect("Consts.GRID_EFFECT_CANCEL", new GridChoosenEffect()//new GridCancelEffect()
            {
                //ApplyEndPosition = false,
            });
#if ATTRACT_EFFECT
            mCore.Effects[elimlnateName] = new GridAttractOutEffect();
            GridOperater.ElimlnateEffect = new ElimlnateEffectAttract(elimlnateName)
            {
                OnInit = ElimlnateEffectInit,
                SetParamBeforeStart = SetAttractParamBeforeStart,
                OnCompleted = ElimlnateEffectCompleted,
            };
#elif BLAST_EFFECT
            mCore.GridEffects.SetEffect(elimlnateName, new GridBlastEffect()
            {
                OnPlaySound = () =>
                {
                    //Consts.N_PLAY_SOUND.BroadcastWithParam(Consts.SOUND_GRID_BROKEN_1);
                },
                //EffectCenterRange = new Vector2(1.1f, 1.3f),
                //EffectAttrackOutTime = 0.2f,
                //EffectMoveEndPosTime = 0.4f,
            });
            GridEnterEffect gridEnterEffect = mCore.GridEffects.GetEffect(GameEffects.EffectEnter) as GridEnterEffect;
            gridEnterEffect.EndValueOffset = m_GridEnterOffset;//设置消除格下落的起点偏移量

            mCore.GridOperater.ElimlnateEffect = new ElimlnateEffectBlast(elimlnateName)
            {
                OnInit = ElimlnateEffectInit,
                OnCompleted = ElimlnateEffectCompleted,
                FillParamBeforeStart = FillParamBeforeEffectStart
            };
#endif
        }

        private void FillParamBeforeEffectStart(BatchEffect effect)
        {
            //INoticeBase<int> notifyResult = Modulars.NotifyModular(Consts.N_CREATE_MAIN_ROLE_ATK_INFO);
            //IParamNotice<AttackEffectInfo> notice = notifyResult as IParamNotice<AttackEffectInfo>;
            //AttackEffectInfo info = notice.ParamValue;

            //int max = effect.GridCount;
            //ElimlnateGrid item;
            //GridEffectParam param;
            //string name = effect.CurGridEffectName;
            //for (int i = 0; i < max; i++)
            //{
            //    item = effect.GetGrid(i);
            //    param = item.GetEffectParam<GridEffectParam>(name);
            //    int type = item.GridShapeIndex + 1;
            //    if (param != default)
            //    {
            //        param.EndPosition = info.GetPos(type);
            //    }
            //    else { }
            //}
            //mGridElimlnateWaves += 1.2f;
        }

        private void ElimlnateEffectCompleted()
        {
            //UpdaterNotice.SceneCallLater(ShowMainRoleAtkEffect);
        }

        private void ElimlnateEffectInit()
        {
        }

        private void InitGameCore()
        {
            #region 初始化三消玩法核心
            int xSize = m_GridsRowAndCol.x;
            int ySize = m_GridsRowAndCol.y;

            mCore = ElimlnateCore.Instance;
            mCore.GridsContainer = m_GridsContainer;
            mCore.CreateBoard(xSize, ySize, OnAllGridDeactivedEnd, OnRearrangerGrids);//棋盘核心控制器

            mCore.InitGridCreater(m_CellSize, m_EnterEffectDuringTime, m_EffectCurve, m_TileResRaw, OnGetGridTypeDuringCreate);
            mCore.InitGridCreaterCallback(BeforeGridCreate, CreateGridOperateUI, GridCreateCompleted);//消除格核心控制器

            mCore.CreateOperater(new GamingResult(), m_IsInclined, m_ShouldLineMin, m_ShouldComboLineMax);//消除格操作核心控制器
            mCore.DestroyGrid = OnGridDestroy;

            string weigts = "[[1,20],[2,20],[3,20],[4,20],[5,20]]";
            InitGridTypeLibs(weigts);//消除格类型库
            #endregion
        }

        private void OnGridDestroy(GameObject target)
        {
            Destroy(target, 0.5f);
        }

        private void GridCreateCompleted(int id)
        {
            mGridCompMapper[id].Init();
            mGridCompMapper.Remove(id);

            if (mGridCompMapper.Size == 0)
            {
                mCore.ActiveInput();
            }
            else { }
        }

        private void CreateGridOperateUI(ElimlnateGrid grid)
        {
            //mBattleData.SetGridOperateUIWillCraete(grid);
            //mBattleData.DataNotify(Consts.DN_CREATE_GRID_OPERATE_UI);
            //mBattleData.SetGridOperateUIWillCraete(default);
            mCore.Data.DispatchWithParam(EliminateData.N_CREATE_GRID_OPERATE_UI, grid);
        }

        private int BeforeGridCreate(GameObject target)
        {
            GridComponent comp = target.GetComponent<GridComponent>();
            int id = comp.GetInstanceID();
            mGridCompMapper[id] = comp;
            return id;
        }

        private int OnGetGridTypeDuringCreate(int index)
        {
            return mGridTypesInBoard[index];
        }

        /// <summary>
        /// 当消除格出现无法消除的情况时的回调函数
        /// </summary>
        /// <param name="list"></param>
        private void OnRearrangerGrids(List<GridRearrangerInfo> list)
        {
            int max = list.Count;
            if (max > 0)
            {
                //Sequence seq = DOTween.Sequence();
                for (int i = 0; i < max; i++)
                {
                    //seq.Append();
                    list[i].grid.GridTrans.DOMove(list[i].moveEnd, 0.3f)
                        .OnComplete(list[i].UpdateGridData);
                }
            }
            else { }
        }

        /// <summary>
        /// 当所有消除格重置为未激活状态后触发的回调函数
        /// </summary>
        /// <param name="grid"></param>
        private void OnAllGridDeactivedEnd(ElimlnateGrid grid)
        {
            //mBattleData.DataNotify(Consts.DN_GRID_OPERATE_UI_INPUT_END);
            if (grid.GridSprite != default)
            {
                Color color = grid.GridSprite.color;
                color.a = 1f;
                grid.GridSprite.color = color;
            }
            else { }
        }

        private void InitGridTypeLibs(string girdWeights)
        {
            GridTypes gridTypes = mCore.GridTypes;
            gridTypes.NormalGridType = m_NormalGridType;

            TDData td = DataParser.ParseParamToTD(ref girdWeights);

            int weightCount = td.Source.Count;
            KeyValueList<int, int> weightMapper = new KeyValueList<int, int>();
            for (int i = 0; i < weightCount; i++)
            {
                int gridType = td.GetIntValue(i, 0);
                int weight = td.GetIntValue(i, 1);
                weightMapper[gridType] = weight;//定义权重
            }

            GridTypeItem value;
            int max = m_GridTypeItems.Count, key;
            for (int i = 0; i < max; i++)
            {
                value = m_GridTypeItems[i];
                key = value.id;
                string abName = value.prefabABName;
                string assetName = value.prefabResName;
                GameObject creater()
                {
                    AssetBundles abs = Framework.Instance.GetUnit<AssetBundles>(Framework.UNIT_AB);
                    GameObject result = abs.Get<GameObject>(abName, assetName);
                    result = Instantiate(result);
                    return result;
                };

                if (weightCount > 0)
                {
                    int weight = 1;
                    if (key == gridTypes.NormalGridType)
                    {
                        weight = weightCount > 0 ? 0 : 1;//如果包含消除格类型的权重配置则不使用自动的普通消除格类型值
                        gridTypes.SetGridCreater(key, value.isSpriteAsset, creater, weight);
                        "log:Grid type is {0}, ab name is {1}, res name is {2}, asset static is ({3})".Log(key.ToString(), abName, assetName, value.isSpriteAsset.ToString());

                        int m = weightCount;
                        for (int n = 0; n < m; n++)
                        {
                            key = weightMapper.Keys[n];
                            weight = weightMapper[key];
                            gridTypes.SetGridCreater(key, value.isSpriteAsset, creater, weight, gridTypes.NormalGridType);//根据权重配置向全局类型库添加普通消除格类型
                            "log:Grid type is {0}, ab name is {1}, res name is {2}, asset static is ({3})".Log(key.ToString(), abName, assetName, value.isSpriteAsset.ToString());
                        }
                    }
                    else
                    {
                        gridTypes.SetGridCreater(key, value.isSpriteAsset, creater, 0);
                    }
                }
                else
                {
                    int weight = value.isInvalidRandom ? 0 : 1;
                    gridTypes.SetGridCreater(key, value.isSpriteAsset, creater, weight);
                    //gridTypes.AddInvalidWeightGrid(0);
                    //gridTypes.AddInvalidWeightGrid(2);
                    "log:Grid type is {0}, ab name is {1}, res name is {2}, asset static is ({3})".Log(key.ToString(), abName, assetName, value.isSpriteAsset.ToString());
                }
            }
        }

        private void InitGridsData()
        {
            string data = GetGridsData(), rowData;
            string[] splits = data.Split('#');

            string[] types;
            char spliter = StringUtils.SPLIT_CHAR;
            int max = splits.Length, type;
            mGridTypesInBoard = new List<int>();
            for (int i = 0; i < max; i++)
            {
                rowData = splits[i];
                types = rowData.Split(spliter);
                int n = types.Length;
                for (int j = 0; j < n; j++)
                {
                    bool flag = int.TryParse(types[j], out type);
                    if (flag)
                    {
                        mGridTypesInBoard.Add(type);
                    }
                    else { }
                }
            }
        }

        private string GetGridsData()
        {
            return "1,1,1,1,1,1,1#" +
                    "1,1,1,1,1,1,1#" +
                    "1,1,1,1,1,1,1#" +
                    "1,1,1,1,1,1,1#" +
                    "1,1,1,1,1,1,1#" +                    
                    "1,1,1,1,1,1,1#" +
                    "1,1,1,1,1,1,1#";
        }

        public void SetStageBg(Sprite bg)
        {
            m_StageBgRenderer.sprite = bg;
        }

        public void SetBoardBg(Sprite bg)
        {
            m_BoardBgRenderer.sprite = bg;
        }
    }

}