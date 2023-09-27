#define SHOW_MENU_IN_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ShipDock.Editors
{
    public class AssetBundleBuilder
    {
        /// <summary>
        /// 批量删除 Persistent 文件
        /// </summary>
        [MenuItem("ShipDock/Delete PlayerPrefs")]
        public static void DelPlayerPrefs()
        {
            if (EditorUtility.DisplayDialog("警告", string.Format("即将删除用户偏好缓存，操作不可逆，是否继续？", Application.persistentDataPath), "立刻、马上", "我再想想"))
            {
                PlayerPrefs.DeleteAll();
            }
            else { }
        }
        /// <summary>
        /// 批量删除 Persistent 文件
        /// </summary>
        [MenuItem("ShipDock/Delete Persistent Resouce")]
        public static void DelPersistentResource()
        {
            if (EditorUtility.DisplayDialog("警告", string.Format("即将删除 {0} 目录下的资源文件，操作不可逆，是否继续？", Application.persistentDataPath), "立刻、马上", "我再想想"))
            {
                FileOperater.DeleteFileDirection(AppPaths.PersistentResDataRoot);
                AssetDatabase.Refresh();//刷新
            }
            else { }
        }

        /// <summary>
        /// 批量删除 Streaming 文件
        /// </summary>
        [MenuItem("ShipDock/Delete Streaming Resouce")]
        public static void DelStreamingResource()
        {
            if (EditorUtility.DisplayDialog("警告", string.Format("即将删除 {0} 目录下所有文件，操作不可逆，是否继续？", Application.streamingAssetsPath), "立刻、马上", "我再想想"))
            {
                Action<BuildTarget> method = (buildTarget) =>
                {
                    string resRoot = AppPaths.StreamingResDataRoot;
                    string platformRoot = GetSuffix(buildTarget);
                    FileOperater.DeleteFileDirection(resRoot.Append(platformRoot));
                    FileOperater.DeleteFileDirection(resRoot.Append(platformRoot.Append(",meta")));
                };

                method.Invoke(BuildTarget.Android);
                method.Invoke(BuildTarget.iOS);
                method.Invoke(BuildTarget.StandaloneOSX);
                method.Invoke(BuildTarget.StandaloneWindows);
                method.Invoke(BuildTarget.StandaloneWindows64);

                FileOperater.DeleteFileDirection(AppPaths.StreamingResDataRoot);
                FileOperater.DeleteFileDirection(AppPaths.StreamingResDataRoot.Append(",meta"));

                AssetDatabase.Refresh();//刷新
            }
            else { }
        }

        /// <summary>
        /// 批量清楚AB名称
        /// </summary>
        [MenuItem("ShipDock/Clear Asset Bundles")]
        public static void ClearAssetBundleName()
        {
            if (EditorUtility.DisplayDialog("警告", "即将清除所有资源包标签，操作不可逆，是否继续？", "必须的", "我再想想"))
            {
                // UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
                string[] names = AssetDatabase.GetAllAssetBundleNames();
                int length = names.Length;
                string[] oldAssetBundleNames = new string[length];
                for (int i = 0; i < length; i++)
                {
                    oldAssetBundleNames[i] = names[i];
                }

                for (int j = 0; j < oldAssetBundleNames.Length; j++)
                {
                    Debug.Log("Asset bundle name " + oldAssetBundleNames[j] + " has remove.");
                    AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
                }
            }
            else { }
        }

        #region 资源打包相关
#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/IOS")]
#endif
        public static void BuildIOSAB()
        {
            Debug.Log("Start build IOS asset bundles");
            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(BuildTarget.iOS);
        }

#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/ANDROID")]
#endif
        public static void BuildAndroidAB()
        {
            Debug.Log("Start build Android asset bundles");
            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(BuildTarget.Android);
        }

#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/OSX")]
#endif
        public static void BuildOSXAB()
        {
            Debug.Log("Start build OSX asset bundles");
            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(BuildTarget.StandaloneOSX);
        }

#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/WIN")]
#endif
        public static void BuildWinAB()
        {
            Debug.Log("Start build Win asset bundles");
            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(BuildTarget.StandaloneWindows);
        }

#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/WIN64")]
#endif
        public static void BuildWin64AB()
        {
            Debug.Log("Start build win64 asset bundles");
            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(BuildTarget.StandaloneWindows64);
        }

#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/ASSET COORDINATOR")]
#endif
        public static void FromAssetCoordinator()
        {
            Debug.Log("Start build asset bundles from asset coordinator");

            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(CustomAssetCoordinatorInfo.buildTarget, true);
        }
        #endregion

        public static string GetSuffix(BuildTarget buildPlatform)
        {
            string result;
            switch (buildPlatform)
            {
                case BuildTarget.Android:
                    result = "_ANDROID/";
                    break;
                case BuildTarget.iOS:
                    result = "_IOS/";
                    break;
                case BuildTarget.StandaloneWindows:
                    result = "_WIN/";
                    break;
                case BuildTarget.StandaloneWindows64:
                    result = "_WIN64/";
                    break;
                case BuildTarget.StandaloneOSX:
                    result = "_OSX/";
                    break;
                default:
                    result = "_UNKNOWN";
                    break;
            }
            return result;
        }

        public static string GetBuildPlatFromTitle(BuildTarget buildPlatform, out int statu)
        {
            statu = 0;
            string result;
            switch (buildPlatform)
            {
                case BuildTarget.Android:
                    result = "ANDROID";
                    break;
                case BuildTarget.iOS:
                    result = "IOS";
                    break;
                case BuildTarget.StandaloneWindows:
                    result = "WIN";
                    break;
                case BuildTarget.StandaloneWindows64:
                    result = "WIN64";
                    break;
                case BuildTarget.StandaloneOSX:
                    result = "OSX";
                    break;
                default:
                    result = "UNKNOWN";
                    statu = 1;
                    EditorUtility.DisplayDialog("警告", "资源的目标平台未定义，请前往设置", "确定");
                    break;
            }
            return result;
        }

        /// <summary>
        /// 资源打包
        /// </summary>
        public void BuildAssetBundle(BuildTarget buildPlatform, bool isFromAssetCoordinator = false)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            string rootPath = AppPaths.DataPathResDataRoot;
            if (!Directory.Exists(rootPath))
            {
                EditorUtility.DisplayDialog("警告", "所需资源目录不存在：" + rootPath, "确定");
                return;
            }
            else { }

            string[] assetLabelRoots = Directory.GetDirectories(rootPath);
            if (assetLabelRoots.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "资源目录下没用文件！！！", "确定");
                return;
            }
            else { }

            UnityEngine.Object[] selections = null;
            if (isFromAssetCoordinator)
            {
                selections = CheckAssetCoordinator();
                if (selections == null)
                {
                    return;
                }
                else { }
            }
            else
            {
                selections = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
            }

            ShipDockEditorData editorData = ShipDockEditorData.Instance;
            editorData.platformPath = GetSuffix(buildPlatform);
            editorData.buildPlatform = buildPlatform;
            editorData.selections = selections;
            editorData.isBuildFromCoordinator = isFromAssetCoordinator;
            AssetBundleInfoPopupEditor.Popup();
        }

        private UnityEngine.Object[] CheckAssetCoordinator()
        {
            List<UnityEngine.Object> result = new List<UnityEngine.Object>();

            UnityEngine.Object selection = Selection.activeObject;
            if (selection is CustomAssetCoordinatorInfo infos)
            {
                string bundleName;
                UnityEngine.Object res;
                string resName = null;
                CustomAssetInfoSource[] assetList;
                CustomAssetsInfoItemSource itemSource;
                List<CustomAssetsInfoItem> list = infos.GetAssetInfos();

                ShipDockEditorData editorData = ShipDockEditorData.Instance;
                string coordinatorPath = AssetDatabase.GetAssetPath(selection);
                editorData.coordinatorPath = coordinatorPath;

                Dictionary<string, string> assetsFromCoordinator;
                foreach (var item in list)
                {
                    itemSource = item.GetJsonSource() as CustomAssetsInfoItemSource;
                    bundleName = itemSource.bundleName;
                    assetList = itemSource.assets;

                    assetsFromCoordinator = editorData.assetsFromCoordinator;
                    if (assetsFromCoordinator == null)
                    {
                        assetsFromCoordinator = new Dictionary<string, string>();
                        editorData.assetsFromCoordinator = assetsFromCoordinator;
                    }
                    else { }

                    string path;
                    int index = 0;
                    int max = assetList.Length;
                    foreach (var asset in assetList)
                    {
                        path = asset.assetPath;
                        res = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

                        if (assetsFromCoordinator.TryGetValue(path, out resName)) { }
                        else
                        {
                            assetsFromCoordinator[path] = bundleName;
                        }

                        result.Add(res);
                        index++;
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("警告", "所选的资源文件不是资源协调器", "确定");
            }

            return result.Count > 0 ? result.ToArray() : null;
        }

        private static void DeleteFileDirection(string directionName)
        {
            if (!string.IsNullOrEmpty(directionName))
            {
                if (Directory.Exists(directionName))
                {
                    Directory.Delete(directionName, true);//注意：这里参数"true"表示可以删除非空目录

                    string metaFileName = directionName.Append(".meta");
                    if (File.Exists(metaFileName))
                    {
                        File.Delete(metaFileName);//删除 .meta 文件
                    }
                    else { }
                }
                else { }
            }
            else { }
        }
    }
}
