using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using J;
using Cysharp.Threading.Tasks;

namespace DemoProj
{
    public class ManagerRoot : SingletonMonoBehaviour<ManagerRoot>
    {
        public string manifestURL;
        public async UniTask Init()
        {
            DebugManager.Instance.Init();
            UIManager.Instance.Init();
#if !UNITY_EDITOR
            await AssetLoader.LoadManifest(manifestURL).ToUniTask();
#endif
            await UniTask.DelayFrame(1);
        }

        public void Reboot()
        {
            UIManager.nextSceneName = "Title";
            UnityEngine.SceneManagement.SceneManager.LoadScene("EmptyScene");
        }
    }
}
