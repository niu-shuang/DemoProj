using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DemoProj
{
    public class EmptyScene : MonoBehaviour
    {
        // Start is called before the first frame update
        async void Start()
        {
            if (UIManager.nextSceneName == "Title")
            {
                //DestroyImmediate
                Destroy(ManagerRoot.Instance.gameObject);
            }
            await Resources.UnloadUnusedAssets();
            System.GC.Collect();
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UIManager.nextSceneName);
        }

    }
}
