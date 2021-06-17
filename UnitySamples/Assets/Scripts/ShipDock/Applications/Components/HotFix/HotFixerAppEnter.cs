﻿using System;
using UnityEngine;

namespace ShipDock.Applications
{
    public class HotFixerAppEnter<T> : HotFixBridge where T : AppHotFixConfigBase, new()
    {
        [SerializeField]
        private TextAsset m_EnterDll;
        [SerializeField]
        private TextAsset m_EnterPdb;

        public Action ILHotFixerEnterInited { get; set; }
        public GameObject ShipDockAppOwner { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ILHotFixerEnterInited = default;
            m_EnterDll = default;
            m_EnterPdb = default;
        }

        protected override void Awake()
        {
            if (m_StartUpInfo.RunInAwake)
            {
                "debug".Log("HotFixerAppEnter RunInAwake field must be set true.");
            }
            base.Awake();
        }

        protected override void Init()
        {
            if (m_EnterDll == default)
            {
                "debug".Log("HotFixerAppEnter EnterDll field must be set not null object.");
                return;
            }

#if ILRUNTIME
            ShipDockApp app = ShipDockApp.Instance;
            app.SetHotFixSetting(new ILRuntimeHotFix(ShipDockApp.Instance), new T());
            app.ILRuntimeHotFix.Start();
#endif
            base.Init();

            StartHotFixeByAsset(this, m_EnterDll, m_EnterPdb);
        }

        protected override void ILRuntimeLoaded()
        {
            base.ILRuntimeLoaded();

            ILRuntimeUtils.InvokeMethodILR(ShellBridge, m_StartUpInfo.ClassName, "SetEnterInitedCallback", 2, ShipDockAppOwner, ILHotFixerEnterInited);
        }
    }
}