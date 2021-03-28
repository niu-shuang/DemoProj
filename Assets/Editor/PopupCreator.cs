using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DemoProj
{
    public class PopupCreator : OdinEditorWindow
    {
        private const string templatePrefabPath = "Assets/DemoRes/Prefabs/PopupTemplate.prefab";
        private const string templatePopupScriptPath = "Templates/TemplatePopup.tpl";
        private const string templateBindingScriptPath = "Templates/TempletePopup_Binding.tpl";

        private const string scriptSavePath = "Assets/Scripts/UI/Popups/{0}Popup/";
        private const string prefabSavePath = "Assets/DemoRes/_Resources/UI/Popups/{0}Popup.prefab";

        [TitleGroup("Create popup", "type view name below")]
        public string popupName = "";

        [MenuItem("Tools/Create Popup")]
        private static void ShowDialog()
        {
            var window = GetWindow<PopupCreator>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 300);
            window.Show();
        }


        [Button("create popup")]
        private void CreateAsset()
        {
            if (string.IsNullOrEmpty(popupName))
            {
                EditorUtility.DisplayDialog("Error", "popup name is empty, please check!", "Confirm");
                return;
            }
            var templete = GetViewTemplete();
            var prefabPath = string.Format(prefabSavePath, popupName);
            AssetDatabase.DeleteAsset(prefabPath);
            AssetDatabase.Refresh();
            var prefab = PrefabUtility.SaveAsPrefabAsset(templete, prefabPath);
            DestroyImmediate(templete);
            EditorUtility.SetDirty(prefab);
            CreateViewScriptFromTemplete(popupName);
            prefab.GetComponent<UIPanelView>().CreateBindingCode();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static GameObject GetViewTemplete()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(templatePrefabPath);
            var go = PrefabUtility.InstantiatePrefab(asset) as GameObject;
            PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            return go;
        }

        public static void CreateViewScriptFromTemplete(string popupName)
        {
            using (var sr = new StreamReader(templatePopupScriptPath))
            {
                string content = sr.ReadToEnd();
                var newContent = content.Replace("TemplatePopup", popupName + "Popup");
                var dirPath = string.Format(scriptSavePath, popupName);
                var savePath = dirPath + $"{popupName}Popup.cs";
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
