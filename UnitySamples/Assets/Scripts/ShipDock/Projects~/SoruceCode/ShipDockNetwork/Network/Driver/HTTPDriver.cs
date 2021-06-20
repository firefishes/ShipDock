
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using HTTPParam = System.Collections.Generic.Dictionary<string, string>;
using OnError = UnityEngine.Events.UnityAction<int, string, string, LitJson.JsonData>;
using OnErrorResponse = UnityEngine.Events.UnityAction<int, string, string, System.Collections.Generic.Dictionary<string, string>>;
using OnErrorResponseJSON = UnityEngine.Events.UnityAction<int, string, string, LitJson.JsonData>;
using OnSuccess = UnityEngine.Events.UnityAction<string>;

namespace ShipDock.Network
{
    /// <summary>
    /// 
    /// HTTP请求驱动器
    /// 
    /// author ShaoBo.liu
    /// modify Hua.chen
    /// refactoring Minghua.ji
    /// 
    /// </summary>
    public class HTTPDriver
    {
        #region 静态常量
        private const string EMPTY_POST_PARAM = "{}";

        private static string appCode;
        private static string publicKey;

        public static Action<bool, Action> StartUpdater { get; set; }
        #endregion

        #region 字段成员
        protected bool mIsNetWorking = false;
        protected bool mIsNetWorkingNoWaiting = false;
        protected bool mIsRequestInfoListRuning = true;
        protected List<string> mURLRequestBasic;
        protected Dictionary<string, int> mHeaderAPIs;
        protected List<HTTPJsonRequestInfo> mRequestInfoList;//客户端请求队列
        protected List<HTTPJsonRequestInfo> mRequestInfosNoWaiting;//客户端请求队列(不需要转圈的)

        public ServiceURL Services { get; protected set; }
        public Action<bool> onLoadingAlert { get; set; }
        public Func<string> OnGetTokenKey { get; set; }
        public Action<string> onNetError { get; set; }
        public Action<string> onServiceError { get; set; }
        public MonoBehaviour ComponentOnwer { get; set; }
        #endregion

        #region 初始化
        public HTTPDriver()
        {
            "log".Log("Http driver singleton inited.");
            mRequestInfoList = new List<HTTPJsonRequestInfo>();
            mRequestInfosNoWaiting = new List<HTTPJsonRequestInfo>();

            StartUpdater?.Invoke(true, Update);
        }

        public void SetPublicKey(string value)
        {
            publicKey = value;
        }

        public void SetAppCode(string value)
        {
            appCode = value;
        }

        public void SetServicURLs(ServiceURL services)
        {
            Services = services;
        }
        #endregion

        #region 销毁
        public virtual void Clean()
        {
            StartUpdater?.Invoke(false, Update);

            onLoadingAlert = default;
            StartUpdater = default;
            ComponentOnwer = default;
            OnGetTokenKey = default;
        }
        #endregion

        #region 外部常用接口
        /// <summary>
        /// 发送普通请求（无等待环节）
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="requestURL"></param>
        /// <param name="successResponse"></param>
        /// <param name="errorResponse"></param>
        /// <param name="data"></param>
        /// <param name="timeOut"></param>
        /// <param name="headerAPI"></param>
        /// <returns></returns>
        public bool Request(HttpRequestType requestType, string requestURL, OnSuccess successResponse, OnErrorResponse errorResponse, HTTPParam data = default, int timeOut = 10, string headerAPI = "")
        {
            bool allowSend = !mIsNetWorking;
            if (allowSend)
            {
                allowSend = IsRequestValid(ref requestURL, requestType, ref data);
                if (allowSend)
                {
                    IEnumerator enumerator = SendRequest(requestType, requestURL, successResponse, errorResponse, data, timeOut, headerAPI);
                    ComponentOnwer.StartCoroutine(enumerator);
                }
                else { }
            }
            else { }

            return allowSend;
        }

        private IEnumerator SendRequest(HttpRequestType requestType, string requestURL, OnSuccess successResponse, OnErrorResponse errorResponse, HTTPParam data = default, int timeOut = 10, string headerAPIValue = "")
        {
            string debug = string.Empty;
            DateTime debugTime = DateTime.UtcNow;

            LogBeforeSend(ref debug, ref requestType, ref requestURL, ref data);

            UnityWebRequest engine = CreateWebReqeust(requestType, ref requestURL, ref data, timeOut);
            InitAndSetHeaderAPI(ref engine, headerAPIValue);//标记HTTP请求头部数据，区分发送同一个请求的不同业务

            mIsNetWorking = true;
            yield return engine.SendWebRequest();

            mIsNetWorking = false;

            LogTimeUsed(ref debug, ref engine, debugTime);

            string headerAPI = engine.GetRequestHeader("api");
            CheckRequestError(ref engine, ref debug, out int errorStatu);

            Responsed(errorStatu, ref engine, successResponse, errorResponse, ref data, ref requestURL, ref debug);

            engine.Dispose();
            RemoveHeaderAPI(ref headerAPI);
        }

        /// <summary>
        /// 发送以Json对象为参数的请求
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="requestURL"></param>
        /// <param name="successResponse"></param>
        /// <param name="errorResponse"></param>
        /// <param name="data"></param>
        /// <param name="timeOut"></param>
        /// <param name="showWaiting"></param>
        /// <param name="callback"></param>
        /// <param name="apiValue"></param>
        public void JsonDataRequest(
            HttpRequestType requestType, string requestURL, OnSuccess successResponse, OnError errorResponse, JsonData data = default,
            int timeOut = 10, bool showWaiting = true, OnSuccess callback = default, string apiValue = "")
        {
            HTTPJsonRequestInfo info = new HTTPJsonRequestInfo
            {
                requestType = requestType,
                requestURL = requestURL,
                successResponse = successResponse,
                errorResponse = errorResponse,
                data = data,
                timeOut = timeOut,
                showWaiting = showWaiting,
                callback = callback,
                headerAPI = apiValue
            };

            if (showWaiting)
            {
                mRequestInfoList.Add(info);
            }
            else
            {
                mRequestInfosNoWaiting.Add(info);
            }
        }

        /// <summary>
        /// 以构建信息项的方式创建JSON参数的网络请求，并加入网络请求的队列（无等待环节）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool StartRequestNoWaiting(HTTPJsonRequestInfo info)
        {
            bool allowSend = !mIsNetWorkingNoWaiting;
            if (allowSend)
            {
                allowSend = IsRequestValid(ref info, ref mRequestInfosNoWaiting);
                if (allowSend)
                {
                    IEnumerator enumerator = SendRequestNoWaiting(info);
                    ComponentOnwer.StartCoroutine(enumerator);
                }
                else { }
            }
            else { }

            return allowSend;
        }

        /// <summary>
        /// 以构建信息项的方式创建JSON参数的网络请求，并加入网络请求的队列（附带等待环节）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool StartRequestWaiting(HTTPJsonRequestInfo info)
        {
            bool allowSend = !mIsNetWorking;
            if (allowSend)
            {
                allowSend = IsRequestValid(ref info, ref mRequestInfoList);
                if (allowSend)
                {
                    IEnumerator enumerator = SendRequestWaiting(info);
                    ComponentOnwer.StartCoroutine(enumerator);
                }
                else { }
            }
            else { }

            return allowSend;
        }
        #endregion

        #region 检测网络请求队列
        /// <summary>
        /// 检测需要判断是否带有阻塞界面的请求
        /// </summary>
        private void Update()
        {
            HTTPJsonRequestInfo item;
            if (ShouldRequestFromList(ref mRequestInfosNoWaiting, out item))
            {
                StartRequestNoWaiting(item);
            }
            else { }

            if (mIsRequestInfoListRuning)
            {
                if (ShouldRequestFromList(ref mRequestInfoList, out item))
                {
                    StartRequestWaiting(item);
                }
                else { }
            }
            else { }
        }

        private bool ShouldRequestFromList(ref List<HTTPJsonRequestInfo> list, out HTTPJsonRequestInfo item)
        {
            bool result = list.Count > 0;

            item = default;
            if (result)
            {
                item = list[0];
                result = item != default;
                if (!result)
                {
                    list.RemoveAt(0);//移除无效的请求信息，有效的请求会在发送完成后移除
                }
                else { }
            }
            else { }

            return result;
        }

        private bool IsRequestValid(ref string requestURL, HttpRequestType type, ref Dictionary<string, string> data)
        {
            bool allowSend = true;
            switch (type)
            {
                case HttpRequestType.Post:
                    allowSend = data != default && data.Count > 0;

                    if (!allowSend)
                    {
                        Debug.LogError("使用Http的Post方式（DICTIONARY）请求服务器，表单数据不能为空！URL: ".Append(requestURL));
                    }
                    else { }
                    break;
            }
            return allowSend;
        }

        private bool IsRequestValid(ref HTTPJsonRequestInfo info, ref List<HTTPJsonRequestInfo> list)
        {
            bool allowSend = true;
            switch (info.requestType)
            {
                case HttpRequestType.Post:
                    allowSend = info.data != default;

                    if (!allowSend)
                    {
                        Debug.LogError("使用Http的Post方式（JSON）请求服务器，表单数据不能为空！URL: ".Append(info.requestURL));
                        list?.RemoveAt(0);
                    }
                    else { }
                    break;
            }
            return allowSend;
        }

        /// <summary>
        /// 发送需阻塞界面的请求
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private IEnumerator SendRequestWaiting(HTTPJsonRequestInfo info)
        {
            string debug = string.Empty;
            DateTime debugTime = DateTime.UtcNow;

            onLoadingAlert?.Invoke(true);
            BeforeRequest(ref info, out string requestURL, out JsonData data, out UnityWebRequest engine, ref debug);

            mIsNetWorking = true;
            yield return engine.SendWebRequest();

            mIsNetWorking = false;
            mRequestInfoList.RemoveAt(0);

            AfterRequest(ref info, ref requestURL, ref data, ref engine, debugTime, ref debug);
            onLoadingAlert?.Invoke(false);
        }

        /// <summary>
        /// 发送无需阻塞界面的请求
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private IEnumerator SendRequestNoWaiting(HTTPJsonRequestInfo info)
        {
            string debug = string.Empty;
            DateTime debugTime = DateTime.UtcNow;

            BeforeRequest(ref info, out string requestURL, out JsonData data, out UnityWebRequest engine, ref debug);

            mIsNetWorkingNoWaiting = true;
            yield return engine.SendWebRequest();

            mIsNetWorkingNoWaiting = false;
            mRequestInfosNoWaiting.RemoveAt(0);

            AfterRequest(ref info, ref requestURL, ref data, ref engine, debugTime, ref debug);
        }
        #endregion

        #region 创建网络请求
        private void InitURLRequestBasic()
        {
            if (mURLRequestBasic == default)
            {
                mURLRequestBasic = new List<string>()
                {
                    Services.GetHttpURL("user_find"),
                    Services.GetHttpURL("user_register"),
                    Services.GetHttpURL("versionInfo"),
                    "sms/api/send/verifyCode",
                };
            }
            else { }
        }

        private UnityWebRequest CreateWebReqeust(HttpRequestType requestType, ref string requestURL, ref HTTPParam data, int timeOut)
        {
            UnityWebRequest engine = default;
            switch (requestType)
            {
                case HttpRequestType.Get:

                    string GETParam = HttpUtility.GetOriginalDataString(data);
                    engine = CreateRequesterGET(ref requestURL, ref GETParam);
                    break;

                case HttpRequestType.Post:

                    InitURLRequestBasic();
                    engine = CreateRequesterPOST(ref requestURL, ref data);
                    break;
            }
            if (engine != default)
            {
                engine.timeout = timeOut;
            }
            else { }
            return engine;
        }

        private UnityWebRequest CreateWebReqeust(HttpRequestType requestType, ref string requestURL, ref JsonData data, int timeOut)
        {
            UnityWebRequest engine = default;
            switch (requestType)
            {
                case HttpRequestType.Get:

                    string GETParam = JsonMapper.ToJson(data);
                    engine = CreateRequesterGET(ref requestURL, ref GETParam);
                    break;

                case HttpRequestType.Post:
                    InitURLRequestBasic();
                    engine = CreateRequesterPOSTByJSON(ref requestURL, ref data);
                    break;
            }
            if (engine != default)
            {
                engine.timeout = timeOut;
            }
            else { }
            return engine;
        }

        private UnityWebRequest CreateRequesterGET(ref string requestURL, ref string GETParam)
        {
            if (!string.IsNullOrEmpty(GETParam))
            {
                requestURL = requestURL.Append("?", GETParam);
            }
            else { }

            UnityWebRequest engine = UnityWebRequest.Get(requestURL);
            engine.certificateHandler = new HTTPCertificate();
            engine.useHttpContinue = false;

            return engine;
        }

        private UnityWebRequest CreateRequesterPOST(ref string requestURL, ref HTTPParam data)
        {
            UnityWebRequest engine;
            bool isBasicAuthorization = mURLRequestBasic.Contains(requestURL);
            bool isUserTokenURL = requestURL == Services.GetHttpURL("user_token");
            if (isBasicAuthorization || isUserTokenURL)
            {
                if (isBasicAuthorization)
                {
                    engine = CreateRequestByDictionary(requestURL, data);
                    engine.SetRequestHeader("Content-Type", "application/json");
                }
                else
                {
                    engine = CreateRequestByWWWForm(requestURL, data);
                    engine.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                }
                engine.SetRequestHeader("Authorization", "Basic ".Append(appCode));
            }
            else
            {
                engine = CreateRequestByDictionary(requestURL, data);
                engine.SetRequestHeader("Content-Type", "application/json");
                engine.SetRequestHeader("Authorization", "Bearer ".Append(OnGetTokenKey?.Invoke()));
            }
            return engine;
        }

        private UnityWebRequest CreateRequesterPOSTByJSON(ref string requestURL, ref JsonData data)
        {
            UnityWebRequest engine = CreateRequestByJson(requestURL, data);
            engine.SetRequestHeader("Content-Type", "application/json");

            if (mURLRequestBasic.Contains(requestURL))
            {
                engine.SetRequestHeader("Authorization", "Basic ".Append(appCode));
            }
            else
            {
                engine.SetRequestHeader("Authorization", "Bearer ".Append(OnGetTokenKey?.Invoke()));
            }
            return engine;
        }

        private UnityWebRequest CreateRequestByDictionary(string requestURL, Dictionary<string, string> data)
        {
            string textParam = JsonMapper.ToJson(data);
            RSAEncrypt(ref textParam, out byte[] bytesParam);
            BuildRequest(out UnityWebRequest request, ref textParam, ref bytesParam, ref requestURL, true);
            return request;
        }

        private UnityWebRequest CreateRequestByJson(string requestURL, JsonData data)
        {
            string textParam = JsonMapper.ToJson(data);
            RSAEncrypt(ref textParam, out byte[] bytesParam);
            BuildRequest(out UnityWebRequest request, ref textParam, ref bytesParam, ref requestURL, true);
            return request;
        }

        private UnityWebRequest CreateRequestByWWWForm(string requestURL, Dictionary<string, string> data)
        {
            int max = data.Count;

            WWWForm postData = new WWWForm();
            KeyValuePair<string, string> current;
            Dictionary<string, string>.Enumerator enumer = data.GetEnumerator();
            for (int i = 0; i < max; i++)
            {
                enumer.MoveNext();
                current = enumer.Current;
                postData.AddField(current.Key, current.Value);
            }

            string textParam = Encoding.UTF8.GetString(postData.data);

            RSAEncrypt(ref textParam, out byte[] bytesParam);
            BuildRequest(out UnityWebRequest request, ref textParam, ref bytesParam, ref requestURL, true);
            return request;
        }

        private void RSAEncrypt(ref string source, out byte[] dataByte)
        {
            dataByte = default;
            try
            {
                string str = StaticFunction.Instance.RSAEncrypt(source, publicKey);
                dataByte = Encoding.UTF8.GetBytes(str);
            }
            catch
            {
                dataByte = Encoding.UTF8.GetBytes(source);
            }
        }

        private void BuildRequest(out UnityWebRequest request, ref string textParam, ref byte[] bytesParam, ref string requestURL, bool isPOST)
        {
            string httpVerb = isPOST ? UnityWebRequest.kHttpVerbPOST : UnityWebRequest.kHttpVerbGET;
            request = new UnityWebRequest(requestURL, httpVerb);
            if (textParam == EMPTY_POST_PARAM)
            {
                request.uploadHandler = default;
            }
            else
            {
                request.uploadHandler = new UploadHandlerRaw(bytesParam);
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.certificateHandler = new HTTPCertificate();
            request.useHttpContinue = false;
        }
        #endregion

        #region 处理请求
        private void BeforeRequest(ref HTTPJsonRequestInfo info, out string requestURL, out JsonData data, out UnityWebRequest engine, ref string debug)
        {
            data = info.data;
            requestURL = info.requestURL;
            HttpRequestType requestType = info.requestType;

            LogBeforeSend(ref debug, ref requestType, ref requestURL, ref data);

            engine = CreateWebReqeust(requestType, ref requestURL, ref data, info.timeOut);

            InitAndSetHeaderAPI(ref engine, info.headerAPI);//标记HTTP请求头部数据，区分发送同一个请求的不同业务
        }

        private void CheckRequestError(ref UnityWebRequest engine, ref string debug, out int errorStatu)
        {
            errorStatu = 0;

            bool netError = engine.isNetworkError;
            bool httpError = engine.isHttpError;
            if (netError)
            {
                errorStatu = 1;//网络错误
                LogNetError(ref debug, ref engine);
            }
            else
            {
                if (httpError)
                {
                    errorStatu = 2;//服务端请求错误
                    LogServiceError(ref debug, ref engine);
                }
                else { }
            }
        }

        private void Responsed(int errorStatu, ref UnityWebRequest engine, OnSuccess successResponse, OnSuccess callback, OnErrorResponseJSON errorResponse, ref JsonData data, ref string requestURL, ref string debug)
        {
            switch (errorStatu)
            {
                case 0://成功获取服务端返回值
                    string retData = engine.downloadHandler.text;
                    LogResponsed(ref debug, ref retData);
                    successResponse?.Invoke(retData);

                    callback?.Invoke(retData);//定制的回调函数
                    break;
                case 1://相应网络错误
                    errorResponse?.Invoke(0, engine.error, requestURL, data);
                    break;
                case 2://相应服务端报错
                    errorResponse?.Invoke(1, engine.error, requestURL, data);
                    break;
            }
        }

        private void Responsed(int errorStatu, ref UnityWebRequest engine, OnSuccess successResponse, OnErrorResponse errorResponse, ref HTTPParam data, ref string requestURL, ref string debug)
        {
            switch (errorStatu)
            {
                case 0://成功获取服务端返回值
                    string response = engine.downloadHandler.text;
                    LogResponsed(ref debug, ref response);
                    successResponse?.Invoke(response);
                    break;
                case 1://响应网络错误
                case 2://响应服务端报错
                    int statu = errorStatu - 1;
                    errorResponse?.Invoke(statu, engine.error, requestURL, data);
                    break;
            }
        }

        private void AfterRequest(ref HTTPJsonRequestInfo info, ref string requestURL, ref JsonData data, ref UnityWebRequest engine, DateTime debugTime, ref string debug)
        {
            LogTimeUsed(ref debug, ref engine, debugTime);
            string headerAPI = engine.GetRequestHeader("api");
            CheckRequestError(ref engine, ref debug, out int errorStatu);

            Responsed(errorStatu, ref engine, info.successResponse, info.callback, info.errorResponse, ref data, ref requestURL, ref debug);

            engine.Dispose();
            RemoveHeaderAPI(ref headerAPI);
        }
        #endregion

        #region 自定义HTTP消息头相关
        private void InitAndSetHeaderAPI(ref UnityWebRequest engine, string apiValue)
        {
            if (string.IsNullOrEmpty(apiValue))
            {
                return;
            }
            else { }

            engine.SetRequestHeader("api", apiValue);

            if (!string.IsNullOrEmpty(apiValue))
            {
                if (mHeaderAPIs == default)
                {
                    mHeaderAPIs = new Dictionary<string, int>();
                }
                else { }

                int callCount = 0;
                if (mHeaderAPIs.ContainsKey(apiValue))
                {
                    callCount = mHeaderAPIs[apiValue];
                    callCount++;
                }
                else
                {
                    callCount = 1;
                }
                mHeaderAPIs[apiValue] = callCount;
            }
        }

        public bool HasHeaderAPI(string APIValue)
        {
            return mHeaderAPIs.ContainsKey(APIValue) && mHeaderAPIs[APIValue] > 0;
        }

        private void RemoveHeaderAPI(ref string headerApi)
        {
            if (!string.IsNullOrEmpty(headerApi))
            {
                mHeaderAPIs[headerApi] -= 1;
            }
            else { }
        }
        #endregion

        #region 日志相关
        private void LogResponsed(ref string debug, ref string result)
        {
            "log:请求成功: {0}, 服务端返回值：{1}".Log(debug, result);
        }

        private void LogServiceError(ref string debug, ref UnityWebRequest engine)
        {
            "error:服务端报错（{0}） UnityWebRequest error: {1}, 详情：{2}".Log(engine.responseCode.ToString(), engine.error.ToString(), debug);
        }

        private void LogNetError(ref string debug, ref UnityWebRequest engine)
        {
            "error:网络错误：{0}， 详情：{1}".Log(engine.error.ToString(), debug);
            "error".Log(engine.error == "Cannot connect to destination host", "无法连接到网络！请检查网络连接设置！");
            "error".Log(engine.error == "Request timeout", "网络连接超时！");
        }

        private void LogTimeUsed(ref string debug, ref UnityWebRequest engine, DateTime debugTime)
        {
            "log:URL请求：{0} 消耗时间：{1}".Log(engine.url, ((DateTime.UtcNow - debugTime).TotalMilliseconds / 1000).ToString());
        }

        private void LogBeforeSend(ref string debug, ref HttpRequestType requestType, ref string requestURL, ref JsonData data)
        {
            debug = "URL：".Append(requestURL, " 数据：", JsonMapper.ToJson(data), " 请求类型：", requestType.ToString().ToUpper());
            "log:客户端即将发送请求，{0}".Log(debug);
        }

        private void LogBeforeSend(ref string debug, ref HttpRequestType requestType, ref string requestURL, ref HTTPParam data)
        {
            debug = "URL：".Append(requestURL, " 数据：", JsonMapper.ToJson(data), " 请求类型：", requestType.ToString().ToUpper());
            "log:客户端即将发送请求，{0}".Log(debug);
        }
        #endregion
    }
}
