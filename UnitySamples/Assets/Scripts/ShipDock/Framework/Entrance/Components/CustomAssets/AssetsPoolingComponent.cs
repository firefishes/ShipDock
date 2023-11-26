using System;
using UnityEngine;

namespace ShipDock
{
    [Serializable]
    public struct AssetsPoolingDefaultLayer
    {
        public int poolName;
        public LayerMask defaultLayerMask;
    }

    [Serializable]
    public class AssetsPoolingDefaultLayers : SceneInfosMapper<int, AssetsPoolingDefaultLayer>
    {
        public override int GetInfoKey(ref AssetsPoolingDefaultLayer item)
        {
            return item.poolName;
        }
    }

    /// <summary>
    /// 
    /// 游戏对象池组件（单例组件）
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class AssetsPoolingComponent : MonoBehaviour
    {

        public static Vector3 GameObjectReadyPos = new Vector3(10000, 10000, 10000);

        [SerializeField]
        private bool m_ApplyPoolItemParent;
        [SerializeField]
        private bool m_ApplyUnvisibleLayer;
        [SerializeField]
        private LayerMask m_UnvisibleLayer;
        [SerializeField]
        private AssetsPoolingDefaultLayers m_DefaultLayers;

        private ComponentBridge mCompBridge;

        private void Awake()
        {
            name = "AssetsPool";
            transform.position = GameObjectReadyPos;

            m_DefaultLayers.ApplyMapper();
            m_DefaultLayers.InitInfos();

            Reinit();
        }

        private void Reinit()
        {
            mCompBridge = new ComponentBridge(OnInited);
            mCompBridge.Start();
        }

        private void Reset()
        {
            Reinit();
        }

        private void OnInited()
        {
            Utils.Reclaim(mCompBridge);

            Framework.Instance.OnFrameworkStartUp(OnAppStartUp);
        }

        private void OnAppStartUp(bool isStartUp)
        {
            if (isStartUp)
            {
                AssetsPooling assetsPooling = Framework.UNIT_ASSET_POOL.Unit<AssetsPooling>();
                assetsPooling.SetAssetsPoolComp(this);
            }
            else { }
        }

        private bool CheckAndFillDefaultLayer(int poolName, out AssetsPoolingDefaultLayer value)
        {
            value = default;
            return m_ApplyUnvisibleLayer && (poolName >= 0) && m_DefaultLayers.TryGetValue(poolName, out value);
        }

        public void Get(GameObject target, int poolName = -1)
        {
            UpdateTargetParent(ref target, false);

            if (CheckAndFillDefaultLayer(poolName, out AssetsPoolingDefaultLayer value))
            {
                target.layer = value.defaultLayerMask.value;
            }
            else { }
        }

        public void Collect(GameObject target, bool visible = false, int poolName = -1)
        {
            if (CheckAndFillDefaultLayer(poolName, out _))
            {
                target.layer = m_UnvisibleLayer.value;
            }
            else 
            {
                if (target.activeSelf != visible)
                {
                    target.SetActive(visible);
                }
                else { }
            }

            UpdateTargetParent(ref target, true);
        }

        private void UpdateTargetParent(ref GameObject target, bool isCollect)
        {
#if UNITY_EDITOR
            if (m_ApplyPoolItemParent && target != default)
            {
                Transform trans = target.transform;
                if (isCollect)
                {
                    if (trans.parent != transform)
                    {
                        trans.SetParent(transform);
                    }
                    else { }
                }
                else
                {
                    if (trans.parent == transform)
                    {
                        trans.SetParent(default);
                    }
                    else { }
                }
            }
            else { }
#endif
        }
    }

}