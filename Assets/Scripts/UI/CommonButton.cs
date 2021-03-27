using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

namespace DemoProj
{
    public class CommonButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        public ButtonClickedEvent onClick;
        public bool ingoreInteractable = false;

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
        }
        public void OnClick()
        {
            if (UIManager.canPressKey)
            {
                onClick?.Invoke();
            }
            else
            {
                Debug.LogError("can not press key");
            }
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void Reset()
        {
            button = transform.GetOrAddComponent<Button>();
        }
    }
}
