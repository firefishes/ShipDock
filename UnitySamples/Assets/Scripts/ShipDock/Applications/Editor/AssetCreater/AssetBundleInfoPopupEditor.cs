using ShipDock.Applications;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ShipDock.Editors
{
    /// <summary>
    /// 
    /// 资源包打包相关的编辑器扩展工具
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class AssetBundleInfoPopupEditor : ShipDockEditor
    {
        /// <summary>
        /// 弹出编辑器工具弹窗
        /// </summary>
        /// <returns></returns>
        public static AssetBundleInfoPopupEditor Popup()
        {
            InitEditorWindow<AssetBundleInfoPopupEditor>("资源打包与版本配置");//, new Rect(0, 0, 400, 400));
            return focusedWindow as AssetBundleInfoPopupEditor;
        }

        #region 编辑器字段名
        private string mIsBuildABKey = "is_build_ab";
        private string mOverrideToStreamingKey = "override_to_streaming";
        private string mABItemNameKey = "ab_item_name";
        private string mDisplayResShowerKey = "display_res_shower";
        private string mIsBuildVersionsKey = "is_build_versions";
        #endregion
        private Vector2 mResShowerScrollPos;

        public UnityEngine.Object[] ResList { get; set; } = new UnityEngine.Object[0];

        protected override void ReadyClientValues()
        {
            SetValueItem(mIsBuildABKey, "true");//是否生成资源包
            SetValueItem(mOverrideToStreamingKey, "false");//是否将生成的资源包覆盖至Streaming目录
            SetValueItem(mABItemNameKey, string.Empty);
            SetValueItem(mDisplayResShowerKey, "true");//是否显示带打包的资源列表
            SetValueItem(mIsBuildVersionsKey, "true");//是否构建资源版本

            ResDataVersionEditorCreater.SetEditorValueItems(this);
        }

        protected override void UpdateClientValues() { }

        /// <summary>
        /// 更新编辑器的 GUI 界面
        /// </summary>
        protected override void CheckGUI()
        {
            base.CheckGUI();

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            bool isIgnoreRemote = false;
            bool isBuildAB = ValueItemTriggle(mIsBuildABKey, "创建资源包");
            if (isBuildAB)
            {
                ValueItemTriggle(mOverrideToStreamingKey, "    资源打包完成后复制到 SteamingAssets");
            }
            else { }

            bool isBuildVersions = ValueItemTriggle("is_build_versions", "生成资源版本");
            if (isBuildVersions)
            {
                if (isBuildAB)
                {
                    ResDataVersionEditorCreater.CheckEditorGUI(this);
                }
                else
                {
                    ValueItemTriggle("sync_app_version", "    更新版本配置的App版本号");
                    ValueItemTriggle("is_sync_client_versions", "    作为最新版客户端的资源配置模板");
                }

                ResDataVersionEditorCreater.CheckGatewayEditorGUI(this, out isIgnoreRemote);

                string versionFileNameKey = ResDataVersionEditorCreater.ApplyReleaseGateway ? 
                    ResDataVersionEditorCreater.versionFileNameReleaseKey : ResDataVersionEditorCreater.versionFileNameKey;

                ValueItemTextAreaField(versionFileNameKey, true, "    客户端配置资源文件名");
            }
            else { }

            if (isBuildAB)
            {
                ResList = ShipDockEditorData.Instance.selections;

                ShowABWillBuildResult();

                if (GUILayout.Button("Build Assets"))
                {
                    AssetBuilding();
                }
                else { }
            }
            else if (isBuildVersions)
            {
                if (isIgnoreRemote)
                {
                    if (GUILayout.Button("Build Versions Only"))
                    {
                        BuildVersions(default);
                    }
                    else { }
                }
                else
                {
                    if (GUILayout.Button("Sync Versions From Remote"))
                    {
                        BuildVersions(default);
                    }
                    else { }
                }
            }
            else
            {
                ValueItemLabel(string.Empty, "请从以上提供的选项中，选择一个或多个以继续", false);
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void AssetBuilding()
        {
            string abName = string.Empty;
            ShipDockEditorData editorData = ShipDockEditorData.Instance;

            editorData.ABCreaterMapper?.Clear();
            Utils.Reclaim(ref editorData.ABCreaterMapper, false);

            editorData.ABCreaterMapper = new KeyValueList<string, List<ABAssetCreater>>();
            CreateAssetImporters(ref abName, ref editorData.ABCreaterMapper);

            string output = AppPaths.ABBuildOutputRoot;
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }

            BuildAssetByCreater(out List<string> abNames);

            if (EditorUtility.DisplayDialog("提示", string.Format("操作完成，如遇到问题可根据输出日志做出调整"), "朕知道了"))
            {
                AssetDatabase.Refresh();

                bool overrideToStreaming = GetValueItem(mOverrideToStreamingKey).Bool;
                string path;
                int max = abNames.Count;
                byte[] vs;
                DateTime dateTime = DateTime.Now;
                string tempPath = AppPaths.ABBuildOutputTempRoot;//string.Format(AppPaths.ABBuildOutputTempRoot, dateTime.ToFileTime().ToString());
                for (int i = 0; i < max; i++)
                {
                    path = AppPaths.ABBuildOutputRoot.Append(abNames[i]);
                    vs = FileOperater.ReadBytes(path);

                    if (vs == default || vs.Length == 0)
                    {
                        Debug.Log(path);
                    }
                    else { }

                    path = tempPath.Append(abNames[i]);
                    FileOperater.WriteBytes(vs, path);

                    if (overrideToStreaming)
                    {
                        path = AppPaths.StreamingResDataRoot.Append(abNames[i]);
                        FileOperater.WriteBytes(vs, path);
                    }
                }

                BuildVersions(abNames);
            }
        }

        private string GetResItemKey(int index)
        {
            return "res_".Append(index.ToString());
        }

        private void CreateAssetImporters(ref string abName, ref KeyValueList<string, List<ABAssetCreater>> mapper)
        {
            string path;
            string assetItemName;
            string relativeName;
            string starter = StringUtils.PATH_SYMBOL.Append(AppPaths.resDataRoot);

            FileInfo fileInfo;
            List<ABAssetCreater> list;
            string name;
            int starterLen = starter.Length;
            int max = ResList.Length;
            ValueItem item;
            for (int i = 0; i < max; i++)
            {
                name = GetResItemKey(i);
                item = GetValueItem(name);
                if (item == default)
                {
                    continue;
                }
                assetItemName = item.Value;//"res_1"
                relativeName = assetItemName.Replace("Assets/".Append(AppPaths.resDataRoot), string.Empty);
                path = AppPaths.DataPathResDataRoot.Append(relativeName);

                fileInfo = new FileInfo(path);
                string ext = fileInfo.Extension;
                if (ext != ".cs")
                {
                    int index = path.IndexOf(starter, StringComparison.Ordinal);
                    ABAssetCreater creater = new ABAssetCreater(path.Substring(index + starterLen));
                    abName = creater.GetABName();
                    bool isScene = ext == ".unity";
                    if (isScene)
                    {
                        abName = abName.Append("_unityscene");
                    }

                    if (mapper.ContainsKey(abName))
                    {
                        list = mapper[abName];
                    }
                    else
                    {
                        list = new List<ABAssetCreater>();
                        mapper[abName] = list;
                    }
                    creater.Importer = AssetImporter.GetAtPath(assetItemName);
                    list.Add(creater);
                    //Debug.Log("Importers: " + abName);
                    //Debug.Log("FileInfo: " + fileInfo.Name);
                }
            }
        }

        private void BuildAssetByCreater(out List<string> abNames)
        {
            string abName;
            List<ABAssetCreater> list;

            ShipDockEditorData editorData = ShipDockEditorData.Instance;
            int max = editorData.ABCreaterMapper.Size;
            abNames = editorData.ABCreaterMapper.Keys;
            List<List<ABAssetCreater>> creaters = editorData.ABCreaterMapper.Values;
            for (int i = 0; i < max; i++)
            {
                abName = abNames[i];
                list = creaters[i];
                int m = list.Count;
                for (int n = 0; n < m; n++)
                {
                    list[n].Importer.assetBundleName = abName;
                    //Debug.Log(abName);
                }
            }
            BuildPipeline.BuildAssetBundles(AppPaths.ABBuildOutputRoot, BuildAssetBundleOptions.None, editorData.buildPlatform);
        }

        private void BuildVersions(List<string> abNames)
        {
            bool isBuildVersions = GetValueItem(mIsBuildVersionsKey).Bool;
            if (isBuildVersions)
            {
                ResDataVersionEditorCreater.BuildVersions(this, ref abNames);
            }
        }

        private void ShowABWillBuildResult()
        {
            if (ResList == default)
            {
                return;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            bool displayShower = ValueItemTriggle(mDisplayResShowerKey, "查看即将打包的资源");
            if (displayShower)
            {
                EditorGUILayout.LabelField("资源名清单：");
                mResShowerScrollPos = EditorGUILayout.BeginScrollView(mResShowerScrollPos, false, true);
            }
            int max = ResList.Length;
            UnityEngine.Object item;
            string path;
            string key, fieldValue, keyABItemName = mABItemNameKey;
            for (int i = 0; i < max; i++)
            {
                item = ResList[i];
                key = GetResItemKey(i);// key Sample: "res_1"
                path = AssetDatabase.GetAssetPath(item);
                if (!path.EndsWith(".cs"))//排除脚本文件
                {
                    SetValueItem(key, path);
                    if (displayShower && GUILayout.Button(ResList[i].name))
                    {
                        fieldValue = GetValueItem(key).Value;
                        GetValueItem(keyABItemName)?.Change(fieldValue);
                    }
                }
            }
            if(displayShower)
            {
                EditorGUILayout.EndScrollView();
                ValueItemLabel(keyABItemName);
            }
            EditorGUILayout.Space();
            ValueItemLabel(string.Empty, string.Format("即将构建的资源数量：{0}", max.ToString()));
        }
    }
}
