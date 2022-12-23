
#if G_LOG
#define _COMP_DATA_REUSE_LOG
#define _DATA_SIZE_STRETCH_LOG
#endif

using ShipDock.Tools;
using System.Collections.Generic;


namespace ShipDock.ECS
{
    /// <summary>
    /// ECS数据组件
    /// 
    /// 组件与实体为一对多的关系，组件中的数据与实体的关系则是一一对应的关系
    /// 
    /// </summary>
    /// <typeparam name="T">组件数据泛型</typeparam>
    public class DataComponent<T> : LogicComponent, IDataComponent<T> where T : ILogicData, new()
    {
        private int mPoolingSize;
        private int mPoolingPosition;
        private T[] mPooling;
        private Dictionary<ILogicData, int> mUseds;
        private Queue<int> mPoolingEmptyIndexs;

        public DataComponent()
        {
            mUseds = new Dictionary<ILogicData, int>();
            mPoolingEmptyIndexs = new Queue<int>();
        }

        public override void Reset(bool clearOnly = false)
        {
            Utils.Reclaim(ref mPooling, clearOnly);

            ILogicData key;
            int max = mUseds.Count;
            Dictionary<ILogicData, int>.Enumerator enums = mUseds.GetEnumerator();
            for (int i = 0; i < max; i++)
            {
                if (enums.MoveNext())
                {
                    key = enums.Current.Key;
                    key?.Revert();
                }
                else { }
            }

            Utils.Reclaim(ref mUseds, clearOnly);
            Utils.Reclaim(ref mPoolingEmptyIndexs, clearOnly);

            base.Reset(clearOnly);
        }

        protected override void OnResetSuccessive(bool clearOnly = false)
        {
            mPoolingPosition = 0;
            mPoolingSize = mDataSizeStart;

            if (clearOnly)
            {
                //仅清理数据时不做其他处理
            }
            else
            {
                Utils.Stretch(ref mPooling, mPoolingSize);
                FillAllDataInstance();
            }
        }

        private void FillAllDataInstance(int start = 0)
        {
            int max = mPooling.Length;
            for (int i = start; i < max; i++)
            {
                mPooling[i] = new T();
            }

#if DATA_SIZE_STRETCH_LOG
            const string dataSizeStretchLog = "log:[{0}] Data size stretched, max {1} change to {2}";
            dataSizeStretchLog.Log(Name, (mPooling.Length - start).ToString(), mPooling.Length.ToString());
#endif
        }

        protected override ILogicData CreateData(int entitas)
        {
            int index;
            if (mPoolingEmptyIndexs.Count > 0)
            {
                index = mPoolingEmptyIndexs.Dequeue();
            }
            else
            {
                index = mPoolingPosition;
                mPoolingPosition++;

                if (mPoolingPosition > mPoolingSize)
                {
                    mPoolingSize = (int)(mPoolingSize * mDataStretchRatio);
                    Utils.Stretch(ref mPooling, mPoolingSize);
                    FillAllDataInstance(index);
                }
                else { }
            }

            ILogicData result = mPooling[index];

#if COMP_DATA_REUSE_LOG
            const string compDataReuseLog = "log: component data {0} reused, bind to entitas {1}";
            compDataReuseLog.Log(result.ID.ToString(), entitas.ToString());
#endif

            result.IsRecycling = false;

            mUseds[result] = index;
            mPooling[index] = default;

            return result;
        }

        protected override void DropData(ref ILogicData target)
        {
            bool flag = mUseds.TryGetValue(target, out int index);
            target?.Revert();
        }

        protected override void DuringRecycleEntitasData(int index, ILogicData data)
        {
            mPooling[index] = (T)data;
            mPoolingEmptyIndexs.Enqueue(index);
        }

        public void UpdateValidWithType<V>(int entitasID, ref V[] successive, out int dataIndex, V value = default, bool ignoreState = false)
        {
            dataIndex = int.MaxValue;
            ILogicData data = UpdateValid(entitasID, ignoreState);

            if (data is T)
            {
                if (successive != default)
                {
                    dataIndex = data.DataIndex;
                    successive[dataIndex] = value;
                }
                else { }
            }
            else { }
        }

        public V GetDataValueWithType<V>(int entitas, ref V[] successive, out int dataIndex)
        {
            dataIndex = int.MaxValue;
            V result = default;

            ILogicData data = GetEntitasData(entitas);

            if (data != default && successive != default)
            {
                dataIndex = data.DataIndex;
                result = successive[dataIndex];
            }
            else { }

            return result;
        }

    }
}