using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DemoProj
{
    public static class UIPanelHelper
    {
        private const string templateBindingScriptPath = "Templates/TemplateView_Binding.tpl";
        private const string viewScriptSavePath = "Assets/Scripts/UI/Views/{0}/";
        private const string popupScriptSavePath = "Assets/Scripts/UI/Popups/{0}/";

        public static void CreateBindingCode(UIPanelView uiPanel)
        {
            var uiComponentList = uiPanel.uiComponentList;
            using (var sr = new StreamReader(templateBindingScriptPath))
            {
                string content = string.Empty;
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null) break;
                    if (line == "#region auto generated property")
                    {
                        line += "\n";
                        foreach (var item in uiComponentList)
                        {
                            string propertyStr = $"        protected {item.componentType} {item.componentName};\n";
                            line += propertyStr;
                        }
                    }
                    else if (line == "#region auto generated binding code")
                    {
                        line += "\n";
                        for (int i = 0; i < uiComponentList.Count; i++)
                        {
                            string bindingCodeStr = $"            {uiComponentList[i].componentName} = view.uiComponentList[{i}].{uiComponentList[i].componentType};\n";
                            line += bindingCodeStr;
                        }
                    }
                    else
                        line += "\n";
                    content += line;
                }

                var newContent = content.Replace("TemplateView", uiPanel.gameObject.name);
                var scriptSavePath = uiPanel.panelType == UIPanelView.PanelType.View ? viewScriptSavePath : popupScriptSavePath;
                var dirPath = string.Format(scriptSavePath, uiPanel.gameObject.name);
                var savePath = dirPath + $"{uiPanel.gameObject.name}_Binding.cs";
                if (File.Exists(savePath))
                    File.Delete(savePath);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                using (var sw = new StreamWriter(savePath))
                {
                    sw.Write(newContent);
                }
            }
        }
    }

}
