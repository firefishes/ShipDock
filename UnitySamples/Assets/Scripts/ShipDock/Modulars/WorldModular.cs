using System.Collections.Generic;

namespace ShipDock
{
    /// <summary>
    /// ����ģ��
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class WorldModular : QueueableNoticesModular
    {
        /// <summary>�������������Ϣ��</summary>
        private int mAddToWorldMessage;
        /// <summary>�������Ƴ������Ϣ��</summary>
        private int mRemoveFromWorldMessage;
        /// <summary>֡������</summary>
        private MethodUpdater mWorldUpdater;
        /// <summary>֡������Ϣ</summary>
        private IParamNotice<IUpdate> mUpdaterParamNotice;
        /// <summary>��ʱ�洢����Ԫ��</summary>
        private IUpdate mElement;
        /// <summary>���Ƴ�����Ԫ��</summary>
        private List<IUpdate> mElementDeleteds;
        /// <summary>���ڸ����е���Ԫ��</summary>
        private List<IUpdate> mElementUpdaters;

        public WorldModular(int modularName, int addToMessage, int removeFromMessage) : base(modularName)
        {
            mAddToWorldMessage = addToMessage;
            mRemoveFromWorldMessage = removeFromMessage;

            mElementDeleteds = new List<IUpdate>();
            mElementUpdaters = new List<IUpdate>();
            mWorldUpdater = new MethodUpdater();
        }

        public override void InitModular()
        {
            base.InitModular();

            InitMessageSettles();

            mWorldUpdater = new MethodUpdater()
            {
                Update = OnWorldUpdate,
                LateUpdate = OnWorldLateUpdate,
                FixedUpdate = OnWorldFixedUpdate,
            };

            UpdaterNotice.AddSceneUpdater(mWorldUpdater);
        }

        /// <summary>
        /// ��ʼ����Ϣ��������
        /// </summary>
        protected virtual void InitMessageSettles()
        {
            SetMessageSettle(mAddToWorldMessage, OnAddUpdater);
            SetMessageSettle(mRemoveFromWorldMessage, OnRemoveUpdater);
        }

        /// <summary>
        /// ÿ֡���µĻص�����
        /// </summary>
        /// <param name="dTime"></param>
        private void OnWorldUpdate(int dTime)
        {
            int max = mElementDeleteds.Count;
            if (max > 0)
            {
                while (mElementDeleteds.Count > 0)
                {
                    int i = mElementDeleteds.Count - 1;
                    mElement = mElementDeleteds[i];
                    mElementDeleteds.RemoveAt(i);

                    mElementUpdaters.Remove(mElement);
                }
                mElementDeleteds.Clear();
            }
            else { }

            max = mElementUpdaters.Count;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mElement = mElementUpdaters[i];
                    if (mElement != default && mElement.IsUpdate)
                    {
                        mElement.OnUpdate(dTime);
                    }
                    else { }
                }
            }
            else { }

            mElement = default;
        }

        /// <summary>
        /// ÿ֡����֮��Ļص�����
        /// </summary>
        private void OnWorldLateUpdate()
        {
            int max = mElementUpdaters.Count;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mElement = mElementUpdaters[i];
                    if (mElement != default && mElement.IsUpdate)
                    {
                        mElement.OnLateUpdate();
                    }
                    else { }
                }
            }
            else { }

            mElement = default;
        }

        /// <summary>
        /// �̶����µĻص�����
        /// </summary>
        /// <param name="dTime"></param>
        private void OnWorldFixedUpdate(int dTime)
        {
            int max = mElementUpdaters.Count;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mElement = mElementUpdaters[i];
                    if (mElement != default && mElement.IsUpdate)
                    {
                        mElement.OnFixedUpdate(dTime);
                    }
                    else { }
                }
            }
            else { }

            mElement = default;
        }

        /// <summary>
        /// ��Ϣ���д���������
        /// </summary>
        /// <param name="message"></param>
        /// <param name="notice"></param>
        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
            ExecuteMessageSettle(message, ref notice);
        }

        /// <summary>
        /// �Ƴ���������Ϣ����������
        /// </summary>
        /// <param name="param"></param>
        private void OnRemoveUpdater(INoticeBase<int> param)
        {
            mUpdaterParamNotice = param as IParamNotice<IUpdate>;
            IUpdate willDelete = mUpdaterParamNotice.ParamValue;
            int index = mElementDeleteds.IndexOf(willDelete);
            if (index < 0)
            {
                mElementDeleteds.Add(willDelete);
            }
            else { }

            mUpdaterParamNotice.ToPool();
            mUpdaterParamNotice = default;
        }

        /// <summary>
        /// �����������Ϣ����������
        /// </summary>
        /// <param name="param"></param>
        private void OnAddUpdater(INoticeBase<int> param)
        {
            mUpdaterParamNotice = param as IParamNotice<IUpdate>;
            IUpdate updater = mUpdaterParamNotice.ParamValue;
            int index = mElementUpdaters.IndexOf(updater);
            if (index <= 0)
            {
                mElementUpdaters.Add(updater);

                index = mElementDeleteds.IndexOf(updater);
                if (index >= 0)
                {
                    mElementDeleteds.Remove(updater);
                }
                else { }
            }
            else { }

            mUpdaterParamNotice.ToPool();
            mUpdaterParamNotice = default;
        }
    }

}