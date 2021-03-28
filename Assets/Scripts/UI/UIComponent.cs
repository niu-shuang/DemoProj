using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DemoProj
{
    public class UIComponent : MonoBehaviour
    {
        public string componentName;
        public ComponentType componentType;
        public Object targetComponent;

        private void Reset()
        {
            componentName = $"_{gameObject.name}";
            targetComponent = GetComponent<Text>();
            if (targetComponent != null)
            {
                componentType = ComponentType.Text;
                return;
            }

            targetComponent = GetComponent<CommonButton>();
            if (targetComponent != null)
            {
                componentType = ComponentType.CommonButton;
                return;
            }

            targetComponent = GetComponent<Image>();
            if (targetComponent != null)
            {
                componentType = ComponentType.Image;
                return;
            }
            targetComponent = GetComponent<RawImage>();
            if (targetComponent != null)
            {
                componentType = ComponentType.RawImage;
                return;
            }
            targetComponent = GetComponent<Gauge>();
            if (targetComponent != null)
            {
                componentType = ComponentType.Gauge;
                return;
            }
            
            targetComponent = GetComponent<InputField>();
            if (targetComponent != null)
            {
                componentType = ComponentType.InputField;
                return;
            }
            targetComponent = GetComponent<UIPanel>();
            if (targetComponent != null)
            {
                componentType = ComponentType.UIPanel;
                return;
            }
            targetComponent = GetComponent<RectTransform>();
            if(targetComponent != null)
            {
                componentType = ComponentType.RectTransform;
            }
            else
            {
                Debug.LogError($"no matching component on { gameObject.name }");
            }
        }

        public Text Text => targetComponent as Text;
        public Image Image => targetComponent as Image;
        public RawImage RawImage => targetComponent as RawImage;
        public Gauge Gauge => targetComponent as Gauge;
        public CommonButton CommonButton => targetComponent as CommonButton;
        public InputField InputFiled => targetComponent as InputField;
        public UIPanel UIPanel => targetComponent as UIPanel;
        public RectTransform RectTransform => targetComponent as RectTransform;
    }

    public enum ComponentType
    {
        Text,
        Image,
        RawImage,
        Gauge,
        CommonButton,
        InputField,
        UIPanel,
        RectTransform
    }
}
