namespace ShipDock
{
    public interface IECSComponentBase : ITenon 
    {
        void BindEntity(int entity);
        void DebindEntity(int entity);
        IChunkGroup GetDataChunks();
    }
}