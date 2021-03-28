using frame8.Logic.Misc.Other.Extensions;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace DemoProj
{
    public class UIPanelView : MonoBehaviour
    {
        public List<UIComponent> uiComponentList;
        public UITweenSequence uiTweenSequence;
        public PanelType panelType;
        public enum PanelType
        {
            View,
            Popup,
            Panel
        }

        [Button("Fetch UIComponents", ButtonSizes.Gigantic)]
        public void FetchUIComponents()
        {
            uiComponentList = new List<UIComponent>();
            FetchUIComponentsRecursive(transform);
        }
        [PropertySpace]
        [Button("Create bindg Code", ButtonSizes.Gigantic), GUIColor(0, 1, 1, 1)]
        public void CreateBindingCode()
        {
            UIPanelHelper.CreateBindingCode(this);
        }

        private void FetchUIComponentsRecursive(Transform currentTransform)
        {
            var children = currentTransform.GetChildren();
            foreach (var child in children)
            {
                var component = child.GetComponent<UIComponent>();
                if (component != null)
                {
                    uiComponentList.Add(component);
                    if (component.componentType == ComponentType.UIPanel)
                        continue;
                }
                FetchUIComponentsRecursive(child);
            }
        }
    }
}
