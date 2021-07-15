﻿
using ShipDock.Applications;
using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    [ExecuteInEditMode]
    public class CustomAssetCoordinator : MonoBehaviour
    {
        [SerializeField]
        private CustomAssetCoordinatorInfo m_Info;
        [SerializeField]
        private List<CustomAssetComponent> m_Assets;
        [SerializeField]
        private bool m_SyncCusntomList;
        [SerializeField]
        private bool m_RemoveEmptyAssets;

        private ComponentBridge mBridge;
        private CustomAssetBundle mCustomAssetBundle;

        private void Awake()
        {
            if (Application.isPlaying && gameObject.activeSelf && enabled)
            {
                mCustomAssetBundle = new CustomAssetBundle();

                mBridge = new ComponentBridge(OnAppReady);
                mBridge.Start();
            }
            else { }
        }

        private void OnDestroy()
        {
            Utils.Reclaim(mBridge);
            Utils.Reclaim(mCustomAssetBundle);
            Utils.Reclaim(ref m_Assets);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (m_SyncCusntomList)
            {
                m_SyncCusntomList = false;

                m_Assets.Clear();
                CustomAssetComponent[] list = GetComponentsInChildren<CustomAssetComponent>();
                int max = list.Length;
                for (int i = 0; i < max; i++)
                {
                    m_Assets.Add(list[i]);
                }
            }
            else { }

            if (m_RemoveEmptyAssets)
            {
                m_RemoveEmptyAssets = false;
                List<CustomAssetComponent> list = new List<CustomAssetComponent>();
                int max = m_Assets.Count;
                for (int i = 0; i < max; i++)
                {
                    if (m_Assets[i] != default)
                    {
                        list.Add(m_Assets[i]);
                    }
                    else { }
                }
                m_Assets.Clear();
                m_Assets = list;
            }
            else { }
        }
#endif

        private void OnAppReady()
        {
            mCustomAssetBundle?.FillAssets(ref m_Assets);
            ICustomAssetBundle bundle = mCustomAssetBundle as ICustomAssetBundle;
            ShipDockApp.Instance.ABs.SetCustomBundles(ref bundle);
        }

        [ContextMenu("从信息体同步")]
        private void SyncFormInfo()
        {
            int max = m_Assets.Count;
            for (int i = 0; i < max; i++)
            {
                DestroyImmediate(m_Assets[i].gameObject);
            }
            m_Assets.Clear();

            var list = m_Info.assets;
            max = list.Count;
            for (int i = 0; i < max; i++)
            {
                GameObject target = new GameObject();
                CustomAssetComponent comp = target.AddComponent<CustomAssetComponent>();
                target.transform.SetParent(transform);

                var compInfo = m_Info.assets[i];
                comp.SyncFromInfo(ref compInfo, out var customAssets);

                int m = compInfo.assetItems.Count;
                for (int j = 0; j < m; j++)
                {
                    var assetInfo = compInfo.assetItems[j];
                    var asset = new CustomAsset();
                    asset.SyncFromInfo(ref assetInfo);
                    customAssets.Add(asset);
                }
            }
        }

        public void AddCustomAsset(CustomAssetComponent item)
        {
            if (!m_Assets.Contains(item))
            {
                m_Assets.Add(item);
            }
            else { }
        }

        [ContextMenu("写入信息体")]
        private void WriteToInfo()
        {
            var list = m_Assets;
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                var comp = m_Assets[i];
                CustomAssetComponentInfo compInfo;
                if (i < m_Info.assets.Count)
                {
                    compInfo = m_Info.assets[i];
                }
                else
                {
                    compInfo = new CustomAssetComponentInfo();
                    m_Info.assets.Add(compInfo);
                }
                comp.WriteToInfo(ref compInfo, out var customAssets);

                compInfo.assetItems?.Clear();
                compInfo.assetItems = new List<CustomAssetInfo>();

                int m = customAssets.Count;
                for (int j = 0; j < m; j++)
                {
                    var asset = customAssets[j];
                    var info = new CustomAssetInfo();
                    asset.WriteToInfo(ref info);
                    compInfo.assetItems.Add(info);
                }
            }
        }
    }

}