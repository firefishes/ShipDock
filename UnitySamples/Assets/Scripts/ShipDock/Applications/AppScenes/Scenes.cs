using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ShipDock.Applications
{
    public class Scenes
    {
        public UnityAction<Scene, LoadSceneMode> OnSceneLoaded { get; set; }

        private LoadSceneMode mLoadSceneMode;
        private Scene mPrevScene;
        private bool mSceneLoading;
        private int mSceneBuildIndex;
        private int mSceneBuildIndexWillClean;
        private string mSceneNameWillLoad;
        private string mSceneNameWillClean;

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
        public void LoadAndClearActivedScene(string sceneName, string cleanSceneName = "", LoadSceneMode sceneMode = LoadSceneMode.Additive)
        {
            if (mSceneLoading)
            {
                return;
            }
            else { }

            mSceneLoading = true;

            mLoadSceneMode = sceneMode;
            mSceneNameWillLoad = sceneName;
            mSceneNameWillClean = cleanSceneName;

            LoadScene();
        }

        /// <summary>
        /// 清除指定场景，并根据场景构建索引加载新场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadAndClearActivedScene(int sceneIndex, int cleanSceneIndex = -1, LoadSceneMode sceneMode = LoadSceneMode.Additive)
        {
            if (mSceneLoading)
            {
                return;
            }
            else { }

            mSceneLoading = true;

            mLoadSceneMode = sceneMode;
            mSceneBuildIndex = sceneIndex;
            mSceneBuildIndexWillClean = cleanSceneIndex;

            LoadScene();
        }

        private void LoadScene()
        {
            mPrevScene = default;

            if (!string.IsNullOrEmpty(mSceneNameWillClean))
            {
                mPrevScene = SceneManager.GetSceneByName(mSceneNameWillClean);
            }
            else if (mSceneBuildIndexWillClean != -1)
            {
                mPrevScene = SceneManager.GetSceneByBuildIndex(mSceneBuildIndexWillClean);
            }
            else { }

            if (mPrevScene != default)
            {
                Scene scene = SceneManager.CreateScene("ShipDockScene_Unload");
                SceneManager.SetActiveScene(scene);

                GameObject[] list = mPrevScene.GetRootGameObjects();
                int max = list.Length;
                for (int i = 0; i < max; i++)
                {
                    SceneManager.MoveGameObjectToScene(list[i], scene);
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

                if (!string.IsNullOrEmpty(mSceneNameWillLoad))
                {
                    SceneManager.LoadScene(mSceneNameWillLoad, mLoadSceneMode);
                }
                else if (mSceneBuildIndex >= 0)
                {
                    SceneManager.LoadScene(mSceneBuildIndex, mLoadSceneMode);
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

            mSceneBuildIndex = -1;
            mSceneBuildIndexWillClean = -1;
            mSceneNameWillLoad = string.Empty;
            mSceneNameWillClean = string.Empty;
        }
    }

}