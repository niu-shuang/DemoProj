using Cysharp.Threading.Tasks;
using System;
using System.IO;
using J;
using UnityEngine;
using UnityEngine.Networking;

namespace DemoProj
{
    public class DownloadTask
    {
        public string Url { get; private set; }
        public string ETag { get; private set; }
        public string SavePath { get; private set; }
        public string TempPath { get; private set; }
        public bool isHeadFetched { get; private set; }
        public bool isDownloaded { get; private set; }
        public long? Size { get; private set; }

        public Action<CallbackParam> BeforeSave { get; set; }
        public Action<CallbackParam> AfterSave { get; set; }

        public DownloadTask(string url, string savePath, string tempPath = null, string eTag = null)
        {
            Url = url;
            SavePath = savePath;
            TempPath = TempPath;
            ETag = eTag;
        }

        /// <summary>
        /// 通常のダウンロード、ダウンロード成功か否かを返却
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async UniTask<bool> DownloadAsync(DividableProgress progress = null)
        {
            bool suc = false;
            try
            {
                await DoFetch(progress?.Divide(.2f));
                await DoDownload(progress?.DivideRest());
                suc = true;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
                suc = false;
            }

            return suc;
        }

        /// <summary>
        /// FileDownloaderを使わずメモリでバイナリデータを受け取るタスク
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async UniTask<byte[]> DownloadWithoutSave(DividableProgress progress = null)
        {
            var request = new UnityWebRequest(Url);
            try
            {
                var req = await request.SendAsObservable(progress).ToUniTask();
                return req.downloadHandler.data;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// fetchしてダウンロード必要かどうかを返す
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async UniTask<long?> DoFetch(DividableProgress progress = null)
        {
            if (isHeadFetched)
            {
                progress?.Report(1f);
                return Size;
            }

            var req = await UnityWebRequest.Head(Url).SendAsObservable(progress).ToUniTask();
            isHeadFetched = true;
            if(req != null)
            {
                Size = req.GetContentLengthNum();
                if(req.GetETag() == ETag && File.Exists(SavePath))
                {
                    isDownloaded = true;
                }
            }
            progress?.Report(1f);
            return Size;
        }

        /// <summary>
        /// ダウンロードの本体
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public async UniTask DoDownload(DividableProgress progress = null)
        {
            if(isDownloaded)
            {
                progress?.Report(1f);
                return;
            }

            string tempPath = TempPath ?? SavePath + ".tmp";
            var request = new UnityWebRequest(Url);
            var handler = new DownloadHandlerFile(tempPath);
            request.downloadHandler = handler;

            var req = await request.SendAsObservable(progress).ToUniTask();
            req.downloadHandler.Dispose();
            BeforeSave?.Invoke(new CallbackParam(this, req));
            if(File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
            File.Move(tempPath, SavePath);
            AfterSave?.Invoke(new CallbackParam(this, req));
            progress?.Report(1f);
            isDownloaded = true;
        }
    }

    /// <summary>
    /// Downloadのどこかのタイミングで呼び出すコールバックに渡すパラメータ
    /// </summary>
    public struct CallbackParam
    {
        public readonly DownloadTask DownloadTask;
        public readonly UnityWebRequest Request;

        public CallbackParam(DownloadTask downloadTaskr, UnityWebRequest request)
        {
            DownloadTask = downloadTaskr;
            Request = request;
        }
    }
}
