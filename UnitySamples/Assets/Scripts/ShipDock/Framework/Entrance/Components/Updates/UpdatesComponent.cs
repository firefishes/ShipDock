using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock
{
    /// <summary>
    /// 
    /// 总线帧更新组件
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    [DisallowMultipleComponent]
    public class UpdatesComponent : MonoBehaviour, IUpdatesComponent
    {
        #region 编辑器扩展相关
#if ODIN_INSPECTOR
        [TitleGroup("主线帧更新组件")]
#endif
        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("消息名 - 组件就绪"), EnableIf("@false"), Indent(1)]
#endif 
        private int m_ReadyNoticeName = ShipDockConsts.NOTICE_SCENE_UPDATE_READY;

        [SerializeField, Tooltip("触发Unity主线程驱动模式的帧更新消息")]
#if ODIN_INSPECTOR
        [LabelText("消息名 - 新增帧更新项"), EnableIf("@false"), Indent(1)]
#endif
        private int m_AddItemNoticeName = ShipDockConsts.NOTICE_ADD_SCENE_UPDATE;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("消息名 - 移除帧更新项"), EnableIf("@false"), Indent(1)]
#endif
        private int m_RemoveItemNoticeName = ShipDockConsts.NOTICE_REMOVE_SCENE_UPDATE;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("消息名 - 推迟至下一帧更新"), EnableIf("@false"), Indent(1)]
#endif
        private int m_CallLateItemNoticeName = ShipDockConsts.NOTICE_SCENE_CALL_LATE;
        #endregion

        #region 私有字段
        /// <summary>无参数回调的委托</summary>
        private Action mOnSyncToFrame;
        /// <summary>更新管线</summary>
        private UpdatePipeline mUpdatePipline;
        /// <summary>固定帧频更新管线</summary>
        private UpdatePipeline mFixedUpdatePipline;
        /// <summary>帧末更新管线</summary>
        private UpdatePipeline mLateUpdatePipline;
        /// <summary>下一帧更新器</summary>
        private SyncUpdater mSyncUpdater;
        /// <summary>下一帧更新器所需的触发事件</summary>
        private event Action<int, Action<float>> mSyncUpdateEvent;
        #endregion

        private void Awake()
        {
            //初始化下一帧更新器
            mSyncUpdater = new SyncUpdater();
            //将此总线更新组件设置到框架的引用，之后会设置给指定的内核
            Framework.Instance.Updates = this;
        }

        #region 组件销毁
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

        #region 初始化组件, 由框架内核调用
        public void Init()
        {
            if ((int.MinValue != m_AddItemNoticeName) && (int.MinValue != m_RemoveItemNoticeName) && (int.MinValue != m_CallLateItemNoticeName))
            {
                //初始化总线的各类更新器
                mUpdatePipline = new UpdatePipeline(UpdatePipelineType.UpdatePineline);
                mUpdatePipline.SetAddNoticeName(m_AddItemNoticeName);
                mUpdatePipline.SetRemoveNoticeName(m_RemoveItemNoticeName);

                mFixedUpdatePipline = new UpdatePipeline(UpdatePipelineType.FixedUpdatePineline);
                mFixedUpdatePipline.SetAddNoticeName(m_AddItemNoticeName);
                mFixedUpdatePipline.SetRemoveNoticeName(m_RemoveItemNoticeName);

                mLateUpdatePipline = new UpdatePipeline(UpdatePipelineType.LateUpdatePineline);
                mLateUpdatePipline.SetAddNoticeName(m_AddItemNoticeName);
                mLateUpdatePipline.SetRemoveNoticeName(m_RemoveItemNoticeName);

                //初始化下一帧更新器所需的事件处理器函数
                mSyncUpdateEvent += OnAddCallLate;

                if (m_ReadyNoticeName != int.MaxValue)
                {
                    //发送总线更新组件就绪消息
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

        #region 追加无参数的方法至回调委托方法
        public void SyncToFrame(Action method)
        {
            if (method != default)
            {
                mOnSyncToFrame += method;
            }
            else { }
        }
        #endregion

        #region 管理下一帧需要被统一调用的方法相关
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

        #region 在总线中统一的更新方法
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

        #region 向总线中追加统一的更新对象
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
