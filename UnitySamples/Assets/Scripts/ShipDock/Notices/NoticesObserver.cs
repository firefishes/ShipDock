using System;
using OnlyOnceListenerList = System.Collections.Generic.List<System.Action<ShipDock.Notices.INoticeBase<int>>>;
using OnlyOnceListenerMapper = ShipDock.Tools.KeyValueList<int, System.Collections.Generic.List<System.Action<ShipDock.Notices.INoticeBase<int>>>>;

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
        private OnlyOnceListenerMapper mOnlyOnceListeners;
        private HandlerMapper<int> mNoticeMapper;

        public NoticesObserver()
        {
            mObserver = new Observer();
            mNoticeMapper = new HandlerMapper<int>();
            mOnlyOnceListeners = new OnlyOnceListenerMapper();
            mObserver.Add(NoticeHandler);//同时侦听来自广播方式派发的消息
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

        public void AddListener(int noticeName, INoticesHandler handler, bool onlyOnce = false)
        {
            AddListener(noticeName, handler.ListenerHandler, onlyOnce);
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
                AddOnlyOnceListener(noticeName, listener);

                if (HasListener(noticeName, listener))
                {
                    return;
                }
                else { }
            }
            else { }

            handlers.Add(listener);
        }

        private void AddOnlyOnceListener(int noticeName, Action<INoticeBase<int>> listener)
        {
            OnlyOnceListenerList list = default;
            if (mOnlyOnceListeners.ContainsKey(noticeName))
            {
                list = mOnlyOnceListeners[noticeName];
            }
            else
            {
                list = new OnlyOnceListenerList();
                mOnlyOnceListeners[noticeName] = list;
            }
            if (!list.Contains(listener))
            {
                list.Add(listener);
            }
            else { }
        }

        private void ClearOnlyOnceListeners(int noticeName, ref NoticeHandler<int> handlers)
        {
            if (mOnlyOnceListeners.ContainsKey(noticeName))
            {
                OnlyOnceListenerList list = mOnlyOnceListeners[noticeName];

                int max = list.Count;
                for (int i = 0; i < max; i++)
                {
                    handlers.Remove(list[i]);
                }
                list.Clear();
            }
            else { }
        }

        public void RemoveListener(int noticeName, Action<INoticeBase<int>> listener)
        {
            NoticeHandler<int> handlers = mNoticeMapper[noticeName];
            handlers?.Remove(listener);

            if (mOnlyOnceListeners.ContainsKey(noticeName))
            {
                OnlyOnceListenerList list = mOnlyOnceListeners[noticeName];
                if (list.Contains(listener))
                {
                    list.Remove(listener);
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 对观察者本体派发消息
        /// </summary>
        /// <param name="notice"></param>
        public void Dispatch(INoticeBase<int> notice)
        {
            int noticeName = notice.Name;
            if (mNoticeMapper.ContainsKey(noticeName))
            {
                NoticeHandler<int> handlers = mNoticeMapper[noticeName];
                if (handlers.Sender == mObserver)
                {
                    handlers.Invoke(ref notice);

                    ClearOnlyOnceListeners(noticeName, ref handlers);
                }
                else { }
            }
            else { }
        }
        
        private void NoticeHandler(INoticeBase<int> param)
        {
            if (param != default)
            {
                Dispatch(param);
            }
            else { }
        }

        #region 通过已扩展的观察者方法从全局消息中心派发消息，凭借此方式复用消息的快速组装方法
        public void Dispatch(int noticeName, INoticeBase<int> notice = default)
        {
            mObserver.Dispatch(noticeName, notice);
        }

        public T DispatchWithParam<T>(int noticeName, T vs)
        {
            return mObserver.Dispatch(noticeName, vs);
        }
        #endregion
    }
}
