#if UNITY_EDITOR

using System;

namespace DemoProj
{
    /// <summary>
    /// APIデバッグウィンドウに情報を通知.
    /// ゲーム側のアセンブリはEditorのアセンブリに直接アクセスができないので、
    /// コールバックを使ってEditorアセンブリ側からアクセスしてもらう必要がある.
    /// </summary>
    /// <author>y-kawasaki</author>
    public static class APILogNotifier
    {
        // コールバック. APILogEditorから登録.
        public static event Action<string, object, bool> onRequest;
        public static event Action<string, object, bool> onResponse;

        /// <summary>
        /// 送信データ通知.
        /// </summary>
        public static void NotifyRequest(string url, object postParams, bool isDummy)
        {
            if (onRequest != null) onRequest(url, postParams, isDummy);
        }

        /// <summary>
        /// 受信データ通知.
        /// </summary>
        public static void NotifyResponse(string url, object data, bool isDummy)
        {
            if (onResponse != null) onResponse(url, data, isDummy);
        }
    }
}

#endif
