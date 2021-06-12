using LitJson;
using System;
using System.Collections.Generic;
using OnErrorResponse = UnityEngine.Events.UnityAction<int, string, string, System.Collections.Generic.Dictionary<string, string>>;

namespace ShipDock.Network
{
    public class NotificationResponseIniter : CommonResponserIniter
    {
        public int NoticeName { get; private set; }

        public NotificationResponseIniter() { }

        public void Init(int notice, bool applyJSONParam)
        {
            NoticeName = notice;
            ApplyJSONParam = applyJSONParam;
        }

        protected override void BuildResponseSuccess(Action<RequestResponser> success)
        {
            base.BuildResponseSuccess(success);

            OnResponseSuccess = NoticeName.BroadcastWithParam(success);
        }

        protected override void BuildResponseFailed(Action<int> failed)
        {
            base.BuildResponseFailed(failed);

            OnResponseFailed = NoticeName.BroadcastWithParam(failed);
        }

        protected override void BuildResponseError(OnErrorResponse error)
        {
            base.BuildResponseError(error);

            OnErrorNet = NoticeName.BroadcastWithParam(error);
        }

        protected override void CreateJSONParam(ref JsonData json)
        {
            base.CreateJSONParam(ref json);

            NoticeName.BroadcastWithParam(json);
        }

        protected override void CreateDicParam(ref Dictionary<string, string> dic)
        {
            base.CreateDicParam(ref dic);

            NoticeName.BroadcastWithParam(dic);
        }
    }
}
