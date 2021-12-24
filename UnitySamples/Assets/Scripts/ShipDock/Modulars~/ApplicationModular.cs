using ShipDock.Interfaces;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Modulars
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ModularNoticeCreateAttribute : Attribute, IModularMethodPriority
    {
        public int NoticeName { get; set; }
        public int ID { get; set; }
        public int Priority { get; set; }

        public ModularNoticeCreateAttribute(int noticeName, int priority = int.MinValue)
        {
            NoticeName = noticeName;
            Priority = priority;
        }

        public void Dispose() { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ModularNoticeDecoraterAttribute : Attribute, IModularMethodPriority
    {
        public int NoticeName { get; set; }
        public int ID { get; set; }
        public int Priority { get; set; }

        public ModularNoticeDecoraterAttribute(int noticeName, int priority = int.MinValue)
        {
            NoticeName = noticeName;
            Priority = priority;
        }

        public void Dispose() { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ModularNoticeListenerAttribute : Attribute, IModularMethodPriority
    {
        public int NoticeName { get; set; }
        public int ID { get; set; }
        public int Priority { get; set; }

        public ModularNoticeListenerAttribute(int noticeName, int priority = int.MinValue)
        {
            NoticeName = noticeName;
            Priority = priority;
        }

        public void Dispose() { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ModularNotifyAttribute : Attribute
    {
        public int[] Notices { get; set; }
        public int NotifyTiming { get; set; } = ModularNotifyTiming.AFTER;

        public ModularNotifyAttribute(params int[] notices)
        {
            Notices = notices;
        }
    }

    public static class ModularNotifyTiming
    {
        public const int AFTER = 0;
        public const int BEFORE = 1;
        public const int ALWAYS = 2;
    }
    
    /// <summary>
    /// 模块消息生成器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public sealed class ModularNoticeCreater : IModularMethodPriority
    {
        public int ID { get; set; }
        public int Priority { get; set; }
        public int NoticeName { get; set; }
        public Func<int, INoticeBase<int>> Handler { get; set; }

        public ModularNoticeCreater(int noticeName, Func<int, INoticeBase<int>> handler, int priority = int.MinValue)
        {
            NoticeName = noticeName;
            Handler = handler;
            Priority = priority;
        }

        public void Dispose()
        {
            Handler = default;
        }
    }

    /// <summary>
    /// 模块消息装饰器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public sealed class ModularNoticeDecorater : IModularMethodPriority
    {
        public int ID { get; set; }
        public int Priority { get; set; }
        public int NoticeName { get; set; }
        public Action<int, INoticeBase<int>> Handler { get; set; }

        public ModularNoticeDecorater(int noticeName, Action<int, INoticeBase<int>> handler, int priority = int.MinValue)
        {
            NoticeName = noticeName;
            Handler = handler;
            Priority = priority;
        }

        public void Dispose()
        {
            Handler = default;
        }
    }

    /// <summary>
    /// 模块消息侦听器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public sealed class ModularNoticeListener : IModularMethodPriority
    {
        public int ID { get; set; }
        public int Priority { get; set; }
        public int NoticeName { get; set; }
        public Action<INoticeBase<int>> Handler { get; set; }

        public ModularNoticeListener(int noticeName, Action<INoticeBase<int>> handler, int priority = int.MinValue)
        {
            NoticeName = noticeName;
            Handler = handler;
            Priority = priority;
        }

        public void Dispose()
        {
            Handler = default;
        }
    }

    public interface IModularMethodPriority : IDispose
    {
        int ID { get; set; }
        int NoticeName { get; set; }
        int Priority { get; set; }
    }

    /// <summary>
    /// 应用模块抽象类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class ApplicationModular : IModular
    {

        private static readonly Type noticeCreateAttributeType = typeof(ModularNoticeCreateAttribute);
        private static readonly Type noticeDecorateAttributeType = typeof(ModularNoticeDecoraterAttribute);
        private static readonly Type noticeListenAttributeType = typeof(ModularNoticeListenerAttribute);
        private static readonly Type mNoticeNotifyAttributeType = typeof(ModularNotifyAttribute);

        private object[] mModularAttributes;
        private List<ModularNoticeCreater> mCreaterByAttributes;
        private List<ModularNoticeDecorater> mDecoraterByAttributes;
        private List<ModularNoticeListener> mListenerByAttributes;
        private ModularNoticeCreateAttribute mNoticeCreateAttribute;
        private ModularNoticeDecoraterAttribute mNoticeDecoraterAttribute;
        private ModularNoticeListenerAttribute mNoticeListenerAttribute;
        private ModularNotifyAttribute mNoticeNotifyAttribute;

        public virtual ModularNoticeCreater[] NoticeCreates { get; protected set; }
        public virtual ModularNoticeDecorater[] NoticeDecoraters { get; protected set; }
        public virtual ModularNoticeListener[] NoticeListeners { get; protected set; }
        public virtual int ModularName { get; protected set; }

        protected virtual IAppModulars Modulars { get; set; }

        public virtual void Dispose()
        {
            int noticeName;
            ModularNoticeCreater creater;
            ModularNoticeCreater[] createrList = NoticeCreates;
            int max = createrList != default ? createrList.Length : 0;
            for (int i = 0; i < max; i++)
            {
                creater = createrList[i];
                noticeName = creater.NoticeName;
                Modulars.RemoveNoticeCreater(noticeName, creater.Handler);
            }
            ModularNoticeDecorater decorater;
            ModularNoticeDecorater[] decoraterList = NoticeDecoraters;
            max = decoraterList != default ? decoraterList.Length : 0;
            for (int i = 0; i < max; i++)
            {
                decorater = decoraterList[i];
                noticeName = decorater.NoticeName;
                Modulars.RemoveNoticeDecorator(noticeName, decorater.Handler);
            }
            ModularNoticeListener listener;
            ModularNoticeListener[] listenerList = NoticeListeners;
            max = listenerList != default ? listenerList.Length : 0;
            for (int i = 0; i < max; i++)
            {
                listener = listenerList[i];
                noticeName = listener.NoticeName;
                noticeName.Remove(listener.Handler);
            }

            Purge();

            mModularNotifierMapper?.Dispose();
            mModularNotifierMapper = default;
            Modulars = default;
        }

        public abstract void Purge();

        protected void AddNoticeCreater(Func<int, INoticeBase<int>> method, bool inherit = false)
        {
            mModularAttributes = method.Method.GetCustomAttributes(noticeCreateAttributeType, inherit);
            int max = mModularAttributes != default ? mModularAttributes.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mNoticeCreateAttribute = mModularAttributes[i] as ModularNoticeCreateAttribute;
                    if (mNoticeCreateAttribute != default)
                    {
                        int noticeName = mNoticeCreateAttribute.NoticeName;
                        int priority = mNoticeCreateAttribute.Priority;
                        mCreaterByAttributes.Add(new ModularNoticeCreater(noticeName, method, priority));
                    }
                    else { }
                }
                mNoticeCreateAttribute = default;
            }
            else { }
            mModularAttributes = default;
        }

        protected void AddNoticeDecorater(Action<int, INoticeBase<int>> method, bool inherit = false)
        {
            mModularAttributes = method.Method.GetCustomAttributes(noticeDecorateAttributeType, inherit);
            int max = mModularAttributes != default ? mModularAttributes.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mNoticeDecoraterAttribute = mModularAttributes[i] as ModularNoticeDecoraterAttribute;
                    if (mNoticeDecoraterAttribute != default)
                    {
                        int noticeName = mNoticeDecoraterAttribute.NoticeName;
                        int priority = mNoticeDecoraterAttribute.Priority;
                        mDecoraterByAttributes.Add(new ModularNoticeDecorater(noticeName, method, priority));
                    }
                    else { }
                }
                mNoticeDecoraterAttribute = default;
            }
            else { }
            mModularAttributes = default;
        }

        protected void AddNoticeHandler(Action<INoticeBase<int>> method, bool inherit = false)
        {
            mModularAttributes = method.Method.GetCustomAttributes(noticeListenAttributeType, inherit);
            int max = mModularAttributes != default ? mModularAttributes.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    mNoticeListenerAttribute = mModularAttributes[i] as ModularNoticeListenerAttribute;
                    if (mNoticeListenerAttribute != default)
                    {
                        int noticeName = mNoticeListenerAttribute.NoticeName;
                        int priority = mNoticeListenerAttribute.Priority;
                        mListenerByAttributes.Add(new ModularNoticeListener(noticeName, method, priority));
                    }
                    else { }
                }
                mNoticeListenerAttribute = default;
            }
            else { }
            mModularAttributes = default;
        }

        private KeyValueList<Action<INoticeBase<int>>, ModularNotifier> mModularNotifierMapper = new KeyValueList<Action<INoticeBase<int>>, ModularNotifier>();

        protected void AddNotifies(Action<INoticeBase<int>> method, bool inherit = false)
        {
            if (!mModularNotifierMapper.ContainsKey(method))
            {
                mModularAttributes = method.Method.GetCustomAttributes(mNoticeNotifyAttributeType, inherit);
                mNoticeNotifyAttribute = mModularAttributes != default ? mModularAttributes[0] as ModularNotifyAttribute : default;
                if (mNoticeNotifyAttribute != default)
                {
                    mModularNotifierMapper[method] = new ModularNotifier(mNoticeNotifyAttribute.Notices)
                    {
                        NotifyTiming = mNoticeNotifyAttribute.NotifyTiming,
                    };
                }
                else { }
            }
            else { }

            mNoticeNotifyAttribute = default;
            mModularAttributes = default;
        }

        private void GenerateModularHandlers<TMethodPriority>(
            TMethodPriority[] noticeProcess, 
            List<TMethodPriority> noticeProcessByAttributes, 
            Action<TMethodPriority, bool> method) where TMethodPriority : IModularMethodPriority
        {
            TMethodPriority[] list = noticeProcess;

            if (noticeProcessByAttributes != default && noticeProcessByAttributes.Count > 0)
            {
                TMethodPriority[] willContact = noticeProcessByAttributes.ToArray();
                list.ContactToArr(willContact, out list);
                noticeProcessByAttributes.Clear();
            }
            else { }

            int max = list != default ? list.Length : 0;
            if (max > 0)
            {
                TMethodPriority item;
                for (int i = 0; i < max; i++)
                {
                    item = list[i];
                    method.Invoke(item, i == max - 1);
                }
            }
            else { }
        }

        public virtual void InitModular()
        {
            mCreaterByAttributes = new List<ModularNoticeCreater>();
            mDecoraterByAttributes = new List<ModularNoticeDecorater>();
            mListenerByAttributes = new List<ModularNoticeListener>();

            if (NoticeCreates == default)
            {
                NoticeCreates = new ModularNoticeCreater[0];
            }
            else { }

            if (NoticeDecoraters == default)
            {
                NoticeDecoraters = new ModularNoticeDecorater[0];
            }
            else { }

            if (NoticeListeners == default)
            {
                NoticeListeners = new ModularNoticeListener[0];
            }
            else { }

            InitCustomHandlers();

            GenerateModularHandlers();
        }

        protected void GenerateModularHandlers()
        {
            GenerateModularHandlers(NoticeCreates, mCreaterByAttributes, Modulars.AddNoticeCreater);
            GenerateModularHandlers(NoticeDecoraters, mDecoraterByAttributes, Modulars.AddNoticeDecorator);
            GenerateModularHandlers(NoticeListeners, mListenerByAttributes, Modulars.AddNoticeListener);
        }

        protected virtual void InitCustomHandlers() { }

        public virtual INoticeBase<int> NotifyModular(int name, INoticeBase<int> notice = default)
        {
            return Modulars != default ? Modulars.NotifyModular(name, notice) : default;
        }

        public virtual INoticeBase<int> NotifyModularWithParam<T>(int name, T param = default, IParamNotice<T> notice = default)
        {
            return Modulars != default ? Modulars.NotifyModularWithParam(name, param, notice) : default;
        }

        public virtual void NotifyModularAndRelease(int name, INoticeBase<int> notice = default)
        {
            Modulars?.NotifyModularAndRelease(name, notice);
        }

        public void NotifyModular(Action<INoticeBase<int>> method, INoticeBase<int> notice = default)
        {
            if (method != default)
            {
                ModularNotifier notifier = mModularNotifierMapper[method];
                notifier?.Commit(this, method, notice);
            }
            else { }
        }

        public virtual void SetModularManager(IAppModulars modulars)
        {
            Modulars = modulars;
        }

        private class ModularNotifier
        {
            public int[] NoticeNames { get; private set; }
            public int NotifyTiming { get; set; } = ModularNotifyTiming.AFTER;

            public ModularNotifier(int[] noticeNames)
            {
                NoticeNames = noticeNames;
            }

            public void Commit(ApplicationModular modular, Action<INoticeBase<int>> method, INoticeBase<int> notice = default)
            {
                if (NotifyTiming == ModularNotifyTiming.AFTER)
                {
                    method.Invoke(notice);
                }
                else { }

                int max = NoticeNames.Length;
                for (int i = 0; i < max; i++)
                {
                    if (NotifyTiming == ModularNotifyTiming.ALWAYS)
                    {
                        method.Invoke(notice);
                    }
                    else { }

                    modular.NotifyModular(NoticeNames[i]);
                }

                if (NotifyTiming == ModularNotifyTiming.BEFORE)
                {
                    method.Invoke(notice);
                }
                else { }
            }
        }
    }
}
