namespace DemoProj
{
    using Cysharp.Threading.Tasks;
    using UniRx;
    using J;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        public static bool canPressKey => !isLoading;
        public static bool isLoading;

        public Transform viewRoot;
        public Transform popupRoot;
        public LoadingPlate loadingPlate;

        public static Dictionary<ViewDefine.View, ViewBase> loadedView;
        private static Stack<ViewDefine.View> viewStack;
        private bool isSceneLoaded;
        public static string nextSceneName;


        public static ViewDefine.View currentView;
        public static ViewDefine.Scene currentScene;

        public void Init()
        {
            loadedView = new Dictionary<ViewDefine.View, ViewBase>();
            currentView = ViewDefine.View.None;
            viewStack = new Stack<ViewDefine.View>();
            currentScene = ViewDefine.Scene.Title;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene sceneName, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (sceneName.name != "EmptyScene")
            {
                isSceneLoaded = true;
            }
        }

        /// <summary>
        /// Load View, if isReload == true and the view is loaded already, the loaed view will be destroyed and reload
        /// </summary>
        /// <param name="nextView"></param>
        /// <param name="isReload"></param>
        /// <returns></returns>
        public static async UniTaskVoid ChangeView(ViewDefine.View nextView, bool isReload = false)
        {
            var viewData = ViewDefine.viewDict.GetOrDefault(nextView);
            if (viewData == null)
            {
                throw new Exception($"No view data for {nextView}!");
            }
            UIManager.isLoading = true;
            var prevView = currentView;
            currentView = nextView;
            if (isReload)
            {
                if (loadedView.ContainsKey(nextView))
                {
                    var view = loadedView[nextView];
                    Destroy(view.gameObject);
                }
                await Resources.UnloadUnusedAssets();

            }
            DividableProgress progress = new DividableProgress();
            Instance.loadingPlate.SetProgress(progress);
            // view not loaded
            if (!loadedView.ContainsKey(nextView))
            {
                // need to load scene
                if (viewData.scene != currentScene)
                {
                    if (prevView != ViewDefine.View.None)
                    {
                        //Play fade out animation for current view
                        await loadedView[prevView].Hide();
                        loadedView[prevView].OnEndView();
                    }
                    
                    await Instance.loadingPlate.uiAnimation.PlayAsync();
                    ClearLoadedView();
                    //PopupManager.OnEndScene();
                    nextSceneName = viewData.scene.ToString();
                    SceneManager.LoadScene("EmptyScene");
                    await UniTask.WaitUntil(() => Instance.isSceneLoaded);
                    currentScene = viewData.scene;
                    await LoadViewTask(nextView, progress);
                    await Instance.loadingPlate.uiAnimation.PlayReverseAsync();
                    await loadedView[currentView].Show();
                    isLoading = false;
                }
                else //no need to load scene, but view is not loaded
                {
                    await LoadViewTask(nextView, progress);
                    await loadedView[prevView].Hide();
                    loadedView[prevView].OnEndView();
                    await loadedView[currentView].Show();
                    isLoading = false;
                }
            }
            else
            {
                await loadedView[prevView].Hide();
                loadedView[prevView].OnEndView();
                await loadedView[currentView].Show();
                isLoading = false;
            }
        }

        private static void ClearLoadedView()
        {
            foreach (var item in loadedView.Values)
            {
                Destroy(item.gameObject);
            }
        }

        /// <summary>
        /// Load View ->Instantiate On ViewRoot -> Create ViewScript Instance -> Invoke Init -> Run Load Task For Current View
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private static async UniTask LoadViewTask(ViewDefine.View view, DividableProgress progress)
        {
            string viewName = $"{view}View";
            string viewPath = $"UI/Views/{viewName}";
            var panelAsset = await AssetLoader.Load<GameObject>(viewPath);
            var panelGO = Instantiate(panelAsset, Instance.viewRoot);
            
            var logicScript = Activator.CreateInstance(Type.GetType("DemoProj." + viewName)) as ViewBase;
            var viewScript = panelGO.GetComponent<UIPanelView>();
            logicScript.Init(viewScript);
            loadedView[currentView] = logicScript;
            await loadedView[currentView].LoadTask(progress);
            progress.Report(1);
            progress.Dispose();
            loadedView[currentView].OnFinishLoad();
        }

        protected override void SingletonOnDestroy()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }
    }
}

