using ShipDock.Tools;

namespace ShipDock.ECS
{
    /// <summary>
    /// 实体生成器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class LogicEntitas : ILogicEntitas
    {
        private readonly static int[] emptyComponents = new int[0];
        private static int IDEntityas = 0;

        public static int CreateEntitas(int entitasType = 0)
        {
            ILogicContext context = ShipDockECS.Instance.Context;
            ILogicEntitas allEntitas = context.AllEntitas;

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

        private KeyValueList<int, int[]> mEntitasInfo;
        private KeyValueList<int, IdentBitsGroup> mComponentsInfo;

        public LogicEntitas()
        {
            mEntitasInfo = new KeyValueList<int, int[]>()
            {
                [0] = new int[] { },
            };
            mComponentsInfo = new KeyValueList<int, IdentBitsGroup>();
        }

        public void Reclaim()
        {
            IDEntityas = 0;

            Utils.Reclaim(ref mEntitasInfo);
            Utils.Reclaim(ref mComponentsInfo);
        }

        public void BuildEntitasTemplate(int entitasType, params int[] componentNames)
        {
            if (entitasType != 0)
            {
                int max = componentNames.Length;
                int[] info = mEntitasInfo[entitasType];
                if (info == default)
                {
                    info = new int[max];
                    mEntitasInfo[entitasType] = info;
                }
                else { }

                for (int i = 0; i < max; i++)
                {
                    info[i] = componentNames[i];
                }
            }
            else { }
        }

        public void AddEntitas(out int entitasID, int entitasType = 0)
        {
            entitasID = int.MaxValue;

            int[] info = mEntitasInfo[entitasType];
            if (info != default)
            {
                entitasID = IDEntityas;
                IDEntityas++;

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

        public void RemoveEntitas(int entitasID)
        {
            bool flag = HasEntitas(entitasID);
            if (flag)
            {
                IdentBitsGroup identBitsGroup = mComponentsInfo.Remove(entitasID);
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
                    IdentBitsGroup compIDs = mComponentsInfo[entitasID];
                    if (compIDs == default)
                    {
                        compIDs = new IdentBitsGroup();
                        mComponentsInfo[entitasID] = compIDs;
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
                    IdentBitsGroup compIDs = mComponentsInfo[entitasID];
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
            bool result = (mComponentsInfo != default) ? mComponentsInfo.ContainsKey(entitasID) : false;
            return result;
        }

        public bool HasComponent(int entitasID, int componentID)
        {
            bool result = default;
            if (HasEntitas(entitasID))
            {
                IdentBitsGroup compIDs = mComponentsInfo[entitasID];
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
            IdentBitsGroup identBitsGroup = (mComponentsInfo != default) ? mComponentsInfo[entitasID] : default;
            return identBitsGroup != default ? identBitsGroup.GetAllMarks() : emptyComponents;
        }
    }
}
