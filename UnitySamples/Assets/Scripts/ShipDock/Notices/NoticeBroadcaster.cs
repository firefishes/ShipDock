using System;
using UnityEngine;

namespace ShipDock
{
    [Serializable]
    public class NoticeBroadcaster
    {
        [SerializeField]
        private int m_Notice = int.MaxValue;
        [SerializeField]
        private bool m_BroadcastAwake;
        [SerializeField]
        private bool m_BroadcastStart;
        [SerializeField]
        private bool m_BroadcastUpdate;
        [SerializeField]
        private bool m_BroadcastLateUpdate;
        [SerializeField]
        private bool m_BroadcastOnDestroy;
        [SerializeField]
        private NoticeBroadcastHandlerEvent m_NoticeBroadcastEvent = new NoticeBroadcastHandlerEvent();

        private INoticeBase<int> mNotice;

        public int NoticeName
        {
            get
            {
                return m_Notice;
            }
        }

        public bool BroadcastAwake
        {
            get
            {
                return m_BroadcastAwake;
            }
        }

        public bool BroadcastStart
        {
            get
            {
                return m_BroadcastStart;
            }
        }

        public bool BroadcastUpdate
        {
            get
            {
                return m_BroadcastUpdate;
            }
        }

        public bool BroadcastLateUpdate
        {
            get
            {
                return m_BroadcastLateUpdate;
            }
        }

        public bool BroadcastOnDestroy
        {
            get
            {
                return m_BroadcastOnDestroy;
            }
        }

        public void Broadcast()
        {
            mNotice = default;
            m_NoticeBroadcastEvent?.Invoke(mNotice);
            m_Notice.Broadcast(mNotice);
        }
    }
}