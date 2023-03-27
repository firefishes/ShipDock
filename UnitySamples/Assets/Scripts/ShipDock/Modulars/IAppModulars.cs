using System;

namespace ShipDock
{
    public interface IAppModulars
    {
        void AddNoticeCreater(ModularNoticeCreater creater, bool willSort = false);
        void RemoveNoticeCreater(int noticeName, Func<int, INoticeBase<int>> method);
        void AddNoticeDecorator(ModularNoticeDecorater decorater, bool willSort = false);
        void RemoveNoticeDecorator(int noticeName, Action<int, INoticeBase<int>> method);
        void AddNoticeListener(ModularNoticeListener listener, bool willSort = false);
        INoticeBase<int> NotifyModular(int name, INoticeBase<int> param = default);
        INoticeBase<int> NotifyModularWithParam<T>(int noticeName, T param = default, IParamNotice<T> notice = default);
        void NotifyModularAndRelease(int noticeName, INoticeBase<int> param = default, bool isRelease = true);
    }
}