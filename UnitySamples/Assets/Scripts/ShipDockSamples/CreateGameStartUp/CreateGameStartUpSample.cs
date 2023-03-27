using ShipDock;

public class CreateGameStartUpSample : ShipDockAppComponent
{
    private int mReloadTime = 3;
    private string mLog = "Hello ShipDock..";
    private string mSampleExplain = "\n点击编辑器菜单栏中的 ShipDock/Create Application 菜单自动生成框架应用预制体，\n并挂一个继承自 ShipDockAppComponent 的组件（例如本案例的 CreateGameStartUpSample 组件），\n即可开始使用 ShpiDock 框架）";
    private string mLogWithParam = "log:\n|--------------------\n| 即将演示 {0} 次重启，并在最后一次重启关闭 ShipDock 框架\n|--------------------";

    public override void ApplicationCloseHandler()
    {
        base.ApplicationCloseHandler();

        "ShipDock Sample exited..".Log();
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        //从这里开始，编写游戏逻辑
        "log".Log(mLog);
        this.LogAndLocated("log", mSampleExplain);
        mLogWithParam.Log(mReloadTime.ToString());
        //GameObject raw = ShipDockApp.Instance.ABs.Get("aa", "a");
        //Instantiate(raw);

        //UIManager UIMgr = ShipDockApp.Instance.UIs;
        //UIMgr.OpenResourceUI<Transform>("ResourceUISample");

        TimeUpdater.New(5f, () =>
        {
            mReloadTime--;
            if (mReloadTime <= 0)
            {
                Destroy(GameComponent.gameObject);//通过销毁框架模板组件所在的物体，达到关闭框架的目的
            }
            else
            {
                ReloadFrameworkScene();//使用此方法从新加载框架启动组件所在场景，达到重启框架的目的
            }
        });
    }
}
