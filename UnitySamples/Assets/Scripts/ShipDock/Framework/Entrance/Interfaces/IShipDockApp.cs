using System.Collections.Generic;

namespace ShipDock
{
    public interface IShipDockApp
    {
        void CreateTestersHandler();
        void ServerFinishedHandler();
        void InitProfileHandler(IParamNotice<int[]> param);
        void EnterGameHandler();
#if ULTIMATE
        void GetGameServersHandler(IParamNotice<IServer[]> param);
        void GetServerConfigsHandler(IParamNotice<IResolvableConfig[]> param);
        void InitProfileDataHandler(IConfigNotice param);
        void GetLocalsConfigItemHandler(Dictionary<int, string> raw, IConfigNotice param);
        void GetDataProxyHandler(IParamNotice<IDataProxy[]> param);
#endif
        void ApplicationCloseHandler();
        void SetShipDockGame(ShipDockGame comp);
    }
}