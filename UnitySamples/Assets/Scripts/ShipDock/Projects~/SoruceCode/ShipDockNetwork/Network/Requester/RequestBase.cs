﻿using LitJson;
using System;
using System.Collections.Generic;

namespace ShipDock.Network
{
    public abstract class RequestBase : IHttpRequester
    {
        public HTTPDriver Driver { get; private set; }
        public ServiceURL Services { get; private set; }
        public string ServiceURL { get; private set; }
        public string KeyInURLServices { get; private set; }
        public string HeaderAPIKey { get; set; }
        public string ResponseFailedKeyField { get; set; } = "error_code";
        public string ResponseDataKeyField { get; set; } = "data";
        public ResponserIniter ResponserIniter { get; set; }
        public RequestResponser Resposer { get; set; }
        public Action BeforeSend { get; set; }
        public HttpRequestType RequestType { get; private set; } = HttpRequestType.Post;

        public RequestBase(string keyInUrlMrg, HttpRequestType requestType)
        {
            RequestType = requestType;
            KeyInURLServices = keyInUrlMrg;
        }

        public void Init(HTTPDriver driver, ServiceURL services)
        {
            Driver = driver;
            Services = services;
            RevertURL();
        }

        public abstract void Send();

        protected abstract void BuildParam();

        public void Build()
        {
            BeforeSend?.Invoke();
            ResponserIniter.Build();

            BuildCallbacks();

            if (!ResponserIniter.IgnoreParamCreate)
            {
                BuildParam();
            }
            else { }
        }

        private void BuildCallbacks()
        {
            if (Resposer == default)
            {
                Resposer = new RequestResponser()
                {
                    DataKeyField = ResponseDataKeyField,
                    FailedKeyField = ResponseFailedKeyField,
                };
            }
            else { }

            Resposer.OnSuccessed = ResponserIniter.OnResponseSuccess;
            Resposer.SuccessForCustom = ResponserIniter.SuccessForCustom;
            Resposer.Failed = ResponserIniter.OnResponseFailed;
            Resposer.Error = ResponserIniter.OnErrorNet;
        }

        public void RecoverURL(string url)
        {
            ServiceURL = url;
        }

        public void RevertURL()
        {
            ServiceURL = Services.GetHttpURL(KeyInURLServices);
        }
    }

    public class Requester<T> : RequestBase, IRequesterParamer where T : ResponserIniter, new()
    {
        public Dictionary<string, string> RequestParam { get; set; }

        public Requester(string keyInUrlMrg, HttpRequestType requestType = HttpRequestType.Post) : base(keyInUrlMrg, requestType)
        {
            ResponserIniter = new T();
        }

        override public void Send()
        {
            Driver.Request(
                RequestType, 
                ServiceURL, 
                Resposer.Success, 
                Resposer.Error, 
                RequestParam,
                10,
                HeaderAPIKey
            );

            ResponserIniter.Clean();
        }

        protected override void BuildParam()
        {
            RequestParam = ResponserIniter.DicParam;
        }
    }

    public class JsonRequester<T> : RequestBase, IRequesterJsonParamer where T : ResponserIniter, new()
    {
        public bool ShowWaiting { get; set; }
        public JsonData RequestParam { get; set; }

        public JsonRequester(string keyInUrlMrg, HttpRequestType requestType = HttpRequestType.Post) : base(keyInUrlMrg, requestType)
        {
            ResponserIniter = new T();
        }

        override public void Send()
        {
            Driver.JsonDataRequest(
                RequestType, 
                ServiceURL, 
                Resposer.Success, 
                Resposer.ErrorForJson,
                RequestParam,
                10, 
                ShowWaiting, 
                Resposer.SuccessForCustom, 
                HeaderAPIKey
            );

            ResponserIniter.Clean();
        }

        protected override void BuildParam()
        {
            RequestParam = ResponserIniter.JsonParam;
        }
    }
}
