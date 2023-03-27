using ShipDock;

public class LoadSceneSample : ShipDockAppComponent
{
    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        Scenes scenes = new Scenes();
        scenes.OnSceneLoaded += (s, m) =>
        {
            scenes.OnSceneLoaded = default;
            TimeUpdater.New(5f, () =>
            {
                scenes.LoadAndClearAnotherScene("LoadSceneSampleB", "LoadSceneSampleA");
            });
        };
        scenes.LoadAndClearAnotherScene("LoadSceneSampleA", string.Empty);
    }
}
