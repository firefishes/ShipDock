using System;
using System.Collections.Generic;

namespace ShipDock
{
    /// <summary>
    /// ECS 组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ECSComponent<T> : Tenon, IECSComponent<T> where T : struct//IECSData//, new()
    {
        private readonly static T NULL_DATA = new();

        /// <summary>组件对应的内存块组</summary>
        private ChunkGroup<T> mChunkGroup;
        /// <summary>组件已经绑定的实体 id</summary>
        private Dictionary<int, ChunkInfo> mBindedEntias;

        protected override void Purge()
        {
            mChunkGroup = default;
        }

        protected override void Reset()
        {
            base.Reset();

            mBindedEntias = new();
        }

        protected override void CreateData()
        {
            base.CreateData();

            mChunkGroup = ChunkGroup<T>.Instance;
        }

        /// <summary>
        /// 将组件数据与实体绑定
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="Exception"></exception>
        public void BindEntity(int entity)
        {
            ECS.Instance.AllEntitas.GetEntityByID(entity, out bool isValid);
            if (isValid)
            {
                if (mBindedEntias.TryGetValue(entity, out _))
                {
                    throw new Exception("Do not allow bind component to a entity more than once");
                }
                else
                {
                    ChunkInfo info = default;
                    mChunkGroup.Pop(entity, ref info);
                    mBindedEntias[entity] = info;
                }
            }
            else { }
        }

        /// <summary>
        /// 将组件数据与实体解绑
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="Exception"></exception>
        public void DebindEntity(int entity)
        {
            ECS.Instance.AllEntitas.GetEntityByID(entity, out bool isValid);
            if (isValid)
            {
                if (mBindedEntias.TryGetValue(entity, out ChunkInfo info))
                {
                    mBindedEntias.Remove(entity);
                    mChunkGroup.Drop(info.chunkIndex, info.itemIndex);
                }
                else
                {
                    throw new Exception("Do not allow debind component to a entity what no binds");
                }
            }
            else { }
        }

        public T GetEntityData(int entity, out bool hasData)
        {
            hasData = false;
            T result = default;

            ECS.Instance.AllEntitas.GetEntityByID(entity, out bool isValid);
            if (isValid)
            {
                hasData = mBindedEntias.TryGetValue(entity, out ChunkInfo info);
                if (hasData)
                {
                    result = mChunkGroup.GetItem(info.chunkIndex, info.itemIndex);
                }
                else
                {
                    throw new Exception("Do not allow debind component to a entity what no binds");
                }
            }
            else { }

            return result;
        }

        public IChunkGroup GetDataChunks()
        {
            return mChunkGroup;
        }

        public T GetNullData()
        {
            return NULL_DATA;
        }

        //public void UpdateData(T value)
        //{
        //    int chunkIndex = mChunkInfo.chunkIndex;
        //    int itemIndex = mChunkInfo.itemIndex;
        //    mChunkGroup.ChangeItem(chunkIndex, itemIndex, value);
        //}

        //protected override void OnTenonFrameInit(float deltaTime)
        //{
        //    base.OnTenonFrameInit(deltaTime);

        //    T data = GetData();
        //    if (data.IsChanged)
        //    {
        //        DataValid();
        //    }
        //    else { }
        //}
    }

}