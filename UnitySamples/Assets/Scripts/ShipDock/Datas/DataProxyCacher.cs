
using System;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    public class DataProxyCacher
    {
        private List<int> mDataProxyNames;
        private DataProxy[] mProxyList;

        public DataProxyCacher(params DataProxy[] proxies)
        {
            Init(ref proxies);
        }

        public DataProxyCacher(List<IDataProxy> proxyies)
        {
            DataProxy[] list = new DataProxy[proxyies.Count];
            int max = list.Length;
            for (int i = 0; i < max; i++)
            {
                list[i] = proxyies[i] as DataProxy;
            }
            Init(ref list);
        }

        private void Init(ref DataProxy[] proxies)
        {
            mDataProxyNames = new List<int>();

            mProxyList = proxies;
            int max = mProxyList.Length;
            for (int i = 0; i < max; i++)
            {
                mDataProxyNames.Add(mProxyList[i].DataName);
            }
        }

        public void Clean()
        {
            if (mProxyList != default)
            {
                Array.Clear(mProxyList, 0, mProxyList.Length);
            }
            else { }
        }

        public void DataNotifies(int dataName, params int[] keyName)
        {
            int index = mDataProxyNames.IndexOf(dataName);
            if (index >= 0)
            {
                mProxyList[index].DataNotifies(keyName);
            }
            else { }
        }

        public void DataNotify(int dataName, int keyName)
        {
            int index = mDataProxyNames.IndexOf(dataName);
            if (index >= 0)
            {
                mProxyList[index].DataNotify(keyName);
            }
            else { }
        }

        public T GetData<T>(int dataName) where T : DataProxy
        {
            T result = default;
            int index = mDataProxyNames.IndexOf(dataName);
            if (index >= 0)
            {
                result = (T)mProxyList[index];
            }
            else { }
            return result;
        }
    }
}