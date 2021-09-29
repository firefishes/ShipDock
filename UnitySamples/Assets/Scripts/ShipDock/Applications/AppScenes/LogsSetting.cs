using System;
using UnityEngine;
using System.Collections.Generic;
using ShipDock.Testers;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.Tools
{
    [Serializable]
    public class LogEnabledItem
    {
#if ODIN_INSPECTOR
        [SerializeField]
        [LabelText("@m_TitleValue"), PropertyTooltip("@this.decs"), LabelWidth(400f), EnableIf("@false")]
        private string m_Title;
#endif

#if ODIN_INSPECTOR
        [SerializeField, HideInInspector]
        private string m_TitleValue = "";

        public string OnRefreshTitle()
        {
            m_TitleValue = decs.Append(" - ", enabled ? "ON" : "OFF", " - ID: ", logID);
            return m_TitleValue;
        }

        [LabelText("是否可用"), ToggleLeft(), OnValueChanged("OnRefreshTitle"), Indent(1)]
#endif
        public bool enabled;

#if ODIN_INSPECTOR
        [SerializeField]
        [LabelText("编辑"), ToggleLeft(), Indent(1)]
        private bool m_Edit = true;
#endif

#if ODIN_INSPECTOR
        [LabelText("描述"), ShowIf("@this.m_Edit"), OnValueChanged("OnRefreshTitle"), Indent(1)]
#endif
        public string decs;

#if ODIN_INSPECTOR
        [LabelText("日志 ID"), ShowIf("@this.m_Edit"), OnValueChanged("OnRefreshTitle"), Indent(1)]
#endif
        public string logID;

#if ODIN_INSPECTOR
        [LabelText("测试器名"), ShowIf("@UnityEngine.Application.isPlaying"), Indent(1)]
#endif
        public string testerName;

        public LogEnabledSubgroup Subgroup { get; set; }
    }

    [Serializable]
    public class LogsSetting
    {
#if G_LOG
#if UNITY_EDITOR
        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("启用Unity断言"), Indent(1)]
#endif
        private bool m_ApplyUnityAssert;
#endif

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("更新组件"), Indent(1)]
#endif
        private bool m_Sync;
        private bool mChangeAllEnabled;

#if ODIN_INSPECTOR
        private string mEnabledAllTitle = "启用全部";

        private void OnEnabledAll()
        {
            mChangeAllEnabled = true;
            mEnabledAllTitle = m_EnabledAll ? "禁用全部" : "启用全部";
            UpdateSetting();
        }

        [SerializeField]
        [LabelText("@this.mEnabledAllTitle"), OnValueChanged("OnEnabledAll"), Indent(1)]
#endif
        private bool m_EnabledAll;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("更新生效的延时"), PropertyTooltip("编辑器下同样生效"), Indent(1)]
#endif
        private float m_RefreshTime = 1f;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("所有已定义的日志"), Indent(1)]
#endif
        private List<LogEnabledItem> m_Enableds;

        private float mTime;
        private KeyValueList<string, int> mMapper;

        private LogEnabledSubgroup OnLogItemAdded(string logID, LogItem arg)
        {
            LogEnabledSubgroup result = default;
            if (mMapper.ContainsKey(logID))
            {
                int index = mMapper[logID];
                LogEnabledItem item = m_Enableds[index];
                result = new LogEnabledSubgroup()
                {
                    logID = item.logID,
                };
                result.enabled = item.enabled;
                item.Subgroup = result;

#if ODIN_INSPECTOR
                item.OnRefreshTitle();
#endif
            }
            else { }
            return result;
        }

        public void Init()
        {
            mTime = m_RefreshTime;
            Tester tester = Tester.Instance;
            tester.OnLogItemAdded += OnLogItemAdded;

#if UNITY_EDITOR
            if (m_ApplyUnityAssert)
            {
                tester.OnUnityAssert += OnUnityAssert;
            }
            else { }
#endif

            LogEnabledItem item;
            mMapper = new KeyValueList<string, int>();

            int max = m_Enableds != default ? m_Enableds.Count : 0;
            for (int i = 0; i < max; i++)
            {
                item = m_Enableds[i];
                mMapper[item.logID] = i;
            }
            m_Sync = true;
        }

#if UNITY_EDITOR
        private void OnUnityAssert(string target, string correct)
        {
            try
            {
                UnityEngine.Assertions.Assert.AreEqual(target, correct);
            }
            catch (System.Exception _) { }
        }
#endif

        public bool UpdateSetting()
        {
            bool result = false;
            mTime -= Time.deltaTime;

            if (mTime <= 0f)
            {
                mTime = m_RefreshTime;
                if (m_Sync || mChangeAllEnabled)
                {
                    LogEnabledItem item;
                    bool isPlaying = Application.isPlaying;
                    int max = m_Enableds != default ? m_Enableds.Count : 0;
                    for (int i = 0; i < max; i++)
                    {
                        item = m_Enableds[i];
                        if (isPlaying)
                        {
                            if (item.Subgroup != default)
                            {
                                item.testerName = item.Subgroup.testerName;
                                item.Subgroup.enabled = item.enabled;
                                item.Subgroup.Commit();
                            }
                            else { }
                        }
                        else
                        {
#if ODIN_INSPECTOR
                            item.OnRefreshTitle();
#endif
                        }

                        if (mChangeAllEnabled)
                        {
                            item.enabled = m_EnabledAll;
                            if (item.Subgroup != default)
                            {
                                item.Subgroup.enabled = item.enabled;
                                item.Subgroup.Commit();
                            }
                            else { }
#if ODIN_INSPECTOR
                            item.OnRefreshTitle();
#endif
                        }
                        else { }
                    }
                    result = true;
                }
                else { }
                mChangeAllEnabled = false;
                m_Sync = false;
            }
            else { }
            return result;
        }
#endif
                        }
                    }