using System;
using System.Collections.Generic;

namespace ShipDock
{
    /// <summary>
    /// 
    /// 消息泵
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class MessageLooper : IReclaim
    {
        public static void AddMessage(int message, INoticeBase<int> paramNotice)
        {
            MessageLooper messageLooper = Framework.UNIT_MSG_LOOPER.Unit<MessageLooper>();
            MessageNotice msgNotice = MessageNotice.Create(message, paramNotice);
            messageLooper.CommitMessagesAdded(msgNotice);
        }

        public static void AddSettleMessageHandler(Action<IMessageNotice> handler, bool isRemove = false)
        {
            MessageLooper messageLooper = Framework.UNIT_MSG_LOOPER.Unit<MessageLooper>();
            messageLooper.ChangeSettleMessageEvent(handler, isRemove);
        }

        private IPoolable mReclaimParam;
        private IPoolable mMessageParam;
        private MethodUpdater mMessageUpdater;
        private Queue<IPoolable> mMessageParamReclaims;
        private DoubleBuffers<IMessageNotice> mDoubleBuffers;
        /// <summary>添加待处理消息事件</summary>
        private event Action<IMessageNotice> mAddMessageEvent;
        /// <summary>处理消息事件</summary>
        private event Action<IMessageNotice> mSettleMessageEvent;

        public void Reclaim()
        {
            Purge();
        }

        private void Purge()
        {
            UpdaterNotice.RemoveSceneUpdate(mMessageUpdater);
            mMessageUpdater?.Reclaim();

            mAddMessageEvent = default;
            mSettleMessageEvent = default;

            mDoubleBuffers?.Reclaim();
            mMessageParamReclaims?.Clear();
        }

        public void Init()
        {
            mMessageParamReclaims = new Queue<IPoolable>();
            mDoubleBuffers = new DoubleBuffers<IMessageNotice>()
            {
                OnDequeue = OnMessageDequeue,
            };

            mMessageUpdater = new MethodUpdater()
            {
                Update = OnModularUpdate,
            };

            //ShipDockConsts.NOTICE_MSG_ADD.Add(OnMessageAdd);
            mAddMessageEvent += OnMessageAdd;
            UpdaterNotice.AddSceneUpdate(mMessageUpdater);
        }

        public void CommitMessagesAdded(IMessageNotice notice)
        {
            mAddMessageEvent?.Invoke(notice);
        }

        public void ChangeSettleMessageEvent(Action<IMessageNotice> handler, bool isRemove)
        {
            if (isRemove)
            {
                mSettleMessageEvent += handler;
            }
            else
            {
                mSettleMessageEvent -= handler;
            }
        }

        private void OnMessageAdd(IMessageNotice param)
        {
            if (param != default)
            {
                mDoubleBuffers.Enqueue(param);
            }
            else { }
        }

        private void OnModularUpdate(float deltaTime)
        {
            int count = mMessageParamReclaims.Count;
            if (count > 0)
            {
                while (mMessageParamReclaims.Count > 0)
                {
                    mReclaimParam = mMessageParamReclaims.Dequeue();
                    mReclaimParam.ToPool();
                }
                mReclaimParam = default;
            }
            else { }

            mDoubleBuffers.UpdateBuffer(deltaTime);
        }

        private void OnMessageDequeue(float dTime, IMessageNotice param)
        {
            if (param != default)
            {
                //通知其他功能处理消息
                mSettleMessageEvent?.Invoke(param);

                mMessageParam = param.MsgNotice as IPoolable;
                if ((mMessageParam != default) && !mMessageParamReclaims.Contains(mMessageParam))
                {
                    mMessageParamReclaims.Enqueue(mMessageParam);
                }
                else { }

                param.ToPool();

                mMessageParam = default;
            }
            else { }
        }
    }
}