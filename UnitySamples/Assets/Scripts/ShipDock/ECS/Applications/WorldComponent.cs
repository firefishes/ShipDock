namespace ShipDock.Applications
{
    public abstract class WorldComponent : StaticWorldComponent
    {
        public virtual int WorldGroupComponentName { get; } = int.MaxValue;
        public abstract int BehaviaourIDsComponentName { get; }

        //public override void Init(IShipDockComponentContext context)
        //{
        //    base.Init(context);
        //}
    }
}