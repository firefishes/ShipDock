using System;
using System.Collections.Generic;

namespace ShipDock
{
    /// <summary>
    /// ECS 单例
    /// </summary>
    public class ECS : Singletons<ECS>
    {
        /// <summary>所有内存块组</summary>
        private Dictionary<int, IECSSystem> mSystems;
        /// <summary>所有需要执行的系统数组</summary>
        private IECSSystem[] mSystemExecutes;
        /// <summary>所有系统的列表</summary>
        private List<IECSSystem> mSystemList;
        /// <summary>所有内存块组的映射</summary>
        private Dictionary<int, IChunkGroup> mAllChunkGroups;
        /// <summary>所有组件的类型映射</summary>
        private Dictionary<Type, ECSComponentInfo> mAllComponentTypes;
        /// <summary>所有组件的ID映射</summary>
        private Dictionary<int, ECSComponentInfo> mAllComponentIDs;
        /// <summary>所有组件的内存块组映射</summary>
        private Dictionary<int, ECSComponentInfo> mAllComponentChunkGroups;

        private IChunkGroup mChunkChecking;
        private IECSSystem mSystemChecking;
        private List<IChunkGroup> mChunkGroupsSearched;
        private int mEntitasSearchResultIndex;
        private List<EntitySearchResult> mEntitasSearchResult;

        /// <summary>所有实体</summary>
        public ECSEntitas AllEntitas { get; private set; }
        public int MemorySizeInBytes { get; set; } = 14;

        /// <summary>
        /// 初始化 ECS 模块
        /// </summary>
        public void InitECS()
        {
            mSystemList = new List<IECSSystem>();
            mSystems = new Dictionary<int, IECSSystem>();
            mAllChunkGroups = new Dictionary<int, IChunkGroup>();
            mAllComponentTypes = new Dictionary<Type, ECSComponentInfo>();
            mAllComponentIDs = new Dictionary<int, ECSComponentInfo>();
            mAllComponentChunkGroups = new Dictionary<int, ECSComponentInfo>();
            mChunkGroupsSearched = new List<IChunkGroup>();
            //mEntitasSearchResult = new List<EntitySearchResult>();

            Tenons tenons = ShipDockApp.Instance.Tenons;
            AllEntitas = tenons.AddTenonByType<ECSEntitas>(ShipDockConsts.TENON_ECS_ALL_ENTITAS);
        }

        /// <summary>
        /// 初始化新的内存块组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupID"></param>
        /// <param name="sizePerInstance"></param>
        /// <param name="preloadSize"></param>
        /// <param name="totalBytesPerChunk"></param>
        public void InitChunkGroup<T>(int groupID, int sizePerInstance = 0, int preloadSize = 0, int totalBytesPerChunk = 0) where T : struct// IECSData, new()
        {
            ChunkGroup<T>.memorySizeInBytes = MemorySizeInBytes;
            ChunkGroup<T>.Init(groupID, sizePerInstance, preloadSize, totalBytesPerChunk);
            mAllChunkGroups[groupID] = ChunkGroup<T>.Instance;
        }

        /// <summary>
        /// 获取内存块组
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public IChunkGroup GetChunkGroup(int groupID)
        {
            mAllChunkGroups.TryGetValue(groupID, out IChunkGroup result);
            return result;
        }

        /// <summary>
        /// 创建新的 ECS 组件
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentID"></param>
        public void CreateComponent<C, T>(int componentID) where C : class, IECSComponentBase, new()
        {
            Type type = typeof(T);
            if (mAllComponentTypes.TryGetValue(type, out _)) { }
            else
            {
                Tenons tenons = ShipDockApp.Instance.Tenons;
                C component = tenons.AddTenonByType<C>(componentID);
                IChunkGroup chunkGroup = component.GetDataChunks();

                ECSComponentInfo info = new()
                {
                    instanceID = component.GetTenonID(),
                    componentID = componentID,
                };
                mAllComponentTypes[type] = info;
                mAllComponentIDs[componentID] = info;
                mAllComponentChunkGroups[chunkGroup.GroupID] = info;
            }
        }

        /// <summary>
        /// 获取 ECS 组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentID"></param>
        /// <returns></returns>
        public IECSComponent<T> GetComponent<T>(int componentID) where T : struct//IECSData
        {
            IECSComponent<T> result = default;
            Tenons tenons = ShipDockApp.Instance.Tenons;
            bool flag = mAllComponentIDs.TryGetValue(componentID, out ECSComponentInfo info);
            if (flag)
            {
                result = tenons.GetTenon<IECSComponent<T>>(info.instanceID);
            }
            else { }
            return result;
        }

        /// <summary>
        /// 以返回基类的形式获取 ECS 组件
        /// </summary>
        /// <param name="componentID"></param>
        /// <returns></returns>
        public IECSComponentBase GetComponentByBase(int componentID)
        {
            IECSComponentBase result = default;
            Tenons tenons = ShipDockApp.Instance.Tenons;
            bool flag = mAllComponentIDs.TryGetValue(componentID, out ECSComponentInfo info);
            if (flag)
            {
                result = tenons.GetTenon<IECSComponentBase>(info.instanceID);
            }
            else { }
            return result;
        }

        /// <summary>
        /// 根据组件数据的类型获取 ECS 组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IECSComponent<T> GetComponentByType<T>() where T : struct
        {
            Type type = typeof(T);
            IECSComponent<T> result = default;
            if (mAllComponentTypes.TryGetValue(type, out ECSComponentInfo info))
            {
                result = GetComponent<T>(info.componentID);
            }
            else { }
            return result;
        }

        /// <summary>
        /// 运行 ECS 系统
        /// </summary>
        /// <param name="deltaTime"></param>
        public void RunSystems(float deltaTime)
        {
            if (mSystemList != default)
            {
                if (mSystemExecutes == default || mSystemExecutes.Length == 0)
                {
                    mSystemExecutes = mSystemList.ToArray();
                }
                else { }

                int max = mSystemExecutes.Length;
                if (max > 0)
                {
                    IECSSystem system;
                    for (int i = 0; i < max; i++)
                    {
                        system = mSystemExecutes[i];
                        system.Execute();
                    }
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 添加 ECS 系统
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddSystem<T>() where T : IECSSystem, new()
        {
            IECSSystem system = new T();
            mSystems[system.SystemID] = system;
            mSystemList.Add(system);
            system.Init();
            return (T)system;
        }

        /// <summary>
        /// 根据组件数据类型查找组件的内存块组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void FindChunkGroupByType<T>() where T : struct
        {
            IECSComponent<T> component = GetComponentByType<T>();
            if (component == default) { }
            else
            {
                IChunkGroup chunkGroup = component.GetDataChunks();
                mChunkGroupsSearched.Add(chunkGroup);
            }
        }

        private void BeforeFindChunkGroup(ref IECSSystem system, Action<int, int> onSearchResults = default)
        {
            if (onSearchResults != default)
            {
                system.OnSearchResults = onSearchResults;
            }
            else { }

            mSystemChecking = system;
            mChunkGroupsSearched.Clear();
        }

        public void Search<T>(IECSSystem system, Action<int, int> onSearchResults = default) where T : struct
        {
            BeforeFindChunkGroup(ref system, onSearchResults);

            FindChunkGroupByType<T>();

            Filtrate();
        }

        public void Search<T, T1>(IECSSystem system, Action<int, int> onSearchResults = default) where T : struct where T1 : struct
        {
            BeforeFindChunkGroup(ref system, onSearchResults);

            FindChunkGroupByType<T>();
            FindChunkGroupByType<T1>();

            Filtrate();
        }

        public void Search<T, T1, T2>(IECSSystem system, Action<int, int> onSearchResults = default) where T : struct where T1 : struct where T2 : struct
        {
            BeforeFindChunkGroup(ref system, onSearchResults);

            FindChunkGroupByType<T>();
            FindChunkGroupByType<T1>();
            FindChunkGroupByType<T2>();

            Filtrate();
        }

        public void Search<T, T1, T2, T3>(IECSSystem system, Action<int, int> onSearchResults = default) where T : struct where T1 : struct where T2 : struct where T3 : struct
        {
            BeforeFindChunkGroup(ref system, onSearchResults);

            FindChunkGroupByType<T>();
            FindChunkGroupByType<T1>();
            FindChunkGroupByType<T2>();
            FindChunkGroupByType<T3>();

            Filtrate();
        }

        private void Filtrate()
        {
            if (mChunkGroupsSearched.Count > 0)
            {
                mEntitasSearchResultIndex = 0;

                mChunkChecking = mChunkGroupsSearched[0];
                
                if (mEntitasSearchResult == default)
                {
                    mEntitasSearchResult = new List<EntitySearchResult>(mChunkChecking.Total);
                    int newSize = mEntitasSearchResult.Capacity;
                    for (int i = 0; i < newSize; i++)
                    {
                        mEntitasSearchResult.Add(new EntitySearchResult());
                    }
                }
                else { }

                mChunkChecking.TraverseAllChunkInfos(OnChunkInfo);
                mSystemChecking.SetSearchResult(mEntitasSearchResultIndex, mEntitasSearchResult);
                mChunkChecking = default;
                mSystemChecking = default;
            }
            else { }
        }

        private void OnChunkInfo(ChunkInfo info)
        {
            IChunk chunk = mChunkChecking.GetChunkByIndex(info.chunkIndex);
            int entity = chunk.GetEntityByIndex(info.itemIndex, out bool isValid);
            if (isValid)
            {
                IChunkGroup chunkGroup;
                int max = mChunkGroupsSearched.Count;
                if (max > 1)
                {
                    for (int i = 1; i < max; i++)
                    {
                        chunkGroup = mChunkGroupsSearched[i];
                        if (chunkGroup.HasDataItemByEntity(entity)) { }
                        else
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (isValid)
                    {
                        CheckAndUpdateSearchResults(mChunkChecking.GroupID, info);
                    }
                    else { }
                }
                else
                {
                    CheckAndUpdateSearchResults(mChunkChecking.GroupID, info);
                }
            }
            else { }
        }

        private void CheckAndUpdateSearchResults(int groupID, ChunkInfo info)
        {
            if (mEntitasSearchResultIndex >= mEntitasSearchResult.Capacity)
            {
                mEntitasSearchResult.Capacity = mEntitasSearchResult.Count * 2;
                int start = mEntitasSearchResult.Count;
                int newSize = mEntitasSearchResult.Capacity;
                for (int i = start; i < newSize; i++)
                {
                    mEntitasSearchResult.Add(new EntitySearchResult());
                }
            }
            else { }

            EntitySearchResult searchResult = mEntitasSearchResult[mEntitasSearchResultIndex];
            ECSComponentInfo componentInfo = mAllComponentChunkGroups[groupID];
            searchResult.componentID = componentInfo.componentID;
            searchResult.info = info;
            mEntitasSearchResult[mEntitasSearchResultIndex] = searchResult;

            mEntitasSearchResultIndex++;
        }
    }
}
