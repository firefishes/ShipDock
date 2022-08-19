using ShipDock.Pooling;
using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public class ShipDockEntitas : IShipDockEntitas, IPoolable
    {
        public static IShipDockEntitas CreateEntitas()
        {
            return Pooling<ShipDockEntitas>.From();
        }

        private IShipDockComponentContext mContext;

        public List<int> ComponentList { get; } = new List<int>();
        public int ID { get; private set; } = int.MaxValue;
        public bool WillDestroy { get; protected set; } = false;

        private List<int> mBindedToComponentIDs = new List<int>();

        public void Revert() { }

        public void ToPool()
        {
            Pooling<ShipDockEntitas>.To(this);
        }

        public void InitComponents()
        {
            mContext = ShipDockECS.Instance.Context;
        }

        public void Reclaim()
        {
            WillDestroy = true;
        }

        public void AddComponent(int componentName)
        {
            IShipDockComponent component = mContext.RefComponentByName(componentName);
            AddComponent(component);
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public void AddComponent(IShipDockComponent component)
        {
            if ((component != default) && !HasComponent(component.ID))
            {
                int autoID = component.SetEntitas(this);
                ComponentList.Add(component.ID);
                mBindedToComponentIDs.Add(autoID);
            }
            else { }
        }

        public void SetEntitasID(int id)
        {
            if (ID == int.MaxValue)
            {
                ID = id;
            }
            else { }
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public void RemoveComponent(IShipDockComponent component)
        {
            if ((component != default) && HasComponent(component.ID))
            {
                int id = component.ID;

                int index = ComponentList.IndexOf(id);
                if (index >= 0)
                {
                    int entitasID = mBindedToComponentIDs[index];
                    mBindedToComponentIDs.RemoveAt(index);
                    ComponentList.Remove(id);
                    component.DropEntitas(this, entitasID);
                }
                else { }
            }
            else { }

            if (WillDestroy)
            {
                if ((ComponentList != default) && (ComponentList.Count == 0))
                {
                    List<int> list = ComponentList;
                    Utils.Reclaim(ref list);
                    Utils.Reclaim(ref mBindedToComponentIDs, false);
                    ID = int.MaxValue;
                    WillDestroy = false;

                    ToPool();
                }
                else { }
            }
            else { }
        }

        public bool HasComponent(int componentID)
        {
            return (ComponentList != default) && ComponentList.Contains(componentID);
        }

        /// <summary>
        /// 查找本实体在组件中的索引
        /// </summary>
        public int FindEntitasInComponent(IShipDockComponent component)
        {
            int id = component.ID;
            if (HasComponent(id))
            {
                id = ComponentList.IndexOf(id);
                id = mBindedToComponentIDs[id];
            }
            else
            {
                id = -1;
            }
            return id;
        }

        /// <summary>
        /// 获取一个已经添加到实体的组件
        /// </summary>
        public T GetComponentByName<T>(int name) where T : IShipDockComponent
        {
            return (T)mContext.RefComponentByName(name);
        }

        public T GetComponentFromEntitas<T>(int aid) where T : IShipDockComponent
        {
            T result = default;
            if (HasComponent(aid))
            {
                int index = ComponentList.IndexOf(aid);
                result = (T)mContext.RefComponentByName(ComponentList[index]);
            }
            else { }

            return result;
        }
    }
}
