using ShipDock.Applications;
using ShipDock.Testers;

public class ColorableLogSample : ShipDockAppComponent
{
    protected override string GetLocalsDescription<T>(ref string locals, ref T item)
    {
        return string.Empty;
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        bool logFilters = true;

        "log".Log(logFilters, "ShipDock 提供了许多日志方式，可以根据第一个参数（logFilters）指示此日志是否显示，这是普通日志");
        "warning".Log(logFilters, "ShipDock 这是警告日志");
        "error".Log(logFilters, "这是错误日志");
        "todo".Log(logFilters, "这是待做事务日志");
        "debug".Log(logFilters, "这是Unity日志");
        "log:这是携带参数的日志：{0}".Log("参数内容需要使用大括号进行格式化");
        "log:像这样未预置的自定义内容日志须携带一个或一个以上的格式化参数 {0} ".Log(logFilters, "才能显示");
        this.LogAndLocated("log", "这是可定位到场景物体的日志，第一个参数未 日志id，用于识别使用哪种日志进行输出");

        "log".Log("即将展示用于测试驱动的方式进行开发的测试日志：\n测试流程日志，用于定制测试流程，将不同步骤的日志调用，分散到正式的业务代码\n根据检测日志是否按照预定流程显示而确认 Bug 是否解决");
        Tester.Instance.AddAsserter("TestLog", false, new string[] { "测试流程1", "测试流程2", "测试流程3" });

        "log:{0}".AssertLog(logFilters, "TestLog", "测试流程1", "即将通过测试步骤 1");
        "log:{0}".AssertLog(logFilters, "TestLog", "测试流程2", "即将通过测试步骤 2");
        "log:{0}".AssertLog(logFilters, "TestLog", "测试流程3", "即将通过测试步骤 3，流程全部命中，测试通过");

        "log".Log("若测试日志未按照预定方式调用，则表示在开发过程或修改现有代码的时发生了问题：");
        Tester.Instance.AddAsserter("TestLog2", false, new string[] { "测试流程1", "测试流程2", "测试流程3" });

        "log:{0}".AssertLog(logFilters, "TestLog2", "测试流程1", "即将通过测试步骤 1");
        "log:{0}".AssertLog(logFilters, "TestLog2", "测试流程3", "若未按照预定步骤调用测试日志（正确的是流程2 应在 流程3 前显示）");
        "log:{0}".AssertLog(logFilters, "TestLog2", "测试流程2", "测试步骤未通过，开发出现了问题");
    }
}
