using ShipDock.Applications;

public class LoadSceneSample : ShipDockAppComponent
{
    protected override string GetLocalsDescription<T>(ref string locals, ref T item)
    {
        return string.Empty;
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        Scenes scenes = new Scenes();
        scenes.OnSceneLoaded += (s, m) =>
        {
            scenes.OnSceneLoaded = default;
            TimeUpdater.New(5f, () =>
            {
                scenes.LoadAndClearActivedScene("LoadSceneSampleB", "LoadSceneSampleA");
            });
        };
        scenes.LoadAndClearActivedScene("LoadSceneSampleA", string.Empty);
    }
}
