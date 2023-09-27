#define _G_LOG

using System;
using UnityEngine;

namespace ShipDock
{
    public class ResPrefabBridge : ResBridge, IResPrefabBridge
    {
        protected Action afterRawCreated { get; set; }

        protected override void Init()
        {
            base.Init();

            if (m_IsCreateInAwake)
            {
                CreateRaw();
                Instantiate(Prefab);
                afterRawCreated?.Invoke();
                afterRawCreated = default;
            }
            else { }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Prefab = default;
        }

        public void CreateRaw()
        {
            if ((m_Asset != default) && (Prefab == default))
            {
                if (Assets == default)
                {
                    Assets = ShipDockApp.Instance.ABs;
                }
                else { }

                string abName = m_Asset.GetABName();
                string assetName = m_Asset.GetAssetName();
                GameObject source = Assets.Get(abName, assetName);
                Prefab = source;

            }
            else { }
        }

        public GameObject CreateAsset(bool isCreateFromPool = false)
        {
#if LOG
            const string errorLog = "error: Res prefab bridge raw is null, ABName = {0}, AssetName = {1}";
            errorLog.Log(Prefab == default, m_Asset.GetABName(), m_Asset.GetAssetName());
#endif
            GameObject result = Prefab.Create(isCreateFromPool ? m_PoolID : int.MaxValue);
            return result;
        }

        public void CollectAsset(GameObject target)
        {
            target.Terminate(m_PoolID);
        }

        public void SetSubgroup(string ab, string asset)
        {
            m_Asset.SetSubgroup(ab, asset);
        }

        public void FillRaw(GameObject raw)
        {
            if (Prefab == default)
            {
                Prefab = raw;
            }
            else { }
        }

        public GameObject Prefab { get; private set; }
    }

}