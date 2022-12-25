﻿using ShipDock.Notices;
using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{
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

        private bool mIsDestroyed;
        private ComponentBridge mCompBridge;

        private void Awake()
        {
            name = "AssetsPool";
            mIsDestroyed = false;
            transform.position = GameObjectReadyPos;

            mCompBridge = new ComponentBridge(OnInited);
            mCompBridge.Start();
        }

        private void OnDestroy()
        {
            Utils.Reclaim(mCompBridge);
            mIsDestroyed = true;
        }

        private void OnInited()
        {
            ShipDockConsts.NOTICE_APPLICATION_CLOSE.Add(OnAppClose);

            ShipDockApp.Instance.AssetsPooling.SetAssetsPoolComp(this);
        }

        private void OnAppClose(INoticeBase<int> obj)
        {
            if (mIsDestroyed)
            {
                return;
            }
            else { }

            GameObject item;
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                item = transform.GetChild(i).gameObject;
                Destroy(item);
            }
        }

        public void Get(GameObject target)
        {
            //if (target.transform.parent == transform)
            //{
            //    target.transform.SetParent(null);
            //}
            //else { }
            if (target.activeSelf)
            {

            }
            else
            {
                target.SetActive(true);
            }
            
        }

        public void Collect(GameObject target, bool visible = false)
        {
            target.transform.localPosition = GameObjectReadyPos;
            //target.transform.position = GameObjectReadyPos;
            //target.SetActive(false);
#if UNITY_EDITOR
            //if (m_ApplyPoolItemParent && target.transform.parent != transform)
            //{
            //    target.transform.SetParent(transform);
            //}
            //else { }
#endif
        }
    }

}