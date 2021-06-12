using LitJson;
using System;
using System.Collections.Generic;
using OnErrorResponse = UnityEngine.Events.UnityAction<int, string, string, System.Collections.Generic.Dictionary<string, string>>;

namespace ShipDock.Network
{
    public class CommonResponserIniter : ResponserIniter
    {
        public CommonResponserIniter(bool applyJSONParam = false)
        {
            ApplyJSONParam = applyJSONParam;
        }

        public override void Build()
        {
            Action<RequestResponser> success = default;
            BuildResponseSuccess(success);

            Action<int> failed = default;
            BuildResponseFailed(failed);

            OnErrorResponse error = default;
            BuildResponseError(error);

            if (!IgnoreParamCreate)
            {
                if (ApplyJSONParam)
                {
                    JsonData data = default;
                    CreateJSONParam(ref data);
                    JsonParam = data;
                }
                else
                {
                    Dictionary<string, string> data = default;
                    CreateDicParam(ref data);
                    DicParam = data;
                }
            }
            else { }
        }

        protected virtual void BuildResponseSuccess(Action<RequestResponser> success)
        {
            success += OnSuccess;
            OnResponseSuccess = success;
        }

        protected virtual void BuildResponseFailed(Action<int> failed)
        {
            failed += OnFailed;
            OnResponseFailed = failed;
        }

        protected virtual void BuildResponseError(OnErrorResponse error)
        {
            error += OnError;
            OnErrorNet = error;
        }

        protected virtual void CreateJSONParam(ref JsonData json)
        {
            json = new JsonData();
            json.SetJsonType(JsonType.Object);
        }

        protected virtual void CreateDicParam(ref Dictionary<string, string> dic)
        {
            dic = new Dictionary<string, string>();
        }

        protected virtual void OnError(int statu, string engineError, string url, Dictionary<string, string> data) { }

        protected virtual void OnFailed(int obj) { }

        protected virtual void OnSuccess(RequestResponser obj) { }
    }
}
