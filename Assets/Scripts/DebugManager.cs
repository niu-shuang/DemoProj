using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using J;
using UnityEngine.UI;

namespace DemoProj
{
    public class DebugManager : SingletonMonoBehaviour<DebugManager>
    {
        [SerializeField]
        private Text output;

        private float showTime;
        private float clearTimer;

        public void Init()
        {
            Application.logMessageReceived += Application_logMessageReceived;
            showTime = Time.realtimeSinceStartup;
        }

        public static void Log(object obj)
        {
            if (Instance.output.text == null) return;
            Instance.output.text = obj.ToString();
            Instance.clearTimer = 0;
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                if (output.text == null) return;
                output.text = condition;
                clearTimer = 0;
            }
        }

        private void Update()
        {
            float time = Time.realtimeSinceStartup;
            if (showTime < time)
            {
                showTime = time + 1;

            }

            clearTimer += Time.deltaTime;
            if (clearTimer > 5)
            {
                clearTimer = 0;
                ClearScreen();
            }
        }

        public void ClearScreen()
        {
            //output.text = string.Empty;
        }
    }
}

