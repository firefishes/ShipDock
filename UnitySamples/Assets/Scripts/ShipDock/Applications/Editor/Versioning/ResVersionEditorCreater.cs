using ShipDock.Applications;
using ShipDock.Versioning;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Editors
{
    /// <summary>
    /// 
    /// 资源版本相关的编辑器扩展工具
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class ResDataVersionEditorCreater
    {
        public static string versionFileNameKey = "client_version_filename";
        public static string versionFileNameReleaseKey = "client_version_filename_release";
        public static string applyResVersionGatewayReleaseKey = "apply_res_version_gateway_release";

        private static string resVersionRootURLKey = "res_version_root_url";
        private static string updateTotalVersionKey = "update_total_version";
        private static string updateAdditionVersionKey = "update_addition_version";
        private static string resVersionRootUrlReleaseKey = "res_version_root_url_release";

        public static bool ApplyReleaseGateway { get; private set; }

        public static void SetEditorValueItems(ShipDockEditor editor)
        {
            editor.SetValueItem(resVersionRootURLKey, "http://127.0.0.1");
            editor.SetValueItem(resVersionRootUrlReleaseKey, string.Empty);
            editor.SetValueItem("is_ignore_remote", "false");
            editor.SetValueItem("is_sync_client_versions", "true");
            editor.SetValueItem(updateAdditionVersionKey, "true");
            editor.SetValueItem(updateTotalVersionKey, "false");
            editor.SetValueItem("sync_app_version", "false");
            editor.SetValueItem(applyResVersionGatewayReleaseKey, "false");
            editor.SetValueItem(versionFileNameKey, "ClientResVersions");
            editor.SetValueItem(versionFileNameReleaseKey, "ClientResVersions_Release");
        }

        public static void CheckEditorGUI(ShipDockEditor editor)
        {
            //editor.ValueItemTriggle("is_zip_patch", "zip压缩包：");
            editor.ValueItemTriggle("sync_app_version", "    更新版本配置的App版本号");
            editor.ValueItemTriggle(updateTotalVersionKey, "    提升版本配置的总版本号");
            editor.ValueItemTriggle(updateAdditionVersionKey, "    更新各增量资源版本号");
            editor.ValueItemTriggle("is_sync_client_versions", "    作为最新版客户端的资源配置模板");
        }

        public static void CheckGatewayEditorGUI(ShipDockEditor editor, out bool isIgnoreRemote)
        {
            isIgnoreRemote = editor.ValueItemTriggle("is_ignore_remote", "    不基于远程版本配置生成新版本配置");
            if (!isIgnoreRemote)
            {
                ApplyReleaseGateway = editor.ValueItemTriggle(applyResVersionGatewayReleaseKey, "    使用发布版 Gateway");
                string key = ApplyReleaseGateway ? resVersionRootUrlReleaseKey : resVersionRootURLKey;//区分测试和发布版
                editor.ValueItemTextAreaField(key, true, "远程版本配置所在服务端 URL".Append(ApplyReleaseGateway ? "(发布版)" : string.Empty), false);
            }
            else { }
        }

        public static void BuildVersions(ShipDockEditor editor, ref List<string> abNames)
        {
            string remoteGateway = editor.GetValueItem(resVersionRootURLKey).Value;
            bool isIgnoreRemote = editor.GetValueItem("is_ignore_remote").Bool;
            bool isSyncClientVersions = editor.GetValueItem("is_sync_client_versions").Bool;
            bool isUpdateAdditionVersion = editor.GetValueItem(updateAdditionVersionKey).Bool;
            bool isUpdateResVersion = editor.GetValueItem(updateTotalVersionKey).Bool;
            bool isSyncAppVersion = editor.GetValueItem("sync_app_version").Bool;
            string clientVersionsFileName = editor.GetValueItem(versionFileNameKey).Value;

            ResDataVersionEditorCreater creater = new ResDataVersionEditorCreater()
            {
                ABNamesWillBuild = abNames,
                resRemoteGateWay = remoteGateway,
                isSyncClientVersions = isSyncClientVersions,
                isUpdateVersion = isUpdateAdditionVersion,
                isUpdateResVersion = isUpdateResVersion,
                isSyncAppVersion = isSyncAppVersion,
                ClientVersionFileName = clientVersionsFileName,
            };
            creater.CreateResDataVersion(isIgnoreRemote);
        }

        public bool isUpdateVersion;
        public bool isSyncClientVersions;
        public bool isUpdateResVersion;
        public bool isSyncAppVersion;
        /// <summary>即将创建资源包的名称列表</summary>
        public List<string> ABNamesWillBuild;
        /// <summary>远程资源服务器网关</summary>
        public string resRemoteGateWay;

        /// <summary>
        /// 创建资源版本
        /// </summary>
        /// <param name="abNames">用于创建资源包的名称列表</param>
        /// <param name="resGateway">远程资源服务器网关</param>
        /// <param name="isIgnoreRemote">是否忽略基于线上的版本创建新的版本</param>
        public void CreateResDataVersion(bool isIgnoreRemote = false)
        {
            if (!isIgnoreRemote && string.IsNullOrEmpty(resRemoteGateWay))
            {
                Debug.LogError("Remote gateway do not allow empty when non neglect remote versions.");
                return;
            }
            else
            {
                if (isIgnoreRemote)
                {
                    OnGetRemoteVersion(false, default);
                }
                else
                {
                    Loader.Loader loader = new Loader.Loader
                    {
                        ApplyLoom = false
                    };
                    loader.CompleteEvent.AddListener(OnGetRemoteVersion);
                    loader.Load(resRemoteGateWay.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));
                }
            }
        }

        public string ClientVersionFileName { get; set; }

        private void OnGetRemoteVersion(bool flag, Loader.Loader ld)
        {
            ResDataVersion remoteVers = default;
            if (flag)
            {
                //string data = ld.TextData;
                string data = System.Text.Encoding.UTF8.GetString(ld.ResultData);
                if (data.Contains("}{"))
                {
                    Debug.Log("Match the '}{'");
                    data = data.Split(new string[] { "}{" }, System.StringSplitOptions.None)[0].Append("}");
                }
                remoteVers = JsonUtility.FromJson<ResDataVersion>(data);

                if (remoteVers == default)
                {
                    Debug.Log("Do not exists remote versions.");
                }
                ld.Dispose();
            }
            else
            {
                Debug.Log("Do not get remote versions.");
            }
            BuildVersionConfig(ref remoteVers);
        }

        /// <summary>
        /// 创建资源版本
        /// </summary>
        /// <param name="remoteVers"></param>
        private void BuildVersionConfig(ref ResDataVersion remoteVers)
        {
            string versions = default;
            ResDataVersion resDataVersion = default;

            GetVersionDataFromRemote(ref remoteVers, ref versions, ref resDataVersion);

            string[] abNamesValue = ABNamesWillBuild != default ? ABNamesWillBuild.ToArray() : new string[0];
            resDataVersion.CreateNewResVersion(ref resRemoteGateWay, isUpdateVersion, isUpdateResVersion, isSyncAppVersion, ref remoteVers, ref abNamesValue);
            resDataVersion.Refresh();

            bool applyClientGateway = false;
            if (isSyncClientVersions)
            {
                List<ScriptableObject> list = default;
                ShipDockEditorUtils.FindAssetInEditorProject(ref list, "t:ScriptableObject", @"Assets\Prefabs");
                ClientResVersion clientRes = default;// = (ClientResVersion)list[0];
                foreach (ScriptableObject item in list)
                {
                    if (item.name == ClientVersionFileName)
                    {
                        clientRes = (ClientResVersion)item;
                        break;
                    }
                    else { }
                }

                applyClientGateway = clientRes.ApplyCurrentResGateway;
                if (applyClientGateway)
                {
                    resDataVersion.res_gateway = clientRes.ClientResVersionGateway();
                }

                clientRes.Versions.CloneVersionsFrom(ref resDataVersion);
                clientRes.SetChanges(resDataVersion.ResChanges);
                UnityEditor.EditorUtility.SetDirty(clientRes);
            }
            versions = JsonUtility.ToJson(resDataVersion);
            string stamp = "_".Append(System.DateTime.Now.ToShortDateString().Replace("/", "_"));
            FileOperater.WriteUTF8Text(versions, Application.dataPath.Append("/Prefabs/ClientResVersions_Previews/", ResDataVersion.FILE_RES_DATA_VERSIONS_NAME, stamp, ".json"));//仅用于查看
            FileOperater.WriteBytes(versions, AppPaths.ABBuildOutputRoot.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));//位于资源主目录里的正式文件

            resDataVersion.Clean();
            remoteVers?.Clean();
        }

        private void GetVersionDataFromRemote(ref ResDataVersion remoteVers, ref string versions, ref ResDataVersion resDataVersion)
        {
            if (remoteVers == default)
            {
                versions = FileOperater.ReadUTF8Text(AppPaths.ABBuildOutputRoot.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));
                resDataVersion = JsonUtility.FromJson<ResDataVersion>(versions);
                if (resDataVersion == default)
                {
                    resDataVersion = new ResDataVersion
                    {
                        app_version = Application.version,
                        res_version = remoteVers != default ? remoteVers.res_version : 0,//根据是否存在线上版本同步资源号
                    };
                }
            }
            else
            {
                resDataVersion = new ResDataVersion();
                resDataVersion.CloneVersionsFrom(ref remoteVers);
            }
        }
    }
}