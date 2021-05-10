using ShipDock.Notices;
using ShipDock.Ticks;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.Applications
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
        [TitleGroup("��������Ϣ")]
#endif
        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("��Ϣ�� - �������"), Indent(1)]
#endif 
        private int m_ReadyNoticeName = ShipDockConsts.NOTICE_SCENE_UPDATE_READY;

        [SerializeField, Tooltip("����Unity���߳�����ģʽ��֡������Ϣ")]
#if ODIN_INSPECTOR
        [LabelText("��Ϣ�� - ����֡������"), Indent(1)]
#endif
        private int m_AddItemNoticeName = ShipDockConsts.NOTICE_ADD_SCENE_UPDATE;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("��Ϣ�� - �Ƴ�֡������"), Indent(1)]
#endif
        private int m_RemoveItemNoticeName = ShipDockConsts.NOTICE_REMOVE_SCENE_UPDATE;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("��Ϣ�� - �Ƴ�����һ֡����"), Indent(1)]
#endif
        private int m_CallLateItemNoticeName = ShipDockConsts.NOTICE_SCENE_CALL_LATE;

        private UpdatesCacher mUpdatesCacher;

        private void Awake()
        {
            Framework.Instance.Updates = this;
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
            int time = (int)(Time.deltaTime * UpdatesCacher.UPDATE_CACHER_TIME_SCALE);
            mUpdatesCacher?.Update(time);
        }

        private void FixedUpdate()
        {
            int time = (int)(Time.fixedDeltaTime * UpdatesCacher.UPDATE_CACHER_TIME_SCALE);
            mUpdatesCacher?.FixedUpdate(time);
        }

        private void LateUpdate()
        {
            mUpdatesCacher?.LateUpdate();
            mUpdatesCacher?.CheckDeleted();
        }
    }
}
