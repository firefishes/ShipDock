using ShipDock.Notices;
using System;

namespace ShipDock.Modulars
{
    public interface IAppModulars
    {
        void AddNoticeCreater(ModularNoticeCreater creater);
        void RemoveNoticeCreater(int noticeName, Func<int, INoticeBase<int>> method);
        void AddNoticeDecorator(ModularNoticeDecorater decorater);
        void RemoveNoticeDecorator(int noticeName, Action<int, INoticeBase<int>> method);
        void AddNoticeListener(ModularNoticeListener listener);
        INoticeBase<int> NotifyModular(int name, INoticeBase<int> param = default);
        INoticeBase<int> NotifyModularWithParam<T>(int noticeName, T param = default, IParamNotice<T> notice = default);
        void NotifyModularAndRelease(int noticeName, INoticeBase<int> param = default, bool isRelease = true);
    }
}