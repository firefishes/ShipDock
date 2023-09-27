using System;
using System.Collections.Generic;

namespace ShipDock
{
    public class MessageLooper : IReclaim
    {
        public static void AddMessage(int message, INoticeBase<int> paramNotice)
        {
            MessageNotice msgNotice = MessageNotice.Create(message, paramNotice);
            ShipDockConsts.NOTICE_MSG_ADD.Broadcast(msgNotice);
        }

        public static void OnMessage(Action<INoticeBase<int>> handler, bool isRemove = false)
        {
            if (isRemove)
            {
                ShipDockConsts.NOTICE_MSG_QUEUE.Remove(handler);
            }
            else
            {
                ShipDockConsts.NOTICE_MSG_QUEUE.Add(handler);
            }
        }

        private bool mHasMessageNotice;
        private IPoolable mReclaimParam;
        private IPoolable mMessageParam;
        private IUpdate mMessageUpdater;
        private IMessageNotice mNoticeWillAdd;
        private Queue<IPoolable> mMessageParamReclaims;
        private DoubleBuffers<IMessageNotice> mDoubleBuffers;

        public void Reclaim()
        {
            Purge();
        }

        private void Purge()
        {
            UpdaterNotice.RemoveSceneUpdater(mMessageUpdater);

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

            ShipDockConsts.NOTICE_MSG_ADD.Add(OnMessageAdd);
            UpdaterNotice.AddSceneUpdater(mMessageUpdater);
        }

        private void OnMessageAdd(INoticeBase<int> param)
        {
            mNoticeWillAdd = param as IMessageNotice;
            if (mNoticeWillAdd != default)
            {
                if (mNoticeWillAdd != default)
                {
                    mDoubleBuffers.Enqueue(mNoticeWillAdd);
                }
                else { }
            }
            else { }

            mNoticeWillAdd = default;
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
            mHasMessageNotice = param != default;

            if (mHasMessageNotice)
            {
                //派发处理消息的模块消息
                ShipDockConsts.NOTICE_MSG_QUEUE.Broadcast(param);

                mMessageParam = param.MsgNotice as IPoolable;
                if ((mMessageParam != default) && !mMessageParamReclaims.Contains(mMessageParam))
                {
                    mMessageParamReclaims.Enqueue(mMessageParam);
                }
                else { }

                param.ToPool();
            }
            else { }

            mHasMessageNotice = false;
            mMessageParam = default;
        }
    }
}