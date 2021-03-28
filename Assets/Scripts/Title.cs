using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace DemoProj
{
    public class Title : MonoBehaviour
    {
        public Text uid;

        public void Start()
        {
            uid.text = $"uid : {PlayerPrefs.GetString("uid")}";
        }

        public async void StartGame()
        {
            await ManagerRoot.Instance.Init();
            UIManager.ChangeView(ViewDefine.View.Home).Forget();
        }

        public async void CreatePlayer()
        {
            var api = new CreatePlayerAPI();
            var result = await api.Connect();
            if(result.succeeded)
            {
                PlayerPrefs.SetString("uid", result.dto.uid);
                uid.text = $"uid : {result.dto.uid}";
            }
        }
    }
}
