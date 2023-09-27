using System.Collections.Generic;
using UnityEngine;

namespace ShipDock
{
    [ExecuteInEditMode]
    public class CustomAssetComponent : MonoBehaviour
    {
        [SerializeField]
        private bool m_Valid;
        [SerializeField]
        private string m_BundleName;
        [SerializeField]
        private List<CustomAsset> m_Assets;

        private void Awake()
        {
#if UNITY_EDITOR
            Update();
#endif
        }

        private void OnDestroy()
        {
            Utils.Reclaim(ref m_Assets);
        }

        public string GetBundleName()
        {
            return m_BundleName;
        }

        public void SetBundleName(string abName)
        {
            m_BundleName = abName;
        }

        public void SetAssets(List<CustomAsset> assets)
        {
            m_Assets = assets;
        }

#if UNITY_EDITOR
        public void Valid()
        {
            m_Valid = true;
            Update();
        }

        private void Update()
        {
            if (m_Valid && !Application.isPlaying)
            {
                int max = m_Assets.Count;
                for (int i = 0; i < max; i++)
                {
                    if (m_Assets[i] != default && m_Assets[i].refresh != default)
                    {
                        m_Assets[i].refresh = false;
                        m_Assets[i].UpdateCustomAssetName();
                        m_Assets[i].SyncCustomAssetType();
                        break;
                    }
                }
                if (max == 1)
                {
                    name = m_BundleName.Append(": ", m_Assets[0].assetName);
                }
                else if (max > 1)
                {
                    name = m_BundleName.Append(": ", m_Assets[0].assetName, ", others ", max.ToString());
                }
                else
                {
                    name = m_BundleName.Append(" have nothing,. ", max.ToString());
                }
            }
        }
#endif

        public List<CustomAsset> Assets
        {
            get
            {
                return m_Valid ? m_Assets : default;
            }
        }
    }

}