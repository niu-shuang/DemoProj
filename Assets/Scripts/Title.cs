using UnityEngine;
using Cysharp.Threading.Tasks;

namespace DemoProj
{
    public class Title : MonoBehaviour
    {

        public async void StartGame()
        {
            await ManagerRoot.Instance.Init();
            UIManager.ChangeView(ViewDefine.View.Home).Forget();
        }
    }
}
