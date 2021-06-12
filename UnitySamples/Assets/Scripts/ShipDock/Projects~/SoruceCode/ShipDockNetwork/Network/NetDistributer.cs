using LitJson;
using ShipDock.Testers;
using ShipDock.Tools;

namespace ShipDock.Network
{
    public class NetDistributer
    {
        private KeyValueList<string, IHttpRequester> mHttpReqeuesters;

        public NetDistributer()
        {
            Tester.Instance.AddLogger("HTTP url", "log:发送服务端请求 URL: {0}");

            mHttpReqeuesters = new KeyValueList<string, IHttpRequester>();
        }

        public virtual void Clean()
        {
            mHttpReqeuesters?.Clear();
        }

        public void InitRequests(HTTPDriver driver, ServiceURL services)
        {
            IHttpRequester requester;
            int max = mHttpReqeuesters.Size;
            for (int i = 0; i < max; i++)
            {
                requester = mHttpReqeuesters.GetValueByIndex(i);
                requester.Init(driver, services);
            }
        }

        public void AddRequester(IHttpRequester requester)
        {
            string keyInServices = requester.KeyInURLServices;
            mHttpReqeuesters[keyInServices] = requester;
        }

        public T GetHTTPIniter<T>(string keyInURLManager) where T : ResponserIniter
        {
            T result = default;
            IHttpRequester request = mHttpReqeuesters[keyInURLManager];
            if (request != default)
            {
                result = (T)request.ResponserIniter;
            }
            else { }
            return result;
        }

        public void SendRequest(string keyInURLManager)
        {
            IHttpRequester requester = mHttpReqeuesters[keyInURLManager];
            requester?.Build();
            requester?.Send();
        }

        public void SendRequest(string keyInURLManager, JsonData jsonParam, string headerAPI = "")
        {
            IHttpRequester requester = mHttpReqeuesters[keyInURLManager];

            if (requester is IRequesterJsonParamer jsonParamRequest)
            {
                jsonParamRequest.RequestParam = jsonParam;

                ResponserIniter initer = requester.ResponserIniter;
                initer.IgnoreParamCreate = true;

                requester.HeaderAPIKey = headerAPI;
                requester?.Build();
                requester?.Send();

                initer.IgnoreParamCreate = false;
            }
            else { }
        }
    }
}
