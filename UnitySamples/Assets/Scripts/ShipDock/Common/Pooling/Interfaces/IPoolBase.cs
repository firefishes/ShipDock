
namespace ShipDock
{
    public interface IPoolBase
    {
        IPoolable Create();
        void Reserve(ref IPoolable item);
    }

}