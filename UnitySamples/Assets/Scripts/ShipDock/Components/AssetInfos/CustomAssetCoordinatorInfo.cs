using LitJson;
using ShipDock.Scriptables;
using UnityEditor;
using UnityEngine;

namespace ShipDock.Loader
{
    [CreateAssetMenu(fileName = "ShipDockAssetCoordinatorInfo", menuName = "ShipDock : 资源协调器信息", order = 100)]
    public class CustomAssetCoordinatorInfo : ScriptableItems<CustomAssetsInfoItem>
    {
        public override void InitCollections()
        {
            ScriptableItem.InitCollections(ref mMapper, ref m_Collections);
        }

        public override CustomAssetsInfoItem GetItem(int id)
        {
            return mMapper[id];
        }

        public override void FillFromDataRaw(ref string source)
        {
            JsonData jsonData = JsonMapper.ToObject(source);

            bool flag;
            string IDValue;
            JsonData item;
            JsonData assets;
            CustomAssetsInfoItem data;
            int count = jsonData.Count;
            for (int i = 0; i < count; i++)
            {
                item = jsonData[i];

                flag = bool.TryParse(item["valid"].ToString(), out flag) ? flag : false;

                IDValue = item.ContainsKey("id") ? item["id"].ToString() : "0";
                int.TryParse(IDValue, out int id);

                data = new CustomAssetsInfoItem()
                {
                    id = id,
                    valid = flag,
                    name = item["bundleName"].ToString(),
                };
                assets = item["assets"];
                data.AddAssetFromDataRaw(assets);

                //data.AfterInitFromJSON();
                m_Collections.Add(data);

                Debug.Log("[Info - JSON] Assets info item ".Append(data.name, " has load."));
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.Refresh();
        }

        protected override void BeforeSaveGameItems(ref CustomAssetsInfoItem item)
        {
            base.BeforeSaveGameItems(ref item);

            item?.Refresh();
        }

        protected override object GetJsonSource()
        {
            int max = m_Collections.Count;
            object[] result = new object[max];
            for (int i = 0; i < max; i++)
            {
                result[i] = m_Collections[i].GetJsonSource();
            }

            if (max <= 0)
            {
                Debug.Log("[Info - JSON] Assets info item has saved, length is 0");
            }
            else { }

            return result;
        }
    }
}