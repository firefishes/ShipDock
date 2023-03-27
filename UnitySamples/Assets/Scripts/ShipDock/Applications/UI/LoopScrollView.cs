using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShipDock
{
    /// <summary>
    /// 
    /// UI 循环列表组件
    /// 
    /// add by Peng.jia
    /// modified by Minghua.ji
    /// 
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class LoopScrollView : MonoBehaviour
    {
        /// <summary>有效的起始索引号</summary>
        private const int INVALIID_START_INDEX = -1;

        /// <summary>缓存的条目渲染器数量，推荐使用与每屏能显示的条目渲染器数相同值</summary>
        [SerializeField]
        private int m_CacheCount = 4;
        /// <summary>滚动内容的水平锚点</summary>
        [SerializeField]
        private Rect m_ContentAnchorH = new Rect(0f, 0f, 0f, 1f);
        /// <summary>滚动内容的垂直锚点</summary>
        [SerializeField]
        private Rect m_ContentAnchorV = new Rect(0.5f, 1f, 0.5f, 1f);
        /// <summary>渲染条目的中心点</summary>
        [SerializeField]
        private Vector2 m_ItemPivot = new Vector2(0.5f, 1f);
        /// <summary>条目渲染器对象的锚点区间最小值</summary>
        [SerializeField]
        private Vector2 m_ItemAnchorMin = new Vector2(0.5f, 1f);
        /// <summary>条目渲染器对象的锚点区间最大值</summary>
        [SerializeField]
        private Vector2 m_ItemAnchorMax = new Vector2(0.5f, 1f);
        /// <summary>条目渲染器模板组件类</summary>
        [SerializeField]
        private LoopScrollItem m_PrefabItem;

        private int mMaxCount;
        private int mDataCount;
        private int mCreateCount;
        private int mStartIndex;
        private float mItemX;
        private float mItemY;
        private float mContentH;
        private float mContentW;
        private float mCellPadding;
        private Vector2 mCellSize;
        private Vector2 mScrollRectSize;
        private ScrollRect mScrollRect;
        private RectTransform mContentRectTrans;
        private LoopScrollItem mPrefabItem;
        private LoopScrollItem mUpdateItem;
        private LoopScrollItem mChangingItem;
        private List<LoopScrollItem> mItemList;
        private Queue<LoopScrollItem> mItemQueue;
        private Dictionary<LoopScrollItem, int> mItemIndexDic;
        private Action<LoopScrollItem, int> mUpdateCell;
        private List<ILoopScrollItemData> mItemData;

        private void OnDestroy()
        {
            mScrollRect = default;
            mContentRectTrans = default;
            mUpdateCell = default;
            mUpdateItem = default;
            m_PrefabItem = default;
            mPrefabItem = default;
            mChangingItem = default;

            Utils.Reclaim(ref mItemData);
            Utils.Reclaim(ref mItemList);
            Utils.Reclaim(ref mItemQueue);
            Utils.Reclaim(ref mItemIndexDic);
        }

        public void ResetView()
        {
            mUpdateCell = default;

            LoopScrollItem itemRenderer;
            int max = mItemList.Count;
            for (int i = 0; i < max; i++)
            {
                itemRenderer = mItemList[i];
                Destroy(itemRenderer.gameObject);
            }

            Utils.Reclaim(ref mItemData, false);
            Utils.Reclaim(ref mItemList, false);
            Utils.Reclaim(ref mItemQueue, false);
            Utils.Reclaim(ref mItemIndexDic, false);
        }

        public void InitView(ref List<ILoopScrollItemData> infos, LoopScrollItem itemRenderer, float padding = 0f, Action<LoopScrollItem, int> onUpdateCell = default)
        {
            mItemData = infos;
            mUpdateCell += UpdateItemInfo;

            if (onUpdateCell != default)
            {
                mUpdateCell += onUpdateCell;
            }
            else { }

            Show(infos.Count, itemRenderer, padding);
        }

        private void UpdateItemInfo(LoopScrollItem item, int index)
        {
            ILoopScrollItemData info = mItemData[index];
            info.FillInfoToItem(ref item);
        }

        private void Show(int dataCount, LoopScrollItem itemRenderer, float padding = 0f)
        {
            mDataCount = dataCount;
            mCellPadding = padding;

            mPrefabItem = (m_PrefabItem == default && itemRenderer != default) ? itemRenderer : m_PrefabItem;

            mItemList = new List<LoopScrollItem>();
            mItemQueue = new Queue<LoopScrollItem>();
            mItemIndexDic = new Dictionary<LoopScrollItem, int>();

            if (mScrollRect == default)
            {
                mScrollRect = GetComponent<ScrollRect>();
                mContentRectTrans = mScrollRect.content;
            }
            else { }

            mScrollRectSize = mScrollRect.GetComponent<RectTransform>().sizeDelta;
            mCellSize = itemRenderer.GetComponent<RectTransform>().sizeDelta;

            mStartIndex = 0;
            mCreateCount = 0;
            mMaxCount = GetMaxCount();

            if (mScrollRect.horizontal)
            {
                mContentRectTrans.anchorMin = new Vector2(m_ContentAnchorH.x, m_ContentAnchorH.y);
                mContentRectTrans.anchorMax = new Vector2(m_ContentAnchorH.width, m_ContentAnchorH.height);
            }
            else
            {
                mContentRectTrans.anchorMin = new Vector2(m_ContentAnchorV.x, m_ContentAnchorV.y);
                mContentRectTrans.anchorMax = new Vector2(m_ContentAnchorV.width, m_ContentAnchorV.height);
            }
            mScrollRect.onValueChanged.RemoveAllListeners();
            mScrollRect.onValueChanged.AddListener(OnValueChanged);
            ResetSize(dataCount);
        }

        public void ResetSize(int dataCount)
        {
            mDataCount = dataCount;
            mContentRectTrans.sizeDelta = GetContentSize();

            LoopScrollItem item;
            for (int i = mItemList.Count - 1; i >= 0; i--)
            {
                item = mItemList[i];
                RecoverItem(item);
            }
            
            mCreateCount = Mathf.Min(dataCount, mMaxCount);
            for (int i = 0; i < mCreateCount; i++)
            {
                CreateItem(i);
            }

            mStartIndex = -1;
            mContentRectTrans.anchoredPosition = Vector3.zero;
            OnValueChanged(Vector2.zero);
        }

        public void UpdateList()
        {
            if (mUpdateCell != default)
            {
                int max = mItemList == default ? 0 : mItemList.Count;
                for (int i = 0; i < max; i++)
                {
                    mUpdateItem = mItemList[i];
                    int index = mItemIndexDic[mUpdateItem];
                    mUpdateCell?.Invoke(mUpdateItem, index);
                }
            }
            else { }
        }

        private void CreateItem(int index)
        {
            LoopScrollItem item;
            RectTransform rect;
            if (mItemQueue.Count > 0)
            {
                item = mItemQueue.Dequeue();
                mItemIndexDic[item] = index;
                item.gameObject.SetActive(true);

                rect = item.transform as RectTransform;
            }
            else
            {
                item = Instantiate(m_PrefabItem, mContentRectTrans.transform);
                mItemIndexDic.Add(item, index);

                rect = item.transform as RectTransform;
                rect.pivot = m_ItemPivot;
                rect.anchorMin = m_ItemAnchorMin;
                rect.anchorMax = m_ItemAnchorMax;
                item.transform.localScale = Vector3.one;
                rect.anchoredPosition = Vector3.zero;
            }

            mItemList.Add(item);

            rect.anchoredPosition = GetPosition(index);
            mUpdateCell(item, index);
        }

        private void RecoverItem(LoopScrollItem item)
        {
            item.gameObject.SetActive(false);
            mItemList.Remove(item);
            mItemQueue.Enqueue(item);
            mItemIndexDic[item] = INVALIID_START_INDEX;
        }

        private void OnValueChanged(Vector2 vec)
        {
            int curmStartIndex = GetmStartIndex();
            if ((mStartIndex != curmStartIndex) && (curmStartIndex > INVALIID_START_INDEX))
            {
                mStartIndex = curmStartIndex;

                for (int i = mItemList.Count - 1; i >= 0; i--)
                {
                    mChangingItem = mItemList[i];
                    int index = mItemIndexDic[mChangingItem];
                    if (index < mStartIndex || index > (mStartIndex + mCreateCount - 1))
                    {
                        RecoverItem(mChangingItem);
                    }
                    else { }
                }

                for (int i = mStartIndex; i < mStartIndex + mCreateCount; i++)
                {
                    if (i >= mDataCount)
                    {
                        break;
                    }
                    else { }

                    bool isExist = false;
                    for (int j = 0; j < mItemList.Count; j++)
                    {
                        mChangingItem = mItemList[j];
                        int index = mItemIndexDic[mChangingItem];
                        if (index == i)
                        {
                            isExist = true;
                            break;
                        }
                        else { }
                    }
                    if (isExist)
                    {
                        continue;
                    }
                    else { }

                    CreateItem(i);
                }
            }
        }

        private int GetMaxCount()
        {
            if (mScrollRect.horizontal)
            {
                return Mathf.CeilToInt(mScrollRectSize.x / (mCellSize.x + mCellPadding)) + m_CacheCount;
            }
            else
            {
                return Mathf.CeilToInt(mScrollRectSize.y / (mCellSize.y + mCellPadding)) + m_CacheCount;
            }
        }

        private int GetmStartIndex()
        {
            if (mScrollRect.horizontal)
            {
                return Mathf.FloorToInt(-mContentRectTrans.anchoredPosition.x / (mCellSize.x + mCellPadding));
            }
            else
            {
                return Mathf.FloorToInt(mContentRectTrans.anchoredPosition.y / (mCellSize.y + mCellPadding));
            }
        }

        private Vector2 GetPosition(int index)
        {
            if (mScrollRect.horizontal)
            {
                mItemX = index * (mCellSize.x + mCellPadding);
                return new Vector2(mItemX, 0f);
            }
            else
            {
                mItemY = index * -(mCellSize.y + mCellPadding);
                return new Vector2(0f, mItemY);
            }
        }

        private Vector2 GetContentSize()
        {
            if (mScrollRect.horizontal)
            {
                mContentW = mCellSize.x * mDataCount + mCellPadding * (mDataCount - 1);
                mContentH = mContentRectTrans.sizeDelta.y;
            }
            else
            {
                mContentW = mContentRectTrans.sizeDelta.x;
                mContentH = mCellSize.y * mDataCount + mCellPadding * (mDataCount - 1);
            }
            return new Vector2(mContentW, mContentH);
        }
    }

}