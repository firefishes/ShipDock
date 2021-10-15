using ShipDock.Applications;
using ShipDock.Pooling;
using ShipDock.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static ShipDock.Pooling.AssetsPooling;

namespace ShipDock.UI
{
    /// <summary>
    /// 
    /// 界面节点控制器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class UINodeControl
    {
        /// <summary>资源对象池</summary>
        private AssetsPooling mResPooling;
        /// <summary>场景节点子组缓存</summary>
        private KeyValueList<string, SceneNodeSubgroup> mSceneNodesCache;

        /// <summary>UI界面节点</summary>
        public UINodes Nodes { get; private set; }

        public UINodeControl(UINodes nodes)
        {
            Nodes = nodes;
            mResPooling = ShipDockApp.Instance.AssetsPooling;
            mSceneNodesCache = new KeyValueList<string, SceneNodeSubgroup>();
        }

        public void Clear()
        {
            Nodes = default;
            mResPooling = default;
            mSceneNodesCache.Clear();
        }

        private void CheckAndFillSceneNodeCache(ref string keyName, out SceneNodeSubgroup sceneNode)
        {
            if (mSceneNodesCache.ContainsKey(keyName))
            {
                sceneNode = mSceneNodesCache[keyName];
            }
            else
            {
                sceneNode = Nodes.GetSceneNode(ref keyName);
                mSceneNodesCache[keyName] = sceneNode;
            }
        }

        public void SetNodeVisible(string keyName, bool value)
        {
            CheckAndFillSceneNodeCache(ref keyName, out SceneNodeSubgroup subgroup);
            if (subgroup.value != default)
            {
                subgroup.value.SetActive(value);
            }
            else { }
        }

        #region 按钮节点控制
        public void AddClickHandler(string keyName, UnityAction onClick)
        {
            CheckAndFillSceneNodeCache(ref keyName, out SceneNodeSubgroup subgroup);
            subgroup.button.onClick.AddListener(onClick);
        }

        public void RemoveClickHandler(string keyName, UnityAction onClick)
        {
            CheckAndFillSceneNodeCache(ref keyName, out SceneNodeSubgroup subgroup);
            subgroup.button.onClick.RemoveListener(onClick);
        }

        public void ReferenceButton(string keyName, out Button button)
        {
            CheckAndFillSceneNodeCache(ref keyName, out SceneNodeSubgroup subgroup);
            button = subgroup.button;
        }
        #endregion

        #region 文本节点控制
        public void SetLabelContent(string keyName, string content)
        {
            CheckAndFillSceneNodeCache(ref keyName, out SceneNodeSubgroup subgroup);
            subgroup.Label.text = content;
        }

        public void GetLabel(string keyName, out Text text)
        {
            CheckAndFillSceneNodeCache(ref keyName, out SceneNodeSubgroup subgroup);
            text = subgroup.Label;
        }
        #endregion

        #region 图片节点控制
        public void GetImage(string keyName, out Image image)
        {
            CheckAndFillSceneNodeCache(ref keyName, out SceneNodeSubgroup subgroup);
            image = subgroup.image;
        }

        public void SetImageSprite(string keyName, Sprite sprite)
        {
            CheckAndFillSceneNodeCache(ref keyName, out SceneNodeSubgroup subgroup);
            subgroup.image.overrideSprite = sprite;
        }
        #endregion

        #region 条目渲染器控制
        public void RefItemRenderer(string keyName, out GameObject itemRenderer)
        {
            CheckAndFillSceneNodeCache(ref keyName, out SceneNodeSubgroup subgroup);
            itemRenderer = subgroup.value;
        }

        public GameObject CreateItemRenderer(string keyName, int poolName = int.MaxValue, OnGameObjectPoolItem callback = default, bool visible = true)
        {
            RefItemRenderer(keyName, out GameObject raw);

            GameObject result;
            if (poolName != int.MaxValue)
            {
                result = mResPooling.FromPool(poolName, ref raw, callback, visible);
            }
            else
            {
                result = Object.Instantiate(raw);
            }
            return result;
        }

        public void RevertItemRenderer(GameObject result, int poolName = int.MaxValue)
        {
            if (poolName != int.MaxValue)
            {
                mResPooling.ToPool(poolName, result);
            }
            else
            {
                Object.Destroy(result);
            }
        }
        #endregion
    }
}