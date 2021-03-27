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
            UIManager.Instance.Init();
#if UNITY_EDITOR
            manifestURL = "file://" + Application.dataPath + "AssetBundles/Android/Android";
#endif
            AssetLoader.LoadManifest(manifestURL);
            await UniTask.DelayFrame(1);
        }

        public void Reboot()
        {
            UIManager.nextSceneName = "Title";
            UnityEngine.SceneManagement.SceneManager.LoadScene("EmptyScene");
        }
    }
}
