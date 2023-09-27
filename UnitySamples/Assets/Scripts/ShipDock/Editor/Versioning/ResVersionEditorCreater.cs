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
        public const string KEY_VERSION_FILE_NAME = "client_version_filename";
        public const string KEY_VERSION_FILE_NAME_RELEASE = "client_version_filename_release";
        public const string KEY_APPLY_RES_VERSION_GATEWAY_RELEASE = "apply_res_version_gateway_release";

        private const string resVersionRootURLKey = "res_version_root_url";
        private const string KEY_UPDATE_TOTAL_VERSION = "update_total_version";
        private const string KEY_UPDATE_ADDITION_VERSION = "update_addition_version";
        private const string resVersionRootUrlReleaseKey = "res_version_root_url_release";
        public const string KEY_IS_SYNC_TO_CLIENT_VERSIONS = "is_sync_client_versions";
        public const string KEY_SYNC_APP_VERSION = "sync_app_version";
        public const string KEY_IS_IGNORE_REMOTE = "is_ignore_remote";

        public static bool ApplyReleaseGateway { get; private set; }

        public static void SetEditorValueItems(ShipDockEditor editor)
        {
            editor.SetValueItem(resVersionRootURLKey, "http://127.0.0.1");
            editor.SetValueItem(resVersionRootUrlReleaseKey, string.Empty);
            editor.SetValueItem(KEY_IS_IGNORE_REMOTE, ShipDockEditor.FALSE);
            editor.SetValueItem(KEY_IS_SYNC_TO_CLIENT_VERSIONS, ShipDockEditor.TRUE);
            editor.SetValueItem(KEY_UPDATE_ADDITION_VERSION, ShipDockEditor.TRUE);
            editor.SetValueItem(KEY_UPDATE_TOTAL_VERSION, ShipDockEditor.FALSE);
            editor.SetValueItem(KEY_SYNC_APP_VERSION, ShipDockEditor.FALSE);
            editor.SetValueItem(KEY_APPLY_RES_VERSION_GATEWAY_RELEASE, ShipDockEditor.FALSE);
            editor.SetValueItem(KEY_VERSION_FILE_NAME, "ClientResVersions");
            editor.SetValueItem(KEY_VERSION_FILE_NAME_RELEASE, "ClientResVersions_Release");
        }

        public static void RefreshEditorGUI(ShipDockEditor editor)
        {
            const string triggerTitle_SyncAppVersionDuringCheck = "    更新App版本号";
            const string triggerTitle_UpdateTotalVersionDuringCheck = "    提升资源总版本号";
            const string triggerTitle_UpdateAdditionVersionDuringCheck = "    更新增量资源版本号";
            const string triggerTitle_IsSyncClientVersionsDuringCheck = "    同步至本地客户端资源配置";

            //editor.ValueItemTriggle("is_zip_patch", "zip压缩包：");
            editor.ValueItemTriggle(KEY_SYNC_APP_VERSION, triggerTitle_SyncAppVersionDuringCheck);
            editor.ValueItemTriggle(KEY_UPDATE_TOTAL_VERSION, triggerTitle_UpdateTotalVersionDuringCheck);
            editor.ValueItemTriggle(KEY_UPDATE_ADDITION_VERSION, triggerTitle_UpdateAdditionVersionDuringCheck);
            editor.ValueItemTriggle(KEY_IS_SYNC_TO_CLIENT_VERSIONS, triggerTitle_IsSyncClientVersionsDuringCheck);
        }

        public static void CheckGatewayEditorGUI(ShipDockEditor editor, out bool isIgnoreRemote)
        {
            const string triggerTitle_IsIgnoreRemote = "    不基于远程版本配置生成新的版本配置";

            isIgnoreRemote = editor.ValueItemTriggle(KEY_IS_IGNORE_REMOTE, triggerTitle_IsIgnoreRemote);
            if (isIgnoreRemote)
            {
                //启用此选项，将不通过与远程服务器通信的方式生成本地版本配置文件
            }
            else
            {
                //根据启用的选项是测试还是发布版创建编辑器内容
                const string triggerTitle_ApplyResVersionGatewayRelease = "    使用发布版 Gateway";
                const string textAreaTitle_GatewayHead = "远程版本配置所在服务端 URL";
                const string textAreaTitle_GatewayHeadForRelease = "(发布版)";

                //获取启用项的值
                ApplyReleaseGateway = editor.ValueItemTriggle(KEY_APPLY_RES_VERSION_GATEWAY_RELEASE, triggerTitle_ApplyResVersionGatewayRelease);

                string appendValue = ApplyReleaseGateway ? textAreaTitle_GatewayHeadForRelease : string.Empty;
                string titleContent = textAreaTitle_GatewayHead.Append(appendValue);

                //排版
                string keyFieldName = ApplyReleaseGateway ? resVersionRootUrlReleaseKey : resVersionRootURLKey;
                editor.ValueItemTextAreaField(keyFieldName, true, titleContent, false);
            }
        }

        public static void BuildVersions(ShipDockEditor editor, ref List<string> abNames)
        {
            string remoteGateway = editor.GetValueItem(resVersionRootURLKey).Value;
            bool isIgnoreRemote = editor.GetValueItem(KEY_IS_IGNORE_REMOTE).Bool;
            bool isSyncToClientVersions = editor.GetValueItem(KEY_IS_SYNC_TO_CLIENT_VERSIONS).Bool;
            bool isUpdateAdditionVersion = editor.GetValueItem(KEY_UPDATE_ADDITION_VERSION).Bool;
            bool isUpdateResVersion = editor.GetValueItem(KEY_UPDATE_TOTAL_VERSION).Bool;
            bool isSyncAppVersion = editor.GetValueItem(KEY_SYNC_APP_VERSION).Bool;
            string clientVersionsFileName = editor.GetValueItem(KEY_VERSION_FILE_NAME).Value;

            ResDataVersionEditorCreater creater = new ResDataVersionEditorCreater()
            {
                ABNamesWillBuild = abNames,
                resRemoteGateWay = remoteGateway,
                isSyncToClientVersions = isSyncToClientVersions,
                isUpdateVersion = isUpdateAdditionVersion,
                isUpdateResVersion = isUpdateResVersion,
                isSyncAppVersion = isSyncAppVersion,
                ClientVersionFileName = clientVersionsFileName,
            };
            creater.CreateResDataVersion(isIgnoreRemote);
        }

        public bool isUpdateVersion;
        public bool isSyncToClientVersions;
        public bool isUpdateResVersion;
        public bool isSyncAppVersion;
        /// <summary>即将创建资源包的名称列表</summary>
        public List<string> ABNamesWillBuild;
        /// <summary>远程资源服务器网关</summary>
        public string resRemoteGateWay;

        public string ClientVersionFileName { get; set; }

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
                    Loader loader = new Loader
                    {
                        ApplyLoom = false
                    };
                    loader.CompleteEvent.AddListener(OnGetRemoteVersion);
                    loader.Load(resRemoteGateWay.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));
                }
            }
        }

        private void OnGetRemoteVersion(bool flag, Loader ld)
        {
            ResDataVersion remoteVers = default;
            if (flag)
            {
                const string splitKey = "}{";
                const string splitEndKey = "}";

                string data = System.Text.Encoding.UTF8.GetString(ld.ResultData);
                if (data.Contains(splitKey))
                {
                    Debug.Log("Match the '}{'");

                    string[] pattern = new string[] { splitKey };
                    string[] splited = data.Split(pattern, System.StringSplitOptions.None);
                    data = splited[0].Append(splitEndKey);
                }
                else { }

                remoteVers = JsonUtility.FromJson<ResDataVersion>(data);

                if (remoteVers == default)
                {
                    Debug.Log("Remote versions do not exists.");
                }
                else { }

                ld.Reclaim();
            }
            else
            {
                Debug.Log("Do not get remote versions.");
            }

            BuildVersionConfig(ref remoteVers);
        }

        /// <summary>
        /// 尝试创建一个可供后续操作的版本数据对象
        /// </summary>
        /// <param name="remoteVers"></param>
        /// <param name="versions"></param>
        /// <param name="resDataVersion"></param>
        private void TryFillVersionData(ref ResDataVersion remoteVers, out ResDataVersion resDataVersion)
        {
            string versions = string.Empty;
            resDataVersion = default;

            if (remoteVers == default)
            {
                //远端版本数据为空，则尝试读取本地资源版本数据
                string path = AppPaths.ABBuildOutputRoot.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME);
                versions = FileOperater.ReadUTF8Text(path);
                resDataVersion = JsonUtility.FromJson<ResDataVersion>(versions);

                if (resDataVersion == default)
                {
                    //未读取到本地保存资源版本数据文件，创建新的资源版本对象
                    resDataVersion = new ResDataVersion
                    {
                        app_version = Application.version,
                        res_version = remoteVers != default ? remoteVers.res_version : 0,//根据是否存在线上版本同步资源号
                    };
                }
                else { }
            }
            else
            {
                //使用远端版本数据克隆一个资源版本对象
                resDataVersion = new ResDataVersion();
                resDataVersion.CloneVersionsFrom(ref remoteVers);
            }
        }

        /// <summary>
        /// 创建资源版本
        /// </summary>
        /// <param name="remoteVers"></param>
        private void BuildVersionConfig(ref ResDataVersion remoteVers)
        {
            TryFillVersionData(ref remoteVers, out ResDataVersion resDataVersion);

            string[] abNamesValue = ABNamesWillBuild != default ? ABNamesWillBuild.ToArray() : new string[0];
            resDataVersion.BuildDuringEditor(ref resRemoteGateWay, isUpdateVersion, isUpdateResVersion, isSyncAppVersion, ref remoteVers, ref abNamesValue);
            resDataVersion.Refresh();

            //根据在编辑器中的设置决定是否将当前操作的资源版本对象同步到客户端安装包资源版本配置中
            if (isSyncToClientVersions)
            {
                List<ScriptableObject> list = default;
                ClientResVersion clientRes = default;// = (ClientResVersion)list[0];
                ShipDockEditorUtils.FindAssetInEditorProject(ref list, "t:ScriptableObject", @"Assets\Prefabs");

                foreach (ScriptableObject item in list)
                {
                    if (item.name == ClientVersionFileName)
                    {
                        //根据资源版本配置文件名查找客户端安装包资源版本配置
                        clientRes = (ClientResVersion)item;
                        break;
                    }
                    else { }
                }

                bool applyClientGateway = clientRes.ApplyCurrentResGateway;
                if (applyClientGateway)
                {
                    //如有必要，使用客户端安装包资源版本配置中选取的资源服务器网关地址
                    resDataVersion.res_gateway = clientRes.ClientResVersionGateway();
                }
                else { }

                //将操作后的资源版本对象数据填充到客户端安装包资源版本配置
                clientRes.Versions.CloneVersionsFrom(ref resDataVersion);
                clientRes.SetChanges(resDataVersion.ResChanges);
                UnityEditor.EditorUtility.SetDirty(clientRes);
            }
            else { }

            //将操作后的资源版本对象写入本地文件
            string versions = JsonUtility.ToJson(resDataVersion);
            string timeValue = System.DateTime.Now.ToShortDateString();
            string stamp = "_".Append(timeValue.Replace("/", "_"));

            //写入仅用于开发时查看的文件路径
            FileOperater.WriteUTF8Text(versions, Application.dataPath.Append("/Prefabs/ClientResVersions_Previews/", ResDataVersion.FILE_RES_DATA_VERSIONS_NAME, stamp, ".json"));
            //写入位于资源包输出路径下的正式文件
            FileOperater.WriteBytes(versions, AppPaths.ABBuildOutputRoot.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));

            resDataVersion.Clean();
            remoteVers?.Clean();
        }
    }
}