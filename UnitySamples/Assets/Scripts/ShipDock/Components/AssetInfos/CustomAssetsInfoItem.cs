using LitJson;
using ShipDock.Scriptables;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace ShipDock.Loader
{
    [Serializable]
    public class CustomAssetsInfoItem : ScriptableItem
    {
        public bool valid = true;

#if ODIN_INSPECTOR
        [ShowIf("@this.valid")]
#endif
        public CustomAssetInfo[] assets;

        public CustomAssetsInfoItem()
        {
            mIDEditEnabled = false;
            mTitleFieldName = "AssetBundleName";
        }

        public override void AutoFill()
        {
        }

        public void AddAssetFromDataRaw(JsonData jsonData)
        {
            JsonData item, otherAssetPaths;
            CustomAssetInfo info;
            int count = jsonData.Count;
            assets = new CustomAssetInfo[count];
            for (int i = 0; i < count; i++)
            {
                item = jsonData[i];

                info = new CustomAssetInfo
                {
                    assetName = item["assetName"].ToString(),
                    assetPath = item["assetPath"].ToString(),
                    assetType = (CustomAssetType)int.Parse(item["assetType"].ToString())
                };

                info.SetValid(bool.Parse(item["valid"].ToString()));

                otherAssetPaths = item["otherAssetPaths"];
                if (otherAssetPaths != default)
                {
                    int n = otherAssetPaths.Count;
                    for (int j = 0; j < n; j++)
                    {
                        info.RefreshAssetByAssetType(otherAssetPaths[j].ToString(), (CustomAssetType)j);
                    }
                }
                else { }

                assets[i] = info;
            }
        }

        public void Refresh()
        {
            CustomAssetInfo item;
            int max = assets.Length;
            for (int i = 0; i < max; i++)
            {
                item = assets[i];
                item.Refresh();
            }
        }

        public object GetJsonSource()
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Asset bundle name is null");
            }
            else { }

            CustomAssetsInfoItemSource result = new CustomAssetsInfoItemSource()
            {
                id = id,
                valid = valid,
                bundleName = name,
            };

            int max = assets.Length;
            result.assets = new CustomAssetInfoSource[max];

            if (!valid)
            {
                Debug.Log("Assets info item ".Append(name, " is invalided."));
            }
            else { }

            CustomAssetInfoSource infoItem;
            for (int i = 0; i < max; i++)
            {
                infoItem = assets[i].GetInfoJsonSource();
                result.assets[i] = infoItem;

                if (!infoItem.valid)
                {
                    Debug.Log("Assets info sub asset ".Append(infoItem.assetName, " is invalided."));
                }
                else { }
            }

            Debug.Log("Assets info item ".Append(name, " has saved."));

            return result;
        }

        protected override void OnEditEnabledChanged()
        {
            base.OnEditEnabledChanged();

            CustomAssetInfo item;
            int max = assets.Length;
            for (int i = 0; i < max; i++)
            {
                item = assets[i];
                item.SetEditable(EditEnabled);
            }
        }

        protected override bool ShouldNameEnabled()
        {
            return valid;
        }

        protected override bool ShouldEditChooseShow()
        {
            return valid;
        }
    }

    public class CustomAssetsInfoItemSource
    {
        public int id;
        public bool valid;
        public string bundleName;
        public CustomAssetInfoSource[] assets;
    }
}
