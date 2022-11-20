using ShipDock.Modulars;

namespace IsKing
{
    public abstract class BaseModular : QueueableNoticesModular
    {
        public BaseModular(int modularName) : base(modularName)
        {
        }
    }

}