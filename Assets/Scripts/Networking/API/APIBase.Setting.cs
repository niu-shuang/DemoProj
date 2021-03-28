namespace DemoProj
{
    public abstract partial class APIBase<T>
    {
        // 通信タイムアウト時のリトライ回数.
        protected virtual int allowRetryCount => 3;

        // デフォルトのPOST,GETパラメタを送信するかどうか.
        protected virtual bool sendDefaultParam => true;

        // 通信中にローディングを表示するかどうか.
        protected virtual bool enableLoadingView => true;

        // 通信エラーのハンドリングを行うかどうか.
        protected virtual bool handleError => true;
    }
}
