using Cysharp.Threading.Tasks;
using UniRx;

namespace DemoProj
{
    public static class APIConnectEvent
    {
        public static AsyncEvent showLoadingViewEvent;
        public static AsyncEvent hideLoadingViewEvent;
        public static ApiErrorHandler apiErrorHandler;

        /// <summary>
        /// retryするかどうかを返す非同期イベント
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public delegate UniTask<bool> ApiErrorHandler(ApiErrorInfo info);

        /// <summary>
        /// 汎用的非同期イベント
        /// </summary>
        /// <returns></returns>
        public delegate UniTask<Unit> AsyncEvent();
    }
}
