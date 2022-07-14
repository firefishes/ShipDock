﻿using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Modulars
{
    /// <summary>
    /// 
    /// 修饰化模块处理器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    /// <typeparam name="TMethod">模块内各环节的处理方法</typeparam>
    /// <typeparam name="TMethodPriority">描述模块优先级的对象</typeparam>
    public class ModularHandlers<TMethod, TMethodPriority> where TMethodPriority : IModularMethodPriority
    {
        /// <summary>处理器函数委托链映射</summary>
        private KeyValueList<int, TMethod> mHandlers;
        /// <summary>优先级列表</summary>
        private List<TMethod> mSourceMethod;
        /// <summary>优先级列表</summary>
        private List<TMethodPriority> mSourceMethodPriority;
        /// <summary>需要做优先级排序的数据映射</summary>
        private KeyValueList<int, List<TMethodPriority>> mPriorities;
        /// <summary>用于优先级排序的临时成员</summary>
        private List<TMethodPriority> mWillSorts;

        /// <summary>模块装饰相关委托的多播连接器方法</summary>
        private Func<TMethod, TMethodPriority, bool, TMethod> OnHandlerSetter { get; set; }
        /// <summary>模块装饰相关方法的读取器方法</summary>
        private Func<TMethodPriority, TMethod> OnHandlerGetter { get; set; }

        public bool HasPriorityMin { get; private set; }
        public Action<int, TMethod> BeforeHandlersSort { get; set; }
        public Action<int, TMethod> AfterHandlersSorted { get; set; }

        /// <summary>
        /// 修饰化模块处理器构造函数
        /// </summary>
        /// <param name="hasPriorityMin"></param>
        /// <param name="setter">模块内各环节的处理方法修改器</param>
        /// <param name="getter">模块内各环节的处理方法读取器</param>
        public ModularHandlers(bool hasPriorityMin, Func<TMethod, TMethodPriority, bool, TMethod> setter, Func<TMethodPriority, TMethod> getter)
        {
            mSourceMethod = new List<TMethod>();
            mSourceMethodPriority = new List<TMethodPriority>();

            mPriorities = new KeyValueList<int, List<TMethodPriority>>();
            mHandlers = new KeyValueList<int, TMethod>();

            HasPriorityMin = hasPriorityMin;

            OnHandlerSetter = setter;
            OnHandlerGetter = getter;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            Utils.Reclaim(ref mHandlers, true);
            Utils.Reclaim(ref mSourceMethodPriority, true, true);

            OnHandlerSetter = default;
            OnHandlerGetter = default;
            mHandlers = default;
            AfterHandlersSorted = default;
        }

        /// <summary>
        /// 根据消息获取各处理器函数的委托链
        /// </summary>
        /// <param name="noticeName"></param>
        /// <returns></returns>
        public TMethod GetHandler(int noticeName)
        {
            return mHandlers[noticeName];
        }

        /// <summary>
        /// 添加模块内环节处理器函数的委托
        /// </summary>
        public virtual void AddHandler(ref TMethodPriority target, bool willSort = false)
        {
            if (!mSourceMethodPriority.Contains(target))
            {
                TMethod method = OnHandlerGetter.Invoke(target);
                mSourceMethod.Add(method);
                mSourceMethodPriority.Add(target);

                int noticeName = target.NoticeName;
                if (mPriorities.ContainsKey(noticeName))
                {
                    mWillSorts = mPriorities[noticeName];
                }
                else
                {
                    mWillSorts = new List<TMethodPriority>();
                    mPriorities[noticeName] = mWillSorts;
                }

                mWillSorts.Add(target);

                if (willSort)
                {
                    int max = mPriorities.Size;
                    List<int> keys = mPriorities.Keys;
                    for (int i = 0; i < max; i++)
                    {
                        mWillSorts = mPriorities.GetValueByIndex(i);

                        //优先级排序
                        noticeName = keys[i];
                        SortHandlers(noticeName);
                        mWillSorts = default;
                    }
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 优先级排序
        /// </summary>
        /// <param name="noticeName"></param>
        /// <param name="target"></param>
        /// <param name="willSort"></param>
        private void SortHandlers(int noticeName)
        {
            TMethodPriority item = default;
            TMethod handler = default, value = default;

            mHandlers[noticeName] = default;

            RefillHandler(noticeName, ref item, ref handler, ref value, false, BeforeHandlersSort);

            mWillSorts.Sort(OnSort);

            RefillHandler(noticeName, ref item, ref handler, ref value, true, AfterHandlersSorted);
        }

        private void RefillHandler(int noticeName, ref TMethodPriority item, ref TMethod handler, ref TMethod value, bool isAdd, Action<int, TMethod> onRefill = default)
        {
            int max = mWillSorts.Count;
            for (int i = 0; i < max; i++)
            {
                item = mWillSorts[i];

                if (onRefill != default)
                {
                    handler = OnHandlerGetter != default ? OnHandlerGetter.Invoke(item) : default;

                    if (handler != null)
                    {
                        onRefill?.Invoke(noticeName, handler);
                    }
                    else { }
                }
                else
                {
                    SetHandler(noticeName, ref item, ref handler, ref value, isAdd);
                }
            }
        }

        /// <summary>
        /// 修改处理器函数
        /// </summary>
        /// <param name="target"></param>
        private void SetHandler(int noticeName, ref TMethodPriority item, ref TMethod handler, ref TMethod value, bool isAdd)
        {
            handler = OnHandlerGetter != default ? OnHandlerGetter.Invoke(item) : default;

            if (handler != null)
            {
                if (!mHandlers.ContainsKey(noticeName))
                {
                    mHandlers[noticeName] = default;
                }
                else { }

                if (OnHandlerSetter != default)
                {
                    value = mHandlers[noticeName];
                    value = OnHandlerSetter.Invoke(value, item, isAdd);
                    mHandlers[noticeName] = value;
                }
                else { }

                "log:Set notice {0} decorator, priority is {1}, ".Log(noticeName.ToString(), item.Priority.ToString());
            }
            else { }
        }

        private int OnSort(TMethodPriority x, TMethodPriority y)
        {
            if (x.Priority > y.Priority)
            {
                return 1;
            }
            else if (x.Priority < y.Priority)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 根据消息移除环节处理器函数
        /// </summary>
        /// <param name="noticeName"></param>
        /// <param name="method"></param>
        public void RemoveHandler(int noticeName, TMethod method)
        {
            int index = mSourceMethod.IndexOf(method);
            if (index >= 0)
            {
                mSourceMethod.RemoveAt(index);

                TMethodPriority target = mSourceMethodPriority[index];
                mSourceMethodPriority.RemoveAt(index);

                List<TMethodPriority> prioriteList = mPriorities[noticeName];
                prioriteList.Remove(target);

                if (mHandlers.ContainsKey(noticeName))
                {
                    "Creater {0} removed.".Log(noticeName.ToString());

                    TMethod value = mHandlers[noticeName];
                    value = OnHandlerSetter.Invoke(value, target, false);
                    mHandlers[noticeName] = value;
                }
                else { }
            }
            else { }
        }
    }
}
