using LitJson;
using System;
using System.Collections;
using UnityEngine.Events;
using OnErrorJsonResponse = UnityEngine.Events.UnityAction<int, string, string, LitJson.JsonData>;
using OnErrorResponse = UnityEngine.Events.UnityAction<int, string, string, System.Collections.Generic.Dictionary<string, string>>;

namespace ShipDock.Network
{
    public class RequestResponser
    {
        public bool ShowWaiting { get; set; }
        public JsonData ResultRoot { get; set; }
        public JsonData ResultData { get; set; }
        public Action<RequestResponser> OnSuccessed { get; set; }
        public UnityAction<string> SuccessForCustom { get; set; }
        public Action<int> Failed { get; set; }
        public OnErrorResponse Error { get; set; }
        public OnErrorJsonResponse ErrorForJson { get; set; }

        private IDictionary mDataMapper;

        public void Success(string data)
        {
            ResultRoot = JsonMapper.ToObject(data);

            IDictionary mapper = ResultRoot as IDictionary;
            if (mapper.Contains("error_code"))
            {
                string errorCodeValue = ResultRoot["error_code"].ToString();
                int errorCode = int.Parse(errorCodeValue);
                Failed?.Invoke(errorCode);
            }
            else
            {
                if (mapper.Contains("data"))
                {
                    ResultData = ResultRoot["data"];
                }
                else
                {
                    ResultData = ResultRoot;
                }
                mDataMapper = ResultData as IDictionary;
                OnSuccessed?.Invoke(this);
            }

            ResultRoot?.Clear();
            ResultData?.Clear();

            ResultRoot = default;
            ResultData = default;
            OnSuccessed = default;
            SuccessForCustom = default;
            Failed = default;
            Error = default;
        }

        public string GetString(string key)
        {
            return GetRawFromData(key);
        }

        public bool GetBool(string key)
        {
            string value = GetRawFromData(key);
            return bool.Parse(value);
        }

        public int GetInt(string key)
        {
            string value = GetRawFromData(key);
            return int.Parse(value);
        }

        public float GetFloat(string key)
        {
            string value = GetRawFromData(key);
            return float.Parse(value);
        }

        private string GetRawFromData(string key)
        {
            return HasData(key) ? mDataMapper[key].ToString() : string.Empty;
        }

        public bool HasData(string key)
        {
            return mDataMapper.Contains(key);
        }
    }
}
