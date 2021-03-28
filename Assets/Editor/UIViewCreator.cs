using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.OdinInspector;
using System.IO;
using System.Text;

namespace DemoProj
{
    public class UIViewCreator : OdinEditorWindow
    {
        private const string templatePrefabPath = "Assets/DemoRes/Prefabs/ViewTemplate.prefab";
        private const string templateViewScriptPath = "Templates/TemplateView.tpl";
        private const string templateBindingScriptPath = "Templates/TemplateView_Binding.tpl";

        private const string scriptSavePath = "Assets/Scripts/UI/Views/{0}View/";
        private const string viewDefinePath = "Assets/Scripts/UI/ViewDefine.cs";
        private const string prefabSavePath = "Assets/DemoRes/_Resources/UI/Views/{0}View.prefab";

        [TitleGroup("Create view", "type view name below")]
        public string viewName = "";
        [Title("scene name that view belongs to")]
        public string sceneName = "";

        [MenuItem("Tools/Create View")]
        private static void ShowDialog()
        {
            var window = GetWindow<UIViewCreator>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 300);
            window.Show();
        }

        
        [Button("create view")]
        private void CreateAsset()
        {
            if(string.IsNullOrEmpty(viewName) || string.IsNullOrEmpty(sceneName))
            {
                EditorUtility.DisplayDialog("Error", "view name or scene name empty, please check!", "Confirm");
                return;
            }
            var templete = GetViewTemplete();
            var prefabPath = string.Format(prefabSavePath, viewName);
            AssetDatabase.DeleteAsset(prefabPath);
            AssetDatabase.Refresh();
            var prefab = PrefabUtility.SaveAsPrefabAsset(templete, prefabPath);
            DestroyImmediate(templete);
            EditorUtility.SetDirty(prefab);
            CreateViewScriptFromTemplete(viewName);
            prefab.GetComponent<UIPanelView>().CreateBindingCode();
            CreateOrChangeViewDefine(viewName, sceneName);
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

        public static void CreateViewScriptFromTemplete(string viewName)
        {
            using(var sr = new StreamReader(templateViewScriptPath))
            {
                string content = sr.ReadToEnd();
                var newContent = content.Replace("TemplateView", viewName + "View");
                var dirPath = string.Format(scriptSavePath, viewName);
                var savePath =  dirPath + $"{viewName}View.cs";
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

        private enum eReadingState
        {
            None,
            ReadingView,
            ReadingScene,
            ReadingViewDict,
        }
        private static void CreateOrChangeViewDefine(string viewName,string sceneName)
        {
            StringBuilder sb = new StringBuilder();
            List<string> views = new List<string>();
            List<string> scenes = new List<string>();
            string viewNameToWrite = $"            {viewName},";
            string sceneNameToWrite = $"            {sceneName},";
            string dictData = $"            {{ View.{viewName},new ViewData(View.{viewName}, Scene.{sceneName})}},";
            bool isNewView = false;
            eReadingState state = eReadingState.None;
            using (var sr = new StreamReader(viewDefinePath))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("public enum View"))
                    {
                        sb.AppendLine(line);
                        sb.AppendLine(sr.ReadLine());
                        state = eReadingState.ReadingView;
                        continue;
                    }
                    else if(line.Contains("public enum Scene"))
                    {
                        sb.AppendLine(line);
                        sb.AppendLine(sr.ReadLine());
                        state = eReadingState.ReadingScene;
                        continue;
                    }
                    else if(line.Contains("public static Dictionary<View, ViewData> viewDict = new Dictionary<View, ViewData>()"))
                    {
                        sb.AppendLine(line);
                        sb.AppendLine(sr.ReadLine());
                        state = eReadingState.ReadingViewDict;
                        continue;
                    }

                    if(state == eReadingState.ReadingView)
                    {
                        
                        if (line.Contains("}"))
                        {
                            state = eReadingState.None;
                            if(!views.Contains(viewNameToWrite))
                            {
                                isNewView = true;
                                sb.AppendLine(viewNameToWrite);
                            }
                            sb.AppendLine(line);
                            continue;
                        }
                        else
                        {
                            views.Add(line);
                            sb.AppendLine(line);
                        }
                    } 

                    if(state == eReadingState.ReadingScene)
                    {
                        if (line.Contains("}"))
                        {
                            state = eReadingState.None;
                            if (!scenes.Contains(sceneNameToWrite))
                            {
                                sb.AppendLine(sceneNameToWrite);
                            }
                            sb.AppendLine(line);
                            continue;
                        }
                        else
                        {
                            scenes.Add(line);
                            sb.AppendLine(line);
                        }
                    }

                    if (state == eReadingState.ReadingViewDict)
                    {
                        if (line.Contains("}"))
                        {
                            state = eReadingState.None;
                            if (isNewView)
                            {
                                sb.AppendLine(dictData);
                            }
                            sb.AppendLine(line);
                            continue;
                        }
                        else
                        {
                            scenes.Add(line);
                            sb.AppendLine(line);
                        }
                    }

                    if (state == eReadingState.None)
                    {
                        sb.AppendLine(line);
                    }
                        
                }
                
                
            }

            if (File.Exists(viewDefinePath))
                File.Delete(viewDefinePath);
            using (var sw = new StreamWriter(viewDefinePath))
            {
                sw.Write(sb.ToString());
            }

        }

    }
}
