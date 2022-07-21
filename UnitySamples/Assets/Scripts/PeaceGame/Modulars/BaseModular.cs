using ShipDock.Modulars;

namespace Peace
{
    public abstract class BaseModular : QueueableNoticesModular
    {
        public BaseModular(int modularName) : base(modularName)
        {
        }
    }

}