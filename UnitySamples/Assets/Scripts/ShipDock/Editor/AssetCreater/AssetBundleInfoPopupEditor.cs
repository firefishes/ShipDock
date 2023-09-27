#define _LOG_ENABLED

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
        private const string FILE_CS_EXT = ".cs";
        private const string FILE_UNITY_SCENE_EXT = ".unity";
        private const string KEY_AB_UNITY_SCENE = "_unityscene";

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
        public const string KEY_IS_BUILD_AB = "is_build_ab";
        public const string KEY_OVERRIDE_TO_STREAMING = "override_to_streaming";
        public const string KEY_AB_ITEM_NAME = "ab_item_name";
        public const string KEY_DISPLAY_RES_SHOWER = "display_res_shower";
        public const string KEY_IS_BUILD_VERSION = "is_build_versions";
        public const string KEY_COORDINATOR_PATH = "coordinator_path";
        public const string KEY_BUILD_TARGET_TITLE = "build_target";
        #endregion

        private int mBuildTargetStatu;
        private Vector2 mResShowerScrollPos;

        public UnityEngine.Object[] ResList { get; set; } = new UnityEngine.Object[0];

        protected override void ReadyClientValues()
        {
            ShipDockEditorData editorData = ShipDockEditorData.Instance;
            string buildTargetTitle = AssetBundleBuilder.GetBuildPlatFromTitle(editorData.buildPlatform, out mBuildTargetStatu);

            SetValueItem(KEY_IS_BUILD_AB, TRUE);//是否生成资源包
            SetValueItem(KEY_OVERRIDE_TO_STREAMING, FALSE);//是否将生成的资源包覆盖至Streaming目录
            SetValueItem(KEY_AB_ITEM_NAME, string.Empty);
            SetValueItem(KEY_DISPLAY_RES_SHOWER, TRUE);//是否显示带打包的资源列表
            SetValueItem(KEY_IS_BUILD_VERSION, TRUE);//是否构建资源版本
            SetValueItem(KEY_COORDINATOR_PATH, editorData.coordinatorPath);//资源协调器路径
            SetValueItem(KEY_BUILD_TARGET_TITLE, buildTargetTitle);//构建资源的目标平台

            ResDataVersionEditorCreater.SetEditorValueItems(this);
        }

        protected override void UpdateClientValues() { }

        /// <summary>
        /// 更新编辑器的 GUI 界面
        /// </summary>
        protected override void CheckGUI()
        {
            base.CheckGUI();

            const string triggerTitle_IsBuildAB = "创建资源包";
            const string triggerTitle_OverrideToStreaming = "    资源打包完成后复制到 SteamingAssets";
            const string triggerTitle_IsBuildVersions = "生成资源版本";

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            bool isIgnoreRemote = false;
            bool isBuildAB = ValueItemTriggle(KEY_IS_BUILD_AB, triggerTitle_IsBuildAB);
            if (isBuildAB)
            {
                //在AB资源包构建被启用时才出现此勾选项
                ValueItemTriggle(KEY_OVERRIDE_TO_STREAMING, triggerTitle_OverrideToStreaming);
            }
            else { }

            bool isBuildVersions = ValueItemTriggle(KEY_IS_BUILD_VERSION, triggerTitle_IsBuildVersions);
            if (isBuildVersions)
            {
                RefreshBuildVersionView(isBuildAB, ref isIgnoreRemote);
            }
            else { }

            if (isBuildAB)
            {
                //启用了构建AB资源包的选项则直接开始构建资源包
                RefreshBuildABView();
            }
            else if (isBuildVersions)
            {
                //只启用了构建资源版本选项则开始单独构建资源版本
                RefreshBuildVersionsView(isIgnoreRemote);
            }
            else
            {
                const string warningInfo = "请从以上提供的选项中，选择一个或多个以继续";
                ValueItemLabel(string.Empty, warningInfo, false);
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void RefreshBuildVersionsView(bool isIgnoreRemote)
        {
            const string buttonTitle_BuildVersionsOnly = "Build Versions Only";
            const string buttonTitle_BuildVersionsFromRemote = "Sync Versions From Remote";

            string buttonTitle = isIgnoreRemote ? buttonTitle_BuildVersionsOnly : buttonTitle_BuildVersionsFromRemote;
            if (GUILayout.Button(buttonTitle))
            {
                BuildVersions(default);
            }
            else { }
        }

        private void RefreshBuildABView()
        {
            const string labelTitle_TargetPlatform = "目标平台";
            const string labelTitle_CoordinatorPath = "   资源协调器路径";
            const string buttonTitle_BuildAssets = "Build Assets";

            ShipDockEditorData editorData = ShipDockEditorData.Instance;
            ResList = editorData.selections;

            ValueItemLabel(KEY_BUILD_TARGET_TITLE, labelTitle_TargetPlatform);

            if (editorData.isBuildFromCoordinator)
            {
                //若是从资源协调器构建AB资源包，则显示资源协调器所在的路径
                ValueItemLabel(KEY_COORDINATOR_PATH, labelTitle_CoordinatorPath);
            }
            else { }

            ShowABWillBuildResult();

            if (mBuildTargetStatu == 0)
            {
                if (GUILayout.Button(buttonTitle_BuildAssets))
                {
                    AssetBuilding();
                }
                else { }
            }
            else { }
        }

        private void RefreshBuildVersionView(bool isBuildAB, ref bool isIgnoreRemote)
        {
            const string triggerTitle_SyncAppVersion = "    更新App版本号";
            const string triggerTitle_IsSyncClientVersions = "    同步至本地客户端资源配置";
            const string triggerTitle_VersionFileName = "    客户端资源配置文件名";

            if (isBuildAB)
            {
                //构建资源包的同时也构建资源版本
                ResDataVersionEditorCreater.RefreshEditorGUI(this);
            }
            else
            {
                //仅构建资源版本
                ValueItemTriggle(ResDataVersionEditorCreater.KEY_SYNC_APP_VERSION, triggerTitle_SyncAppVersion);
                ValueItemTriggle(ResDataVersionEditorCreater.KEY_IS_SYNC_TO_CLIENT_VERSIONS, triggerTitle_IsSyncClientVersions);
            }

            ResDataVersionEditorCreater.CheckGatewayEditorGUI(this, out isIgnoreRemote);

            bool flag = ResDataVersionEditorCreater.ApplyReleaseGateway;
            string keyFieldName = flag ?
                ResDataVersionEditorCreater.KEY_VERSION_FILE_NAME_RELEASE :
                ResDataVersionEditorCreater.KEY_VERSION_FILE_NAME;

            //显示资源版本的文件名，若没保存过此值则自动创建
            ValueItemTextAreaField(keyFieldName, true, triggerTitle_VersionFileName);
        }

        private void AssetBuilding()
        {
            ShipDockEditorData editorData = ShipDockEditorData.Instance;

            editorData.ABCreaterMapper?.Clear();
            Utils.Reclaim(ref editorData.ABCreaterMapper, false);

            editorData.ABCreaterMapper = new KeyValueList<string, List<ABAssetCreater>>();
            CreateAssetImporters(ref editorData.ABCreaterMapper);

            string output = AppPaths.ABBuildOutputRoot;
            if (Directory.Exists(output))
            {
                //目录已存在，不做处理
            }
            else
            {
                Directory.CreateDirectory(output);
            }

            BuildAssetByCreater(out List<string> abNames);

            if (EditorUtility.DisplayDialog("提示", string.Format("操作完成，如遇到问题可根据输出日志做出调整"), "朕知道了"))
            {
                AssetDatabase.Refresh();

                bool overrideToStreaming = GetValueItem(KEY_OVERRIDE_TO_STREAMING).Bool;
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
            const string resKey = "res_";
            return resKey.Append(index.ToString());
        }

        private void CreateAssetImporters(ref KeyValueList<string, List<ABAssetCreater>> mapper)
        {
            string assetPath, path, assetItemName, relativeName;

            ShipDockEditorData editorData = ShipDockEditorData.Instance;
            Dictionary<string, string> coordinatorABNames = editorData.assetsFromCoordinator;
            bool isBuildFromCoordinator = editorData.isBuildFromCoordinator;

            string name, abName ;
            ValueItem item;
            FileInfo fileInfo;
            List<ABAssetCreater> list;

            const string assetsPathRoot = "Assets/";

            string starter = StringUtils.PATH_SYMBOL.Append(AppPaths.resDataRoot);
            int starterLen = starter.Length;
            int max = ResList.Length;
            for (int i = 0; i < max; i++)
            {
                name = GetResItemKey(i);
                item = GetValueItem(name);
                if (item == default)
                {
                    //资源数据项不存在
                    continue;
                }
                else { }

                assetPath = assetsPathRoot.Append(AppPaths.resDataRoot);
                assetItemName = item.Value;//"res_1"
                relativeName = assetItemName.Replace(assetPath, string.Empty);
                path = AppPaths.DataPathResDataRoot.Append(relativeName);

                fileInfo = new FileInfo(path);
                string ext = fileInfo.Extension;
                if (ext != FILE_CS_EXT)
                {
                    int index = path.IndexOf(starter, StringComparison.Ordinal);
                    path = path.Substring(index + starterLen);

                    ABAssetCreater creater = new ABAssetCreater(path);
                    abName = isBuildFromCoordinator ? coordinatorABNames[assetItemName] : creater.GetABName();
                    bool isScene = ext == FILE_UNITY_SCENE_EXT;
                    if (isScene)
                    {
                        abName = abName.Append(KEY_AB_UNITY_SCENE);
                    }
                    else { }

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
#if LOG_ENABLED
                    Debug.Log("Importers: " + abName);
                    Debug.Log("FileInfo: " + fileInfo.Name);
#endif
                }
                else { }
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
#if LOG_ENABLED
                    Debug.Log(abName);
#endif
                }
            }
            BuildPipeline.BuildAssetBundles(AppPaths.ABBuildOutputRoot, BuildAssetBundleOptions.None, editorData.buildPlatform);
        }

        private void BuildVersions(List<string> abNames)
        {
            bool isBuildVersions = GetValueItem(KEY_IS_BUILD_VERSION).Bool;
            if (isBuildVersions)
            {
                ResDataVersionEditorCreater.BuildVersions(this, ref abNames);
            }
            else { }
        }

        private void ShowABWillBuildResult()
        {
            if (ResList == default)
            {
                return;
            }
            else { }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            const string triggerTitle_DisplayResShower = "查看即将打包的资源";
            const string labelTitle_ResList = "资源名清单：";

            bool displayShower = ValueItemTriggle(KEY_DISPLAY_RES_SHOWER, triggerTitle_DisplayResShower);
            if (displayShower)
            {
                EditorGUILayout.LabelField(labelTitle_ResList);
                mResShowerScrollPos = EditorGUILayout.BeginScrollView(mResShowerScrollPos, false, true);
            }
            else { }

            ValueItem valueItem;
            UnityEngine.Object item;
            string path, key, fieldValue, buttonTitle;

            int max = ResList.Length;
            for (int i = 0; i < max; i++)
            {
                item = ResList[i];
                
                key = GetResItemKey(i);// key Sample: "res_1"
                path = AssetDatabase.GetAssetPath(item);

                if (path.EndsWith(FILE_CS_EXT))
                {
                    //排除脚本文件
                }
                else
                {
                    //设置资源的路径数据
                    SetValueItem(key, path);

                    buttonTitle = item.name;
                    if (displayShower && GUILayout.Button(buttonTitle))
                    {
                        valueItem = GetValueItem(key);
                        fieldValue = valueItem.Value;

                        //更新资源选中项的数据
                        valueItem = GetValueItem(KEY_AB_ITEM_NAME);
                        valueItem?.Change(fieldValue);
                    }
                    else { }
                }
            }

            if (displayShower)
            {
                //更新资源选中项的显示
                EditorGUILayout.EndScrollView();
                ValueItemLabel(KEY_AB_ITEM_NAME);
            }
            else { }

            EditorGUILayout.Space();

            const string labelTitle_WillBuildResCount = "即将构建的资源数量：{0}";
            string label = string.Format(labelTitle_WillBuildResCount, max.ToString());
            ValueItemLabel(string.Empty, label);
        }
    }
}
