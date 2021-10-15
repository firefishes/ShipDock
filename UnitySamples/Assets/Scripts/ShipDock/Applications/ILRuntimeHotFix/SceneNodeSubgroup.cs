#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShipDock.Applications
{
    /// <summary>
    /// 场景节点类型枚举
    /// </summary>
    public enum SceneNodeType
    {
        GAME_OBJECT = 1,
        ANI_CURVE,
        SPRITE,
        TEXTURE,
        CAMERA,
        ANIMATOR,
        UI_TEXT,
        UI_BUTTON,
        UI_IMAGE,
        UI_LAYOUT_GROUP,
        UI_TOGGLE,
        UI_TOGGLE_GROUP,
        UI_SLIDER,
        UI_SCROLL_BAR,
        UI_DROP_DOWN,
        UI_INPUT_FIELD,
        UI_CANVAS,
        UI_EVENT_SYSTEM,
        UI_EVENT_TRIGGER,
        TRANSFORM,
        MATERIAL,
        SPRITE_RENDERER,
        MESH_FILTER,
        AUDIO_SOURCE,
        ILRUNTIME_HOTFIX,
        ILRUNTIME_HOTFIX_UI,
        BYTES,
    }

    /// <summary>
    /// 
    /// 场景节点引用子组
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    [Serializable]
    public class SceneNodeSubgroup
    {
        /// <summary>节点字段名</summary>
#if ODIN_INSPECTOR
        [EnableIf("@this.editable")]
#endif
        public string keyField;

#if ODIN_INSPECTOR
        [EnumPaging, Indent(1), ShowIf("@this.editable")]
#endif
        public SceneNodeType valueType;

#if ODIN_INSPECTOR
        /// <summary>是否启用修改</summary>
        [LabelText("修改"), ToggleLeft(), Indent(1)]
        public bool editable;
#endif

        /// <summary>场景物体节点（包括3D、2D物体）</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.GAME_OBJECT), ShowIf("@this.editable"), Indent(1)]
#endif
        public GameObject value;

        /// <summary>动画曲线节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ANI_CURVE), ShowIf("@this.editable"), Indent(1)]
#endif
        public AnimationCurve animationCurve;

        /// <summary>动画节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ANIMATOR), ShowIf("@this.editable"), Indent(1)]
#endif
        public Animator animator;

        /// <summary>相机节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.CAMERA), ShowIf("@this.editable"), Indent(1)]
#endif
        public Camera lens;

        /// <summary>2D精灵节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.SPRITE), ShowIf("@this.editable"), Indent(1)]
#endif
        public Sprite sprite;

        /// <summary>贴图节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.TEXTURE), ShowIf("@this.editable"), Indent(1)]
#endif
        public Texture texture;

        /// <summary>按钮节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_BUTTON), ShowIf("@this.editable"), Indent(1)]
#endif
        public Button button;

        /// <summary>图片节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_IMAGE), ShowIf("@this.editable"), Indent(1)]
#endif
        public Image image;

        /// <summary>文本节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_TEXT), ShowIf("@this.editable"), Indent(1)]
#endif
        public Text Label;

        /// <summary>布局节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_LAYOUT_GROUP), ShowIf("@this.editable"), Indent(1)]
#endif
        public LayoutGroup layoutGroup;

        /// <summary>单选按钮节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_TOGGLE), ShowIf("@this.editable"), Indent(1)]
#endif
        public UnityEngine.UI.Toggle toggle;

        /// <summary>单选按钮组节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_TOGGLE_GROUP), ShowIf("@this.editable"), Indent(1)]
#endif
        public ToggleGroup toggleGroup;

        /// <summary>滑动组件节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_SLIDER), ShowIf("@this.editable"), Indent(1)]
#endif
        public Slider slider;

        /// <summary>滚动栏节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_SCROLL_BAR), ShowIf("@this.editable"), Indent(1)]
#endif
        public Scrollbar scrollBar;

        /// <summary>下拉选单节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_DROP_DOWN), ShowIf("@this.editable"), Indent(1)]
#endif
        public Dropdown dropDown;

        /// <summary>输入文本节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_INPUT_FIELD), ShowIf("@this.editable"), Indent(1)]
#endif
        public InputField inputField;

        /// <summary>UI 画布节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_CANVAS), ShowIf("@this.editable"), Indent(1)]
#endif
        public Canvas canvas;

        /// <summary>事件系统节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_EVENT_SYSTEM), ShowIf("@this.editable"), Indent(1)]
#endif
        public EventSystem eventSystem;

        /// <summary>事件触发器节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.UI_EVENT_TRIGGER), ShowIf("@this.editable"), Indent(1)]
#endif
        public EventTrigger eventTrigger;

        /// <summary>变换组件节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.TRANSFORM), ShowIf("@this.editable"), Indent(1)]
#endif
        public Transform trans;

        /// <summary>材质节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.MATERIAL), ShowIf("@this.editable"), Indent(1)]
#endif
        public Material materialNode;

        /// <summary>精灵渲染器节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.SPRITE_RENDERER), ShowIf("@this.editable"), Indent(1)]
#endif
        public SpriteRenderer spriteRendererNode;

        /// <summary>网格节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.MESH_FILTER), ShowIf("@this.editable"), Indent(1)]
#endif
        public MeshFilter meshFilterNode;

        /// <summary>声源节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.AUDIO_SOURCE), ShowIf("@this.editable"), Indent(1)]
#endif
        public AudioSource audioSource;

        /// <summary>热更组件节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ILRUNTIME_HOTFIX), ShowIf("@this.editable"), Indent(1)]
#endif
        public HotFixerComponent hotFixer;

        /// <summary>热更 UI 节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.ILRUNTIME_HOTFIX_UI), ShowIf("@this.editable"), Indent(1)]
#endif
        public HotFixerUIAgent hotFixerUI;

        /// <summary>二进制对象节点</summary>
#if ODIN_INSPECTOR
        [ShowIf("valueType", SceneNodeType.BYTES), ShowIf("@this.editable"), Indent(1)]
#endif
        public TextAsset bytes;

    }

}