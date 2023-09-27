using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ShipDock
{
    public class Tenons : IReclaim
    {
        private static int IDSeed;
        private static int instanceIndex;
        private static List<int> idleIDs = new List<int>();

        private static int GetAdvancedID()
        {
            int idleIDCount = idleIDs.Count;
            bool hasIdleID = idleIDCount > 0;
            int result = hasIdleID  ? idleIDs[0] : IDSeed;
            if (hasIdleID)
            {
                idleIDs.RemoveAt(0);
            }
            else
            {
                IDSeed++;
            }
            return result;
        }

        private ITenon mTenon;
        private List<ITenon> mTenonValues;
        private List<ITenon> mDropeds;
        private Dictionary<int, int> mInstanceCounts;
        private Dictionary<int, IECSSystem> mSystems;
        private IECSSystem[] mSystemQueues;
        private List<IECSSystem> mSystemList;
        private KeyValueList<int, ITenon> mMapper;
        private KeyValueList<int, List<ITenon>> mPoolings;

        public Tenons()
        {
            Init();
        }

        public void Reclaim()
        {
            DropsAll(true);
        }

        public void Clear()
        {
            DropsAll(false);
        }

        private void DropsAll(bool isDestroyPools = true)
        {
            int tenonType;
            List<int> keys = mMapper.Keys;
            mTenonValues = mMapper.Values;

            int max = mTenonValues.Count;
            for (int i = max; i >= 0; i--)
            {
                mTenon = mMapper[i];
                if (isDestroyPools) { }
                else
                {
                    mMapper[i] = default;
                    idleIDs.Add(keys[i]);
                }

                if (mTenon != default)
                {
                    tenonType = mTenon.GetTenonType();
                    bool flag = mPoolings.TryGetValue(tenonType, out List<ITenon> pooling);
                    if (flag) { }
                    else
                    {
                        if (isDestroyPools) { }
                        else
                        {
                            pooling = new List<ITenon>();
                            mPoolings[tenonType] = pooling;
                        }
                    }

                    if (isDestroyPools)
                    {
                        ((IReclaim)mTenon).Reclaim();
                    }
                    else
                    {
                        mTenon.Clear();
                        pooling.Add(mTenon);
                    }
                }
                else { }
            }

            if (isDestroyPools)
            {
                IDSeed = 0;
                instanceIndex = 0;

                mMapper.Clear();
                mMapper.TrimExcess();
                mPoolings.Clear();
                mPoolings.TrimExcess();
                mInstanceCounts.Clear();
                idleIDs.Clear();
                idleIDs.TrimExcess();
            }
            else { }

            mTenon = default;
            mTenonValues = default;
        }

        private void Init()
        {
            mInstanceCounts = new Dictionary<int, int>();
            mMapper = new KeyValueList<int, ITenon>();
            mPoolings = new KeyValueList<int, List<ITenon>>();
            mDropeds = new List<ITenon>();
            mSystems = new Dictionary<int, IECSSystem>();
            mSystemList = new List<IECSSystem>();

            mMapper.ApplyMapper();
            mPoolings.ApplyMapper();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T CreateTenonInstance<T>(int tenonType) where T : class, ITenon, new()
        {
            T result = new T();

            int count = GetAndUpdateInstanceCount(tenonType);
            result.SetInstanceIndex(count);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetAndUpdateInstanceCount(int tenonType, bool isReduce = default)
        {
            bool flag = mInstanceCounts.TryGetValue(tenonType, out int count);
            if (flag) { }
            else
            {
                count = 1;
            }

            int result = count;

            count = isReduce ? count - 1 : count + 1;
            count = Mathf.Max(0, count);
            mInstanceCounts[tenonType] = count;

            return result;
        }

        public void FillTenonsByType(int tenonType, out List<ITenon> outResult)
        {
            outResult = new List<ITenon>();
            mTenonValues = mMapper.Values;
            int max = mTenonValues.Count;
            for (int i = 0; i < max; i++)
            {
                mTenon = mTenonValues[i];
                if ((mTenon != default) && tenonType == mTenon.GetTenonType())
                {
                    outResult.Add(mTenon);
                }
                else { }
            }
            mTenon = default;
            mTenonValues = default;
        }

        public T GetTenon<T>(int tenonID) where T : class, ITenon
        {
            bool flag = mMapper.TryGetValue(tenonID, out ITenon tenon);
            T result = flag && (tenon != default) ? (T)tenon : default;
            return result;
        }

        public T AddTenonByType<T>(int tenonType) where T : class, ITenon, new()
        {
            T result = default;
            int id = GetAdvancedID();
            bool willCreateNew = default;
            bool flag = mMapper.TryGetValue(id, out ITenon tenon);
            if (flag && tenon != default)
            {
                result = (T)tenon;
            }
            else
            {
                flag = mPoolings.TryGetValue(tenonType, out List<ITenon> pooling);
                if (flag)
                {
                    int max = pooling.Count;
                    if (max > 0)
                    {
                        int index = max - 1;
                        result = (T)pooling[index];
                        pooling.RemoveAt(index);
                    }
                    else
                    {
                        willCreateNew = true;
                    }
                }
                else
                {
                    willCreateNew = true;

                    pooling = new List<ITenon>();
                    mPoolings[tenonType] = pooling;
                }

                if (willCreateNew)
                {
                    result = CreateTenonInstance<T>(tenonType);
                }
                else { }

                mMapper[id] = result;

                result.SetTenonID(id);
                result.SetTenonType(tenonType);
                result.SetupTenon(this);
            }
            return result;
        }

        private void CheckAndDrops()
        {
            int max = mDropeds.Count - 1;
            for (int i = max; i >= 0; i--)
            {
                mTenon = mDropeds[i];
                int tenonType = mTenon.GetTenonType();
                bool flag = mPoolings.TryGetValue(tenonType, out List<ITenon> pooling);
                if (flag)
                {
                    //不处理对象已存在池子的情况
                }
                else
                {
                    pooling = new List<ITenon>();
                    mPoolings[tenonType] = pooling;
                }

                int id = mTenon.GetTenonID();
                mMapper[id] = default;
                idleIDs.Add(id);

                pooling.Add(mTenon);
                mTenon.Clear();
                mDropeds.RemoveAt(i);
            }

            mTenon = default;
        }

        public void SimulateUpdateInit(float deltaTime)
        {
            CheckAndDrops();

            mTenonValues = mMapper.Values;
            int max = mTenonValues.Count;
            for (int i = 0; i < max; i++)
            {
                mTenon = mTenonValues[i];
                mTenon?.PerFrameInit(deltaTime);
            }

            mTenon = default;
            mTenonValues = default;
        }

        public void SimulateUpdate(float deltaTime)
        {
            mTenonValues = mMapper.Values;
            int max = mTenonValues.Count;
            for (int i = 0; i < max; i++)
            {
                mTenon = mTenonValues[i];
                if (mTenon != default)
                {
                    if (mTenon.IsDroped()) { }
                    else
                    {
                        mTenon.PerFrameReady(deltaTime);

                        if (mTenon.IsEnabled())
                        {
                            mTenon.PerFrame(deltaTime);
                        }
                        else { }
                    }
                }
                else { }
            }

            mTenon = default;
            mTenonValues = default;
        }

        public void SimulateLateUpdate()
        {
            mTenonValues = mMapper.Values;
            int max = mTenonValues.Count;
            for (int i = 0; i < max; i++)
            {
                mTenon = mTenonValues[i];
                if (mTenon != default)
                {
                    if (mTenon.IsDroped()) { }
                    else
                    {
                        if (mTenon.IsEnabled())
                        {
                            mTenon.PerFrameLate();
                        }
                        else { }
                    }
                }
                else { }
            }

            mTenon = default;
            mTenonValues = default;
        }

        public void SimulateFixtedUpdate(float deltaTime)
        {
            mTenonValues = mMapper.Values;
            int max = mTenonValues.Count;
            for (int i = 0; i < max; i++)
            {
                mTenon = mTenonValues[i];
                if (mTenon != default)
                {
                    if (mTenon.IsDroped()) { }
                    else
                    {
                        if (mTenon.IsEnabled())
                        {
                            mTenon.PerFrameFixed(deltaTime);
                        }
                        else { }
                    }
                }
                else { }
            }

            mTenon = default;
            mTenonValues = default;
        }

        public void SimulateUpdateEnd()
        {
            mTenonValues = mMapper.Values;
            int max = mTenonValues.Count;
            for (int i = 0; i < max; i++)
            {
                mTenon = mTenonValues[i];
                if (mTenon != default)
                {
                    if (mTenon.IsDroped())
                    {
                        mDropeds.Add(mTenon);
                    }
                    else
                    {
                        if (mTenon.IsEnabled())
                        {
                            mTenon.PerFrameEnd();
                        }
                        else { }
                    }
                }
                else { }
            }

            mTenon = default;
            mTenonValues = default;
        }

        public void RunSystems(float deltaTime)
        {
            if (mSystemQueues == default || mSystemQueues.Length == 0)
            {
                mSystemQueues = mSystemList.ToArray();
            }
            else { }

            int max = mSystemQueues.Length;
            if (max > 0)
            {
                IECSSystem system;
                for (int i = 0; i < max; i++)
                {
                    system = mSystemQueues[i];
                    system.Execute();
                }
            }
            else { }
        }

        public T AddSystem<T>() where T : IECSSystem, new()
        {
            IECSSystem system = new T();
            mSystems[system.SystemID] = system;
            mSystemList.Add(system);
            system.Init(this);
            return (T)system;
        }

        //public void BindSystem<T>(ITenon<T> tenon, int systemID) where T : ITenonData
        //{
        //    bool flag = mSystems.TryGetValue(systemID, out ITenonSystem system);
        //    if (flag)
        //    {
        //        system.BindData(tenon);
        //    }
        //    else { }
        //}

        //public void DebindSystem<T>(ITenon<T> tenon, int systemID) where T : ITenonData
        //{
        //    bool flag = mSystems.TryGetValue(systemID, out ITenonSystem system);
        //    if (flag)
        //    {
        //        system.DeBindData(tenon);
        //    }
        //    else { }
        //}
    }
}