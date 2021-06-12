namespace ShipDock.Network
{
    public interface IHttpRequester
    {
        HTTPDriver Driver { get; }
        ServiceURL Services { get; }
        ResponserIniter ResponserIniter { get; set; }
        RequestResponser Resposer { get; set; }
        string KeyInURLServices { get; }
        string HeaderAPIKey { get; set; }
        void Send();
        void Build();
        void Init(HTTPDriver driver, ServiceURL services);
    }
}