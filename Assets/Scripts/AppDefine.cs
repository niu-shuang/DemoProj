using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoProj
{
    public class AppDefine
    {
        /// <summary>
        /// root url
        /// </summary>
        public const string API_DOMAIN = "";

        /// <summary>
        /// milliseconds
        /// </summary>
        public const int API_TIMEOUT = 30000;

        //�쥹�ݥ���Ҏ��F�����å�(json)
        public const string JSON_REGEX = @"\{(.*)\}";

        public const string POST_KEY_RESULT = "result";
    }
}
