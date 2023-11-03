using Spine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// �ڴ�ع�����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChunkGroup<T> : IChunkGroup where T : struct//IECSData, new()
    {
        /// <summary>�ڴ��Ĭ�ϵĴ�С, �Ա���Ϊ��λ</summary>
        public static int memorySizeInBytes = 14;
        /// <summary>�ڴ���鵥��</summary>
        private static ChunkGroup<T> instance;

        /// <summary>
        /// ��ʼ���ڴ����
        /// </summary>
        /// <param name="groupID">����</param>
        /// <param name="sizePerInstance">�ڴ���е���ʵ�����ڴ��С</param>
        /// <param name="preloadSize">Ԥ��������</param>
        /// <param name="totalBytesPerChunk">����ÿ���ڴ��Ĵ�С���Ա���Ϊ��λ</param>
        public static void Init(int groupID, int sizePerInstance = 0, int preloadSize = 0, int totalBytesPerChunk = 0)
        {
            if (instance == default)
            {
                instance = new ChunkGroup<T>(sizePerInstance, preloadSize, totalBytesPerChunk)
                {
                    GroupID = groupID
                };
            }
            else { }
        }

        public static ChunkGroup<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChunkGroup<T>();
                }
                else { }
                return instance;
            }
        }

        /// <summary>�����ڴ��Ĵ�С</summary>
        private int mSizePerChunk;
        /// <summary>�ڴ���д洢�Ķ���ռ�õ��ֽ���</summary>
        private int mObjectSize;
        /// <summary>�����ڴ��ռ�õ��ֽ���</summary>
        private int mTotalBytesPerChunk;
        /// <summary>�����ڴ��洢�Ķ�������</summary>
        private int mMaxObjects;
        /// <summary>ʵ��δ�þ����ڴ�����</summary>
        private Queue<Chunk<T>> mNotFulls;
        /// <summary>ʵ��δ�þ����ڴ��ӳ�䣬���ڱ�֤���ظ�����ʵ��δ������</summary>
        private Dictionary<Chunk<T>, bool> mNotFullsMap;
        /// <summary>�ڴ����������ʵ���ӳ��</summary>
        private Dictionary<int, ChunkInfo> mEntitasMap;
        /// <summary>���������ڴ��</summary>
        private List<Chunk<T>> mChunks;
        /// <summary>��ǰ����ʹ�õ��ڴ��</summary>
        private Chunk<T> mCurrentChunk;

        /// <summary>�ڴ�����</summary>
        public int GroupID { get; private set; }
        /// <summary>�Ƿ����ò��б���</summary>
        public bool ApplyParallelFor { get; set; } = true;

        public int Total
        {
            get
            {
                return mChunks.Count * mCurrentChunk.Size;
            }
        }

        public ChunkGroup(int sizePerInstance = 0, int preloadSize = 0, int totalBytesPerChunk = 0)
        {
            mObjectSize = sizePerInstance == 0 ? Marshal.SizeOf<T>() : sizePerInstance;
            mTotalBytesPerChunk = totalBytesPerChunk == 0 ? memorySizeInBytes : totalBytesPerChunk;
            mMaxObjects = mTotalBytesPerChunk * 1024 / mObjectSize;

            //Debug.Log("maxObjects " + maxObjects);
            mSizePerChunk = mMaxObjects;
            mChunks = new List<Chunk<T>>();
            mNotFulls = new Queue<Chunk<T>>();
            mNotFullsMap = new Dictionary<Chunk<T>, bool>();
            mEntitasMap = new Dictionary<int, ChunkInfo>();

            if (preloadSize > 0)
            {
                Chunk<T> chunk;
                preloadSize = (int)Mathf.Ceil(preloadSize / mSizePerChunk);
                for (int i = 0; i < preloadSize; i++)
                {
                    chunk = CreateChunk();
                    if (i == 0)
                    {
                        mCurrentChunk = chunk;
                    }
                    else 
                    {
                        mNotFulls.Enqueue(chunk);
                        mNotFullsMap[chunk] = true;
                    }
                }
            }
            else
            {
                mCurrentChunk = CreateChunk();
            }

            LogChunkInfo();
        }

        public void LogChunkInfo()
        {
            const string logFormater = "Chunk created: Size of bytes per instance = {0}B. Size of bytes per chunk = {1}B. objects count per chunk = {2}. group id = {3}";
            string log = string.Format(logFormater, mObjectSize, mTotalBytesPerChunk, mMaxObjects, GroupID);
            "log".Log(log);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="groupID"></param>
        public void SetGroupID(int groupID)
        {
            if (GroupID == 0)
            {
                GroupID = groupID;
            }
            else
            {
                throw new Exception("Do not allow set ChunkGroup id more than once");
            }
        }

        /// <summary>
        /// ���ڴ�����л�ȡһ��δ�õ�ʵ��
        /// </summary>
        /// <param name="entity">ʵ����</param>
        /// <param name="info">�ڴ����Ϣ</param>
        /// <returns></returns>
        public T Pop(int entity, ref ChunkInfo info)
        {
            if (mCurrentChunk.IsFull)
            {
                if (mNotFulls.Count > 0)
                {
                    mCurrentChunk = mNotFulls.Dequeue();

                    if (mNotFullsMap.TryGetValue(mCurrentChunk, out _))
                    {
                        mNotFullsMap.Remove(mCurrentChunk);
                    }
                    else { }
                }
                else
                {
                    mCurrentChunk = CreateChunk();
                }
            }
            else { }

            T result = mCurrentChunk.Pop(entity, out int index);
            info.itemIndex = index;
            info.chunkIndex = mCurrentChunk.ChunkIndex;
            info.entity = mCurrentChunk.GetEntityByIndex(index, out _);

            mEntitasMap[entity] = info;

            return result;
        }

        /// <summary>
        /// �����ڴ�����е�ʵ��
        /// </summary>
        /// <param name="chunkIndex"></param>
        /// <param name="itemIndex"></param>
        public void Drop(int chunkIndex, int itemIndex)
        {
            Chunk<T> chunk = mChunks[chunkIndex];
            int entity = chunk.GetEntityByIndex(itemIndex, out bool isValid);
            chunk.Drop(itemIndex);

            if (isValid)
            {
                mEntitasMap.Remove(entity);
            }
            else { }

            if (mNotFullsMap.TryGetValue(chunk, out _)) { }
            else
            {
                mNotFulls.Enqueue(chunk);
                mNotFullsMap[chunk] = true;
            }
        }

        /// <summary>
        /// ��ȡ��ǰ�ڴ�������ڴ�������
        /// </summary>
        /// <returns></returns>
        public int GetChunkCount()
        {
            return mChunks.Count;
        }

        /// <summary>
        /// ����һ���ڴ��
        /// </summary>
        /// <returns></returns>
        private Chunk<T> CreateChunk()
        {
            Chunk<T> newChunk = new(mSizePerChunk)
            {
                ChunkIndex = GetChunkCount(),
                Group = this,
            };
            mChunks.Add(newChunk);

            Parallel.For(0, mSizePerChunk, i =>
            {
                T item = new();
                newChunk.Ready(i, item);
            });

            return newChunk;
        }

        /// <summary>
        /// ��ȡһ���ڴ���е�ʵ��
        /// </summary>
        /// <param name="chunkIndex"></param>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T GetItem(int chunkIndex, int itemIndex)
        {
            if (chunkIndex < 0 || chunkIndex >= GetChunkCount())
            {
                throw new IndexOutOfRangeException("Invalid chunk index");
            }
            else { }

            Chunk<T> chunk = mChunks[chunkIndex];
            return chunk.Get(itemIndex);
        }

        public bool HasDataItemByEntity(int entity)
        {
            bool result = mEntitasMap.TryGetValue(entity, out ChunkInfo info);
            if (result)
            {
                result = info.entity == entity;
                if (result) { }
                else
                {
                    throw new Exception("Invalid entity value during get item");
                }
            }
            else { }
            return result;
        }

        public T GetItemByEntity(int entity, out bool isValid)
        {
            T result = default;
            isValid = HasDataItemByEntity(entity);
            if (isValid)
            {
                ChunkInfo info = mEntitasMap[entity];
                result = GetItem(info.chunkIndex, info.chunkIndex);
            }
            else { }
            return result;
        }

        /// <summary>
        /// �����ڴ���е�ʵ��
        /// </summary>
        /// <param name="chunkIndex"></param>
        /// <param name="itemIndex"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void ChangeItem(int chunkIndex, int itemIndex, T value)
        {
            if (chunkIndex < 0 || chunkIndex >= GetChunkCount())
            {
                throw new IndexOutOfRangeException("Invalid chunk index");
            }
            else { }

            Chunk<T> chunk = mChunks[chunkIndex];
            chunk.ChangeItemByIndex(itemIndex, value);
        }

        //private ParallelOptions mOption = new()
        //{
        //    MaxDegreeOfParallelism = 3,
        //};

        /// <summary>
        /// ������ʼλ�ú��յ�λ�ñ����ڴ������ڴ���ʵ��
        /// </summary>
        /// <param name="startChunkIndex"></param>
        /// <param name="endChunkIndex"></param>
        /// <param name="onDataItem"></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private void Traverse(int startChunkIndex, int endChunkIndex, Action<T> onDataItem = default, Action<ChunkInfo> onChunkInfo = default)
        {
            int count = GetChunkCount();
            if (startChunkIndex < 0 || startChunkIndex >= count)
            {
                throw new IndexOutOfRangeException("Invalid start chunk index");
            }
            else { }

            if (endChunkIndex < 0 || endChunkIndex >= count)
            {
                throw new IndexOutOfRangeException("Invalid end chunk index");
            }
            else { }

            if (ApplyParallelFor)
            {
                //Debug.Log("startChunkIndex, endChunkIndex + 1" + endChunkIndex + 1);
                Parallel.For(startChunkIndex, endChunkIndex + 1, i =>
                {
                    T item;
                    Chunk<T> chunk = mChunks[i];
                    int count = chunk.Count;
                    for (int j = 0; j < count; j++)
                    {
                        if (chunk.IsValid(j))
                        {
                            //Demo.a++;
                            item = chunk.Get(j);
                            if (onChunkInfo == default) { }
                            else
                            {
                                ChunkInfo info = new()
                                {
                                    entity = chunk.GetEntityByIndex(j, out _),
                                    chunkIndex = chunk.ChunkIndex,
                                    itemIndex = j,
                                };
                                onChunkInfo.Invoke(info);
                            }

                            onDataItem?.Invoke(item);
                        }
                        else { }
                    }
                });
            }
            else
            {
                //Debug.Log("startChunkIndex, endChunkIndex + 1" + endChunkIndex + 1);
                T item;
                Chunk<T> chunk;
                int len = endChunkIndex + 1;
                for (int i = startChunkIndex; i < len; i++)
                {
                    chunk = mChunks[i];
                    for (int j = 0; j < chunk.Count; j++)
                    {
                        if (chunk.IsValid(j))
                        {
                            item = chunk.Get(j);
                            onDataItem(item);
                        }
                        else { }
                    }
                }
            }
        }

        /// <summary>
        /// �����ڴ������ڴ�������ʵ��
        /// </summary>
        /// <param name="onDataItem"></param>
        public void TraverseAll(Action<T> onDataItem)
        {
            int count = GetChunkCount();
            Traverse(0, count - 1, onDataItem);
        }

        public void TraverseAllChunkInfos(Action<ChunkInfo> onChunkInfo)
        {
            int count = GetChunkCount();
            Traverse(0, count - 1, default, onChunkInfo);
        }

        public IChunk GetChunkByIndex(int index)
        {
            return mChunks.Count > index ? mChunks[index] : default;
        }
    }
}