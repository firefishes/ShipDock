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
        #region �༭����չ���
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
        #endregion

        #region ˽���ֶ�
        /// <summary>�޲����ص���ί��</summary>
        private Action mOnSyncToFrame;
        /// <summary>���¹���</summary>
        private UpdatePipeline mUpdatePipline;
        /// <summary>�̶�֡Ƶ���¹���</summary>
        private UpdatePipeline mFixedUpdatePipline;
        /// <summary>֡ĩ���¹���</summary>
        private UpdatePipeline mLateUpdatePipline;
        /// <summary>��һ֡������</summary>
        private SyncUpdater mSyncUpdater;
        /// <summary>��һ֡����������Ĵ����¼�</summary>
        private event Action<int, Action<float>> mSyncUpdateEvent;
        #endregion

        private void Awake()
        {
            //��ʼ����һ֡������
            mSyncUpdater = new SyncUpdater();
            //�������߸���������õ���ܵ����ã�֮������ø�ָ�����ں�
            Framework.Instance.Updates = this;
        }

        #region �������
        private void OnDestroy()
        {
            mUpdatePipline?.Reclaim();
            mFixedUpdatePipline?.Reclaim();
            mLateUpdatePipline?.Reclaim();
            mSyncUpdater?.Reclaim();

            mUpdatePipline = default;
            mFixedUpdatePipline = default;
            mLateUpdatePipline = default;
            mOnSyncToFrame = default;
            mSyncUpdateEvent = default;
            mSyncUpdater = default;
        }
        #endregion

        #region ��ʼ�����, �ɿ���ں˵���
        public void Init()
        {
            if ((int.MinValue != m_AddItemNoticeName) && (int.MinValue != m_RemoveItemNoticeName) && (int.MinValue != m_CallLateItemNoticeName))
            {
                //��ʼ�����ߵĸ��������
                mUpdatePipline = new UpdatePipeline(UpdatePipelineType.UpdatePineline);
                mUpdatePipline.SetAddNoticeName(m_AddItemNoticeName);
                mUpdatePipline.SetRemoveNoticeName(m_RemoveItemNoticeName);

                mFixedUpdatePipline = new UpdatePipeline(UpdatePipelineType.FixedUpdatePineline);
                mFixedUpdatePipline.SetAddNoticeName(m_AddItemNoticeName);
                mFixedUpdatePipline.SetRemoveNoticeName(m_RemoveItemNoticeName);

                mLateUpdatePipline = new UpdatePipeline(UpdatePipelineType.LateUpdatePineline);
                mLateUpdatePipline.SetAddNoticeName(m_AddItemNoticeName);
                mLateUpdatePipline.SetRemoveNoticeName(m_RemoveItemNoticeName);

                //��ʼ����һ֡������������¼�����������
                mSyncUpdateEvent += OnAddCallLate;

                if (m_ReadyNoticeName != int.MaxValue)
                {
                    //�������߸������������Ϣ
                    m_ReadyNoticeName.Broadcast();
                }
                else { }
            }
            else 
            {
                throw new Exception("Please set the update pipeines notice names");
            }
        }
        #endregion

        #region ׷���޲����ķ������ص�ί�з���
        public void SyncToFrame(Action method)
        {
            if (method != default)
            {
                mOnSyncToFrame += method;
            }
            else { }
        }
        #endregion

        #region ������һ֡��Ҫ��ͳһ���õķ������
        public void AddCallLate(Action<float> target)
        {
            mSyncUpdateEvent?.Invoke(m_CallLateItemNoticeName, target);
        }

        private void OnAddCallLate(int noticeName, Action<float> target)
        {
            if (noticeName == m_CallLateItemNoticeName)
            {
                mSyncUpdater.CallLater(target);
            }
            else { }
        }
        #endregion

        #region ��������ͳһ�ĸ��·���
        private void Update()
        {
            mUpdatePipline?.CheckDeleted();
            mFixedUpdatePipline?.CheckDeleted();
            mLateUpdatePipline?.CheckDeleted();

            mOnSyncToFrame?.Invoke();
            mOnSyncToFrame = default;

            float dTime = (Time.timeScale != 0f) ? (Time.deltaTime * Time.timeScale) : Time.deltaTime;
            mSyncUpdater.Update(dTime);
            mUpdatePipline?.Update(dTime);
        }

        private void FixedUpdate()
        {
            float dTime = (Time.timeScale != 0f) ? (Time.fixedDeltaTime * Time.timeScale) : Time.fixedDeltaTime;
            mFixedUpdatePipline?.FixedUpdate(dTime);
        }

        private void LateUpdate()
        {
            mLateUpdatePipline?.LateUpdate();
        }
        #endregion

        #region ��������׷��ͳһ�ĸ��¶���
        public void AddUpdate(IUpdate target)
        {
            mUpdatePipline?.AddUpdate(target);
        }

        public void RemoveUpdate(IUpdate target)
        {
            mUpdatePipline?.RemoveUpdate(target);
        }

        public void AddFixedUpdate(IUpdate target)
        {
            mFixedUpdatePipline?.AddUpdate(target);
        }

        public void RemoveFixedUpdate(IUpdate target)
        {
            mFixedUpdatePipline?.RemoveUpdate(target);
        }

        public void AddLateUpdate(IUpdate target)
        {
            mLateUpdatePipline?.AddUpdate(target);
        }

        public void RemoveLateUpdate(IUpdate target)
        {
            mLateUpdatePipline?.RemoveUpdate(target);
        }
        #endregion
    }
}
