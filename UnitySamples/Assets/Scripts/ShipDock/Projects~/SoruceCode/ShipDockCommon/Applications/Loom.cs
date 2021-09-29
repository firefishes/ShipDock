using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>
/// 多线程协同组件
/// </summary>
public class Loom : MonoBehaviour
{
    public class NoDelayedQueueItem
    {
        public Action<object> action;
        public object param;
    }

    public class DelayedQueueItem
    {
        public float time;
        public Action<object> action;
        public object param;
    }

    public static int maxThreads = 8;

    private static int numThreads;
    private static bool initialized;
    private static Loom instance;

    public static Loom Current
    {
        get
        {
            Initialize();
            return instance;
        }
    }

    public static void Initialize()
    {
        if (!initialized)
        {
            if (Application.isPlaying)
            {
                initialized = true;

                GameObject loom = new GameObject("Loom");
                instance = loom.AddComponent<Loom>();

                DontDestroyOnLoad(loom);
            }
            else { }
        }
        else { }
    }

    public static void QueueOnMainThread(Action<object> taction, object tparam)
    {
        QueueOnMainThread(taction, tparam, 0f);
    }

    public static void QueueOnMainThread(Action<object> handler, object param, float time)
    {
        if (time != 0)
        {
            lock (Current.mDelayed)
            {
                Current.mDelayed.Add(new DelayedQueueItem
                {
                    time = Time.time + time,
                    action = handler,
                    param = param
                });
            }
        }
        else
        {
            lock (Current.mActions)
            {
                Current.mActions.Add(new NoDelayedQueueItem {
                    action = handler,
                    param = param
                });
            }
        }
    }

    public static Thread RunAsync(Action threadMethod)
    {
        Initialize();

        while (numThreads >= maxThreads)
        {
            Thread.Sleep(100);
        }

        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, threadMethod);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
        }
    }

    private List<NoDelayedQueueItem> mCurrentActions = new List<NoDelayedQueueItem>();
    private List<NoDelayedQueueItem> mActions = new List<NoDelayedQueueItem>();
    private List<DelayedQueueItem> mDelayed = new List<DelayedQueueItem>();
    private List<DelayedQueueItem> mCurrentDelayed = new List<DelayedQueueItem>();

    private void OnDestroy()
    {
        if (instance == this)
        {
            initialized = false;
            instance = null;
        }
        else { }
    }

    // Update is called once per frame
    private void Update()
    {
        if (mActions.Count > 0)
        {
            lock (mActions)
            {
                mCurrentActions.Clear();
                mCurrentActions.AddRange(mActions);
                mActions.Clear();
            }
            for (int i = 0; i < mCurrentActions.Count; i++)
            {
                mCurrentActions[i].action.Invoke(mCurrentActions[i].param);
            }
        }
        else { }

        if (mDelayed.Count > 0)
        {
            lock (mDelayed)
            {
                mCurrentDelayed.Clear();
                mCurrentDelayed.AddRange(mDelayed.Where(d => d.time <= Time.time));

                for (int i = 0; i < mCurrentDelayed.Count; i++)
                {
                    mDelayed.Remove(mCurrentDelayed[i]);
                }
            }

            for (int i = 0; i < mCurrentDelayed.Count; i++)
            {
                mCurrentDelayed[i].action(mCurrentDelayed[i].param);
            }
        }
        else { }
    }
}