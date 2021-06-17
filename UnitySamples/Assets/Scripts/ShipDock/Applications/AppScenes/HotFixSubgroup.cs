using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.Applications
{
    [Serializable]
    public class HotFixSubgroup
    {
        [Tooltip("是否应用ILRuntime热更")]
#if ODIN_INSPECTOR
        [LabelText("启用ILRuntime")]
#endif 
        public bool applyILRuntime;
        [SerializeField, Tooltip("位于 Assets/Resouce/ 目录下的预制体名")]
#if ODIN_INSPECTOR
        [LabelText("热更启动器名称"), ShowIf("@this.applyILRuntime == true")]
#endif 
        public string initerNameInResource = "Initer";

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("热更启动器类名"), SuffixLabel(".cs"), ShowIf("@this.applyILRuntime == true")]
#endif 
        public string initerClassName = "ShipDock.GameRunner";

        [SerializeField, Tooltip("向热更启动器传递模板组件的方法名")]
#if ODIN_INSPECTOR
        [LabelText("模板组件传导方法"), ShowIf("@this.applyILRuntime == true")]
#endif 
        public string initerGameCompSetter = "SetGameComponent";
    }
}