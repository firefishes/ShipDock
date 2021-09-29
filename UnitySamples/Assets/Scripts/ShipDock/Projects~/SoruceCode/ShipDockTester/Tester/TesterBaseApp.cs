namespace ShipDock.Testers
{
    public class TesterBaseApp : ITester
    {
        public virtual string Name { get; set; } = "TesterBaseApp";

        public TesterBaseApp()
        {
            Tester tester = Tester.Instance;
            tester.AddTester(this);
            tester.AddLogger(this, "debug", "{0}");
            tester.AddLogger(this, "log", "log:{0} ");
            tester.AddLogger(this, "warning", "warning:{0} ");
            tester.AddLogger(this, "error", "error:{0} ");
            tester.AddLogger(this, "todo", "todo:{0} ");
            tester.AddLogger(this, "notice add", "log: {0}- Notice {1} registered");
            tester.AddLogger(this, "notice send", "log: {0}- Send notice {1} ");
            tester.AddLogger(this, "notice rm", "log: {0}- Remove notice {1}");
            tester.AddLogger(this, "pool type error", "error: A wrong pooling revert, instance type is {0}");
            tester.AddLogger(this, "asset priv", "log: Asset is from priv: {0}, path key is: {1}");
            tester.AddLogger(this, "asset path", "log: Asset path is : {0}");
            tester.AddLogger(this, "loader failed", "error: Loader load failed, Url is {0}");
            tester.AddLogger(this, "loader success", "log: Loader successed: {0}");
            tester.AddLogger(this, "loader deps", "log: 资源加载器已完成，开始加载下一个依赖资源: {0}");
            tester.AddLogger(this, "empty deps", "warning: 资源依赖为空");
            tester.AddLogger(this, "deps", "log: 资源依赖关系如下（被依赖资源优先加载）: {0} 依赖于 {1}");
            tester.AddLogger(this, "walk deps error", "error: 资源依赖递归层数超出定义的最大值, {0} 与 {1} 之间存在循环依赖");
            tester.AddLogger(this, "load res", "log: 加载依赖资源，加载的开端文件位置: {0}");
            tester.AddLogger(this, "fsm changed", "{0} FSM state changed : {1} -> {2}");
            tester.AddLogger(this, "fsm state repeate", "error: state repeate, FSM name is {0}");
            tester.AddLogger(this, "wr file", "Writing File: {0}");
        }
    }
}