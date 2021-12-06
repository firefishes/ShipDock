using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Modulars
{
    public class ModularHandlers<A, T> where T : IPriority
    {
        /// <summary>处理器函数的映射</summary>
        private KeyValueList<int, A> mHandlers;
        /// <summary>处理器函数顺序映射</summary>
        private LinkedList<T> mHandlersRank;
        /// <summary>处理器函数优先级映射</summary>
        private KeyValueList<int, List<A>> mHandlersPriorities;

        private Func<A, T, A> OnHandlerCache { get; set; }

        public Func<T, A> OnGetHandlerForPriority { get; set; }
        public Action<int, A> OnAfterHandlerReset { get; set; }
        public bool HasPriorityMin { get; private set; }

        public ModularHandlers(bool hasPriorityMin, Func<A, T, A> method)
        {
            mHandlersRank = new LinkedList<T>();
            mHandlers = new KeyValueList<int, A>()
            {
                [int.MinValue] = default,
            };

            HasPriorityMin = hasPriorityMin;
            if (hasPriorityMin)
            {
                mHandlersPriorities = new KeyValueList<int, List<A>>();
            }
            else { }

            OnHandlerCache = method;
        }

        public A GetHandler(int noticeName)
        {
            return mHandlers[noticeName];
        }

        public virtual void AddHandler(int noticeName, T target)
        {
            Reset(noticeName);//清理

            if(HasPriorityMin)
            {
                SetHandler(noticeName, target);
            }
            else
            {
                T nodeValue;
                LinkedListNode<T> node = mHandlersRank.First;

                if (node == default)
                {
                    mHandlersRank.AddFirst(target);//首次添加
                    SetHandler(noticeName, target);
                }
                else
                {
                    bool isGreater = false;
                    while (node != default)
                    {
                        nodeValue = node.Value;
                        SetHandler(noticeName, nodeValue);//优先级较小

                        if (target.Priority >= nodeValue.Priority)
                        {
                            node = mHandlersRank.AddAfter(node, target);
                            SetHandler(noticeName, target);//优先级最大
                            isGreater = true;
                        }
                        else { }

                        node = node.Next;
                    }

                    if (false == isGreater)
                    {
                        mHandlersRank.AddFirst(target);
                        SetHandler(noticeName, target);//优先级最小
                    }
                    else { }
                }
            }
        }

        public void Reset(int noticeName)
        {
            if(HasPriorityMin)
            {
                A handler;
                List<A> list = mHandlersPriorities[noticeName];
                if (list != default)
                {
                    int max = list.Count;
                    for (int i = 0; i < max; i++)
                    {
                        handler = list[i];
                        OnAfterHandlerReset?.Invoke(noticeName, handler);
                    }

                    list.Clear();
                    list.TrimExcess();
                }
                else { }

                handler = mHandlers.GetValue(noticeName, true);
                if (handler != default)
                {
                    OnAfterHandlerReset?.Invoke(noticeName, handler);
                }
                else { }
            }
            else
            {
                mHandlers[noticeName] = default;
            }
        }

        private void SetHandler(int noticeName, T target)
        {
            if (!mHandlers.ContainsKey(noticeName))
            {
                mHandlers[noticeName] = default;
            }
            else { }

            "log:Set notice {0} decorator, priority is {1}, ".Log(noticeName.ToString(), target.Priority.ToString());

            if(HasPriorityMin)
            {
                if (int.MinValue == target.Priority)
                {
                    if (OnHandlerCache != default)
                    {
                        A value = mHandlers[noticeName];
                        value = OnHandlerCache.Invoke(value, target);
                        mHandlers[noticeName] = value;
                    }
                    else { }
                }
                else
                {
                    List<A> list;
                    if (mHandlersPriorities.ContainsKey(noticeName))
                    {
                        list = mHandlersPriorities[noticeName];
                    }
                    else
                    {
                        list = new List<A>();
                        mHandlersPriorities[noticeName] = list;
                    }

                    if (OnGetHandlerForPriority != default)
                    {
                        A method = OnGetHandlerForPriority.Invoke(target);

                        bool flag = list.Contains(method);
                        if (!flag)
                        {
                            //"log:Set notice {0} listener, priority is {1}, ".Log(noticeName.ToString(), listener.Priority.ToString());

                            list.Add(method);
                        }
                        else { }
                    }
                    else { }
                }
            }
            else
            {
                A value = mHandlers[noticeName];
                OnHandlerCache?.Invoke(value, target);
                mHandlers[noticeName] = value;
            }
        }

        public void RemoveHandler(int noticeName)
        {
            if (mHandlers.ContainsKey(noticeName))
            {
#if LOG_MODULARS
                "Creater {0} removed.".Log(noticeName.ToString());
#endif
                mHandlers.Remove(noticeName);
            }
            else { }
        }
    }
}
