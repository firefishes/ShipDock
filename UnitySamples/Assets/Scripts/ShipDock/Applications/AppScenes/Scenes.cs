using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ShipDock
{
    public class Scenes
    {
        public UnityAction<Scene, LoadSceneMode> OnSceneLoaded { get; set; }

        private LoadSceneMode mLoadSceneMode;
        private Scene mPrevScene;
        private bool mSceneLoading;
        private int mSceneIndexLoad;
        private int mSceneIndexClean;
        private string mSceneNameLoad;
        private string mSceneNameClean;

        public Scenes() { }

        public void Clean()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            OnSceneLoaded = default;
        }

        /// <summary>
        /// 清除指定场景，并根据场景名称加载新场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadAndClearAnotherScene(string sceneName, string cleanSceneName = "", LoadSceneMode sceneMode = LoadSceneMode.Additive)
        {
            if (mSceneLoading)
            {
                return;
            }
            else { }

            mSceneLoading = true;

            mLoadSceneMode = sceneMode;
            mSceneNameLoad = sceneName;
            mSceneNameClean = cleanSceneName;

            LoadScene();
        }

        /// <summary>
        /// 清除指定场景，并根据场景构建索引加载新场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadAndClearAnotherScene(int sceneIndex, int cleanSceneIndex = -1, LoadSceneMode sceneMode = LoadSceneMode.Additive)
        {
            if (mSceneLoading)
            {
                return;
            }
            else { }

            mSceneLoading = true;

            mLoadSceneMode = sceneMode;
            mSceneIndexLoad = sceneIndex;
            mSceneIndexClean = cleanSceneIndex;

            LoadScene();
        }

        private void LoadScene()
        {
            mPrevScene = default;

            if (!string.IsNullOrEmpty(mSceneNameClean))
            {
                mPrevScene = SceneManager.GetSceneByName(mSceneNameClean);
            }
            else if (mSceneIndexClean != -1)
            {
                mPrevScene = SceneManager.GetSceneByBuildIndex(mSceneIndexClean);
            }
            else { }

            if (mPrevScene != default)
            {
                Scene scene = SceneManager.CreateScene("ShipDockScene_Unload");
                SceneManager.SetActiveScene(scene);

                GameObject item;
                GameObject[] list = mPrevScene.GetRootGameObjects();
                int max = list.Length;
                for (int i = 0; i < max; i++)
                {
                    item = list[i];
                    SceneManager.MoveGameObjectToScene(item, scene);
                }

                SceneManager.sceneUnloaded += RemoveUnloadScene;
                SceneManager.UnloadSceneAsync(scene);
            }
            else
            {
                LoadNextScene();
            }
        }

        private void RemoveUnloadScene(Scene scene)
        {
            SceneManager.sceneUnloaded -= RemoveUnloadScene;

            SceneManager.UnloadSceneAsync(mPrevScene);
            mPrevScene = default;

            LoadNextScene();
        }

        private void LoadNextScene()
        {
            if (Application.isPlaying)
            {
                SceneManager.sceneLoaded += OnLoadNextScene;

                if (OnSceneLoaded != default)
                {
                    SceneManager.sceneLoaded += OnSceneLoaded;
                }
                else { }

                if (!string.IsNullOrEmpty(mSceneNameLoad))
                {
                    SceneManager.LoadSceneAsync(mSceneNameLoad, mLoadSceneMode);
                }
                else if (mSceneIndexLoad >= 0)
                {
                    SceneManager.LoadSceneAsync(mSceneIndexLoad, mLoadSceneMode);
                }
                else { }
            }
            else { }
        }

        private void OnLoadNextScene(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnLoadNextScene;

            if (OnSceneLoaded != default)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
            else { }

            SceneManager.SetActiveScene(scene);
            mSceneLoading = false;

            mSceneIndexLoad = -1;
            mSceneIndexClean = -1;
            mSceneNameLoad = string.Empty;
            mSceneNameClean = string.Empty;
        }
    }

}