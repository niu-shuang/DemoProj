using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using J;

namespace DemoProj
{
    public class UploadTask : HttpTask
    {
        public string path { get; private set; }
        public UploadTask(string url, string path) : base(url)
        {
            this.path = path;
        }

        public async UniTask<ApiResponseInfo> UploadAsync(DividableProgress progress = null)
        {
            var binary = await Utilities.LoadBinaryAsync(path, progress?.Divide(.2f));

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
            form.AddBinaryData("File", binary);
            var webRequest = UnityWebRequest.Post(url, form);
            webRequest.timeout = AppDefine.API_TIMEOUT / 1000;
            var req = await webRequest.SendAsObservable(progress?.DivideRest()).ToUniTask();
            GenerateRespInfo(req);
            //レスポンスでポストデータがある場合(この時点ではdataにレスポンスJsonが全部入り)
            if (!string.IsNullOrEmpty(apiResponseInfo.data))
            {
                //コア共通パラメータをチェック(明示的に参照渡し)
                ReceptCommonPostParam(ref apiResponseInfo);
            }
            webRequest.Dispose();
            return apiResponseInfo;
        }
    }
}

