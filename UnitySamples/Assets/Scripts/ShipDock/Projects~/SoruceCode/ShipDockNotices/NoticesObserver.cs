using System;

namespace ShipDock.Notices
{
    /// <summary>
    /// 
    /// 消息观察者
    /// 
    /// 用于建立独立监听消息的观察者对象
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class NoticesObserver
    {
        private class Observer : INotificationSender { }

        private Observer mObserver;
        private HandlerMapper<int> mNoticeMapper;

        public NoticesObserver()
        {
            mObserver = new Observer();
            mNoticeMapper = new HandlerMapper<int>();
            mObserver.Add(NoticeHandler);
        }

        public void Clean()
        {
            RemoveAllListeners();

            mObserver.Remove(NoticeHandler);
            mObserver = default;
        }

        public void RemoveAllListeners()
        {
            mNoticeMapper.Clear();
        }

        public bool HasListener(int noticeName, Action<INoticeBase<int>> listener)
        {
            bool result = false;
            NoticeHandler<int> handlers;
            if (mNoticeMapper.ContainsKey(noticeName))
            {
                handlers = mNoticeMapper[noticeName];
                result = handlers.HasHandler(listener);
            }
            else { }
            return result;
        }

        public void AddListener(int noticeName, Action<INoticeBase<int>> listener, bool onlyOnce = false)
        {
            NoticeHandler<int> handlers;
            if (mNoticeMapper.ContainsKey(noticeName))
            {
                handlers = mNoticeMapper[noticeName];
            }
            else
            {
                handlers = NoticeHandler<int>.Create(noticeName, mObserver);
                mNoticeMapper[noticeName] = handlers;
            }
            if (onlyOnce)
            {
                if (HasListener(noticeName, listener))
                {
                    return;
                }
                else { }
            }
            else { }

            handlers.Add(listener);
        }

        public void RemoveListener(int noticeName, Action<INoticeBase<int>> listener)
        {
            NoticeHandler<int> handlers = mNoticeMapper[noticeName];
            handlers?.Remove(listener);
        }

        public void Dispatch(INoticeBase<int> notice)
        {
            mObserver.Dispatch(notice);
        }

        public void Dispatch(int noticeName, INoticeBase<int> notice = default)
        {
            mObserver.Dispatch(noticeName, notice);
        }

        public T Dispatch<T>(int noticeName, T vs)
        {
            return mObserver.Dispatch(noticeName, vs);
        }
        
        private void NoticeHandler(INoticeBase<int> param)
        {
            if (param != default)
            {
                NoticeHandler<int> handler = mNoticeMapper[param.Name];
                handler?.Invoke(ref param);
            }
            else { }
        }
    }
}
