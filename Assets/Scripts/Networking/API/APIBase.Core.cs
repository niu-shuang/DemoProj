using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UniRx;
using Cysharp.Threading.Tasks;
using LitJson;

namespace DemoProj
{
    /// <summary>
    /// API通信の結果オブジェクト.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct APIResult<T>
    {
        public bool succeeded;
        public T dto;
        public ApiErrorInfo errorInfo;

        public APIResult(bool succeeded, T dto, ApiErrorInfo errorInfo) : this()
        {
            this.succeeded = succeeded;
            this.dto = dto;
            this.errorInfo = errorInfo;
        }
    }

    public abstract partial class APIBase<T>
    {
        // 空配列を表すJSON文字列.
        private const string EMPTY_ARRAY_JSON = "[]";

        // 通信パラメタ.
        private Dictionary<string, object> _postParamSet;
        private Dictionary<string, object> _getParamSet;

        protected abstract string apiName { get; }

        // 結果オブジェクト生成.
        public static APIResult<T> SucceededResult(T dto)
        {
            return new APIResult<T>(true, dto, null);
        }
        public static APIResult<T> FailedResult(ApiErrorInfo errorInfo)
        {
            return new APIResult<T>(false, default, errorInfo);
        }

        /// <summary>
        /// API通信
        /// </summary>
        /// <returns></returns>
        public async UniTask<APIResult<T>> Connect()
        {

            // HttpWebRequestで生成されるURLが正しくなるよう、"/"の個数を調整.
            var modifiedApiName = AppDefine.API_DOMAIN.EndsWith("/") ? apiName : "/" + apiName;
            var httpTask = new HttpTask(modifiedApiName);
            httpTask.SetRequestParam(_postParamSet, _getParamSet);
#if UNITY_EDITOR
            // 送信ログ.
            AddRequestLog(httpTask);
#endif
            for (int i = 0; i < allowRetryCount; i++)
            {
                // ローディングビュー表示.
                if (enableLoadingView)
                {
                    await APIConnectEvent.showLoadingViewEvent.Invoke();
                }
                var resp = await httpTask.ConnectAsync();
                // ローディングを非表示.
                if (enableLoadingView)
                {
                    await APIConnectEvent.hideLoadingViewEvent.Invoke();
                }
                if(resp.errorInfo != null)
                {
                    if(handleError)
                    {
                        resp.errorInfo.retryCount = i;
                        var doRetry = await APIConnectEvent.apiErrorHandler(resp.errorInfo);
                        if(doRetry)
                        {
                            continue;
                        }
                    }
                    return FailedResult(resp.errorInfo);
                }
                else
                {
                    if(resp.data == null || resp.data == EMPTY_ARRAY_JSON)
                    {
                        return SucceededResult(default);
                    }
#if UNITY_EDITOR
                    // 受信ログ.
                    AddResponseLogAndCache(resp);
#endif
                    // レスポンスをパースし、コールバック.
                    try
                    {
                        var dto = JsonMapper.ToObject<T>(resp.data);
                        return SucceededResult(dto);
                    }
                    catch (JsonException e)
                    {
                        Debug.LogError(e.Message + "@" + e.StackTrace);
                        ApiErrorInfo parseError = new ApiErrorInfo((int)ApiErrorDefine.ErrorStatus.Exception, "Parse Error");
                        await HandleError(parseError);
                        return FailedResult(parseError);
                    }
                }

            }
            ApiErrorInfo errorInfo = new ApiErrorInfo((int)ApiErrorDefine.ErrorStatus.Exception, "Default Error");
            return FailedResult(errorInfo);
        }

        /// <summary>
        /// 通信エラーハンドリング.
        /// </summary>
        /// <param name="errorInfo">エラー情報.</param>
        /// <param name="notifyRetry">ハンドリング完了時にこのメソッドを呼ぶ. リトライする場合true、しない場合falseを渡す.</param>
        private async UniTask<bool> HandleError(ApiErrorInfo errorInfo)
        {
            return await APIConnectEvent.apiErrorHandler.Invoke(errorInfo);
        }

        // ------------------------------------------------------------
        // 通信パラメタ設定.
        // ------------------------------------------------------------
        /// <summary>
        /// POSTパラメタを追加.
        /// </summary>
        public void AddPostParam(string key, object value)
        {
            if (_postParamSet == null) _postParamSet = new Dictionary<string, object>();
            _postParamSet.Add(key, value);
        }

        /// <summary>
        /// GETパラメタを追加.
        /// </summary>
        public void AddGetParam(string key, object value)
        {
            if (_getParamSet == null) _getParamSet = new Dictionary<string, object>();

            // GETパラメタはV-CoDe内でobjectをToStringで文字列変換しているが、
            // これだとArrayなどに対応できないため、コレクション型が渡された場合はJSONパースを行う.
            if (value is IList || value is IDictionary)
            {
                _getParamSet.Add(key, JsonMapper.ToJson(value));
            }
            else
            {
                _getParamSet.Add(key, value);
            }
        }

        /// <summary>
        /// POSTパラメタをオブジェクトでまとめて追加.
        /// </summary>
        public void AddPostParamSet(object paramObj)
        {
            AddParamSetFromObject(paramObj, true);
        }

        /// <summary>
        /// GETパラメタをオブジェクトでまとめて追加.
        /// </summary>
        public void AddGetParamSet(object paramObj)
        {
            AddParamSetFromObject(paramObj, false);
        }

        /// <summary>
        /// 任意オブジェクトの「変数名：値」をDictionaryに格納.
        /// </summary>
        private void AddParamSetFromObject(object paramObj, bool isPostParam)
        {
            var type = paramObj.GetType();

            foreach (var prop in type.GetProperties())
            {
                var key = prop.Name;
                var value = prop.GetValue(paramObj, null);

                if (isPostParam) AddPostParam(key, value);
                else AddGetParam(key, value);
            }
            foreach (FieldInfo field in type.GetFields())
            {
                var key = field.Name;
                var value = field.GetValue(paramObj);

                if (isPostParam) AddPostParam(key, value);
                else AddGetParam(key, value);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 受信ログをJsonモニターに表示.
        /// </summary>
        private void AddResponseLogAndCache(ApiResponseInfo response)
        {
            if (response == null) return;

            Observable.TimerFrame(3).Subscribe(_ =>
            {
                APILogNotifier.NotifyResponse(response.url, response.data, false);
            });
            // 通信結果をキャッシュ.
            APISimulationInfo.CacheJson(apiName, response.data);
        }

        private void AddRequestLog(HttpTask request)
        {
            var requestInfo = request.apiRequestInfo;
            var fullURL = AppDefine.API_DOMAIN + requestInfo.url + requestInfo.GetParameterToString();
            APILogNotifier.NotifyRequest(fullURL, requestInfo.postDataMap, false);
        }
#endif
    }
}