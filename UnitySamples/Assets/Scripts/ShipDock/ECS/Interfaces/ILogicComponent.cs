using ShipDock.Pooling;

namespace ShipDock.ECS
{
    public interface ILogicComponent : IECSLogic, IDataValidable
    {
        bool HasDataChanged { get; }
        int DataPosition { get; }

        void SetEntitas(int entitasID);
        int GetEntitasState(int entitasID, out bool flag);
        void CheckAllDropedEntitas();
        void FillEntitasData(int entitasID, ILogicData data);
        void CheckAllDataValided();
        bool IsStateRegular(int entitasID, out bool hasEntitas);
        ILogicData GetEntitasData(int entitasID);
        ILogicData GetDataByIndex(int index);
        int GetEntitasIDByIndex(int index);
        bool IsDatasChanged(int index);
    }

    public interface IDataComponent<T> : ILogicComponent
    {
        void UpdateValidWithType<D>(int entitasID, ref D[] successive, out int dataIndex, D value, bool ignoreState = false);
        V GetDataValueWithType<V>(int entitas, ref V[] successive, out int dataIndex);
    }

    public interface IDataValidable
    {
        ILogicData UpdateValid(int entitasID, bool ignoreState = false);
        void WillDrop(int entitasID);
    }

    public interface ILogicData : IPoolable
    {
        int DataIndex { get; }
        bool IsValided { get; }

        void BindComponent(int entitasID, IDataValidable component);
        void SetDataIndex(int index);
        void Drop();
    }

    public class LogicData : ILogicData
    {
        public int DataIndex { get; private set; }
        public bool IsValided { get; private set; }

        protected int EntitasID { get; private set; }

        protected IDataValidable DataValidbler { get; private set; }

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
                    DataValidbler.UpdateValid(EntitasID);
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