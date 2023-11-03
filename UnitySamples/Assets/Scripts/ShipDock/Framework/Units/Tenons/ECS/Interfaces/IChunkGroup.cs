using System;

namespace ShipDock
{
    public interface IChunkGroup
    {
        int GroupID { get; }
        int Total { get; }
        int GetChunkCount();
        void SetGroupID(int groupID);
        void TraverseAllChunkInfos(Action<ChunkInfo> onChunkInfo);
        IChunk GetChunkByIndex(int index);
        bool HasDataItemByEntity(int entity);
    }
}