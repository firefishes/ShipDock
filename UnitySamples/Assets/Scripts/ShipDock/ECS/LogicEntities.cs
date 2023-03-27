using System;
using System.Collections.Generic;

namespace ShipDock
{
    /// <summary>
    /// 实体生成器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class LogicEntities : ILogicEntities
    {
        /// <summary>
        /// 实体信息
        /// </summary>
        public struct EntityInfo
        {
            public int chunkIndex;
            public int offset;
        }

        /// <summary>实体数据字段长度：整型</summary>
        public static int sizeOfInt32 = 4;
        /// <summary>实体数据字段长度：布尔型</summary>
        public static int sizeOfBoolean = 1;
        /// <summary>实体数据字段长度：浮点型</summary>
        public static int sizeOfFloat = 4;
        /// <summary>实体数据字段长度：无符号长整型</summary>
        public static int sizeOfULong = 8;
        /// <summary>实体数据字段长度：长整型</summary>
        public static int sizeOfLong = 8;
        /// <summary>实体数据字段长度：双精度浮点型</summary>
        public static int sizeOfDouble = 8;
        /// <summary>实体数据字段长度：双精度浮点型</summary>
        public static int sizeOfString = 64;

        /// <summary>默认的空组件列表</summary>
        private readonly static int[] emptyComponents = new int[0];
        /// <summary>实体 ID 起始值</summary>
        private static int IDEntityas = 1000;

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="entitasType"></param>
        /// <returns></returns>
        public static int CreateEntitas(int entitasType = 0)
        {
            ILogicContext context = ShipDockECS.Instance.Context;
            ILogicEntities allEntitas = context.AllEntitas;

            allEntitas.AddEntitas(out int result, entitasType);

            return result;
        }

        /// <summary>
        /// ECS 上下文管理器
        /// </summary>
        private ILogicContext Context
        {
            get
            {
                return ShipDockECS.Instance.Context;
            }
        }

        /// <summary>实体组件模板</summary>
        private Dictionary<int, int[]> mEntityCompsTemplate;
        /// <summary>实体与组件的映射数据</summary>
        private KeyValueList<int, Dictionary<int, int>> mEntitiesCompBindeds;
        /// <summary>实体类型映射数据</summary>
        private KeyValueList<int, EntityType> mEntitesTypes;
        /// <summary>实体中组件数据所有可能使用到的字段类型长度</summary>
        private Dictionary<Type, int> mCompDataTypeSizes;
        /// <summary>实体信息映射数据</summary>
        private Dictionary<int, EntityInfo> mEntityInfos;
        /// <summary>所有实体</summary>
        private Dictionary<int, Entity> mEntities;

        /// <summary>实体内存池</summary>
        public Chunks Chunks { get; private set; }

        public LogicEntities()
        {
            mEntityCompsTemplate = new Dictionary<int, int[]>()
            {
                //定义默认的实体模板（无组件的实体）
                [0] = new int[] { },
            };
            mEntitiesCompBindeds = new KeyValueList<int, Dictionary<int, int>>();

            mEntitesTypes = new KeyValueList<int, EntityType>();
            mCompDataTypeSizes = new Dictionary<Type, int>();
            mEntities = new Dictionary<int, Entity>();

            #region 定义组件数据有可能使用到的字段类型长度
            AddTypeSizeOf(typeof(int), sizeOfInt32);
            AddTypeSizeOf(typeof(bool), sizeOfBoolean);
            AddTypeSizeOf(typeof(float), sizeOfFloat);
            AddTypeSizeOf(typeof(ulong), sizeOfULong);
            AddTypeSizeOf(typeof(long), sizeOfLong);
            AddTypeSizeOf(typeof(double), sizeOfDouble);
            #endregion

            Chunks = new Chunks();
            mEntityInfos = new Dictionary<int, EntityInfo>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Reclaim()
        {
            IDEntityas = 0;

            Utils.Reclaim(ref mEntityCompsTemplate);
            Utils.Reclaim(ref mEntitiesCompBindeds);
        }

        /// <summary>
        /// 根据对象类型增加组件数据有可能使用到的字段类型长度
        /// </summary>
        /// <param name="type"></param>
        /// <param name="byteSize"></param>
        public void AddTypeSizeOf(Type type, int byteSize)
        {
            mCompDataTypeSizes[type] = byteSize;
        }

        /// <summary>
        /// 构建实体模板
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="componentNames"></param>
        public void BuildEntitasTemplate(int entityType, params int[] componentNames)
        {
            if (entityType != 0)
            {
                int max = componentNames.Length;
                bool flag = mEntityCompsTemplate.TryGetValue(entityType, out int[] comps);
                if (flag) { }
                else
                {
                    //根据组件列表添加新模板
                    comps = new int[max];
                    mEntityCompsTemplate[entityType] = comps;
                }

                flag = mEntitesTypes.TryGetValue(entityType, out EntityType entityTypeValue);
                if (flag) { }
                else
                {
                    //根据实体类型添加新的实体类型信息
                    entityTypeValue = new EntityType(entityType);
                    mEntitesTypes[entityType] = entityTypeValue;
                }

                Type type;
                Type[] types;
                string[] keys;
                ILogicComponent comp;
                int sizeOf, compType, count, sizePerData;
                for (int i = 0; i < max; i++)
                {
                    compType = componentNames[i];
                    comps[i] = compType;
                    comp = Context.RefComponentByName(compType);
                    
                    //按照组件的所有数据字段长度定义实体实例的数据长度
                    types = comp.GetEntityDataSizeOf();
                    keys = comp.GetEntityDataKeys();

                    count = types.Length;
                    for (int j = 0; j < count; j++)
                    {
                        type = types[i];
                        sizeOf = mCompDataTypeSizes[type];
                        entityTypeValue.AddCompSizePerData(compType, sizeOf, count, j, keys[j]);
                    }

                    //标记数组数据段的位置并设置组件中单个实体所需的数据长度
                    entityTypeValue.MarkComponentSection(compType);
                    sizePerData = entityTypeValue.SizePerEntityData;
                    comp.SetSizePerData(sizePerData);
                }
            }
            else { }
        }

        /// <summary>
        /// 设置所有已构建的实体类型的基本信息
        /// </summary>
        public void MakeChunks()
        {
            EntityType entityType;
            List<EntityType> values = mEntitesTypes.Values;
            int max = values.Count;
            for (int i = 0; i < max; i++)
            {
                entityType = values[i];
                entityType.InitCapacityPerChunk();
            }
        }

        public EntityType GetEntityType(int entityID, out int index, out int chunkIndex)
        {
            index = -1;
            chunkIndex = -1;
            EntityType result = default;

            bool flag = mEntities.TryGetValue(entityID, out Entity entity);
            if (flag)
            {
                int entityType = entity.entityType;
                chunkIndex = entity.chunkIndex;
                index = entity.index;
                mEntitesTypes.TryGetValue(entityType, out result);
            }
            else { }

            return result;
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entityID"></param>
        /// <param name="entityType"></param>
        public void AddEntitas(out int entityID, int entityType = 0)
        {
            entityID = int.MaxValue;

            int[] info = mEntityCompsTemplate[entityType];
            if (info != default)
            {
                
                entityID = IDEntityas;
                IDEntityas++;

                //EntityType entityTypeResult = mEntitesTypes[entityType];
                //entityTypeResult.

                bool isValid = HasEntitas(entityID);
                if (isValid) { }
                else
                {
                    bool flag = mEntitesTypes.TryGetValue(entityType, out EntityType entityTypeValue);
                    if (flag)
                    {
                        //通过内存池创建实体
                        Entity item = entityTypeValue.BindEntityToChunk(entityID, entityType, Chunks);
                        mEntities[entityID] = item;
                    }
                    else { }

                    //为实体添加组件
                    int compName;
                    int max = info.Length;
                    for (int i = 0; i < max; i++)
                    {
                        compName = info[i];
                        AddComponent(entityID, compName);
                    }
                }
            }
            else { }
        }

        public void RemoveEntitas(int entitas)
        {
            bool flag = HasEntitas(entitas);
            if (flag)
            {
#if LOG
                const string removeEntitasLog = "log: entitas {0} drop, remove form {1}, data index is {2}";
                string entitasContent = entitas.ToString();
#endif

                ILogicData data;
                ILogicComponent component;
                int[] comps = GetComponentList(entitas);
                if (comps != default)
                {
                    for (int i = 0; i < comps.Length; i++)
                    {
                        component = Context != default ? Context.RefComponentByName(comps[i]) : default;
                        //data = component.GetEntitasData(entitas);
                        RemoveComponent(entitas, component);

    #if LOG
                        removeEntitasLog.Log(entitasContent, component.Name, data.DataIndex.ToString());
    #endif
                    }
                }
                else { }

                Dictionary<int, int> identBitsGroup = mEntitiesCompBindeds.Remove(entitas);
                Utils.Reclaim(ref identBitsGroup);
                //identBitsGroup.Reclaim();
            }
            else { }
        }

        public void AddComponent(int entity, int componentName)
        {
            ILogicComponent component = Context != default ? Context.RefComponentByName(componentName) : default;
            if (component != default)
            {
                AddComponent(entity, component);
            }
            else { }
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public void AddComponent(int entity, ILogicComponent component)
        {
            if (component != default)
            {
                int compID = component.ID;
                if (HasComponent(entity, compID)) { }
                else
                {
                    bool flag = mEntitiesCompBindeds.TryGetValue(entity, out Dictionary<int, int> compNames);
                    if (compNames == default)
                    {
                        compNames = new Dictionary<int, int>();
                        mEntitiesCompBindeds[entity] = compNames;
                    }
                    else { }

                    flag = compNames.TryGetValue(compID, out int compStatu);
                    if (flag) { }
                    else
                    {
                        compNames[compID] = 1;
                        component.SetEntitas(entity);
                    }
                }
            }
            else { }
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public void RemoveComponent(int entitasID, ILogicComponent component)
        {
            if (component != default)
            {
                int compID = component.ID;
                if (HasComponent(entitasID, compID))
                {
                    bool flag = mEntitiesCompBindeds.TryGetValue(entitasID, out Dictionary<int, int> compIDs);
                    if (flag)
                    {
                        flag = compIDs.TryGetValue(compID, out _);
                        if (flag)
                        {
                            compIDs.Remove(compID);
                            component.WillDrop(entitasID);
                        }
                        else { }
                    }
                    else { }
                }
                else { }
            }
            else { }
        }

        public bool HasEntitas(int entitasID)
        {
            bool result = (mEntitiesCompBindeds != default) ? mEntitiesCompBindeds.ContainsKey(entitasID) : false;
            return result;
        }

        public bool HasComponent(int entitasID, int componentID)
        {
            bool result = default;
            if (HasEntitas(entitasID))
            {
                bool flag = mEntitiesCompBindeds.TryGetValue(entitasID, out Dictionary<int, int> compIDs);
                result = compIDs.TryGetValue(componentID, out _);
            }
            else { }
            return result;
        }

        /// <summary>
        /// 获取一个已经添加到实体的组件
        /// </summary>
        public T GetComponentFromEntitas<T>(int entitasID, int componentID) where T : ILogicComponent
        {
            T result = default;
            if (HasComponent(entitasID, componentID))
            {
                result = (T)Context.RefComponentByName(componentID);
            }
            else { }

            return result;
        }

        public int[] GetComponentList(int entity)
        {
            int[] result = default;
            bool flag = mEntitiesCompBindeds.TryGetValue(entity, out Dictionary<int, int> compIDs);
            if (flag)
            {
                int max = compIDs.Count;
                result = new int[max];

                KeyValuePair<int, int> item;
                Dictionary<int, int>.Enumerator enumer = compIDs.GetEnumerator();
                for (int i = 0; i < max; i++)
                {
                    enumer.MoveNext();
                    item = enumer.Current;
                    result[i] = item.Key;
                }
            }
            else { }
            return result;
        }
    }
}
