using ShipDock.Notices;
using System;

namespace ShipDock.Modulars
{
    public sealed class ModularNoticeCreater
    {
        public int NoticeName { get; set; }
        public Func<int, INoticeBase<int>> Handler { get; set; }

        public ModularNoticeCreater(int noticeName, Func<int, INoticeBase<int>> handler)
        {
            NoticeName = noticeName;
            Handler = handler;
        }
    }

    public sealed class ModularNoticeDecorater
    {
        public int NoticeName { get; set; }
        public Action<int, INoticeBase<int>> Handler { get; set; }

        public ModularNoticeDecorater(int noticeName, Action<int, INoticeBase<int>> handler)
        {
            NoticeName = noticeName;
            Handler = handler;
        }
    }

    public sealed class ModularNoticeListener
    {
        public int NoticeName { get; set; }
        public Action<INoticeBase<int>> Handler { get; set; }

        public ModularNoticeListener(int noticeName, Action<INoticeBase<int>> handler)
        {
            NoticeName = noticeName;
            Handler = handler;
        }
    }

    public abstract class ApplicationModular : IModular
    {
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

            Modulars = default;
        }

        public virtual void InitModular()
        {
            int noticeName;
            ModularNoticeCreater creater;
            ModularNoticeCreater[] createrList = NoticeCreates;
            int max = createrList != default ? createrList.Length : 0;
            for (int i = 0; i < max; i++)
            {
                creater = createrList[i];
                noticeName = creater.NoticeName;
                Modulars.AddNoticeCreater(noticeName, creater.Handler);
            }
            ModularNoticeDecorater decorater;
            ModularNoticeDecorater[] decoraterList = NoticeDecoraters;
            max = decoraterList != default ? decoraterList.Length : 0;
            for (int i = 0; i < max; i++)
            {
                decorater = decoraterList[i];
                noticeName = decorater.NoticeName;
                Modulars.AddNoticeDecorator(noticeName, decorater.Handler);
            }
            ModularNoticeListener listener;
            ModularNoticeListener[] listenerList = NoticeListeners;
            max = listenerList != default ? listenerList.Length : 0;
            for (int i = 0; i < max; i++)
            {
                listener = listenerList[i];
                noticeName = listener.NoticeName;
                noticeName.Add(listener.Handler);
            }
        }

        public abstract void Purge();

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

        public virtual void SetModularManager(IAppModulars modulars)
        {
            Modulars = modulars;
        }
    }
}
