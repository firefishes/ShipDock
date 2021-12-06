﻿using LitJson;
using ShipDock.Testers;
using ShipDock.Tools;
using System;
using UnityEngine;

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

        public void Init(Func<HTTPDriver> httpDriverGetter, Func<ServiceURL> serviceURLGetter, Action<bool, Action> driverStartUpdater, MonoBehaviour componentOwner)
        {
            HTTPDriver.StartUpdater = driverStartUpdater;

            HTTPDriver driverInstance = httpDriverGetter.Invoke();
            driverInstance.ComponentOwner = componentOwner;

            ServiceURL serviceURL = serviceURLGetter.Invoke();
            InitRequests(driverInstance, serviceURL);
        }

        private void InitRequests(HTTPDriver driver, ServiceURL services)
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

        public void SendRequest(string keyInURLManager, bool showWaiting = false)
        {
            IHttpRequester requester = mHttpReqeuesters[keyInURLManager];
            if (requester != default)
            {
                if (requester is IRequesterJsonParamer jsonParamRequest)
                {
                    jsonParamRequest.ShowWaiting = showWaiting;
                }
                else { }

                requester.Build();
                requester.Send();
            }
            else { }
        }

        public void SendRequest(string keyInURLManager, JsonData jsonParam, string headerAPI = "", bool showWaiting = false)
        {
            IHttpRequester requester = mHttpReqeuesters[keyInURLManager];

            if (requester is IRequesterJsonParamer jsonParamRequest)
            {
                jsonParamRequest.ShowWaiting = showWaiting;
                jsonParamRequest.RequestParam = jsonParam;

                requester.ResponserIniter.IgnoreParamCreate = true;

                requester.HeaderAPIKey = headerAPI;
                requester.Build();
                requester.Send();

                requester.ResponserIniter.IgnoreParamCreate = false;
            }
            else { }
        }

        public void SendRequestByOverrideURL(string keyInURLManager, string url, JsonData jsonParam, string headerAPI = "", bool showWaiting = false)
        {
            IHttpRequester requester = mHttpReqeuesters[keyInURLManager];

            bool isJSONParamReqeust = false;
            if (requester is IRequesterJsonParamer jsonParamRequest && jsonParam != default)
            {
                isJSONParamReqeust = true;
                jsonParamRequest.ShowWaiting = showWaiting;
                jsonParamRequest.RequestParam = jsonParam;

                requester.ResponserIniter.IgnoreParamCreate = true;
            }
            else { }

            requester.HeaderAPIKey = headerAPI;
            requester.RecoverURL(url);//覆盖为新的 URL 链接
            requester.Build();
            requester.Send();
            requester.RevertURL();//发送请求后还原为之前的 URL 链接

            if (isJSONParamReqeust)
            {
                requester.ResponserIniter.IgnoreParamCreate = false;
            }
            else { }
        }
    }
}
