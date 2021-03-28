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

        //¥ì¥¹¥Ý¥ó¥¹ÕýÒŽ±í¬F¥Á¥§¥Ã¥¯(json)
        public const string JSON_REGEX = @"\{(.*)\}";

        public const string POST_KEY_RESULT = "result";
    }
}
