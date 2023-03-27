using LitJson;
using System;
using System.Collections.Generic;
using OnErrorResponse = UnityEngine.Events.UnityAction<int, string, string, System.Collections.Generic.Dictionary<string, string>>;

namespace ShipDock
{
    public class NotificationRequester<T> : JsonRequester<T> where T : NotificationResponseIniter, new()
    {
        public NotificationRequester(int noticeName, string keyInUrlMrg, HttpRequestType requestType = HttpRequestType.Post) : base(keyInUrlMrg, requestType)
        {
            noticeName.Add(OnResponserInit);
            NotificationResponseIniter initer = ResponserIniter as NotificationResponseIniter;
            initer.Init(noticeName, true);
        }

        private void OnResponserInit(INoticeBase<int> param)
        {
            if (param is IParamNotice<Action<RequestResponser>> sucNotice)
            {
                sucNotice.ParamValue += OnSuccess;
            }
            else if (param is IParamNotice<Action<int>> failNotice)
            {
                failNotice.ParamValue += OnFailed;
            }
            else if (param is IParamNotice<OnErrorResponse> errorNotice)
            {
                errorNotice.ParamValue += OnError;
            }
            else if (param is IParamNotice<Dictionary<string, string>> paramNotice)
            {
                Dictionary<string, string> dic = paramNotice.ParamValue;
                if (dic == default)
                {
                    dic = new Dictionary<string, string>();
                }
                else { }
                CreateDicParam(ref dic);
            }
            else if (param is IParamNotice<JsonData> jsonNotice)
            {
                JsonData json = jsonNotice.ParamValue;
                if (json == default)
                {
                    json = new JsonData();
                }
                else { }
                CreateJSONParam(ref json);
            }
            else { }
        }

        protected virtual void CreateJSONParam(ref JsonData json) { }

        protected virtual void CreateDicParam(ref Dictionary<string, string> dic) { }

        protected virtual void OnError(int statu, string engineError, string url, Dictionary<string, string> data) { }

        protected virtual void OnFailed(int obj) { }

        protected virtual void OnSuccess(RequestResponser obj) { }
    }

}
