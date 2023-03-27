using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock
{
    public class NoticeComponent : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [Title("消息组件")]
        [LabelText("广播器")]
#endif
        [SerializeField]
        private NoticeBroadcaster m_Broadcaster = new NoticeBroadcaster();

#if ODIN_INSPECTOR
        [LabelText("侦听器")]
#endif
        [SerializeField]
        private List<NotificationInfo> m_Notices;

        private bool mIsApplicationExited;

        private void Awake()
        {
            int max = m_Notices == default ? 0 : m_Notices.Count;
            for (int i = 0; i < max; i++)
            {
                m_Notices[i].Init();
            }

            if (m_Broadcaster.BroadcastAwake)
            {
                m_Broadcaster.Broadcast();
            }
            else { }
        }

        private void OnDestroy()
        {
            int max = m_Notices == default ? 0 : m_Notices.Count;
            for (int i = 0; i < max; i++)
            {
                m_Notices[i].Deinit();
            }

            if (m_Broadcaster.BroadcastOnDestroy)
            {
                m_Broadcaster.Broadcast();
            }
            else { }
        }

        private void Start()
        {
            if (m_Broadcaster.BroadcastStart)
            {
                m_Broadcaster.Broadcast();
            }
            else { }
        }

        private void Update()
        {
            if (m_Broadcaster.BroadcastUpdate)
            {
                m_Broadcaster.Broadcast();
            }
            else { }
        }

        private void LateUpdate()
        {
            if (m_Broadcaster.BroadcastLateUpdate)
            {
                m_Broadcaster.Broadcast();
            }
            else { }
        }
    }
}
