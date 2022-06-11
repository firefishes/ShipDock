using ShipDock.Notices;
using ShipDock.Tools;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace ShipDock.UI
{
    [Serializable]
    public class TaskerChangeEvent : UnityEvent<UI, TimeGapper> { }

    [Serializable]
    public abstract class UISubgroup : MonoBehaviour, IUISubgroup
    {
        [SerializeField]
        private TaskerChangeEvent m_TaskerChange = new TaskerChangeEvent();
        [SerializeField]
        protected UI m_UIOwner;

        public abstract string ChangerTaskName { get; protected set; }
        public abstract float ChangerTaskerDuring { get; protected set; }
        public Action<TimeGapper> ChangerTaskerHandler { get; private set; }

        protected virtual void Start()
        {
            string changerTaskName = GetChangerTaskName();

            if (string.IsNullOrEmpty(changerTaskName)) { }
            else
            {
                ChangerTaskName = changerTaskName;
            }

            ChangerTaskerHandler = OnTaskerChange;
            IUISubgroup subgroup = this;
            m_UIOwner?.GetInstanceID().BroadcastWithParam(subgroup);
            m_UIOwner?.Add(OnUIHandler);
        }

        protected virtual string GetChangerTaskName()
        {
            return string.Empty;
        }

        protected virtual void OnUIHandler(INoticeBase<int> param) { }

        private void OnDestroy()
        {
            m_UIOwner?.Remove(OnUIHandler);
            m_UIOwner = default;
            ChangerTaskerHandler = default;
        }

        private void OnTaskerChange(TimeGapper timeGapper)
        {
            TaskerChange(m_UIOwner, timeGapper);
        }

        protected virtual void TaskerChange(UI ui, TimeGapper timeGapper)
        {
            m_TaskerChange?.Invoke(ui, timeGapper);
        }
    }

}