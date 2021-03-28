using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using J;
using UnityEngine;
using UnityEngine.Networking;

namespace DemoProj
{
    public static partial class Utilities
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        private const string PROTOCOL_FILE = "file:///";
#else
        private const string PROTOCOL_FILE = "file://";

# endif

        /// <summary>
        /// LocalファイルからTexture2Dを生成、PNGは最適化
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async UniTask<Texture2D> GetTextureFromLocalFile(string path)
        {
            if (!File.Exists(path)) return null;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fs.Length];
                await fs.ReadAsync(bytes, 0, bytes.Length);
                //Vector2Int size = bytes.CalcSizeIfPNG();
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                bool suc = texture.LoadImage(bytes);
                if (suc)
                {
                    return texture;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// LocalファイルからAudioClipを生成,デフォルトはmp3
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async UniTask<AudioClip> GetAudioClipFromLocalFile(string path, AudioType audioType = AudioType.MPEG)
        {
#if UNITY_STANDALONE_WIN
            return null;
#else
            using (var www = UnityWebRequestMultimedia.GetAudioClip(PROTOCOL_FILE + path, audioType))
            {
                await www.SendWebRequest();
                return DownloadHandlerAudioClip.GetContent(www);
            }
#endif
        }

        /// <summary>
        /// バイナリの非同期ロード（ファイルアップロード用
        /// </summary>
        /// <param name="path"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static async UniTask<byte[]> LoadBinaryAsync(string path, DividableProgress progress = null)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fs.Length];
                await fs.ReadAsync(bytes, 0, bytes.Length);
                progress?.Report(1);
                return bytes;
            }
        }

        /// <summary>
        /// bytesがPNGかどうか
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool IsPNG(this byte[] bytes)
        {
            bool isPNG = false;
            if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47
                && bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A)
            {
                isPNG = true;
            }
            return isPNG;
        }

        /// <summary>
        /// Texture2Dを指定したディレクトリにhashファイル名で保存.
        /// </summary>
        public static async UniTask<string> SaveTextureToHashedFileName(Texture2D tex, string outputPath, bool appendTimeStamp = true)
        {
            var bytes = tex.EncodeToPNG();
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            var hashBytes = provider.ComputeHash(bytes);
            provider.Clear();
            StringBuilder sb = new StringBuilder();
            foreach (var item in hashBytes)
            {
                sb.Append(item.ToString("x2"));
            }
            // 時刻も追加.
            if (appendTimeStamp)
            {
                sb.Append("_");
                sb.Append(DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            }
            sb.Append(".png");
            var hashedFileName = sb.ToString();
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            var outputFilePath = Path.Combine(outputPath, hashedFileName);
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }
            using (FileStream fs = new FileStream(outputFilePath, FileMode.CreateNew, FileAccess.Write))
            {
                await fs.WriteAsync(bytes, 0, bytes.Length);
            }
            return hashedFileName;
        }

        public static Vector2Int CalcScaledSize(this Texture2D input, int preferedWidth, int preferedHeight)
        {
            var originSize = new Vector2Int(input.width, input.height);
            float wRatio = originSize.x / (float)preferedWidth;
            float hRatio = originSize.y / (float)preferedHeight;
            var size = new Vector2Int();
            if (wRatio < 1 + Mathf.Epsilon && hRatio < 1 + Mathf.Epsilon)
            {
                size = originSize;
            }
            else if (wRatio > hRatio)
            {
                size.x = Mathf.CeilToInt(originSize.x / wRatio);
                size.y = Mathf.CeilToInt(originSize.y / wRatio);
            }
            else if (wRatio < hRatio)
            {
                size.x = Mathf.CeilToInt(originSize.x / hRatio);
                size.y = Mathf.CeilToInt(originSize.y / hRatio);
            }
            else
            {
                size.x = Mathf.CeilToInt(originSize.x / wRatio);
                size.y = Mathf.CeilToInt(originSize.y / wRatio);
            }
            return size;
        }


        /// <summary>
        /// Texture2Dをダウンロードしてロード
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <param name="eTag"></param>
        /// <returns></returns>
        public static async UniTask<Texture2D> DownloadTexture2D(string url,string savePath, string eTag = null, DividableProgress progress = null)
        {
            var downloader = new DownloadTask(url, savePath, eTag);
            if(string.IsNullOrEmpty(savePath))
            {
                var data = await downloader.DownloadWithoutSave(progress);
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                bool suc = texture.LoadImage(data);
                if (suc)
                {
                    return texture;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var suc = await downloader.DownloadAsync(progress?.Divide(.8f));
                if (suc)
                {
                    var texture = await GetTextureFromLocalFile(savePath);
                    progress?.Report(1f);
                    return texture;
                }
                else
                {
                    progress?.Report(1f);
                    return null;
                }
            }
        }
    }
}
