using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    public class DataProxy : IDataProxy
    {
        private Queue<int> mDataNotifies;
        private List<IDataExtracter> mDataHandlers;
        private Action<IDataProxy, int> mOnDataProxyNotify;

        public DataProxy()
        {
            mDataNotifies = new Queue<int>();
            mDataHandlers = new List<IDataExtracter>();
        }

        public DataProxy(int dataName) : this()
        {
            DataName = dataName;
        }

        public virtual void Dispose()
        {
            Utils.Reclaim(ref mDataNotifies);
            Utils.Reclaim(ref mDataHandlers);
            mOnDataProxyNotify = default;
        }

        public void DataNotifies(params int[] keys)
        {
            int max = keys.Length;
            if (max > 0)
            {
                int keyName;
                for (int i = 0; i < max; i++)
                {
                    keyName = keys[i];
                    DataNotify(keyName);
                }
            }
            else
            {
                DataNotify(int.MaxValue);
            }
        }

        public void DataNotify(int keyName)
        {
            mOnDataProxyNotify?.Invoke(this, keyName);
        }

        public virtual void Register(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            else { }

            mDataHandlers.Add(dataHandler);
            AddDataProxyNotify(dataHandler.OnDataProxyNotify);
        }

        public virtual void Unregister(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || mDataHandlers == default || !mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            else { }

            mDataHandlers.Remove(dataHandler);
            RemoveDataProxyNotify(dataHandler.OnDataProxyNotify);
        }

        public void AddDataProxyNotify(Action<IDataProxy, int> notifyHandler)
        {
            mOnDataProxyNotify += notifyHandler;
        }

        public void RemoveDataProxyNotify(Action<IDataProxy, int> notifyHandler)
        {
            mOnDataProxyNotify -= notifyHandler;
        }

        public virtual int DataName { get; private set; }
    }
}
