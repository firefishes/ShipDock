﻿#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ShipDock
{
    public enum ResDataVersionType
    {
        /// <summary>空的资源版本</summary>
        Empty = 0,
        /// <summary>客户端上的资源版本</summary>
        Client,
        /// <summary>远端的资源版本</summary>
        Remote,
        /// <summary>已缓存的资源版本</summary>
        Cached,
    }

    /// <summary>
    /// 
    /// 资源版本
    /// 
    /// TODO 
    /// 需要增加zip包更新的功能（这个功能与单独文件更新该如何配合？）
    /// 
    /// </summary>
    [Serializable]
    public class ResDataVersion
    {
        /// <summary>默认的版本号初始值</summary>
        public const int DEFAULT_VERSION = 100;
        /// <summary>版本配置文件的文件名</summary>
        public static string FILE_RES_DATA_VERSIONS_NAME = "res_data_versions.sd";

        #region 编辑器相关
#if UNITY_EDITOR
        //发生版本变化的版本项列表
        private List<ResVersion> mResChanged;

        /// <summary>本版数据包含的所有变更过的资源版本</summary>
        public ResVersion[] ResChanges { get; private set; }

        /// <summary>
        /// 在编辑器中构建资源版本
        /// </summary>
        /// <param name="remoteRootURL">远端资源服务器网关</param>
        /// <param name="isUpdateVersion">是否迭代资源版本</param>
        /// <param name="isUpdateResVersion">是否迭代资源大版本</param>
        /// <param name="isSyncAppVersion">是否同步应用版本</param>
        /// <param name="remoteVers">远端资源服务器的资源配置</param>
        /// <param name="abNames">需要迭代版本的资源名</param>
        public void BuildDuringEditor(ref string remoteRootURL, bool isUpdateVersion, bool isUpdateResVersion, bool isSyncAppVersion, ref ResDataVersion remoteVers, ref string[] abNames)
        {
            res_gateway = remoteRootURL;

            //初始化正在操作的资源版本对象
            Init();
            //初始化从远端获取数据时创建的资源版本对象
            remoteVers?.Init();

            CheckMainManifestVersion(ref abNames);
            FillChangedABList(out List<string> changeds);

            int max = abNames.Length;
            CheckTotalVersion(max, isUpdateResVersion, isSyncAppVersion);
            CheckResVersions(ref abNames, ref remoteVers, isUpdateVersion, ref changeds);
        }

        /// <summary>
        /// 检测各个资源版本号是否需要升版
        /// </summary>
        /// <param name="abNames"></param>
        /// <param name="remoteVers"></param>
        /// <param name="isUpdateVersion"></param>
        /// <param name="changeds"></param>
        private void CheckResVersions(ref string[] abNames, ref ResDataVersion remoteVers, bool isUpdateVersion, ref List<string> changeds)
        {
            string abName = string.Empty;
            ResVersion item, remote = default;

            int max = abNames.Length;
            for (int i = 0; i < max; i++)
            {
                abName = abNames[i];
                item = GetResVersion(abName);
                int baseVersion = item == default ? DEFAULT_VERSION : item.version;//若未找到对应的资源版本，基于默认的版本号进行设置

                if (remoteVers != default)
                {
                    remote = remoteVers.GetResVersion(abName);
                    baseVersion = remote != default ? remote.version : baseVersion;//若已存在线上版本，基于线上版本号进行设置
                }
                else { }

                CheckAndAdvanceResVersion(baseVersion, isUpdateVersion, ref item, ref abName);
                RecordVersionChange(ref changeds, ref abName, ref item);
            }
        }

        /// <summary>
        /// 检测App版本号、资源总版本号是否需要升版
        /// </summary>
        /// <param name="changeABCount"></param>
        /// <param name="isUpdateResVersion"></param>
        /// <param name="isSyncAppVersion"></param>
        private void CheckTotalVersion(int changeABCount, bool isUpdateResVersion, bool isSyncAppVersion)
        {
            if (isUpdateResVersion && changeABCount > 0)
            {
                res_version++;
            }
            else { }

            if (isSyncAppVersion)
            {
                app_version = Application.version;
            }
            else { }
        }

        /// <summary>
        /// 填充发生变化的资源包版本清单
        /// </summary>
        /// <param name="changeds"></param>
        private void FillChangedABList(out List<string> changeds)
        {
            changeds = new List<string>();

            string abName = string.Empty;
            int max = ResChanges != default ? ResChanges.Length : 0;
            for (int i = 0; i < max; i++)
            {
                abName = ResChanges[i].name;
                changeds.Add(abName);
            }
        }

        /// <summary>
        /// 迭代资源总依赖文件的版本
        /// </summary>
        /// <param name="abNames"></param>
        private void CheckMainManifestVersion(ref string[] abNames)
        {
            ResVersion resData = GetResVersion(AppPaths.resData);
            if (resData == default)
            {
                AddNewRes(AppPaths.resData, DEFAULT_VERSION);
            }
            else
            {
                if (abNames.Length > 0)
                {
                    resData.version++;
                }
                else { }
            }
        }

        /// <summary>
        /// 对具体资源进行升版
        /// </summary>
        /// <param name="baseVersion"></param>
        /// <param name="isUpdateVersion"></param>
        /// <param name="item"></param>
        /// <param name="name"></param>
        private void CheckAndAdvanceResVersion(int baseVersion, bool isUpdateVersion, ref ResVersion item, ref string name)
        {
            if (item == default)
            {
                //追加新的资源版本
                item = AddNewRes(name, DEFAULT_VERSION);
                mResIndexMapper[name] = mRes.Count;
            }
            else
            {
                //资源升版
                int version = isUpdateVersion ? baseVersion + 1 : baseVersion;
                item.version = version;
            }
        }

        /// <summary>
        /// 记录发生版本变化的版本项
        /// </summary>
        /// <param name="changeds"></param>
        /// <param name="abName"></param>
        /// <param name="item"></param>
        private void RecordVersionChange(ref List<string> changeds, ref string abName, ref ResVersion item)
        {
            if (changeds.Contains(abName))
            {
                //版本项已在列表中则直接更新版本号
                int index = changeds.IndexOf(abName);
                ResVersion resChangedItem = mResChanged[index];
                resChangedItem.version = item.version;
            }
            else
            {
                if (item != default)
                {
                    mResChanged.Add(item);
                }
                else
                {
                    Debug.LogError("Version item is null druing record the changes");
                }
            }
        }

        private void FillRepeatsVersionRes(out List<ResVersion> repeates)
        {
            repeates = new List<ResVersion>();
            KeyValueList<string, ResVersion> realMapper = new KeyValueList<string, ResVersion>();

            string name;
            foreach (ResVersion item in mRes)
            {
                name = item.name;
                if (realMapper.ContainsKey(name))
                {
                    if (realMapper[name].version >= item.version)
                    {
                        Debug.LogWarning("Repeate version item, name is " + name);
                        repeates.Add(item);
                    }
                    else
                    {
                        realMapper[name] = item;
                    }
                }
                else
                {
                    realMapper[name] = item;
                }
            }
        }
#endif

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void InitResChangedInEditor()
        {
#if UNITY_EDITOR
            mResChanged = ResChanges == default ? new List<ResVersion>() : new List<ResVersion>(ResChanges);
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void RefreshResChangedInEditor()
        {
#if UNITY_EDITOR
            if (mResChanged != default)
            {
                ResChanges = mResChanged.ToArray();
            }
            else { }
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void RefreshInEditor()
        {
#if UNITY_EDITOR
            if (mRes != default)
            {
                FillRepeatsVersionRes(out List<ResVersion> repeates);

                int max = repeates.Count;
                Debug.Log("Repeate items total " + max);
                for (int i = 0; i < max; i++)
                {
                    mRes.Remove(repeates[i]);
                }

                CreateIndexsMapper();

                res = mRes.ToArray();
            }
            else { }
#endif
        }
        #endregion

        /// <summary>
        /// 作为本地正式缓存写入本地文件
        /// </summary>
        public void WriteAsCached()
        {
            Refresh();
            SaveUpdatings();

            string versions = JsonUtility.ToJson(this);
            string path = AppPaths.PersistentResDataRoot.Append(FILE_RES_DATA_VERSIONS_NAME);
#if !RELEASE
            FileOperater.WriteUTF8Text(versions, path.Append(".json~"));//写入一个用于查看的文件
#endif
            "todo".Log("资源版本写入设备时需要加密");
            FileOperater.WriteBytes(versions, path);
        }

        /// <summary>资源配置的类别</summary>
#if ODIN_INSPECTOR
        [LabelText("资源版本类型"), ReadOnly]
#endif
        public ResDataVersionType resVersionType = ResDataVersionType.Empty;

        /// <summary>资源配置版本号</summary>
#if ODIN_INSPECTOR
        [LabelText("资源配置版本号")]
#endif
        public int res_version;

        /// <summary>App版本号</summary>
#if ODIN_INSPECTOR
        [LabelText("App版本号")]
#endif
        public string app_version;

        /// <summary>远程资源服务器网关</summary>
#if ODIN_INSPECTOR
        [LabelText("远程资源服务器网关")]
#endif
        public string res_gateway;

        /// <summary>总资源数</summary>
#if ODIN_INSPECTOR
        [LabelText("资源总数")]
#endif
        public int res_total;

        /// <summary>更新的资源数</summary>
#if ODIN_INSPECTOR
        [LabelText("更新中断等待重连数")]
#endif
        public int updating_total;

        /// <summary>本版数据包含的所有资源版本</summary>
#if ODIN_INSPECTOR
        [LabelText("资源版本列表")]
#endif
        public ResVersion[] res;

        /// <summary>需要加载的资源映射，用于排除重复更新/summary>
#if ODIN_INSPECTOR
        [LabelText("等待重连列表")]
#endif
        public ResUpdating[] updatings;

        private int mUpdatingCount;
        private List<ResVersion> mRes;
        private List<ResUpdating> mUpdatings;
        private Dictionary<string, int> mResIndexMapper;
        private Dictionary<string, ResUpdating> mUpdatingMapper;

        public int UpdatingMax { get; private set; }
        public Action UpdateCompleted { get; set; }

        public int UpdatingLoaded
        {
            get
            {
                int result = UpdatingMax - mUpdatingCount;

                Mathf.Max(0, result);
                Mathf.Min(UpdatingMax, result);
                return result;
            }
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            InitTemporaryData();

            InitResChangedInEditor();
        }

        private void InitTemporaryData()
        {
            mRes?.Clear();
            mUpdatings?.Clear();

            CreateIndexsMapper();

            mRes = IsVersionsEmpty() ? new List<ResVersion>() : new List<ResVersion>(res);
            if (updatings != default)
            {
                mUpdatings = new List<ResUpdating>(updatings);
                Array.Clear(updatings, 0, updatings.Length);
            }
            else
            {
                mUpdatings = new List<ResUpdating>();
            }
        }

        /// <summary>
        /// 创建版本列表的索引映射
        /// </summary>
        private void CreateIndexsMapper()
        {
            mResIndexMapper?.Clear();
            mResIndexMapper = new Dictionary<string, int>();

            string resName;
            ResVersion item;

            int max = IsVersionsEmpty() ? 0 : res.Length;
            for (int i = 0; i < max; i++)
            {
                item = res[i];
                resName = item.name;
                mResIndexMapper[resName] = i;
            }
        }

        private void CleanResAndUpdatinsRaw()
        {
            if (res != default)
            {
                Array.Clear(res, 0, res.Length);
                res = new ResVersion[0];
            }
            else { }

            if (updatings != default)
            {
                Array.Clear(updatings, 0, updatings.Length);
                updatings = new ResUpdating[0];
            }
            else { }
        }

        public void Clean(bool isRrefresh = true, bool deleteAll = false)
        {
            if (isRrefresh)
            {
                Refresh();
            }
            else { }

            if (deleteAll)
            {
                CleanResAndUpdatinsRaw();
            }
            else { }

            mRes?.Clear();
            mUpdatings?.Clear();
            mResIndexMapper?.Clear();
            mUpdatingMapper?.Clear();

            UpdateCompleted = default;
            mResIndexMapper = default;
            mUpdatingMapper = default;
            mUpdatings = default;
        }

        /// <summary>
        /// 将中间数据更新到此对象的正式数据
        /// </summary>
        public void Refresh()
        {
            RefreshInEditor();
            RefreshResChangedInEditor();

            res_total = res.Length;
            updating_total = updatings.Length;
        }

        /// <summary>
        /// 保存正在等待更新的资源版本
        /// </summary>
        private void SaveUpdatings()
        {
            mUpdatings?.Clear();
            if (mUpdatings == default)
            {
                mUpdatings = new List<ResUpdating>();
            }
            else { }

            int max = mUpdatingMapper != default ? mUpdatingMapper.Count : 0;
            if (max > 0)
            {
                ResUpdating item;
                var enumer = mUpdatingMapper.GetEnumerator();
                for (int i = 0; i < max; i++)
                {
                    item = enumer.Current.Value;
                    if (item != default)
                    {
                        mUpdatings.Add(item);
                    }
                    else { }

                    enumer.MoveNext();
                }
            }
            else { }

            updatings = new ResUpdating[max];
            updatings = mUpdatings.ToArray();
            updating_total = updatings.Length;
            "log: Updating saved, total {0}".Log(mUpdatings.Count > 0, mUpdatings.Count.ToString());
        }

        /// <summary>
        /// 从另一个资源版本对象复制数据
        /// </summary>
        /// <param name="copyFrom"></param>
        public void CloneVersionsFrom(ref ResDataVersion copyFrom)
        {
            Clean(false, true);

            if (copyFrom.IsVersionsEmpty())
            {
                copyFrom.Init();
            }
            else { }

            app_version = copyFrom.app_version;
            res_version = copyFrom.res_version;
            res_gateway = copyFrom.res_gateway;

            ResVersion item;

            int resSize = copyFrom.res.Length;
            res = new ResVersion[resSize];
            for (int i = 0; i < resSize; i++)
            {
                item = copyFrom.res[i];
                res[i] = ResVersion.CreateNew(item.name, item.version, item.file_size);
            }
            
            int updatingsSize = copyFrom.updatings != default ? copyFrom.updatings.Length : 0;
            updatings = new ResUpdating[updatingsSize];
            for (int i = 0; i < updatingsSize; i++)
            {
                updatings[i] = new ResUpdating()
                {
                    name = copyFrom.updatings[i].name,
                    version = copyFrom.updatings[i].version,
                };
            }

            res_total = res.Length;
            updating_total = updatings.Length;
        }

        /// <summary>
        /// 检查版本差异
        /// </summary>
        /// <param name="clientVersions"></param>
        /// <param name="remoteVersions"></param>
        /// <returns></returns>
        public List<ResVersion> CheckUpdates(ResDataVersion clientVersions, ref ResDataVersion remoteVersions)
        {
            if (clientVersions.resVersionType != ResDataVersionType.Client || 
                remoteVersions.resVersionType != ResDataVersionType.Remote)
            {
                return new List<ResVersion>(0);
            }
            else { }

            bool isVersionsEmpty = IsVersionsEmpty();
            if (isVersionsEmpty)
            {
                CloneVersionsFrom(ref clientVersions);//复制安装包中默认的资源版本
            }
            else { }

            Init();

            ResVersion remoteItem = default, cachedItem = default;
            if (isVersionsEmpty)
            {
                AddUpdatingsFromExisted(ref remoteItem, ref cachedItem);
            }
            else { }

            SyncCachedUpdatingToMapper();
            AddUpdatingsFromRemote(ref remoteVersions, ref remoteItem, ref cachedItem);

            Refresh();

            GetWillUpdateList(out List<ResVersion> result);
            return result;
        }

        /// <summary>
        /// 获取更新列表
        /// </summary>
        /// <param name="result"></param>
        private void GetWillUpdateList(out List<ResVersion> result)
        {
            mUpdatingCount = mUpdatings.Count;
            UpdatingMax = mUpdatings.Count;
            result = new List<ResVersion>();
            string abName;
            ResVersion item;
            int max = mUpdatings.Count;
            for (int i = 0; i < max; i++)
            {
                abName = mUpdatings[i].name;
                item = GetResVersion(abName);

                if (item == default)
                {
                    item = ResVersion.CreateNew(abName, mUpdatings[i].version, mUpdatings[i].file_size);
                }
                else { }

                item.Url = res_gateway.Append(abName);
                result.Add(item);
            }
        }

        /// <summary>
        /// 从远端资源版本对比资源差异，并标记需要更新的资源
        /// </summary>
        /// <param name="remoteVersions"></param>
        /// <param name="remoteItem"></param>
        /// <param name="cachedItem"></param>
        private void AddUpdatingsFromRemote(ref ResDataVersion remoteVersions, ref ResVersion remoteItem, ref ResVersion cachedItem)
        {
            if (remoteVersions.resVersionType == ResDataVersionType.Remote)
            {
                bool isResExist;
                ResVersion[] list = remoteVersions.res;
                int max = list.Length, statu = 0;
                for (int i = 0; i < max; i++)
                {
                    statu = 0;
                    remoteItem = list[i];
                    isResExist = IsResExist(ref remoteItem);

                    if (isResExist)
                    {
                        CampareVersion(ref cachedItem, ref remoteItem, out statu);
                    }
                    else
                    {
                        statu = 1;//服务端的文件不存在于本地，新建版本并更新
                    }

                    switch (statu)
                    {
                        case 1:
                        case 2:
                            if (statu == 1)
                            {
                                cachedItem = AddNewRes(remoteItem.name, remoteItem.version);//增加本地新资源的版本
                                "warning: Res {0} version is new one or deleted, will add and update".Log(cachedItem.name);
                            }
                            else { }
                            AddToUpdate(ref remoteItem, cachedItem);
                            break;
                    }
                }
            }
            else { }
        }

        /// <summary>
        /// 标记本版本包含的所有资源为需要更新
        /// </summary>
        /// <param name="remoteItem"></param>
        /// <param name="cachedItem"></param>
        private void AddUpdatingsFromExisted(ref ResVersion remoteItem, ref ResVersion cachedItem)
        {
            int willUpdate = mRes.Count;
            for (int i = 0; i < willUpdate; i++)
            {
                remoteItem = mRes[i];
                AddToUpdate(ref remoteItem);
            }
        }

        /// <summary>
        /// 同步上次记录的更新列表（仅对类别为本地缓存的资源版本配置有效）
        /// </summary>
        private void SyncCachedUpdatingToMapper()
        {
            if (resVersionType == ResDataVersionType.Cached)
            {
                mUpdatingMapper?.Clear();//清空，为重新映射数据做准备
                if (mUpdatingMapper == default)
                {
                    mUpdatingMapper = new Dictionary<string, ResUpdating>();
                }
                else { }

                string abName;
                ResUpdating resUpdate;
                int max = mUpdatings.Count;
                for (int i = 0; i < max; i++)
                {
                    resUpdate = mUpdatings[i];
                    abName = resUpdate.name;
                    abName = abName.ToLower();//资源打包后的文件名均为小写
                    if (!mUpdatingMapper.ContainsKey(abName))
                    {
                        mUpdatingMapper[abName] = resUpdate;
                    }
                    else { }
                }
            }
            else { }
        }

        /// <summary>
        /// 将资源版本标记为需要更新
        /// </summary>
        /// <param name="remoteItem"></param>
        private void AddToUpdate(ref ResVersion remoteItem, ResVersion cached = default)
        {
            bool hasUpdating = mUpdatingMapper.TryGetValue(remoteItem.name, out _);
            if (!hasUpdating)
            {
                string name = remoteItem.name.ToLower();
                ResUpdating newUpdating = new ResUpdating()
                {
                    name = name,
                    version = remoteItem.version,
                };
                mUpdatings.Add(newUpdating);
                mUpdatingMapper[name] = newUpdating;
                "log: Res {0} version changed ({1} -> {2}), will update".Log(remoteItem.name, (cached != default) ? cached.version.ToString() : "...", remoteItem.version.ToString());
            }
            else { }
        }

        /// <summary>
        /// 查找本地没有的资源及版本号不同的更新
        /// </summary>
        /// <param name="cachedItem"></param>
        /// <param name="remoteItem"></param>
        /// <param name="needUpdate"></param>
        private void CampareVersion(ref ResVersion cachedItem, ref ResVersion remoteItem, out int statu)
        {
            statu = 0;
            cachedItem = GetResVersion(remoteItem.name);
            if (cachedItem == default)
            {
                statu = 1;//未包含此资源版本
            }
            else
            {
                if (cachedItem.version < remoteItem.version)
                {
                    "log: Version comparing: prev is {0}, newer is {1}".Log(cachedItem.version.ToString(), remoteItem.version.ToString());
                    statu = 2;//需要更新资源的版本
                    cachedItem.version = remoteItem.version;
                }
                else { }
            }
        }

        /// <summary>
        /// 是否为不同的资源版本配置（对比应用版本及资源大版本）
        /// </summary>
        /// <param name="remoteVersions"></param>
        /// <returns></returns>
        private bool IsDiffVersionData(ref ResDataVersion remoteVersions)
        {
            return (res_version != remoteVersions.res_version) ||
                    (app_version != remoteVersions.app_version);
        }

        /// <summary>
        /// 判断资源版本所代表的文件是否存在于本地缓存
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private bool IsResExist(ref ResVersion version)
        {
            string filePath = AppPaths.PersistentResDataRoot.Append(version.name);
            return (version != default) && File.Exists(filePath);
        }

        /// <summary>
        /// 检测此版本对象是否一个空的资源版本配置
        /// </summary>
        /// <returns></returns>
        public bool IsVersionsEmpty()
        {
            return res == default || res.Length == 0;
        }

        /// <summary>
        /// 新增一个资源版本
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private ResVersion AddNewRes(string abName, int version)
        {
            ResVersion item = ResVersion.CreateNew(abName, version, 0);
            mRes.Add(item);
            return item;
        }

        /// <summary>
        /// 获取资源版本项
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public ResVersion GetResVersion(string abName)
        {
            ResVersion result = default;
            if (mResIndexMapper != default)
            {
                bool hasValue = mResIndexMapper.TryGetValue(abName, out int value);
                if (hasValue)
                {
                    result = res[value];
                }
                else { }
            }
            else { }

            return result;
        }

        /// <summary>
        /// 从更新列表中移除一个已更新的资源版本
        /// </summary>
        /// <param name="abName"></param>
        public void RemoveUpdate(string abName)
        {
            if (mUpdatingMapper.ContainsKey(abName))
            {
                mUpdatingMapper.Remove(abName);
                mUpdatingCount--;
            }
            else { }

            "log: Patch updated, remains {0}".Log(mUpdatingCount.ToString());

            if (mUpdatingCount <= 0)
            {
                UpdateCompleted?.Invoke();
                UpdateCompleted = default;

                "log".Log("Patches update Completed!");
            }
            else { }
        }

        public void ResetUpdatingsCount()
        {
            mUpdatingCount = UpdatingMax;
        }
    }
}
