using System;

namespace ShipDock
{
    public interface IECSSystem
    {
        int SystemID { get; }
        void Init(Tenons tenons);
        void Execute();
        //ITenonSystemDatas<T> GetDataCreater<T>(int tenonType) where T : ITenonData;
        //void AddDataCreater<T>(int tenonType, ITenonSystemDatas<T> datas) where T : ITenonData;
        //void BindData<T>(ITenon<T> tenon) where T : ITenonData;
        //void DeBindData<T>(ITenon<T> tenon) where T : ITenonData;
    }

    public interface ITenonSystemDataCreater 
    {
        void SettleDatas(ref Tenons tenons, IECSSystem system);
    }

    public interface ITenonSystemDatas<T> : ITenonSystemDataCreater where T : IECSData
    {
        void OnExecuter(Action<int, T> action);
        Action<int, T> GetExecuter();
        //KeyValueList<int, T> GetDatas();
        void AddData(int tenonID, T dataItem);
        void RemoveData(int tenonID, T dataItem);
        T GetData(int tenonID);
    }

    public interface IECSData
    {
        //bool TenonID { get; }
        //void SetTenonID(int tenonID);
        bool IsChanged { get; set; }
    }

}