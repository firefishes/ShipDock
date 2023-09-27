#define LOG_ENABLED

using ShipDock;
using System;
using System.IO;
using UnityEngine;

namespace HotFixClient
{
    public interface IStartupLoadingUIModular : IUIModular
    {
        void SetDownloadAppCallback(Action<bool> callback);
        void SetResVersion(string resVersionValue);
        void SetLoadingCount(int updatingMax);
        void SetLoaded(int updatingLoaded);
    }

    public class HotFixRunner : HotFixBase
    {
        protected virtual string HotFixABName { get; set; } = "hotfix";

        private HotFixer mHotFixer;
        private ShipDockGame mGameMain;
        private ClientResVersion mClientVersions;
        private IStartupLoadingUIModular mStartupUIModular;

        public override void ShellInited(MonoBehaviour target)
        {
#if LOG_ENABLED
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
            "log".Log("热更端启动器已运行");//#223122
            mHotFixer = target as HotFixer;
        }

        public void SetGameComponent(ShipDockGame main)
        {
            mGameMain = main;
            mClientVersions = mGameMain.DevelopSetting.remoteAssetVersions;

            OpenStartUpLoadingUI(false);
        }

        protected virtual string GetLoadingUIABName()
        {
            return string.Empty;
        }

        private void OpenStartUpLoadingUI(bool fromPersistent)
        {
            string abName = GetLoadingUIABName();// "ui/ui_loading";
            if (fromPersistent && !string.IsNullOrEmpty(abName))
            {
                string path = AppPaths.PersistentResDataRoot.Append(abName);
                if (File.Exists(path))
                {
                    GetUIAsset(ref abName, true);
                }
                else
                {
                    GetUIAsset(ref abName, false);
                }
            }
            else
            {
                GetUIAsset(ref abName, false);
            }
        }

        private void GetUIAsset(ref string path, bool isPersistent)
        {
            AssetsLoader loader = new AssetsLoader();
            loader.CompleteEvent.AddListener(OnAssetLoaded);

            loader.AddManifest(AppPaths.resData, isPersistent);
            loader.Add(path, true, isPersistent);

            loader.Load(out _);
        }

        protected virtual IStartupLoadingUIModular OpenStartupLoadingUIModular()
        {
            return default;
        }

        protected virtual void OpenDownloadNewestAppUIMudular(Action<bool> callback)
        {
            mStartupUIModular?.SetDownloadAppCallback(callback);
        }

        protected virtual void OnGetNewestAppCallback(bool confirm)
        {
            if (confirm)
            {
                string url = "https://mhjcats.oss-cn-beijing.aliyuncs.com/mhj_download/EliminatesNewest.apk";
                Application.OpenURL(url.Append(mClientVersions.RemoteAppVersion, ".apk"));
            }
            else
            {
                Application.Quit();
            }
        }

        private void OnAssetLoaded(bool successed, AssetsLoader loader)
        {
            loader.Reclaim();

            if (successed)
            {
                UIManager UIs = ShipDockApp.Instance.UIs;
                UIs.UIRoot.UICamera.orthographic = true;

                mStartupUIModular = OpenStartupLoadingUIModular();
                
                mClientVersions.LoadRemoteVersion(OnLoadComplete, OnVersionInvalid, out _);
            }
            else
            {
                "error".Log("加载开屏资源更新界面失败");
            }
        }

        private bool OnVersionInvalid()
        {
            string version = mClientVersions.RemoteAppVersion;
            string[] splits = version.Split(StringUtils.DOT_CHAR);
            int packVersion = int.Parse(splits[0]);
            int patchVersion = int.Parse(splits[1]);

            version = Application.version;
            splits = version.Split(StringUtils.DOT_CHAR);

            bool result = packVersion > int.Parse(splits[0]) || patchVersion > int.Parse(splits[1]);
            if (result)
            {
                OpenDownloadNewestAppUIMudular(OnGetNewestAppCallback);
            }
            else { }

            return result;
        }

        private void OnLoadComplete(bool isComplete, float progress)
        {
            int resVersionValue = mClientVersions.CachedVersion.res_version;
            mStartupUIModular?.SetResVersion(resVersionValue.ToString());
            mStartupUIModular?.SetLoadingCount(mClientVersions.UpdatingMax);

            if (isComplete)
            {
                mStartupUIModular?.Name.Close();

                AssetBundles abs = Framework.Instance.GetUnit<AssetBundles>(Framework.UNIT_AB);

                string UIABName = GetLoadingUIABName();

                if (!string.IsNullOrEmpty(UIABName))
                {
                    abs.Remove(UIABName, true);
                }
                else { }

                abs.Remove(HotFixABName, true);
                abs.Remove(AppPaths.resData, true);

                ShipDockApp.Instance.ILRuntimeHotFix.Clear();

                mGameMain.PreloadAsset();

                UnityEngine.Object.Destroy(mHotFixer.gameObject);
            }
            else
            {
                mStartupUIModular?.SetLoaded(mClientVersions.UpdatingLoaded);
            }
        }

        public override void FixedUpdate()
        {
        }

        public override void LateUpdate()
        {
        }

        public override void Update()
        {
        }
    }
}
