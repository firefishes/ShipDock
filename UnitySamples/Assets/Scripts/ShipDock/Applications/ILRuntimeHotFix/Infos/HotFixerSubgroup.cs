
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using System;
using UnityEngine;

namespace ShipDock
{
    [Serializable]
    public class HotFixerSubgroup : UINodes
    {
        [Header("桥接至热更端的绑定信息")]
        [SerializeField, Tooltip("AsstBundle资源名")]
#if ODIN_INSPECTOR
        [LabelText("热更文件所在资源包名")]
#endif
        protected string m_HotFixABName;

        [SerializeField, Tooltip("dll热更资源名")]
#if ODIN_INSPECTOR
        [LabelText("热更文件（dll）"), SuffixLabel(".dll")]
#endif
        protected string m_HotFixDLL;

        [SerializeField, Tooltip("pdb符号表文件资源名")]
#if ODIN_INSPECTOR
        [LabelText("热更文件（pdb）"), SuffixLabel(".pdb")]
#endif
        protected string m_HotFixPDB;

        public string HotFixDLL
        {
            get
            {
                return m_HotFixDLL;
            }
        }

        public string HotFixPDB
        {
            get
            {
                return m_HotFixPDB;
            }
        }

        public string HotFixABName
        {
            get
            {
                return m_HotFixABName;
            }
        }
    }
}