using UnityEngine;

namespace ShipDock
{
    public class ECSComponent<T> : Tenon, IECSComponent<T> where T : IECSData, new()
    {
        private ChunkInfo mChunkInfo;
        private ChunkGroup<T> mChunkManager;

        protected override void Purge()
        {
            int chunkIndex = mChunkInfo.chunkIndex;
            int itemIndex = mChunkInfo.itemIndex;
            mChunkManager.Drop(chunkIndex, itemIndex);
            mChunkManager = default;
        }

        protected override void CreateData()
        {
            base.CreateData();

            mChunkManager = ChunkGroup<T>.Instance;
            //mChunkManager.Pop(ref mChunkInfo);
        }

        //protected override void BindSystem(ref Tenons tenons, int systemID)
        //{
        //    tenons.BindSystem(this, systemID);
        //}

        //protected override void DebindSystem(ref Tenons tenons, int systemID)
        //{
        //    tenons.DebindSystem(this, systemID);
        //}

        public T GetData()
        {
            int chunkIndex = mChunkInfo.chunkIndex;
            int itemIndex = mChunkInfo.itemIndex;
            T result = mChunkManager.GetItem(chunkIndex, itemIndex);
            return result;
        }

        public void UpdateData(T value)
        {
            int chunkIndex = mChunkInfo.chunkIndex;
            int itemIndex = mChunkInfo.itemIndex;
            mChunkManager.UpdateItem(chunkIndex, itemIndex, value);
        }

        protected override void OnTenonFrameInit(float deltaTime)
        {
            base.OnTenonFrameInit(deltaTime);

            T data = GetData();
            if (data.IsChanged)
            {
                DataValid();
            }
            else { }
        }
    }

}