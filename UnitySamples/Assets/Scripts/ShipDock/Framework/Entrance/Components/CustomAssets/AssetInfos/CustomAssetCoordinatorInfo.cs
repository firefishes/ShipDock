using LitJson;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock
{
    [CreateAssetMenu(fileName = "ShipDockAssetCoordinatorInfo", menuName = "ShipDock : 资源协调器信息", order = 100)]
    public class CustomAssetCoordinatorInfo : ScriptableItems<CustomAssetsInfoItem>
    {
#if UNITY_EDITOR
        public static BuildTarget buildTarget;
        private IdentBits mABBtnIdentBits = new IdentBits();

#if ODIN_INSPECTOR
        [Button(name: "Build Assets"), ShowIf("@this.mABBtnIdentBits.Check(1) == false && this.mABBtnIdentBits.Check(2) == false")]
#endif
        private void BuildAssetBundles()
        {
            mABBtnIdentBits.Reclaim();
            mABBtnIdentBits.Mark(1);
        }

        private void BuildPlatfromChoosen(BuildTarget buildTarget)
        {
            if (mABBtnIdentBits.Check(1))
            {
                CustomAssetCoordinatorInfo.buildTarget = buildTarget;
                mABBtnIdentBits.Mark(2);
                mABBtnIdentBits.DeMark(1);
            }
            else { }
        }

#if ODIN_INSPECTOR
        [ButtonGroup("PlatfromChoose"), ShowIf("@this.mABBtnIdentBits.Check(1) == true")]
#endif
        private void Android()
        {
            BuildPlatfromChoosen(BuildTarget.Android);
        }

#if ODIN_INSPECTOR
        [ButtonGroup("PlatfromChoose"), ShowIf("@this.mABBtnIdentBits.Check(1) == true")]
#endif
        private void IOS()
        {
            BuildPlatfromChoosen(BuildTarget.iOS);
        }

#if ODIN_INSPECTOR
        [ButtonGroup("PlatfromChoose"), ShowIf("@this.mABBtnIdentBits.Check(1) == true")]
#endif
        private void OSX()
        {
            BuildPlatfromChoosen(BuildTarget.StandaloneOSX);
        }

#if ODIN_INSPECTOR
        [ButtonGroup("PlatfromChoose"), ShowIf("@this.mABBtnIdentBits.Check(1) == true")]
#endif
        private void WIN()
        {
            BuildPlatfromChoosen(BuildTarget.StandaloneWindows);
        }

#if ODIN_INSPECTOR
        [ButtonGroup("PlatfromChoose"), ShowIf("@this.mABBtnIdentBits.Check(1) == true")]
#endif
        private void WIN64()
        {
            BuildPlatfromChoosen(BuildTarget.StandaloneWindows64);
        }

#if ODIN_INSPECTOR
        [Button(name: "Back"), ShowIf("@this.mABBtnIdentBits.Check(1) == true")]
#endif
        private void BackToABConfig()
        {
            mABBtnIdentBits.Reclaim();
        }

#if ODIN_INSPECTOR
        [ButtonGroup("WillBuild"), ShowIf("@this.mABBtnIdentBits.Check(2) == true")]
#endif
        private void BuildConfirm()
        {
            if (mABBtnIdentBits.Check(2))
            {
                mABBtnIdentBits.Reclaim();
            }
            else { }
        }

#if ODIN_INSPECTOR
        [ButtonGroup("WillBuild"), ShowIf("@this.mABBtnIdentBits.Check(2) == true")]
#endif
        private void BuildCancel()
        {
            if (mABBtnIdentBits.Check(2))
            {
                mABBtnIdentBits.Reclaim();
            }
            else { }
        }
#endif

        public virtual List<CustomAssetsInfoItem> GetAssetInfos()
        {
            return m_Collections;
        }

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

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.Refresh();
#endif
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