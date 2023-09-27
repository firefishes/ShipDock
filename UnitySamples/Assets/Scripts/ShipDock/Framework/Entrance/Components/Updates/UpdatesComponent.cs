using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock
{
    /// <summary>
    /// 
    /// ����֡�������
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    [DisallowMultipleComponent]
    public class UpdatesComponent : MonoBehaviour, IUpdatesComponent
    {
#if ODIN_INSPECTOR
        [TitleGroup("����֡�������")]
#endif
        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("��Ϣ�� - �������"), EnableIf("@false"), Indent(1)]
#endif 
        private int m_ReadyNoticeName = ShipDockConsts.NOTICE_SCENE_UPDATE_READY;

        [SerializeField, Tooltip("����Unity���߳�����ģʽ��֡������Ϣ")]
#if ODIN_INSPECTOR
        [LabelText("��Ϣ�� - ����֡������"), EnableIf("@false"), Indent(1)]
#endif
        private int m_AddItemNoticeName = ShipDockConsts.NOTICE_ADD_SCENE_UPDATE;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("��Ϣ�� - �Ƴ�֡������"), EnableIf("@false"), Indent(1)]
#endif
        private int m_RemoveItemNoticeName = ShipDockConsts.NOTICE_REMOVE_SCENE_UPDATE;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("��Ϣ�� - �Ƴ�����һ֡����"), EnableIf("@false"), Indent(1)]
#endif
        private int m_CallLateItemNoticeName = ShipDockConsts.NOTICE_SCENE_CALL_LATE;

        private Action mOnSyncToFrame;
        private UpdatesCacher mUpdatesCacher;

        private void Awake()
        {
            Framework.Instance.Updates = this;
        }

        private void OnDestroy()
        {
            mOnSyncToFrame = default;

            mUpdatesCacher?.Reclaim();
            mUpdatesCacher = default;
        }

        public void Init()
        {
            if ((int.MaxValue != m_AddItemNoticeName) && (int.MinValue != m_RemoveItemNoticeName))
            {
                mUpdatesCacher = new UpdatesCacher(m_AddItemNoticeName, m_RemoveItemNoticeName, m_CallLateItemNoticeName);
            }
            else { }

            if(m_ReadyNoticeName != int.MaxValue)
            {
                m_ReadyNoticeName.Broadcast();
            }
            else { }
        }

        private void Update()
        {
            mOnSyncToFrame?.Invoke();
            mOnSyncToFrame = default;

            mUpdatesCacher?.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            float dTime = (Time.timeScale != 0f) ? (Time.fixedDeltaTime * Time.timeScale) : Time.fixedDeltaTime;
            mUpdatesCacher?.FixedUpdate(dTime);
        }

        private void LateUpdate()
        {
            mUpdatesCacher?.LateUpdate();
            mUpdatesCacher?.CheckDeleted();
        }

        public void SyncToFrame(Action method)
        {
            if (method != default)
            {
                mOnSyncToFrame += method;
            }
            else { }
        }
    }
}
