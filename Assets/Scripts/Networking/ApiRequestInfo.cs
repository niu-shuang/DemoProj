using UnityEngine;
using System.Collections;
using MiniJSON;
using System.Collections.Generic;

namespace DemoProj
{

    public class ApiRequestInfo
    {
        public string url;          //送信先URL
        public string aesKey;
        public string uid;
        public Dictionary<string, object> postDataMap;//送信POSTパラメータ
        public Dictionary<string, object> getDataMap;   //送信GETパラメータ
        public bool isPlain;        //コア支援(Domain付与/Post暗号化/レスポンス形成/リトライ支援etc)無しフラグ
                                    /// <summary>
                                    /// コンストラクタ.
                                    /// </summary>
        public ApiRequestInfo()
        {
            //初期化
            url = "";
            aesKey = "";
            uid = "";
            getDataMap = null;
            postDataMap = null;
            isPlain = false;
        }

        /// <summary>
        /// Getパラメータ文字列の生成.
        /// </summary>
        public string GetParameterToString()
        {
            //Getパラメータ配列がnullなら空値を返却
            if (null == getDataMap)
            {
                return "";
            }
            string param = "?";
            foreach (KeyValuePair<string, object> kvp in getDataMap)
            {
                param = string.Format("{0}{1}={2}&", param, kvp.Key, kvp.Value.ToString());
            }
            //ケツの&を省いて返却
            return param.Substring(0, param.Length - 1);
        }

        /// <summary>
        /// PostパラメータJson文字列の生成.
        /// </summary>
        public string PostParameterToJson()
        {
            string json = "";
            if (postDataMap != null)
            {
                try
                {
                    //MiniJsonを利用したシリアライズ
                    json = Json.Serialize(postDataMap);
                }
                catch
                {
                    //シリアライズに失敗した場合は空値を返却
                    Debug.LogError("PostParameterToJson Serialize Error...");
                }
            }
            return json;
        }
    }
}
