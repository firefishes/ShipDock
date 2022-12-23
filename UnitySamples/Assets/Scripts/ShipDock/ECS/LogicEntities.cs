using ECS;
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

        public static long sizeOfBytesPerChunk = 16 * 1000;

        private readonly static int[] emptyComponents = new int[0];
        private static int IDEntityas = 1000;
        private Chunks mAllChunks { get; set; }

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

        private KeyValueList<int, int[]> mEntityCompsRelation;
        private KeyValueList<int, IdentBitsGroup> mComponentsInfos;
        private KeyValueList<int, EntityType> mEntitesTypes;
        private Dictionary<Type, int> mCompTypeSizes;
        private Dictionary<int, EntityInfo> mEntityInfos;

        public LogicEntities()
        {
            mEntityCompsRelation = new KeyValueList<int, int[]>()
            {
                [0] = new int[] { },
            };
            mComponentsInfos = new KeyValueList<int, IdentBitsGroup>();

            mEntitesTypes = new KeyValueList<int, EntityType>();
            mCompTypeSizes = new Dictionary<Type, int>();

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
            mCompTypeSizes[type] = byteSize;
        }

        public void Reclaim()
        {
            IDEntityas = 0;

            Utils.Reclaim(ref mEntityCompsRelation);
            Utils.Reclaim(ref mComponentsInfos);
        }

        public void BuildEntitasTemplate(int entityType, params int[] componentNames)
        {
            if (entityType != 0)
            {
                int max = componentNames.Length;
                int[] info = mEntityCompsRelation[entityType];
                if (info == default)
                {
                    info = new int[max];
                    mEntityCompsRelation[entityType] = info;
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
                    info[i] = componentNames[i];
                    comp = Context.RefComponentByName(info[i]);
                    types = comp.GetEntityDataSizeOf();

                    for (int j = 0; j < types.Length; j++)
                    {
                        sizeOf = mCompTypeSizes[types[i]];
                        typeResult.AddComponentSizePerData(sizeOf);
                    }
                }

                //typeResult.SetEntityMax();
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
                item.CapacityPerChunk = sizeOfBytesPerChunk / item.SizePerData;
            }
        }

        public void AddEntitas(out int entitasID, int entityType = 0)
        {
            entitasID = int.MaxValue;

            int[] info = mEntityCompsRelation[entityType];
            if (info != default)
            {
                
                entitasID = IDEntityas;
                IDEntityas++;

                EntityType entityTypeResult = mEntitesTypes[entityType];
                //entityTypeResult.

                bool isValid = HasEntitas(entitasID);
                if (isValid) { }
                else
                {
                    int compName;
                    int max = info.Length;
                    for (int i = 0; i < max; i++)
                    {
                        compName = info[i];
                        AddComponent(entitasID, compName);
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

                ILogicComponent component;
                ILogicData data;
                int[] comps = GetComponentList(entitas);
                for (int i = 0; i < comps.Length; i++)
                {
                    component = Context != default ? Context.RefComponentByName(comps[i]) : default;
                    data = component.GetEntitasData(entitas);
                    RemoveComponent(entitas, component);

#if LOG
                    removeEntitasLog.Log(entitasContent, component.Name, data.DataIndex.ToString());
#endif
                }

                IdentBitsGroup identBitsGroup = mComponentsInfos.Remove(entitas);
                identBitsGroup.Reclaim();
            }
            else { }
        }

        public void AddComponent(int entitasID, int componentName)
        {
            ILogicComponent component = Context != default ? Context.RefComponentByName(componentName) : default;
            if (component != default)
            {
                AddComponent(entitasID, component);
            }
            else { }
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public void AddComponent(int entitasID, ILogicComponent component)
        {
            if (component != default)
            {
                int compID = component.ID;
                if (HasComponent(entitasID, compID)) { }
                else
                {
                    IdentBitsGroup compIDs = mComponentsInfos[entitasID];
                    if (compIDs == default)
                    {
                        compIDs = new IdentBitsGroup();
                        mComponentsInfos[entitasID] = compIDs;
                    }
                    else { }

                    if (compIDs.Check(compID)) { }
                    else
                    {
                        compIDs.Mark(compID);
                        component.SetEntitas(entitasID);
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
                    IdentBitsGroup compIDs = mComponentsInfos[entitasID];
                    if (compIDs != default)
                    {
                        bool flag = compIDs.Check(compID);
                        if (flag)
                        {
                            compIDs.DeMark(compID);
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
            bool result = (mComponentsInfos != default) ? mComponentsInfos.ContainsKey(entitasID) : false;
            return result;
        }

        public bool HasComponent(int entitasID, int componentID)
        {
            bool result = default;
            if (HasEntitas(entitasID))
            {
                IdentBitsGroup compIDs = mComponentsInfos[entitasID];
                result = compIDs != default ? compIDs.Check(componentID) : false;
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

        public int[] GetComponentList(int entitasID)
        {
            IdentBitsGroup identBitsGroup = (mComponentsInfos != default) ? mComponentsInfos[entitasID] : default;
            return identBitsGroup != default ? identBitsGroup.GetAllMarks() : emptyComponents;
        }
    }
}
