using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.UI
{
    /// <summary>
    /// 
    /// UI根节点组件
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class UIRoot : MonoBehaviour, IUIRoot
    {

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("UI相机")]
#endif
        private Camera m_UICamera;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("主画布")]
#endif
        private Canvas m_MainCanvas;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("画布缩放器")]
#endif
        private CanvasScaler m_CanvasScaler;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("窗口层"), Indent(1)]
#endif
        private RectTransform m_Windows;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("挂件层"), Indent(1)]
#endif
        private RectTransform m_Widgets;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("弹窗层"), Indent(1)]
#endif
        private RectTransform m_Popups;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("组件唤醒事件")]
#endif
        private OnUIRootAwaked m_OnAwaked = new OnUIRootAwaked();

        public Canvas MainCanvas { get; private set; }
        public RectTransform Widgets { get; private set; }
        public RectTransform Popups { get; private set; }
        public RectTransform Windows { get; private set; }
        public Camera UICamera { get; private set; }
        public float ScaleRatio { get; private set; }

        private void Awake()
        {
            UICamera = m_UICamera;
            MainCanvas = m_MainCanvas;
            Widgets = m_Widgets;
            Windows = m_Windows;
            Popups = m_Popups;
        }

        private void Start()
        {
            UpdateScaleRatio();
            m_OnAwaked?.Invoke(this);
        }

        private void OnDestroy()
        {
            m_OnAwaked?.RemoveAllListeners();
            m_OnAwaked = default;
        }

        public void UpdateScaleRatio()
        {
            Vector2 resolution = m_CanvasScaler.referenceResolution;
            float resol = Mathf.Max(resolution.x, resolution.y);
            float maxW = Mathf.Max(resolution.x, resolution.y);

            int screenW = Screen.width;
            int screenH = Screen.height;

            bool isMatchWidth = screenW > screenH;
            m_CanvasScaler.matchWidthOrHeight = isMatchWidth ? 0f : 1f;
            ScaleRatio = isMatchWidth ? resolution.x / screenW : resolution.y / screenH;
        }

        public void AddAwakedHandler(UnityAction<IUIRoot> handler)
        {
            m_OnAwaked.AddListener(handler);
        }
    }
}

