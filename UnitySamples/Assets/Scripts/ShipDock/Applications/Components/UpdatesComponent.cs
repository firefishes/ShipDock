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
    /// 总线帧更新组件
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    [DisallowMultipleComponent]
    public class UpdatesComponent : MonoBehaviour, IUpdatesComponent
    {
#if ODIN_INSPECTOR
        [TitleGroup("组件相关消息")]
#endif
        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("消息名 - 组件就绪"), Indent(1)]
#endif 
        private int m_ReadyNoticeName = ShipDockConsts.NOTICE_SCENE_UPDATE_READY;

        [SerializeField, Tooltip("触发Unity主线程驱动模式的帧更新消息")]
#if ODIN_INSPECTOR
        [LabelText("消息名 - 新增帧更新项"), Indent(1)]
#endif
        private int m_AddItemNoticeName = ShipDockConsts.NOTICE_ADD_SCENE_UPDATE;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("消息名 - 移除帧更新项"), Indent(1)]
#endif
        private int m_RemoveItemNoticeName = ShipDockConsts.NOTICE_REMOVE_SCENE_UPDATE;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("消息名 - 推迟至下一帧更新"), Indent(1)]
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
