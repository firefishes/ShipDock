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
    /// �ڴ�ؿ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Chunk<T> : IChunk where T : struct//IECSData, new()
    {
        /// <summary>��ʾδ�����ݵı�ʶֵ</summary>
        public const int NULL_DATA = -1;

        /// <summary>�ڴ���е�����</summary>
        private T[] mData;
        /// <summary>��ʾ�ڴ���������Ƿ���Ч�ı�ʶ</summary>
        private int[] mValids;
        /// <summary>���п������õ���������</summary>
        private int[] mReuseds;
        /// <summary>ָ��ǰ���п������õ�����������λ��</summary>
        private int mReusedIndex;

        /// <summary>�ڴ�ؿ��Ƿ�����</summary>
        public bool IsFull
        {
            get
            {
                return Count == mData.Length;
            }
        }

        public int Size { get; private set; }

        /// <summary>��ǰ�ڴ�ؿ�����ʹ�õ�ʵ������</summary>
        public int Count { get; private set; }
        /// <summary>�ڴ�ؿ����ڴ�����е�����</summary>
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
        /// ����
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
        /// �ڴ�ؿ��ʼ��
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
        /// ���ڴ�ؿ��л�ȡʵ������
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
        /// �����ڴ�ؿ��е�ʵ��
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
        /// ���ݲ�������ڴ�ؿ��ж�Ӧλ�õ�ʵ���Ƿ�����ʹ��
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
        /// �����ڴ�ؿ���ָ��λ�õ�ʵ��
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
        /// ���ڴ�ؿ���ȡ��һ��ʵ��
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