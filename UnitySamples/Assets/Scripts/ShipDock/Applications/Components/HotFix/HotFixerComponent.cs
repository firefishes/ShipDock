﻿
using ILRuntime.Runtime.Enviorment;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;
#endif
using UnityEngine;

namespace ShipDock.Applications
{
    public class HotFixerComponent : HotFixer
    {

        [SerializeField]
#if ODIN_INSPECTOR
        [TitleGroup("基础信息及引用绑定")]
#endif
        private HotFixerSubgroup m_Settings = new HotFixerSubgroup();

        [SerializeField]
#if ODIN_INSPECTOR
        [TitleGroup("就绪消息"), HideIf("@this.m_StartUpInfo.IsMonoBehaviorMode == true")]
#endif
        private HotFixerLoadedNoticeInfo m_LoadedNoticeInfo = new HotFixerLoadedNoticeInfo();

        [Tooltip("设置后将覆盖动态加载的热更文件")]
#if ODIN_INSPECTOR
        [TitleGroup("文件覆盖"), LabelText("热更文件资源")]
#endif
        public TextAsset hotFixAsset;

        private ComponentBridge mCompBridge;
        private INoticeBase<int> mIDAsNotice;

        public int HotFixerReadID { get; private set; } = int.MaxValue;

        public string HotFixCompClassName
        {
            get
            {
                return m_StartUpInfo != default ? m_StartUpInfo.ClassName : string.Empty;
            }
        }

        protected override void Awake()
        {
            m_StartUpInfo.ApplyRunStandalone = false;

            base.Awake();

#if UNITY_EDITOR
            m_Settings?.Sync();
#endif
        }

        protected override void Purge()
        {
            mCompBridge?.Dispose();
            mCompBridge = default;
            m_Settings.Clear();
            m_Settings = default;
        }

        protected override void RunWithinFramework()
        {
            mCompBridge = new ComponentBridge(Init);
            mCompBridge.Start();
        }

        protected override void InitILRuntime()
        {
            base.InitILRuntime();

            ShipDockApp.Instance.ILRuntimeHotFix.Start();
        }

        protected override void Init()
        {
            base.Init();

            m_Settings.Init();

            if (ILRuntimeIniter.HasLoadAnyAssembly)
            {
                ILRuntimeLoaded();
            }
            else
            {
                TextAsset dll, pdb = default;
                if (hotFixAsset != default)
                {
                    dll = hotFixAsset;
                }
                else
                {
                    AssetBundles abs = ShipDockApp.Instance.ABs;
                    dll = abs.Get<TextAsset>(m_Settings.HotFixABName, m_Settings.HotFixDLL);
                    pdb = abs.Get<TextAsset>(m_Settings.HotFixABName, m_Settings.HotFixPDB);
                }

                StartHotFixeByAsset(this, dll, pdb);
            }
        }

        protected override void ILRuntimeLoaded()
        {
            base.ILRuntimeLoaded();

            if (!m_LoadedNoticeInfo.IsSendInRenderObject)
            {
                SendLoadedNotice();
            }
            else { }
        }

        private void OnRenderObject()
        {
            if (m_LoadedNoticeInfo.IsSendInRenderObject && !m_LoadedNoticeInfo.IsReadyNoticeSend)
            {
                SendLoadedNotice();
            }
            else { }
        }

        public int GetReadyNoticeName(out bool hasEnabled)
        {
            hasEnabled = m_LoadedNoticeInfo.IsSendIDAsNotice;
            return m_LoadedNoticeInfo.ApplyGameObjectID ? gameObject.GetInstanceID() : GetInstanceID();
        }

        public void ListenReadyNotice(Action<INoticeBase<int>> handler, int readyID = int.MaxValue)
        {
            if (m_LoadedNoticeInfo.IsSendIDAsNotice)
            {
                HotFixerReadID = readyID;
                int noticeName = GetReadyNoticeName(out _);
                noticeName.Add(handler);
            }
            else { }
        }

        private void SendLoadedNotice()
        {
            if (m_LoadedNoticeInfo.IsSendIDAsNotice && !m_LoadedNoticeInfo.IsReadyNoticeSend)
            {
                if (m_LoadedNoticeInfo.ApplyDefaultNoticeType)
                {
                    mIDAsNotice = Pooling<Notice>.From();
                }
                else
                {
                    ApplyCustomNotice();
                }

                if (m_LoadedNoticeInfo.ApplyCallLate)
                {
                    UpdaterNotice.SceneCallLater(SendLoadedNoticeAndRelease);
                }
                else
                {
                    SendLoadedNoticeAndRelease(0);
                }
            }
            else { }
        }

        private void SendLoadedNoticeAndRelease(int time)
        {
            if (m_Settings == default)
            {
                return;
            }
            else { }

            int noticeName = GetReadyNoticeName(out _);
            noticeName.Broadcast(mIDAsNotice);
            if (mIDAsNotice is IPoolable item)
            {
                item.ToPool();
            }
            else
            {
                mIDAsNotice?.Dispose();
            }

            if (m_LoadedNoticeInfo.IsSendInRenderObject)
            {
                bool flag = m_LoadedNoticeInfo.IsSendOnceInRenderObject;
                m_LoadedNoticeInfo.SetReadyNoticeSend(flag);
            }
            else { }
        }

        private void ApplyCustomNotice()
        {
            string methodName = m_LoadedNoticeInfo.GetIDAsCustomNoticeMethod;
            if (string.IsNullOrEmpty(methodName))
            {
                mIDAsNotice = Pooling<Notice>.From();
            }
            else
            {
                ILRuntimeUtils.InvokeMethodILR(ShellBridge, m_StartUpInfo.ClassName, methodName, 0, OnGetIDAsNoticeHandler);
                if (mIDAsNotice == default)
                {
                    mIDAsNotice = Pooling<Notice>.From();
                }
                else { }
            }
        }

        private void OnGetIDAsNoticeHandler(InvocationContext context)
        {
            mIDAsNotice = context.ReadObject<INoticeBase<int>>();
        }

        public ValueSubgroup GetDataField(string keyField)
        {
            return m_Settings.GetDataField(ref keyField);
        }

        public SceneNodeSubgroup GetSceneNode(string keyField)
        {
            return m_Settings.GetSceneNode(ref keyField);
        }

        public void DisableReadyNotice()
        {
            m_LoadedNoticeInfo?.DisableReadyNotice();
        }

        protected void EnableReadyNotice()
        {
            m_LoadedNoticeInfo?.EnableReadyNotice();
        }
    }
}