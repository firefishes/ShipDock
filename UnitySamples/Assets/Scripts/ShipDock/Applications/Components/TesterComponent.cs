using System;
#if G_LOG
using ShipDock.Testers;
#endif
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.Tools
{
    [ExecuteAlways]
    public class TesterComponent : MonoBehaviour
    {
#if G_LOG
        [Serializable]
        public class TesterBroker
        {
            public bool isValid;
            public string logID;
            public bool applyArgBreak;
            public int argIndex;
            public string testValue;
        }

#if ODIN_INSPECTOR
        [Title("日志设置组件")]
#endif
        [SerializeField]
#if ODIN_INSPECTOR
        [Title("日志断点")]
#endif
        private TesterBroker[] m_TesterBrokers;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("日志可用性")]
#endif
        private LogsSetting m_LogsSetting = new LogsSetting();

        private void Awake()
        {
            m_LogsSetting.Init();
            Tester.Instance.SetTestBrokerHandler(OnTestBrokerHandler);
        }

#if UNITY_EDITOR
        [ExecuteAlways]
        private void Update()
        {
            bool flag = m_LogsSetting != default ? m_LogsSetting.UpdateSetting() : false;
            if (flag)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
            else { }
        }
#endif

        private void OnTestBrokerHandler(string logID, string[] args)
        {
            TesterBroker item;
            int max = m_TesterBrokers.Length;
            for (int i = 0; i < max; i++)
            {
                item = m_TesterBrokers[i];
                if (item.isValid && (logID == item.logID))
                {
                    if (item.applyArgBreak)
                    {
                        if ((args.Length > item.argIndex) && (args[item.argIndex] == item.testValue))
                        {
                            Debug.Log("Tester said: value = ".Append(args[item.argIndex]));
                        }
                        else { }
                    }
                    else
                    {
                        Debug.Log("Tester broken: ".Append(item.testValue));
                    }
                }
                else { }
            }
        }
#endif
    }
}