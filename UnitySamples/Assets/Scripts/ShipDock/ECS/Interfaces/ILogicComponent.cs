using System;
using System.Collections.Generic;

namespace ShipDock
{
    public interface ILogicComponent : IECSLogic, IDataValidable
    {
        bool HasDataChanged { get; }
        byte[] DataBuffs { get; }
        List<int> ChunkUnits { get; }

        void SetEntitas(int entitasID);
        int GetEntitasState(int entitasID, out bool flag);
        void CheckAllDropedEntitas();
        void CheckAllDataValided();
        bool IsStateRegular(int entitasID, out bool hasEntitas);
        void SetSizePerData(int size);
        int GetSizePerData();

        Type[] GetEntityDataSizeOf();
        string[] GetEntityDataKeys();
    }

    //public interface IDataComponent<T> : ILogicComponent
    //{
    //    void UpdateValidWithType<D>(int entitasID, ref D[] successive, out int dataIndex, D value, bool ignoreState = false);
    //    V GetDataValueWithType<V>(int entitas, ref V[] successive, out int dataIndex);
    //}

    public interface IDataValidable
    {
        //ILogicData UpdateValid(int entitasID, bool ignoreState = false);
        void WillDrop(int entitasID);
    }

    public interface ILogicData : IPoolable
    {
        int ID { get; }
        int DataIndex { get; }
        bool IsValided { get; }
        int EntitasID { get; }
        bool IsRecycling { get; set; }

        void BindComponent(int entitasID, IDataValidable component);
        void SetDataIndex(int index);
        void Drop();
    }

    public class LogicData : ILogicData
    {
        private static int instanceCount = 10000;

        public int ID { get; private set; }
        public int DataIndex { get; private set; }
        public bool IsValided { get; private set; }
        public bool IsRecycling { get; set; }

        public int EntitasID { get; private set; }

        protected IDataValidable DataValidbler { get; private set; }

        public LogicData()
        {
            ID = instanceCount;
            instanceCount++;
        }

        public void Revert()
        {
            EntitasID = int.MaxValue;
            DataValidbler = default;
        }

        public void ToPool() { }

        public void SetDataIndex(int index)
        {
            DataIndex = index;
        }

        public void BindComponent(int entitasID, IDataValidable dataValidabler)
        {
            EntitasID = entitasID;
            DataValidbler = dataValidabler;
        }

        protected void DataValid()
        {
            if (DataValidbler != default)
            {
                if (IsValided) { }
                else
                {
                    //DataValidbler.UpdateValid(EntitasID);
                }
            }
            else { }
        }

        public void Drop()
        {
            DataValidbler?.WillDrop(EntitasID);
        }
    }
}