using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    /// <summary>
    /// 数据代理
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class DataProxy : IDataProxy
    {
        /// <summary>最大的消息堆栈数</summary>
        public static int allowLoopedStackOverflow = 5;

        /// <summary>前一个数据通知名</summary>
        private int mLooped;
        /// <summary>前一个数据通知名</summary>
        private int mInterceptName;
        /// <summary>数据通知栈</summary>
        private Stack<int> mDataNotifyStack;
        /// <summary>数据提取器列表</summary>
        private List<IDataExtracter> mDataHandlers;
        /// <summary>数据代理通知回调</summary>
        private Action<IDataProxy, int> mOnDataProxyNotify;
        /// <summary>数据代理通知回调（无提取器）</summary>
        private Action<IDataProxy, int> mOnDataProxyNotifyForMethod;

        /// <summary>数据代理名</summary>
        public virtual int DataName { get; private set; }

        public DataProxy()
        {
            mInterceptName = int.MinValue;
            mDataNotifyStack = new Stack<int>();
            mDataHandlers = new List<IDataExtracter>();
        }

        public DataProxy(int dataName) : this()
        {
            DataName = dataName;
        }

        public virtual void Dispose()
        {
            Utils.Reclaim(ref mDataNotifyStack);
            Utils.Reclaim(ref mDataHandlers);

            mLooped = 0;
            mInterceptName = int.MinValue;
            mOnDataProxyNotify = default;
            mOnDataProxyNotifyForMethod = default;
        }

        /// <summary>
        /// 批量发送数据变更通知
        /// </summary>
        /// <param name="keys"></param>
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

        /// <summary>
        /// 发送数据变更通知
        /// </summary>
        /// <param name="keyName"></param>
        public void DataNotify(int keyName)
        {
            bool flag = mInterceptName == keyName;
            if (flag)
            {
                mLooped++;
            }
            else { }

            //检测循环次数是否大于最大次数
            flag = (mDataNotifyStack.Count > 1) ? (allowLoopedStackOverflow > mLooped) : true;
            if (flag)
            {
                //无循环调用时才可派发数据变更通知
                bool willSetStart = mDataNotifyStack.Count == 0;

                mDataNotifyStack.Push(keyName);

                if (willSetStart)
                {
                    //设置中断通知名
                    mInterceptName = mDataNotifyStack.Peek();
                }
                else { }

                //调用数据变更回调
                mOnDataProxyNotify?.Invoke(this, keyName);
                mOnDataProxyNotifyForMethod?.Invoke(this, keyName);

                mDataNotifyStack.Pop();
            }
            else { }

            if (mDataNotifyStack.Count == 0)
            {
                //重置中断通知名
                mInterceptName = int.MinValue;
                mLooped = 0;
            }
            else { }
        }

        /// <summary>
        /// 注册数据提取器对象
        /// </summary>
        /// <param name="dataHandler"></param>
        public virtual void Register(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            else { }

            mDataHandlers.Add(dataHandler);
            AddDataProxyNotify(dataHandler.OnDataProxyNotify, true);
        }

        /// <summary>
        /// 注销数据提取器对象
        /// </summary>
        /// <param name="dataHandler"></param>
        public virtual void Unregister(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || (mDataHandlers == default) || !mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            else { }

            mDataHandlers.Remove(dataHandler);
            RemoveDataProxyNotify(dataHandler.OnDataProxyNotify, true);
        }

        /// <summary>
        /// 添加数据提取回调方法
        /// </summary>
        /// <param name="notifyHandler"></param>
        public void AddDataProxyNotify(Action<IDataProxy, int> notifyHandler, bool isDataExtracter = false)
        {
            if (isDataExtracter)
            {
                mOnDataProxyNotify += notifyHandler;
            }
            else
            {
                mOnDataProxyNotifyForMethod += notifyHandler;
            }
        }

        /// <summary>
        /// 移除数据提取回调方法
        /// </summary>
        /// <param name="notifyHandler"></param>
        public void RemoveDataProxyNotify(Action<IDataProxy, int> notifyHandler, bool isDataExtracter = false)
        {
            if (isDataExtracter)
            {
                mOnDataProxyNotify -= notifyHandler;
            }
            else
            {
                mOnDataProxyNotifyForMethod -= notifyHandler;
            }
        }
    }
}
