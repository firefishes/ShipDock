using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace ShipDock
{
    public struct ChunkInfo
    {
        public int chunkIndex;
        public int itemIndex;
    }

    public interface IChunkGroup
    {
        int GroupID { get; }
        void SetGroupID(int groupID);
        //T Pop(int entitas, ref ChunkInfo info);
    }

    /// <summary>
    /// 内存池管理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChunkGroup<T> : IChunkGroup where T : IECSData, new()
    {
        public static int memorySizeInBytes = 14;

        private static ChunkGroup<T> instance;

        public static void Init(int groupID, int sizePerInstance = 0, int preloadSize = 0, int totalBytesPerChunk = 14)
        {
            if (instance == default)
            {
                instance = new ChunkGroup<T>(sizePerInstance, preloadSize, totalBytesPerChunk);
                instance.GroupID = groupID;
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

        private int mSizePerChunk;
        private Queue<Chunk<T>> mNotFulls;
        private List<Chunk<T>> mChunks;
        private Chunk<T> mCurrentChunk;

        public int GroupID { get; private set; }
        public bool ApplyParallelFor { get; set; } = true;

        public ChunkGroup(int sizePerInstance = 0, int preloadSize = 0, int totalBytesPerChunk = 14)
        {
            int objectSize = sizePerInstance == 0 ? Marshal.SizeOf<T>() : sizePerInstance;
            int maxObjects = totalBytesPerChunk * 1024 / objectSize;

            //Debug.Log("maxObjects " + maxObjects);
            mSizePerChunk = maxObjects;
            mChunks = new List<Chunk<T>>();
            mNotFulls = new Queue<Chunk<T>>();

            mCurrentChunk = CreateChunk();

            if (preloadSize > 0)
            {
                preloadSize = (int)Mathf.Ceil(preloadSize / mSizePerChunk);
                Chunk<T> chunk;
                for (int i = 0; i < preloadSize; i++)
                {
                    chunk = CreateChunk();
                    mNotFulls.Enqueue(chunk);
                }
            }
            else { }
        }

        public void SetGroupID(int groupID)
        {
            if (GroupID == 0)
            {
                GroupID = groupID;
            }
            else { }
        }

        public T Pop(int entitas, ref ChunkInfo info)
        {
            if (mCurrentChunk.IsFull)
            {
                if (mNotFulls.Count > 0)
                {
                    mCurrentChunk = mNotFulls.Dequeue();
                }
                else
                {
                    mCurrentChunk = CreateChunk();
                }
            }
            else { }

            T result = mCurrentChunk.Pop(entitas, out int index);
            info.chunkIndex = mCurrentChunk.ChunkIndex;
            info.itemIndex = index;

            return result;
        }

        public void Drop(int chunkIndex, int itemIndex)
        {
            Chunk<T> chunk = mChunks[chunkIndex];
            chunk.Drop(itemIndex);

            mNotFulls.Enqueue(chunk);
        }

        public int GetChunkCount()
        {
            return mChunks.Count;
        }

        private Chunk<T> CreateChunk()
        {
            Chunk<T> newChunk = new(mSizePerChunk)
            {
                ChunkIndex = GetChunkCount()
            };
            mChunks.Add(newChunk);

            Parallel.For(0, mSizePerChunk - 1, i =>
            {
                newChunk.Add(i, new T());
            });

            return newChunk;
        }

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

        public void UpdateItem(int chunkIndex, int itemIndex, T value)
        {
            if (chunkIndex < 0 || chunkIndex >= GetChunkCount())
            {
                throw new IndexOutOfRangeException("Invalid chunk index");
            }
            else { }

            Chunk<T> chunk = mChunks[chunkIndex];
            chunk.Update(itemIndex, value);
        }

        //private ParallelOptions mOption = new()
        //{
        //    MaxDegreeOfParallelism = 3,
        //};

        public void Traverse(int startChunkIndex, int endChunkIndex, Action<T> action)
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
                    for (int j = 0; j < chunk.Count; j++)
                    {
                        if (chunk.IsValid(j))
                        {
                            Demo.a++;
                            item = chunk.Get(j);
                            action(item);
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
                            action(item);
                        }
                        else { }
                    }
                }
            }
        }

        public void TraverseAll(Action<T> action)
        {
            int count = GetChunkCount();
            Traverse(0, count - 1, action);
        }
    }

    /// <summary>
    /// 内存池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Chunk<T> where T : IECSData, new()
    {
        private T[] mData;
        private int[] mValids;
        private int[] mReuseds;
        private int mReusedIndex;

        public bool IsFull
        {
            get
            {
                return Count == mData.Length;
            }
        }

        public int Count { get; private set; }
        public int ChunkIndex { get; set; }

        public Chunk(int size)
        {
            //Debug.Log("size "+ size);
            mData = new T[size];
            mValids = new int[size];
            mReuseds = new int[size];
            Count = 0;
        }

        public void Clear()
        {
            int max = Count;
            for (int i = 0; i < max; i++)
            {
                mValids[i] = 0;
            }

            Count = 0;
            mReusedIndex = 0;
        }

        public void Add(int index, T item)
        {
            if (index < mData.Length)
            {
                mData[index] = item;
            }
            else { }
        }

        public T Get(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }
            else { }

            return mData[index];
        }

        public void Update(int index, T value)
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

        public bool IsValid(int index)
        {
            if (index >= mValids.Length)
            {
                throw new Exception("Error index " + typeof(T).Name + ", length = " + mValids.Length + ", index is " + index);
            }
            else { }
            return mValids[index] != 0;
        }

        public void Drop(int index)
        {
            if (IsValid(index)) { }
            else
            {
                throw new Exception("Try drop a invalid data, class is " + typeof(T).Name);
            }

            mValids[index] = 0;
            Count--;

            mReuseds[mReusedIndex] = index;
            mReusedIndex++;
        }

        public T Pop(int entitas, out int index)
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

            Count++;

            if (IsValid(index))
            {
                throw new Exception("Try get a valid data, class is " + typeof(T).Name);
            }
            else { }

            mValids[index] = 1;
            T result = mData[index];
            return result;
        }
    }

    //public class TenonSystemDatas2<T> : ITenonSystemDatas<T> where T : ITenonData
    //{
    //    public void AddData(int tenonID, T dataItem)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public T GetData(int tenonID)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Action<int, T> GetExecuter()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void OnExecuter(Action<int, T> action)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void RemoveData(int tenonID, T dataItem)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SettleDatas(ref Tenons tenons, ITenonSystem system)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class TenonSystemDatas<T> : ITenonSystemDatas<T> where T : IECSData
    {
        private T mItem;
        private bool mIsInited;
        private ITenon mTenon;
        private List<int> mKeys;
        private List<T> mValues;
        //private Queue<int> mEmptyIndexs = new();
        private Action<int, T> mExecuter;
        private KeyValueList<int, T> mDatas = new();

        public Action<int, T> GetExecuter()
        {
            return mExecuter;
        }

        public T GetData(int tenonID)
        {
            bool flag = mDatas.TryGetValue(tenonID, out T result);
            return flag ? result : default;
        }

        public void AddData(int tenonID, T dataItem)
        {
            if (mIsInited) { }
            else 
            {
                mIsInited = true;
                mDatas.ApplyMapper();
            }

            //if (mEmptyIndexs.Count > 0)
            //{
            //    int index = mEmptyIndexs.Dequeue();
            //    mDatas.Keys[index] = tenonID;
            //    mDatas.Values[index] = dataItem;
            //}
            //else 
            //{
            //}
            mDatas.Put(tenonID, dataItem);
        }

        public void RemoveData(int tenonID, T dataItem)
        {
            //int index = mDatas.KeyIndex(tenonID);
            //if (index >= 0)
            //{
            //    mEmptyIndexs.Enqueue(tenonID);
            //}
            //else { }
            mDatas.Put(tenonID, default);
        }

        public void OnExecuter(Action<int, T> action)
        {
            mExecuter = action;
        }

        public void SettleDatas(ref Tenons tenons, IECSSystem system)
        {
            mValues = mDatas.Values;
            int max = mValues.Count;
            if (max > 0 && mExecuter != default)
            {
                int key;
                mKeys = mDatas.Keys;
                for (int i = 0; i < max; i++)
                {
                    key = mKeys[i];
                    mItem = mValues[i];
                    if (mItem != null)
                    {
                        //mTenon = mItem.Tenon;//.IsBlock
                        //mTenon = tenons.GetTenon<ITenon>(key);
                        if (mTenon.IsBlock())
                        {
                            //忽略启用了阻断标记的组件数据
                        }
                        else
                        {
                            if (mTenon.IsDataChanged())
                            {
                                mExecuter.Invoke(key, mItem);
                            }
                            else { }
                        }
                    }
                    else { }
                }
                mItem = default;
                mTenon = default;
                mValues = default;
                mKeys = default;
            }
            else { }
        }
    }

    public abstract class ECSSystem : IECSSystem
    {
        protected Tenons mTenons;
        
        //private Dictionary<int, ITenonSystemDataCreater> mDataCreaters;

        public abstract int SystemID { get; }

        public abstract void Execute();

        protected virtual void DuringExecute<T>(T data) where T : IECSData
        {
            //ITenonSystemDatas<T> dataCreater = GetDataCreater<T>(tenonType);
            //dataCreater?.SettleDatas(ref mTenons, this);
        }

        public virtual void Init(Tenons tenons)
        {
            mTenons = tenons;
            //mDataCreaters = new Dictionary<int, ITenonSystemDataCreater>();
        }

        //public void AddDataCreater<T>(int tenonType, ITenonSystemDatas<T> datas) where T : ITenonData
        //{
        //    mDataCreaters[tenonType] = datas;
        //}

        //public ITenonSystemDatas<T> GetDataCreater<T>(int tenonType) where T : ITenonData
        //{
        //    bool flag = mDataCreaters.TryGetValue(tenonType, out ITenonSystemDataCreater result);
        //    return flag ? result as ITenonSystemDatas<T> : default;
        //}

        //public void BindData<T>(ITenon<T> tenon) where T : ITenonData
        //{
        //    int tenonType = tenon.GetTenonType();
        //    ITenonSystemDatas<T> creater = GetDataCreater<T>(tenonType);

        //    int tenonID = tenon.GetTenonID();
        //    T dataItem = tenon.GetData();
        //    creater.AddData(tenonID, dataItem);
        //}

        //public void DeBindData<T>(ITenon<T> tenon) where T : ITenonData
        //{
        //    int tenonType = tenon.GetTenonType();
        //    ITenonSystemDatas<T> creater = GetDataCreater<T>(tenonType);

        //    int tenonID = tenon.GetTenonID();
        //    T dataItem = tenon.GetData();
        //    creater.RemoveData(tenonID, dataItem);
        //}
    }

}