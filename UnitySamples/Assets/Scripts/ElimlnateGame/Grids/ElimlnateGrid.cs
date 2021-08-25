#define _LOG_GET_EFFECT_PARAM_NULL_EFFECT
#define _LOG_GRID_SHAPE_RES_NULL

using ShipDock;
using ShipDock.Applications;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Elimlnate
{
    public class GridShapeType
    {
        public string abName;
        public string assetName;
        public string aniName;
        public int normalShapeRelated;
    }

    public abstract class ElimlnateGrid : INotificationSender
    {
        /// <summary>消除游戏核心对象</summary>
        protected ElimlnateCore mCore;
        /// <summary>销毁后的回调函数</summary>
        protected UnityAction<ElimlnateGrid> mDestroyCallback;
        /// <summary>效果映射</summary>
        protected KeyValueList<string, GridEffect> mEffectMapper;

        /// <summary>背景的排序层</summary>
        private int mBgLayerBak;
        /// <summary>消除格上图的排序层</summary>
        private int mGridLayerBak;
        /// <summary>背景色</summary>
        private Color mBgColor;
        /// <summary>本地缩放存放值</summary>
        private Vector3 mLocalScale;
        /// <summary>背景渲染组件</summary>
        private SpriteRenderer mGridBg;
        /// <summary>图渲染组件</summary>
        private SpriteRenderer mGridSprite;
        /// <summary>连线渲染器组件</summary>
        private LineRenderer mLineRenderer;
        /// <summary>静态图库</summary>
        private GridShapeType[] mShapeTypes;

        private List<string> mEffectActived = new List<string>();

        /// <summary>消除格id</summary>
        public int GridID { get; set; }
        /// <summary>图形的图案序号，从 0 开始</summary>
        public int GridShapeIndex { get; private set; }
        /// <summary>消除格以普通类型划分时的图案序号，从 1 开始</summary>
        public int NormalGridType { get; private set; }
        /// <summary>是否可被连线</summary>
        public bool ShouldLine { get; set; }
        /// <summary>是否可作为连线起点</summary>
        public bool ShouldLineAsFirst { get; set; } = true;
        /// <summary>是否参与消除后的变动</summary>
        public bool IsObstacle { get; set; }
        /// <summary>是否为当前可以被选中的图形</summary>
        public bool IsActive { get; set; }
        /// <summary>是否为技能型消除格</summary>
        public bool HasGridSkill { get; set; }
        /// <summary>是否已销毁</summary>
        public bool IsDestroyed { get; protected set; }
        /// <summary>消除格行列坐标</summary>
        public Vector2Int PrevGridPos { get; private set; }
        public Vector2Int GridPos { get; private set; }
        /// <summary>图形的类型</summary>
        public int GridType { get; set; } = 0;
        /// <summary>图形技能的触发器</summary>
        public ISkillGridTrigger ShapeSkillTrigger { get; protected set; }
        /// <summary>消除格变换组件</summary>
        public Transform GridTrans { get; set; }
        /// <summary>消除格碰撞体</summary>
        public BoxCollider GridCollider { get; set; }
        /// <summary>消除格资源列表</summary>
        public Sprite[] GridSpriteList { get; private set; }
        /// <summary>是否可设置连线的颜色</summary>
        public bool ApplyLineColor { get; set; }
        /// <summary>是否已被标记为待消除</summary>
        public bool IsWillElimlnate { get; private set; }
        /// <summary>获取消除格图库</summary>
        public Action<ElimlnateGrid, int> OnGetGridShapeAssets { get; set; }
        /// <summary>节点子集依赖层</summary>
        public IndependentChildren IndependentChildren { get; private set; }

        /// <summary>图库</summary>
        public GridShapeType[] GridShapeTypes
        {
            set
            {
                mShapeTypes = value;

                int max = value.Length;
                CreateGridAssetPool(max);

                AssetBundles abs = Framework.Instance.GetUnit<AssetBundles>(Framework.UNIT_AB);
                string key = string.Empty, abName;
                for (int i = 0; i < max; i++)
                {
                    if (mShapeTypes[i] != default)
                    {
                        abName = mShapeTypes[i].abName;
                        key = mShapeTypes[i].assetName;
                        if (!string.IsNullOrEmpty(key))
                        {
                            AddGridAssetToPool(ref abs, ref abName, ref key, i);
                        }
                        else { }
                    }
                    else { }
                }
            }
            protected get
            {
                return mShapeTypes;
            }
        }

        /// <summary>绘线渲染器组件</summary>
        public LineRenderer LineRenderer
        {
            get
            {
                return mLineRenderer;
            }
            set
            {
                mLineRenderer = value;
                InitLineRenderer();
            }
        }

        /// <summary>消除格本地缩放</summary>
        public Vector3 GridLocalScale
        {
            set
            {
                mLocalScale = value;
                mGridSprite.transform.localScale = mLocalScale;
            }
        }

        public Vector3 GridLocalPosition
        {
            set
            {
                mGridSprite.transform.localPosition = value;
            }
        }

        /// <summary>消除格背景显示的图</summary>
        public SpriteRenderer GridBg
        {
            set
            {
                mGridBg = value;
                mBgColor = mGridBg.color;
                mBgLayerBak = mGridBg.sortingOrder;
            }
            get
            {
                return mGridBg;
            }
        }

        /// <summary>消除格上显示的图</summary>
        public SpriteRenderer GridSprite
        {
            set
            {
                mGridSprite = value;
                mGridLayerBak = mGridSprite.sortingOrder;
            }
            get
            {
                return mGridSprite;
            }
        }

        public ElimlnateGrid()
        {
            mCore = ElimlnateCore.Instance;
            mEffectMapper = mCore.GridEffects.GetEffectsMapper();
            string[] list = GetNamesOfPreparedEffect();
            int max = list.Length;
            string temp;
            GridEffect item;
            for (int i = 0; i < max; i++)
            {
                temp = list[i];
                item = mEffectMapper[temp];
                item?.Commit(this, false);
            }
            IndependentChildren = new IndependentChildren(GridTrans);
        }

        protected virtual void Purge() { }

        public void Dispose()
        {
            if (!IsDestroyed)
            {
                IsDestroyed = true;
                CancelLineShow();

                Purge();

                ShapeSkillTrigger?.Clean();

                List<GridEffect> effects = mEffectMapper.Values;
                int max = effects.Count;
                for (int i = 0; i < max; i++)
                {
                    effects[i].Remove(this);
                }

                Sprite[] list = GridSpriteList;
                Utils.Reclaim(ref list);

                Action<GameObject> destroy = mCore.DestroyGrid;
                if (GridTrans != default)
                {
                    GridTrans.gameObject.SetActive(false);
                    if (destroy == default)
                    {
                        UnityEngine.Object.Destroy(GridTrans.gameObject, 0.5f);
                    }
                    else
                    {
                        destroy.Invoke(GridTrans.gameObject);
                    }
                }
                else { }

                mCore.DestroyGrid = default;
                mGridBg = default;
                mGridSprite = default;
                ShapeSkillTrigger = default;
                mEffectMapper = default;
                mDestroyCallback = default;
                mCore = default;
            }
            else { }
        }

        protected virtual void InitLineRenderer() { }

        /// <summary>是否未普通消除格类型</summary>
        public bool IsNormalGrid()
        {
            return (mCore != default) &&
                (mCore.GridTypes != default) &&
                (GridType == mCore.GridTypes.NormalGridType);
        }

        protected virtual void CreateGridAssetPool(int max)
        {
            GridSpriteList = new Sprite[max];
        }

        protected virtual void AddGridAssetToPool(ref AssetBundles abs, ref string abName, ref string assetName, int index)
        {
            Sprite gridSprite = abs.Get<Sprite>(abName, assetName);
#if LOG_GRID_SHAPE_RES_NULL
            "error:gridSprite is null. ab name is {0}, res name is {1}".Log(gridSprite == default, abName, abName);
            "log:Add grid asset to pool abName is {0}, asset name is {1}, index is {2}".Log(abName, assetName, index.ToString());
#endif
            GridSpriteList[index] = gridSprite;
        }

        protected virtual string[] GetNamesOfPreparedEffect()
        {
            return new string[] { };
        }

        /// <summary>
        /// 初始化图形
        /// </summary>
        /// <param name="info">创建消除格时的信息</param>
        public virtual void SetCreateInfo(GridCreateInfo info)
        {
            if (!string.IsNullOrEmpty(info.enterEffectName))
            {
                StartEffect(info.enterEffectName);
            }
            else { }
            mGridSprite.transform.localScale = mLocalScale;
        }

        /// <summary>
        /// 追加特效（其他消除格将共享此追加的特效）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="effect"></param>
        public void AddEffect(string name, GridEffect effect)
        {
            mEffectMapper[name] = effect;
            effect.Commit(this, false);
        }

        public void StartEffect(string name, bool isUnique = false)
        {
            if (IsDestroyed)
            {
                return;
            }
            else { }

            if (mEffectActived.Contains(name))
            {
                if (isUnique)
                {
                    return;
                }
                else { }
            }
            else
            {
                mEffectActived.Add(name);
            }
            GridEffect effect = mEffectMapper[name];
            effect?.Commit(this);
        }

        public void StopEffect(string name)
        {
            if (mEffectActived.Contains(name))
            {
                mEffectActived.Remove(name);
            }
            else
            {
                return;
            }

            GridEffect effect = mEffectMapper[name];
            effect?.Stop(this);
        }

        public void StopEffect(params string[] name)
        {
            int max = name.Length;
            for (int i = 0; i < max; i++)
            {
                StopEffect(name[i]);
            }
        }

        public GridEffect GetEffect(string name)
        {
            return mEffectMapper != default ? mEffectMapper[name] : default;
        }

        public T GetEffectParam<T>(string name) where T : GridEffectParam
        {
            if (mEffectMapper == default)
            {
                return default;
            }
            else { }

            bool hasEffect = (mEffectMapper != default) && mEffectMapper.ContainsKey(name);
            GridEffect effect = mEffectMapper[name];
#if LOG_GET_EFFECT_PARAM_NULL_EFFECT
            "error: GetEffectParam - effect is null, name is {0}".Log(effect == default, name);
#endif
            T result = hasEffect ? (T)effect.GetEffectParam(this) : default;
            if ((result == default) && hasEffect)
            {
                mEffectMapper[name].Commit(this, false);
                result = (T)mEffectMapper[name].GetEffectParam(this);
            }
            else { }

            return result;
        }

        public void RemoveEffect(string name)
        {
            GridEffect effect = GetEffect(name);
            effect?.Remove(this);
        }

        /// <summary>
        /// 是否可以连接至目标单元格
        /// </summary>
        /// <param name="targetGrid"></param>
        /// <returns></returns>
        public virtual bool ShouldLineToTarget(ref ElimlnateGrid targetGrid)
        {
            return targetGrid != default && GridShapeIndex == targetGrid.GridShapeIndex;
        }

        /// <summary>
        /// 设置当前图形的坐标位置
        /// </summary>
        public void SetGridPos(Vector2Int pos)
        {
            PrevGridPos = GridPos;
            GridPos = pos;
        }

        /// <summary>
        /// 设置当前图形的坐标位置
        /// </summary>
        public void SetGridPos(int col, int row)
        {
            PrevGridPos = GridPos;
            GridPos = new Vector2Int(col, row);
        }

        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="girdLayer"></param>
        public void SetItemSortingLayer(int girdLayer)
        {
            if (mGridSprite == default || GridBg == default)
            {
                return;
            }
            else { }

            mGridSprite.sortingOrder = girdLayer;
            GridBg.sortingOrder = girdLayer - 1;
        }

        /// <summary>
        /// 设置碰撞体大小
        /// </summary>
        /// <param name="size"></param>
        public void SetColliderSize(Vector2 size)
        {
            GridCollider.size = size;
        }

        /// <summary>
        /// 设置背景颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetBgColor(Color color)
        {
            if (GridBg != default)
            {
                GridBg.color = color;
            }
            else { }
        }

        /// <summary>
        /// 销毁当前图形事件
        /// </summary>
        /// <param name="callback"></param>
        public virtual void WillDestroy()
        {
            mCore.BoardGrids.SetGridMapper(GridPos, this, true);
            DropGrid();
        }

        protected virtual void DropGrid()
        {
            mDestroyCallback?.Invoke(this);
            Dispose();
        }

        public void AddDestroyCallback(UnityAction<ElimlnateGrid> callback)
        {
            mDestroyCallback += callback;
        }

        public virtual void TriggerSkill() { }

        /// <summary>
        /// 线特效显示
        /// </summary>
        /// <param name="prePos"></param>
        public virtual void SetLineShow(Vector3 prePos, float lineWidth, Color lineColor)
        {
            IsWillElimlnate = true;
            LineRenderer.enabled = true;
            LineRenderer.positionCount = 2;
            if (ApplyLineColor)
            {
                LineRenderer.startColor = lineColor;
                LineRenderer.endColor = lineColor;
            }
            else { }

            LineRenderer.startWidth = lineWidth;
            LineRenderer.endWidth = lineWidth;

            int sortingLayer = mCore.LineInputer.GetSortingOrder(LineInputCore.SORT_LAYER_INDEX_LINE_SHOW);
            LineRenderer.sortingOrder = sortingLayer;

            float halfW = lineWidth * 0.25f;
            Vector3 pos = new Vector3(prePos.x - halfW, prePos.y - halfW, 0f);
            LineRenderer.SetPosition(0, pos);

            pos = GridTrans.position;
            pos = new Vector3(pos.x - halfW, pos.y - halfW, 0f);
            LineRenderer.SetPosition(1, pos);
        }

        public virtual void CancelLineShow()
        {
            IsWillElimlnate = false;

            if (LineRenderer != default)
            {
                LineRenderer.enabled = false;
            }
        }

        public void SetLineMaterial(ref Material material)
        {
            LineRenderer.material = material;
        }

        public void SetLineMaterial(int index)
        {
            LineRenderer.material = LineRenderer.sharedMaterials[index];
        }

        /// <summary>
        /// 修改当前图形
        /// </summary>
        /// <param name="index"></param>
        public virtual void ChangeSprite(int index)
        {
            int total = GridSpriteList.Length;
            if (total > 0)
            {
                Sprite target;
                if (index >= 0 && index < total)
                {
                    target = GridSpriteList[index];
                }
                else
                {
                    index = total - 1;
                    target = GridSpriteList[index];
                }
                GridSprite.sprite = target;
                SetGridShapeIndex(index);
            }
            else { }
        }

        public virtual void ResetShapeItem()
        {
            if (LineRenderer != default)
            {
                LineRenderer.positionCount = 0;
            }
            else { }

            if ((GridBg != default) && (mGridSprite != default))
            {
                GridBg.sortingOrder = mBgLayerBak;
                mGridSprite.sortingOrder = mGridLayerBak;
            }
            else { }
        }

        public void SetGridShapeIndex(int shapeIndex)
        {
            GridShapeIndex = shapeIndex;
            NormalGridType = shapeIndex + 1;
        }

        /// <summary>
        /// 以消除格的图形库（非全局的消除类型库）为范围随机设置一个图形
        /// </summary>
        public void SetRandomShapeIndex()
        {
            if (GridSpriteList != default && GridSpriteList.Length > 0)
            {
                int index = UnityEngine.Random.Range(0, GridSpriteList.Length);
                ChangeSprite(index);
            }
            else { }
        }

        public void SetShapeSkillTrigger(ISkillGridTrigger skillGridTrigger)
        {
            HasGridSkill = skillGridTrigger != default;
            ShapeSkillTrigger = skillGridTrigger;
        }

        public abstract bool ShouldGridContinuity(ref ElimlnateGrid grid);

        public void AlignmentInGrid(Transform target, bool isInDependent, bool isChildOfShape = true, bool isVisible = true)
        {
            if (isInDependent)
            {
                IndependentChildren.AddChild(target);
            }
            else { }

            if (isChildOfShape)
            {
                target.gameObject.SetChildOf(GridSprite.transform);
                target.SetParent(GridTrans);
            }
            else
            {
                target.gameObject.SetChildOf(GridTrans);
            }
            target.gameObject.SetActive(isVisible);
        }
    }
}
