using ECS;
using ShipDock.Pooling;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    /// <summary>
    /// 实体生成器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class LogicEntities : ILogicEntities
    {
        public struct EntityInfo
        {
            public int chunkIndex;
            public int offset;
        }

        public static int sizeOfInt32 = 4;
        public static int sizeOfBoolean = 1;
        public static int sizeOfFloat = 4;
        public static int sizeOfULong = 8;
        public static int sizeOfLong = 8;
        public static int sizeOfDouble = 8;

        private readonly static int[] emptyComponents = new int[0];
        private static int IDEntityas = 1000;
        private Chunks mAllChunks;

        public static int CreateEntitas(int entitasType = 0)
        {
            ILogicContext context = ShipDockECS.Instance.Context;
            ILogicEntities allEntitas = context.AllEntitas;

            allEntitas.AddEntitas(out int result, entitasType);

            return result;
        }

        private ILogicContext Context
        {
            get
            {
                return ShipDockECS.Instance.Context;
            }
        }

        private Dictionary<int, int[]> mEntityCompsTemplate;
        private KeyValueList<int, Dictionary<int, int>> mEntitiesCompBindeds;
        private KeyValueList<int, EntityType> mEntitesTypes;
        private Dictionary<Type, int> mCompDataTypeSizes;
        private Dictionary<int, EntityInfo> mEntityInfos;
        private Dictionary<int, Entity> mEntities;

        public LogicEntities()
        {
            mEntityCompsTemplate = new Dictionary<int, int[]>()
            {
                [0] = new int[] { },
            };
            mEntitiesCompBindeds = new KeyValueList<int, Dictionary<int, int>>();

            mEntitesTypes = new KeyValueList<int, EntityType>();
            mCompDataTypeSizes = new Dictionary<Type, int>();
            mEntities = new Dictionary<int, Entity>();

            AddTypeSizeOf(typeof(int), sizeOfInt32);
            AddTypeSizeOf(typeof(bool), sizeOfBoolean);
            AddTypeSizeOf(typeof(float), sizeOfFloat);
            AddTypeSizeOf(typeof(ulong), sizeOfULong);
            AddTypeSizeOf(typeof(long), sizeOfLong);
            AddTypeSizeOf(typeof(double), sizeOfDouble);

            mAllChunks = new Chunks();
            mEntityInfos = new Dictionary<int, EntityInfo>();
        }

        public void AddTypeSizeOf(Type type, int byteSize)
        {
            mCompDataTypeSizes[type] = byteSize;
        }

        public void Reclaim()
        {
            IDEntityas = 0;

            Utils.Reclaim(ref mEntityCompsTemplate);
            Utils.Reclaim(ref mEntitiesCompBindeds);
        }

        public void BuildEntitasTemplate(int entityType, params int[] componentNames)
        {
            if (entityType != 0)
            {
                int max = componentNames.Length;
                int[] comps = mEntityCompsTemplate[entityType];
                if (comps == default)
                {
                    comps = new int[max];
                    mEntityCompsTemplate[entityType] = comps;
                }
                else { }

                bool flag = mEntitesTypes.TryGetValue(entityType, out EntityType typeResult);
                if (flag) { }
                else 
                {
                    typeResult = new EntityType(entityType);
                    mEntitesTypes[entityType] = typeResult;
                }

                int sizeOf;
                Type[] types;
                ILogicComponent comp;
                for (int i = 0; i < max; i++)
                {
                    comps[i] = componentNames[i];
                    comp = Context.RefComponentByName(comps[i]);
                    types = comp.GetEntityDataSizeOf();

                    for (int j = 0; j < types.Length; j++)
                    {
                        sizeOf = mCompDataTypeSizes[types[i]];
                        typeResult.AddComponentSizePerData(sizeOf);
                    }
                }
            }
            else { }
        }

        public void MakeChunks()
        {
            EntityType item;
            List<EntityType> values = mEntitesTypes.Values;
            int max = values.Count;
            for (int i = 0; i < max; i++)
            {
                item = values[i];
                item.CapacityPerChunk = ChunkUnit.sizeOfBytesPerChunk / item.SizePerEntity;
            }
        }

        public void AddEntitas(out int entity, int entityType = 0)
        {
            entity = int.MaxValue;

            int[] info = mEntityCompsTemplate[entityType];
            if (info != default)
            {
                
                entity = IDEntityas;
                IDEntityas++;

                EntityType entityTypeResult = mEntitesTypes[entityType];
                //entityTypeResult.

                bool isValid = HasEntitas(entity);
                if (isValid) { }
                else
                {
                    bool flag = mEntitesTypes.TryGetValue(entityType, out EntityType typeResult);
                    if (flag)
                    {
                        Entity item = typeResult.BindEntity(entity, entityType, ref mAllChunks);
                        mEntities[entity] = item;
                    }
                    else { }

                    int compName;
                    int max = info.Length;
                    for (int i = 0; i < max; i++)
                    {
                        compName = info[i];
                        AddComponent(entity, compName);
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
                        data = component.GetEntitasData(entitas);
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
