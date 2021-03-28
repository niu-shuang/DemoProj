#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DemoProj
{
    /// <summary>
    /// API通信のシミュレーションモード設定情報.
    /// （ダミーJSONファイルによる通信、最後に受け取ったレスポンスのキャッシュを返す）
    /// </summary>
    /// <author>y-kawasaki</author>
    public class APISimulationInfo : ScriptableObject
    {
        // ScriptableObject保存先.
        private const string SAVE_PATH = "Assets/Scripts/Networking/API/SimulationInfo.asset";
        private const bool V = false;

        // 最後にキャッシュしたレスポンスを返すモードの設定.
        public enum ReturnCacheMode
        {
            None,       // 全APIで無効.
            All,        // 全APIで有効.
            Custom,     // 個別設定.
        }

        // APIごとのダミーJSONファイル指定.
        [Serializable]
        public class DummyJsonInfo
        {
            public string apiName;
            public bool enabled;
            public TextAsset textAsset;
        }

        // キャッシュを返すAPIの情報.
        [Serializable]
        public class CustomReturnCacheInfo
        {
            public string apiName;
            public bool enabled;
        }

        // APIごとの通信結果キャッシュクラス.
        [Serializable]
        private class CacheInfo
        {
            public string apiName;
            public string response;
        }

        // 保存情報.
        [SerializeField] private bool _enableApiError = false;
        [SerializeField] private float _apiErrorProbability = 1.0f;
        [SerializeField] private int _apiErrorCode = 408;

        [SerializeField] private bool _enableDummyJson = false;
        [SerializeField] private List<DummyJsonInfo> _dummyJsonInfoList = new List<DummyJsonInfo>();

        [SerializeField] private ReturnCacheMode _cacheMode = ReturnCacheMode.None;
        [SerializeField] private List<CustomReturnCacheInfo> _returnCacheInfoList = new List<CustomReturnCacheInfo>();
        [SerializeField] private List<CacheInfo> _cacheList = new List<CacheInfo>();

        // Singleton.
        private static APISimulationInfo s_instance;
        private static APISimulationInfo instance
        {
            get
            {
                if (s_instance != null) return s_instance;

                // Assetがあればそこから読み込み.
                s_instance = AssetDatabase.LoadAssetAtPath<APISimulationInfo>(SAVE_PATH);
                if (s_instance != null) return instance;

                // なければ生成.
                s_instance = CreateInstance<APISimulationInfo>();
                AssetDatabase.CreateAsset(s_instance, SAVE_PATH);
                AssetDatabase.Refresh();
                return s_instance;
            }
        }
        private APISimulationInfo() { }

        //---- データのset・get（Editor側から呼ばれる） ----

        // API通信エラーシミュレーション.
        public static bool enableApiError
        {
            set { instance._enableApiError = value; }
            get { return instance._enableApiError; }
        }

        public static float apiErrorProbability
        {
            set { instance._apiErrorProbability = value; }
            get { return instance._apiErrorProbability; }
        }

        public static int apiErrorCode
        {
            set { instance._apiErrorCode = value; }
            get { return instance._apiErrorCode; }
        }

        // ダミーJSONモード.
        public static bool enableDummyJson
        {
            set { instance._enableDummyJson = value; }
            get { return instance._enableDummyJson; }
        }

        // キャッシュモード.
        public static ReturnCacheMode cacheMode
        {
            set { instance._cacheMode = value; }
            get { return instance._cacheMode; }
        }

        public static List<DummyJsonInfo> dummyJsonInfoList
        {
            set { instance._dummyJsonInfoList = value; }
            get { return instance._dummyJsonInfoList; }
        }

        public static List<CustomReturnCacheInfo> returnCacheInfoList
        {
            set { instance._returnCacheInfoList = value; }
            get { return instance._returnCacheInfoList; }
        }

        /// <summary>
        /// APIキャッシュをクリア.
        /// </summary>
        public static void DeleteAPICache()
        {
            instance._cacheList.Clear();
        }

        /// <summary>
        /// 全情報を消去.
        /// </summary>
        public static void DeleteAll()
        {
            s_instance = null;
            AssetDatabase.DeleteAsset(SAVE_PATH);
        }

        //---- 情報取得（ゲーム側から呼ばれる） ----

        /// <summary>
        /// API通信をエラー通信としてシミュレートする場合、エラーコードを取得.
        /// エラー通信としない場合は0を返す.
        /// </summary>
        public static int GetApiErrorCodeIfSimulation()
        {
            var isError = false;
            if (instance._enableApiError)
            {
                var rand = UnityEngine.Random.Range(0f, 1f);
                isError = rand < instance._apiErrorProbability;
            }
            return isError ? instance._apiErrorCode : 0;
        }

        /// <summary>
        /// ダミーJSONモードが有効である場合に、ダミーJSONのTextAssetを取得する. 無効ならnull.
        /// </summary>
        public static TextAsset GetDummyJsonAssetIfNeeded(string apiName)
        {
            return instance._GetDummyJsonAssetIfNeeded(apiName);
        }
        private TextAsset _GetDummyJsonAssetIfNeeded(string apiName)
        {
            if (!_enableDummyJson) return null;

            var index = _dummyJsonInfoList.FindIndex(i => i.enabled && i.apiName == apiName);
            if (index != -1)
            {
                // テキストが破損している可能性がある場合、エラー表示.
                var textAsset = _dummyJsonInfoList[index].textAsset;
                if (string.IsNullOrEmpty(textAsset.text))
                {
                    Debug.LogError(string.Format("Dummy json file \"{0}\" may be collapsed. Check the text is UTF-8.", textAsset.name));
                }
                return textAsset;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// キャッシュモードが有効である場合に、キャッシュされたレスポンスJSONを取得する. 無効ならnull.
        /// </summary>
        public static string GetCachedJsonIfNeeded(string apiName)
        {
            return instance._GetCachedJsonIfNeeded(apiName);
        }
        private string _GetCachedJsonIfNeeded(string apiName)
        {
            if (_cacheMode == ReturnCacheMode.None) return null;
            else if (_cacheMode == ReturnCacheMode.Custom
                     && !_returnCacheInfoList.Exists(i => i.enabled && i.apiName == apiName)) return null;

            var index = _cacheList.FindIndex(c => c.apiName == apiName);
            return index != -1 ? _cacheList[index].response : null;
        }

        /// <summary>
        /// APIキャッシュJSONを保存.
        /// </summary>
        public static void CacheJson(string apiName, string response)
        {
            instance._CacheJson(apiName, response);
        }
        private void _CacheJson(string apiName, string response)
        {
            var cache = _cacheList.Find(info => info.apiName == apiName);
            if (cache == null)
            {
                _cacheList.Add(new CacheInfo() { apiName = apiName, response = response });
            }
            else
            {
                cache.response = response;
            }
        }
    }
}

#endif
