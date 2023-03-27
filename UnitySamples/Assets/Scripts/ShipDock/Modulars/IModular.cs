namespace ShipDock
{
    public interface IModular
    {
        void SetModularManager(IAppModulars modulars);
        int ModularName { get; }
        void Dispose();
        void InitModular();
        INoticeBase<int> NotifyModular(int name, INoticeBase<int> param = default);
        INoticeBase<int> NotifyModularWithParam<T>(int name, T param = default, IParamNotice<T> notice = default);
        void NotifyModularAndRelease(int name, INoticeBase<int> notice = default);
    }
}
