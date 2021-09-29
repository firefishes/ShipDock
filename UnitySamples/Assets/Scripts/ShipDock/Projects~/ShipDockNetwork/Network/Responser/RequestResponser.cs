using LitJson;
using System;
using System.Collections;
using UnityEngine.Events;
using OnErrorJsonResponse = UnityEngine.Events.UnityAction<int, string, string, LitJson.JsonData>;
using OnErrorResponse = UnityEngine.Events.UnityAction<int, string, string, System.Collections.Generic.Dictionary<string, string>>;

public static class JsonDataExtensions
{
    public static string GetDataFromMapper(this JsonData target, string key)
    {
        IDictionary mapper = target as IDictionary;
        string result = string.Empty;
        if (mapper.Contains(key))
        {
            result = mapper[key].ToString();
        }
        else { }
        return result;
    }

    public static float Float(this JsonData target, string key)
    {
        string value = target.GetDataFromMapper(key);
        return string.IsNullOrEmpty(value) ? 0f : float.Parse(value);
    }

    public static int Int(this JsonData target, string key)
    {
        string value = target.GetDataFromMapper(key);
        return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public static bool Bool(this JsonData target, string key)
    {
        string value = target.GetDataFromMapper(key);
        return bool.Parse(value);
    }

    public static string String(this JsonData target, string key)
    {
        string value = target.GetDataFromMapper(key);
        return string.IsNullOrEmpty(value) ? string.Empty : value;
    }
}

namespace ShipDock.Network
{
    public class RequestResponser
    {
        public string FailedKeyField { get; set; }
        public string DataKeyField { get; set; }
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
            if (mapper.Contains(FailedKeyField))
            {
                string errorCodeValue = ResultRoot[FailedKeyField].ToString();
                int errorCode = int.Parse(errorCodeValue);
                Failed?.Invoke(errorCode);
            }
            else
            {
                if (mapper.Contains(DataKeyField))
                {
                    ResultData = ResultRoot[DataKeyField];
                }
                else
                {
                    ResultData = ResultRoot;
                }
                mDataMapper = ResultData as IDictionary;
                OnSuccessed?.Invoke(this);
                SuccessForCustom?.Invoke(data);
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

        public string DataString(string key)
        {
            return GetRawFromData(key);
        }

        public bool DataBool(string key)
        {
            string value = GetRawFromData(key);
            return string.IsNullOrEmpty(value) ? false : bool.Parse(value);
        }

        public int DataInt(string key)
        {
            string value = GetRawFromData(key);
            return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
        }

        public float DataFloat(string key)
        {
            string value = GetRawFromData(key);
            return string.IsNullOrEmpty(value) ? 0f : float.Parse(value);
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
