using System;
using ShipDock.Applications;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateGameStartUp : ShipDockAppComponent
{
    protected override string GetLocalsDescription<T>(ref string locals, ref T item)
    {
        return string.Empty;
    }

    //public override void ApplicationCloseHandler()
    //{
    //    base.ApplicationCloseHandler();

    //    Scene scene = SceneManager.CreateScene("Empty");
    //    SceneManager.SetActiveScene(scene);
    //    SceneManager.MergeScenes(SceneManager.GetActiveScene(), scene);
    //    GameObject[] list = scene.GetRootGameObjects();
    //    int max = list.Length;
    //    for (int i = 0; i < max; i++)
    //    {
    //        Destroy(list[i]);
    //    }

    //    SceneManager.sceneUnloaded += UnloadEmptyScene;
    //    SceneManager.UnloadSceneAsync(scene);
    //}

    //private void UnloadEmptyScene(Scene scene)
    //{
    //    SceneManager.sceneUnloaded -= UnloadEmptyScene;

    //    if (Application.isPlaying)
    //    {
    //        SceneManager.sceneLoaded += OnReloadStartUpScene;
    //        SceneManager.LoadScene("Sample_CreateGameStartUp");
    //    }
    //    else { }
    //}

    //private void OnReloadStartUpScene(Scene scene, LoadSceneMode mode)
    //{
    //    SceneManager.sceneLoaded -= OnReloadStartUpScene;
    //    SceneManager.SetActiveScene(scene);
    //}

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        GameObject raw = ShipDockApp.Instance.ABs.Get("aa", "a");
        Instantiate(raw);

        TimeUpdater.New(5f, () =>
        {
            ReloadFrameworkScene();
        });
    }
}
