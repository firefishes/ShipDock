using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
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

        public float MatchWidthOrHeight
        {
            get
            {
                return m_CanvasScaler.matchWidthOrHeight;
            }
        }

        public Canvas MainCanvas { get; private set; }
        public RectTransform Widgets { get; private set; }
        public RectTransform Popups { get; private set; }
        public RectTransform Windows { get; private set; }
        public Camera UICamera { get; private set; }
        public float ScaleRatio { get; private set; }
        public float FOVRatio { get; private set; }
        public int ScreenW { get; private set; }
        public int ScreenH { get; private set; }

        private void Awake()
        {
            UICamera = m_UICamera;
            MainCanvas = m_MainCanvas;
            MainCanvas.worldCamera = UICamera;

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

            ScreenW = Screen.width;
            ScreenH = Screen.height;

            bool isMatchWidth = ScreenW > ScreenH;
            m_CanvasScaler.matchWidthOrHeight = isMatchWidth ? 0f : 1f;
            ScaleRatio = isMatchWidth ? resolution.x / ScreenW : resolution.y / ScreenH;

            UpdateFOVRatio(resolution);
        }

        private void UpdateFOVRatio(Vector2 resolution)
        {
            int manualHeight;
            float ratioScreen = Convert.ToSingle(ScreenH) / ScreenW;
            float ratioResolution = Convert.ToSingle(resolution.y) / resolution.x;
            bool willChangeMatch = ratioScreen != ratioResolution;//高宽比不同时重设匹配方式
            //当前屏幕的高宽比 和 自定义高宽比，通过判断大小选用不高度值
            if (ratioScreen > ratioResolution)
            {
                //如果屏幕的高宽比大于自定义的高宽比 。则通过公式  ManualWidth * manualHeight = Screen.width * Screen.height
                //来求得适应的 manualHeight ，用它待求出 实际高度与理想高度的比率 scale
                manualHeight = Mathf.RoundToInt(Convert.ToSingle(resolution.x) / ScreenW * ScreenH);
                if (willChangeMatch)
                {
                    m_CanvasScaler.matchWidthOrHeight = 0f;
                }
                else { }
            }
            else
            {   //否则 直接给manualHeight 自定义的 ManualHeight的值，那么相机的fieldOfView就会原封不动
                manualHeight = Mathf.RoundToInt(Convert.ToSingle(resolution.y));
                if (willChangeMatch)
                {
                    m_CanvasScaler.matchWidthOrHeight = 1f;
                }
                else { }
            }

            FOVRatio = Convert.ToSingle(manualHeight * 1.0f / resolution.y);
        }

        public void AddAwakedHandler(UnityAction<IUIRoot> handler)
        {
            m_OnAwaked.AddListener(handler);
        }
    }
}

