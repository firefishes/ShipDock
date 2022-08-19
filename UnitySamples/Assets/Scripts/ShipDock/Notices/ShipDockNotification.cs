using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using System;

namespace ShipDock.Notices
{
    /// <summary>
    /// 
    /// 通知管理器基类
    /// 
    /// add by Minghua.ji
    /// 
    /// 用于构建使用不同类型作为消息的消息管理器
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NoticeManagerBase<T> : Singletons<NoticeManagerBase<T>>
    {
        public Notifications<T> Notificater { get; private set; }

        public NoticeManagerBase()
        {
            Notificater = new Notifications<T>();
        }

        public override void Reclaim()
        {
            base.Reclaim();

            Notificater.Reclaim();
        }
    }

    /// <summary>
    /// 以整型为消息名的消息管理器
    /// </summary>
    public class NotificatonsInt : NoticeManagerBase<int> { }

}

/// <summary>
/// 
/// 以整型为消息的消息管理器相关的扩展方法类
/// 
/// 提供广播和点对点的观察者消息注册、移除、发送功能
/// 
/// add by Minghua.ji
/// 
/// </summary>
public static class NoticesExtensions
{
    /// <summary>
    /// 添加全局消息侦听
    /// </summary>
    /// <param name="target"></param>
    /// <param name="handler"></param>
    public static void Add(this int target, Action<INoticeBase<int>> handler)
    {
        NotificatonsInt.Instance.Notificater?.Add(target, handler);
    }

    /// <summary>
    /// 添加观察者消息侦听
    /// </summary>
    /// <param name="target"></param>
    /// <param name="handler"></param>
    public static void Add(this INotificationSender target, Action<INoticeBase<int>> handler)
    {
        NotificatonsInt.Instance.Notificater?.Add(target, handler);
    }

    /// <summary>
    /// 移除全局消息侦听
    /// </summary>
    /// <param name="target"></param>
    /// <param name="handler"></param>
    public static void Remove(this int target, Action<INoticeBase<int>> handler)
    {
        NotificatonsInt.Instance.Notificater?.Remove(target, handler);
    }

    /// <summary>
    /// 移除观察者消息侦听
    /// </summary>
    /// <param name="target"></param>
    /// <param name="handler"></param>
    public static void Remove(this INotificationSender target, Action<INoticeBase<int>> handler)
    {
        NotificatonsInt.Instance.Notificater?.Remove(target, handler);
    }

    /// <summary>
    /// 广播全局消息
    /// </summary>
    /// <param name="noticeName"></param>
    /// <param name="notice"></param>
    public static void Broadcast(this int noticeName, INoticeBase<int> notice = default)
    {
        bool defaultNotice = notice == default;
        if (defaultNotice)
        {
            notice = new Notice();
        }
        else { }

        notice.SetNoticeName(noticeName);
        NotificatonsInt.Instance.Notificater?.Broadcast(notice);
        if (defaultNotice)
        {
            notice.Reclaim();
        }
        else { }
    }

    /// <summary>
    /// 广播携带一个参数的全局消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="noticeName"></param>
    /// <param name="vs"></param>
    /// <param name="applyPooling"></param>
    /// <returns></returns>
    public static T BroadcastWithParam<T>(this int noticeName, T vs, bool applyPooling = false)
    {
        ParamNotice<T> notice;
        if (applyPooling)
        {
            notice = Pooling<ParamNotice<T>>.Instance.FromPool();
        }
        else
        {
            notice = new ParamNotice<T>();
        }
        notice.ParamValue = vs;

        notice.SetNoticeName(noticeName);
        NotificatonsInt.Instance.Notificater?.Broadcast(notice);
        T result = notice.ParamValue;

        if (applyPooling)
        {
            Pooling<ParamNotice<T>>.Instance.ToPool(notice);
        }
        else
        {
            notice.Reclaim();
        }
        return result;
    }

    /// <summary>
    /// 派发携带一个参数的观察者消息，消息的参数须在方法调用前定义
    /// </summary>
    /// <param name="target"></param>
    /// <param name="notice"></param>
    public static void Dispatch(this INotificationSender target, INoticeBase<int> notice)
    {
        notice.NotifcationSender = target;
        NotificatonsInt.Instance.Notificater.Dispatch(notice);
    }

    /// <summary>
    /// 派发携带一个参数的观察者消息，并传入消息的参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="noticeName"></param>
    /// <param name="vs"></param>
    /// <returns></returns>
    public static T Dispatch<T>(this INotificationSender target, int noticeName, T vs)
    {
        ParamNotice<T> notice = new ParamNotice<T>
        {
            ParamValue = vs
        };
        notice.SetNoticeName(noticeName);
        notice.NotifcationSender = target;

        NotificatonsInt.Instance.Notificater.Dispatch(notice);

        T result = notice.ParamValue;
        notice.Reclaim();
        return result;
    }

    /// <summary>
    /// 派发观察者消息
    /// </summary>
    /// <param name="target"></param>
    /// <param name="noticeName"></param>
    /// <param name="notice"></param>
    public static void Dispatch(this INotificationSender target, int noticeName, INoticeBase<int> notice = default)
    {
        bool defaultNotice = notice == default;
        if (defaultNotice)
        {
            notice = Pooling<Notice>.From();
        }
        else { }

        notice.SetNoticeName(noticeName);
        notice.NotifcationSender = target;

        NotificatonsInt.Instance.Notificater.Dispatch(notice);

        if (defaultNotice)
        {
            Pooling<Notice>.To(notice as Notice);
        }
        else { }
    }
}