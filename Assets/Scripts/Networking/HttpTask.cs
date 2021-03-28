using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace DemoProj
{
    public class HttpTask
    {
        private const string UID_KEY = "UID";
        public string url { get; protected set; }
        public ApiRequestInfo apiRequestInfo;
        public ApiResponseInfo apiResponseInfo;

        private Regex regex = new Regex(AppDefine.JSON_REGEX);

        public HttpTask(string url)
        {
            apiRequestInfo = new ApiRequestInfo();
            apiRequestInfo.url = url;
            apiRequestInfo.uid = PlayerPrefs.GetString(UID_KEY);
            AddCommonPostParam();
            AddCommonGetParam();
        }

        /// <summary>
        /// ボルテージ共通のパラメータを追加
        /// </summary>
        private void AddCommonPostParam()
        {
            if (apiRequestInfo.postDataMap == null)
            {
                apiRequestInfo.postDataMap = new Dictionary<string, object>();
            }
            //コア共通POSTパラメータの追加
            apiRequestInfo.postDataMap["uid"] = PlayerPrefs.GetString(UID_KEY);
            
        }

        private void AddCommonGetParam()
        {
            if (apiRequestInfo.getDataMap == null)
            {
                apiRequestInfo.getDataMap = new Dictionary<string, object>();
            }
            //コア共通GETパラメータの追加
            apiRequestInfo.getDataMap["uid"] = PlayerPrefs.GetString(UID_KEY);
        }

        public void SetRequestParam(Dictionary<string, object> postDataMap, Dictionary<string, object> getDataMap)
        {
            foreach (var item in postDataMap)
            {
                apiRequestInfo.postDataMap[item.Key] = item.Value;
            }
            foreach (var item in getDataMap)
            {
                apiRequestInfo.getDataMap[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// postデータをformに入れる
        /// </summary>
        /// <param name="enableCrypt"></param>
        /// <returns></returns>
        protected WWWForm CreateForm()
        {
            WWWForm form = new WWWForm();
            form.AddField("param", apiRequestInfo.PostParameterToJson());
            form.AddField("uid", apiRequestInfo.uid);
            return form;
        }

        public virtual async UniTask<ApiResponseInfo> ConnectAsync()
        {
            string paramString = apiRequestInfo.url + apiRequestInfo.GetParameterToString();
            if (AppDefine.API_DOMAIN.EndsWith("/") && paramString.StartsWith("/"))
            {
                url = AppDefine.API_DOMAIN + paramString.Substring(1);
            }
            else if (AppDefine.API_DOMAIN.EndsWith("/") || paramString.StartsWith("/"))
            {
                url = AppDefine.API_DOMAIN + paramString;
            }
            else
            {
                url = AppDefine.API_DOMAIN + "/" + paramString;
            }
            WWWForm form = null;
            if (apiRequestInfo.postDataMap != null)
            {
                Debug.Log("SendParams : " + apiRequestInfo.PostParameterToJson());
                form = CreateForm();
            }

            var webRequest = UnityWebRequest.Post(url, form);
            webRequest.timeout = AppDefine.API_TIMEOUT / 1000;
            var req = await webRequest.SendWebRequest();

            GenerateRespInfo(req);
            //レスポンスでポストデータがある場合(この時点ではdataにレスポンスJsonが全部入り)
            if (apiResponseInfo.errorInfo == null && !string.IsNullOrEmpty(apiResponseInfo.data))
            {
                //コア共通パラメータをチェック(明示的に参照渡し)
                ReceptCommonPostParam(ref apiResponseInfo);
            }
            webRequest.Dispose();
            return apiResponseInfo;
        }


        /// <summary>
        /// 結果処理
        /// </summary>
        /// <param name="req"></param>
        protected void GenerateRespInfo(UnityWebRequest req)
        {
            apiResponseInfo = new ApiResponseInfo();
            apiResponseInfo.url = url;
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error);
                int statusCode;
                try
                {
                    //エラー文字の先頭にエラーコードがあるので、その部分のみ切り出す.
                    statusCode = int.Parse(req.error.Split(' ')[0]);
                }
                catch (Exception)
                {
                    Debug.LogError(req.error);
                    statusCode = 9999;
                }
                apiResponseInfo.errorInfo = new ApiErrorInfo(statusCode, req.error);
            }
            else
            {
                string resp = req.downloadHandler.text;
                Debug.Log("ResponseParams : " + resp);

                //通信結果がJSON形式か判定（Plain指定の時はチェックすらしない）.
                if (regex.IsMatch(resp) || apiRequestInfo.isPlain)
                {
                    //レスポンスに格納
                    apiResponseInfo.data = resp;
                }
                else
                {
                    Debug.LogError("Response data is regex not match !");
                }
            }
        }

        /// <summary>
		/// コア共通パラメータ受け取り.
		/// </summary>
		protected void ReceptCommonPostParam(ref ApiResponseInfo apiResponseInfo)
        {

            //Json受け取り
            string postJson = apiResponseInfo.data;

            //返却初期値
            apiResponseInfo.result = false;

            //Exceptionが発生していた場合はパラメータの展開はしない
            if (apiResponseInfo.errorInfo.errorStatus == ApiErrorDefine.ErrorStatus.None)
            {

                //送信パラメータ配列宣言
                Dictionary<string, object> respDic = new Dictionary<string, object>();

                //Json形式チェックとコンテンツ依存パラメータのパース
                if (postJson != null)
                {
                    if (regex.IsMatch(postJson))
                    {
                        respDic = MiniJSON.Json.Deserialize(postJson) as Dictionary<string, object>;
                    }
                    else
                    {
                        Debug.LogError("PostParam_type is not json matching !!");
                    }
                }

                //dataを詰める
                if (respDic.ContainsKey("data"))
                {
                    apiResponseInfo.data = MiniJSON.Json.Serialize(respDic["data"]);
                }

                //レスポンスとしてAPI結果フラグがあれば詰める
                if (respDic.ContainsKey(AppDefine.POST_KEY_RESULT))
                {
                    string value = respDic[AppDefine.POST_KEY_RESULT].ToString();
                    int intRes = 0;
                    if (int.TryParse(value, out intRes))
                    {
                        // レスポンスが数値形式の場合は、1をtrueとして判定.
                        apiResponseInfo.result = (intRes == 1);
                    }
                    else
                    {
                        apiResponseInfo.result = Boolean.Parse(value);
                    }
                }
            }
        }

    }
}

