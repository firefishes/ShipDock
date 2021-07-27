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
                scenes.ClearAndLoadScene("LoadSceneSampleB", "LoadSceneSampleA");
            });
        };
        scenes.ClearAndLoadScene("LoadSceneSampleA", string.Empty);
    }
}
