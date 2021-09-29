using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ShipDock.UI
{
    public interface IUIPointerArea : IPointerClickHandler
    {
        UIPointerAreaEvent PointerAreaEvent { get; }
        UIPointerAreaInvkedEvent PointerAreaInvokedEvent { get; }
    }

    [Serializable]
    public class UIPointerAreaInvkedEvent : UnityEvent { }
    [Serializable]
    public class UIPointerAreaEvent : UnityEvent<PointerEventData> { }

    public class UIPointerArea : MonoBehaviour, IUIPointerArea
    {
        [SerializeField]
        private bool m_ApplyInverseSet = true;
        [SerializeField]
        private UIPointerAreaEvent m_PointerAreaEvent = new UIPointerAreaEvent();
        [SerializeField]
        private UIPointerAreaInvkedEvent m_PointerAreaInvokedEvent = new UIPointerAreaInvkedEvent();

        private PointerEventData mPointerEventData;
        private Vector2 mPointerPosition;
        private Camera mEventCamera;
        private RectTransform mRectTransform;

        public UIPointerAreaEvent PointerAreaEvent
        {
            get
            {
                return m_PointerAreaEvent;
            }
        }

        public UIPointerAreaInvkedEvent PointerAreaInvokedEvent
        {
            get
            {
                return m_PointerAreaInvokedEvent;
            }
        }

        private void Awake()
        {
            mRectTransform = transform as RectTransform;
            UIManager manager = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
            mEventCamera = manager.UIRoot.UICamera;
        }

        private void OnDestroy()
        {
            m_PointerAreaEvent.RemoveAllListeners();
            m_PointerAreaInvokedEvent.RemoveAllListeners();

            mEventCamera = default;
            m_PointerAreaEvent = default;
            m_PointerAreaInvokedEvent = default;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            mPointerEventData = eventData;
#if UNITY_EDITOR
            mPointerPosition = eventData.position;
            CheckInvertsAreaValidable();
#endif
        }

#if !UNITY_EDITOR
        private void Update()
        {
            if ((mPointerEventData != default) && (mRectTransform != default) && (mEventCamera != default))
            {
                if (Input.touchCount > 0)
                {
                    mPointerPosition = Input.GetTouch(0).position;
                    CheckInvertsAreaValidable();
                }
                else { }

            }
            else { }
        }
#endif

        private void CheckInvertsAreaValidable()
        {
            bool flag = IsPositionSelf(mPointerPosition);
            Debug.Log("CheckInvertsAreaValidable " + flag);

#if UNITY_EDITOR
            flag = m_ApplyInverseSet ? flag : !flag;
#else
            flag = m_ApplyInverseSet ? !flag : flag;
#endif
            if (flag)
            {
                m_PointerAreaEvent.Invoke(mPointerEventData);
                m_PointerAreaInvokedEvent.Invoke();
            }
            else { }
        }

        private bool IsPositionSelf(Vector2 pos)
        {
            bool isContaisScreenPoint = RectTransformUtility.RectangleContainsScreenPoint(mRectTransform, pos, mEventCamera);
            return isContaisScreenPoint;
        }
    }
}
