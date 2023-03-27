namespace ShipDock
{
    public class ShipDockTicks : Singletons<ShipDockTicks>
    {
        public TicksUpdater TicksUpdater { get; private set; }

        public ShipDockTicks()
        {
            TicksUpdater = new TicksUpdater(60);
        }
    }
}
