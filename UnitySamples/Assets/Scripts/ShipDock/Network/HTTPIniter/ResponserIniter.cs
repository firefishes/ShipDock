using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using OnErrorResponse = 
    UnityEngine.Events.UnityAction<int, string, string, 
        System.Collections.Generic.Dictionary<string, string>>;

namespace ShipDock
{
    public abstract class ResponserIniter
    {
        public bool IgnoreParamCreate { get; set; }
        public bool ApplyJSONParam { get; set; }
        public JsonData JsonParam { get; set; }
        public Dictionary<string, string> DicParam { get; set; }
        public Action<RequestResponser> OnResponseSuccess { get; set; }
        public Action<int> OnResponseFailed { get; set; }
        public OnErrorResponse OnErrorNet { get; set; }
        public UnityAction<string> SuccessForCustom { get; set; }

        protected string FailedLog { get; set; } = string.Empty;

        public abstract void Build();

        public void Clean()
        {
            DicParam = default;
            JsonParam = default;
            OnResponseSuccess = default;
            OnResponseFailed = default;
            OnErrorNet = default;
            SuccessForCustom = default;
        }

        public int GetTimeSeed(bool applyOffset = true)
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            long t = (DateTime.Now.Ticks - startTime.Ticks);
            return applyOffset ? (int)(t + 1) : (int)t;
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void LogRequestFailed()
        {
            "warning".Log(GetType().Name.ToString().Append(" request failed.. ", FailedLog));
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void LogRequestError()
        {
            "error".Log(GetType().Name.ToString().Append(" error.. ", FailedLog));
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void LogRequestSuccess(ref RequestResponser responser)
        {
            "log".Log(GetType().Name.ToString().Append(" request success, Json result: ", responser.ResultRoot.ToString()));
        }
    }
}
