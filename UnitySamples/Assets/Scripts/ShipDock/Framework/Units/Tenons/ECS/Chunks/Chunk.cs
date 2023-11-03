using Spine;
using System;

namespace ShipDock
{
    public interface IChunk
    {
        int Size { get; }
        void Clear();
        void Drop(int index);
        bool IsValid(int index);
        int GetEntityByIndex(int index, out bool isValid);
    }

    /// <summary>
    /// 内存池块
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Chunk<T> : IChunk where T : struct//IECSData, new()
    {
        /// <summary>表示未用数据的标识值</summary>
        public const int NULL_DATA = -1;

        /// <summary>内存池中的数据</summary>
        private T[] mData;
        /// <summary>表示内存池中数据是否有效的标识</summary>
        private int[] mValids;
        /// <summary>池中可以重用的数据索引</summary>
        private int[] mReuseds;
        /// <summary>指向当前池中可以重用的数据索引的位置</summary>
        private int mReusedIndex;

        /// <summary>内存池块是否已满</summary>
        public bool IsFull
        {
            get
            {
                return Count == mData.Length;
            }
        }

        public int Size { get; private set; }

        /// <summary>当前内存池块中已使用的实例数量</summary>
        public int Count { get; private set; }
        /// <summary>内存池块在内存块组中的索引</summary>
        public int ChunkIndex { get; set; }

        public ChunkGroup<T> Group { get; set; }

        public Chunk(int size)
        {
            Size = size;

            Count = 0;
            mData = new T[size];
            mValids = new int[size];
            mReuseds = new int[size];
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            int max = Count;
            for (int i = 0; i < max; i++)
            {
                mValids[i] = NULL_DATA;
            }

            Count = 0;
            mReusedIndex = 0;
            Group = default;
        }

        /// <summary>
        /// 内存池块初始化
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Ready(int index, T item)
        {
            if (index < mData.Length)
            {
                mData[index] = item;
                mValids[index] = NULL_DATA;
            }
            else { }
        }

        /// <summary>
        /// 从内存池块中获取实例对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T Get(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }
            else { }

            return mData[index];
        }

        /// <summary>
        /// 更新内存池块中的实例
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        public void ChangeItemByIndex(int index, T value)
        {
            if (IsValid(index))
            {
                mData[index] = value;
            }
            else
            {
                throw new Exception("Try update a invalid data, class is " + typeof(T).Name);
            }
        }

        /// <summary>
        /// 根据参数检测内存池块中对应位置的实例是否正在使用
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool IsValid(int index)
        {
            if (index >= mValids.Length)
            {
                throw new Exception("Error index " + typeof(T).Name + ", length = " + mValids.Length + ", index is " + index);
            }
            else { }
            return mValids[index] != NULL_DATA;
        }

        /// <summary>
        /// 弃用内存池块中指定位置的实例
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="Exception"></exception>
        public void Drop(int index)
        {
            if (IsValid(index)) { }
            else
            {
                throw new Exception("Try drop a invalid data, class is " + typeof(T).Name);
            }

            mValids[index] = NULL_DATA;
            Count--;

            mReuseds[mReusedIndex] = index;
            mReusedIndex++;
        }

        /// <summary>
        /// 从内存池块中取出一个实例
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T Pop(int entity, out int index)
        {
            if (IsFull)
            {
                index = -1;
                return default;
            }
            else { }

            if (mReusedIndex > 0)
            {
                index = mReuseds[mReusedIndex];
                mReusedIndex--;
            }
            else
            {
                index = Count;
            }

            if (IsValid(index))
            {
                Group?.LogChunkInfo();

                const string formater = "Get a valid data during pop, class is {0}, index of chunk is {1}, value of index is {2}, entity is {3}";
                string content = string.Format(formater, typeof(T).Name, ChunkIndex, index, entity);
                throw new Exception(content);
            }
            else { }

            Count++;

            mValids[index] = entity;
            T result = mData[index];
            return result;
        }

        public int GetEntityByIndex(int index, out bool isValid)
        {
            int result = NULL_DATA;
            isValid = IsValid(index);
            if (isValid)
            {
                result = mValids[index];
            }
            return result;
        }
    }
}