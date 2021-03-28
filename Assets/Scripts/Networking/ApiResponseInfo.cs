namespace DemoProj
{
    public class ApiResponseInfo
    {
        public bool result;         //結果（true:成功　false:失敗）
        public string url;          //送信先URL
        public string data;         //受信Jsonデータ
        public ApiErrorInfo errorInfo;  //エラー情報

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public ApiResponseInfo()
        {
            //初期化
            url = "";
            data = "";
            result = false;
        }

    }
}