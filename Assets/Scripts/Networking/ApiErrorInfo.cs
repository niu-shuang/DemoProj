using UnityEngine;
using System;
using System.Collections;

namespace DemoProj
{

    public class ApiErrorInfo
    {
        public ApiErrorDefine.ErrorStatus errorStatus;    //エラーステータス
        public int retryCount;      //リトライ回数
        public int code;            //エラーコード
        public string message;      //エラーメッセージ
        public Exception exception;     //Exception情報（発生時のみ）
        public ApiErrorDefine.ActionType actionType;

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        public ApiErrorInfo(int code, string message)
        {
            retryCount = 0;
            this.code = code;
            this.message = message;
            errorStatus = (ApiErrorDefine.ErrorStatus)code;
        }
    }

}
