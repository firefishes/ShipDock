using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ShipDock.UI
{
    /// <summary>
    /// 
    /// UI 界面容器类，用于各UI组件节点的集中化管理及变化事务的结构化
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class UIContainer : UI
    {
        [SerializeField]
#if ODIN_INSPECTOR
        [Title("UI")]
        [LabelText("广播就绪消息"), Indent(1)]
#endif
        private bool m_ApplyInitSelf;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("物体失活方式控制隐藏"), Indent(1)]
#endif
        private bool m_ActiveSelfControlHide = true;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("界面组件节点"), Indent(1)]
#endif
        private UINodes m_UINodes;

        /// <summary>变化事务管理器</summary>
        protected UIChangingTasker mUIChangerTasker;

        /// <summary>变化事务集合</summary>
        private KeyValueList<string, IUISubgroup> mUISubgroup;

        /// <summary>是否用物体失活方式控制隐藏</summary>
        public bool ActiveSelfControlHide { get; protected set; }
        /// <summary>界面节点控制器</summary>
        public UINodeControl NodesControl { get; private set; }

        public UINodes UINodes
        {
            get
            {
                return m_UINodes;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            ActiveSelfControlHide = m_ActiveSelfControlHide;

            m_UINodes?.Init();
            NodesControl = new UINodeControl(m_UINodes);

            mUIChangerTasker = new UIChangingTasker(this);
            mUISubgroup = new KeyValueList<string, IUISubgroup>();

            int initNotice = GetInstanceID();
            initNotice.Add(OnInitUISubgroup);
        }

        protected override void Start()
        {
            base.Start();

            if (m_ApplyInitSelf)
            {
                IUIStack stack = CreateModular();
                if (stack != default)
                {
                    UIs.Open<UIPuzzlesModular>(stack.Name, () =>
                    {
                        UIs.BindResourcesUIToStack(stack, gameObject, ActiveSelfControlHide);
                        return stack;
                    });
                }
                else { }

                ParamNotice<MonoBehaviour> notice = Pooling<ParamNotice<MonoBehaviour>>.From();
                int noticeName = GetInstanceID();
                noticeName.Broadcast(notice);
                notice.ToPool();
            }
            else { }
        }

        protected override void Purge()
        {
            int initNotice = GetInstanceID();
            initNotice.Remove(OnInitUISubgroup);

            mUISubgroup?.Dispose();

            mUIChangerTasker?.Clean();
            m_UINodes?.Clear();
            NodesControl?.Clear();
        }

        protected virtual IUIStack CreateModular()
        {
            return default;
        }

        private void OnInitUISubgroup(INoticeBase<int> param)
        {
            if(param is IParamNotice<IUISubgroup> notice)
            {
                IUISubgroup element = notice.ParamValue;
                mUISubgroup[element.ChangerTaskName] = element;
            }
            else { }
        }

        public override void UpdateUI()
        {
            mUIChangerTasker?.UpdateUITasks();
        }

        public void UpdatSubgroup(string taskName)
        {
            IUISubgroup element = mUISubgroup[taskName];
            if (element != default)
            {
                mUIChangerTasker.AddChangeTask(element.ChangerTaskName, element.ChangerTaskerDuring, element.ChangerTaskerHandler);
            }
            else { }
        }
    }
}