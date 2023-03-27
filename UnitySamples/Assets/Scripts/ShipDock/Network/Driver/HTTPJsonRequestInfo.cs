using LitJson;
using UnityEngine.Events;

namespace ShipDock
{
    public class HTTPJsonRequestInfo
    {
        public int timeOut = 10;
        public bool showWaiting = true;
        public string headerAPI;
        public string requestURL;
        public JsonData data = null;
        public HttpRequestType requestType;
        public UnityAction<string> callback;
        public UnityAction<string> successResponse;
        public UnityAction<int, string, string, JsonData> errorResponse;
    }
}
