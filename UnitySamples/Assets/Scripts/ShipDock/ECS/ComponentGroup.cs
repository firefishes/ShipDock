using System.Collections.Generic;

namespace ShipDock
{
    public class ComponentGroup<C, K> where C : IECSLogic
    {
        public ComponentGroup()
        {
        }

        public ComponentGroup(K[] keys, int[] componentNames) : this(ShipDockECS.Instance.Context, ref keys, ref componentNames)
        {
        }

        public ComponentGroup(ILogicContext context, ref K[] keys, ref int[] componentNames)
        {
            int max = componentNames.Length;
            Group = new KeyValueList<K, C>(max);

            C groupItem;
            for (int i = 0; i < max; i++)
            {
                groupItem = (C)context.RefComponentByName(componentNames[i]);
                Group.Put(keys[i], groupItem);
            }
        }

        public void SetGroupMapper(ref KeyValueList<K, C> list)
        {
            Group = list;
        }

        public void Clean()
        {
            Group = default;
        }

        public C GetGroupComponent(K key)
        {
            return Group[key];
        }

        private KeyValueList<K, C> Group { get; set; }
    }

    public class DataComponentGroup<C, K> where C : IECSLogic, IDataValidable
    {
        public DataComponentGroup(K[] keys, int[] componentNames) : this(ShipDockECS.Instance.Context, ref keys, ref componentNames)
        {
        }

        public DataComponentGroup(ILogicContext context, ref K[] keys, ref int[] componentNames)
        {
            int max = componentNames.Length;
            Group = new KeyValueList<K, C>(max);
            
            C groupItem;
            for (int i = 0; i < max; i++)
            {
                groupItem = (C)context.RefComponentByName(componentNames[i]);
                Group.Put(keys[i], groupItem);
            }
        }

        public void Clean()
        {
            Group.Clear();
        }

        public C GetGroupComponent(K key)
        {
            return Group[key];
        }

        public void SetDataVailid(int entitasID, bool value)// where E : IShipDockEntitas
        {
            C groupItem;
            List<C> list = Group.Values;
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                groupItem = list[i];
                //groupItem.UpdateValid(entitasID);
                //groupItem.SetDataValidable(value, ref target);
            }
        }

        private KeyValueList<K, C> Group { get; set; }
    }
}