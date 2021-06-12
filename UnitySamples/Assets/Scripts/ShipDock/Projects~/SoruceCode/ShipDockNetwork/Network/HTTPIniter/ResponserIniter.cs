using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using OnErrorResponse = 
    UnityEngine.Events.UnityAction<int, string, string, 
        System.Collections.Generic.Dictionary<string, string>>;

namespace ShipDock.Network
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
    }
}
