using System;
using UnityEngine;

namespace ShipDock
{
    public enum UnitTypes
    {
        UnitData = 0,
    }

    /// <summary>
    /// 
    /// 框架单例
    /// 
    /// 以各个管理单元为主要构成的通用框架层
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class Framework : Singletons<Framework>
    {
        /// <summary>数据管理单元</summary>
        public const int UNIT_DATA = 0;
        /// <summary>IoC容器管理单元</summary>
        public const int UNIT_IOC = 1;
        /// <summary>业务大模块管理单元</summary>
        public const int UNIT_MODULARS = 2;
        /// <summary>ECS管理单元</summary>
        public const int UNIT_ECS = 3;
        /// <summary>资源包管理单元</summary>
        public const int UNIT_AB = 4;
        /// <summary>配置表管理单元</summary>
        public const int UNIT_CONFIG = 5;
        /// <summary>界面管理单元</summary>
        public const int UNIT_UI = 6;
        /// <summary>资源对象池管理单元</summary>
        public const int UNIT_ASSET_POOL = 7;
        /// <summary>状态机管理单元</summary>
        public const int UNIT_FSM = 8;
        /// <summary>音效单元</summary>
        public const int UNIT_SOUND = 9;
        /// <summary>特效单元</summary>
        public const int UNIT_FX = 10;
        /// <summary>消息泵</summary>
        public const int UNIT_MSG_LOOPER = 11;
        /// <summary>测试器</summary>
        public const int UNIT_TESTER = 12;

        /// <summary>保存需要与框架的启动同步调用的外部方法</summary>
        private Action mOnNewFrame;
        /// <summary>总线帧更新组件就绪事件</summary>
        private event Action mUpdateCompReadyEvent;
        /// <summary>框架启动事件</summary>
        private event Action<bool> mFrameworkStartUpEvent;

        /// <summary>框架定制应用对象</summary>
        public ICustomCore App { get; private set; }
        /// <summary>帧更新组件预存对象</summary>
        public IUpdatesComponent Updates { get; set; }
        /// <summary>是否为定制内核模式</summary>
        public bool IsCustomCoreMode { get; private set; }

        /// <summary>定制框架是否已启动</summary>
        public bool IsStarted
        {
            get
            {
                bool result;
                if (IsCustomCoreMode)
                {
                    result = App != default && App.IsStarted;
                }
                else
                {
                    result = mUnits.Size > 0;
                }
                return result;
            }
            set
            {
                if (App != default)
                {
                    App.SetStarted(value);
                }
                else { }
            }
        }

        /// <summary>各管理单元映射</summary>
        private KeyValueList<int, IFrameworkUnit> mUnits;

        public Framework()
        {
            mUnits = new KeyValueList<int, IFrameworkUnit>();
            mUnits.ApplyMapper();
        }

        /// <summary>清除定制框架</summary>
        public void Clean()
        {
            if (IsCustomCoreMode)
            {
                if (IsStarted)
                {
                    IsStarted = false;
                    App?.Clean();
                    App = default;
                    Utils.Reclaim(ref mUnits, true, true);
                    mUnits = new KeyValueList<int, IFrameworkUnit>();
                }
                else { }
            }
            else
            {
                //通知其他功能，框架已关闭
                mFrameworkStartUpEvent?.Invoke(false);
                AllPools.ResetAllPooling();

                Utils.Reclaim(ref mUnits, true, true);
            }

            Updates = default;
            mUpdateCompReadyEvent = default;
            mFrameworkStartUpEvent = default;
        }

        /// <summary>为定制框架创建桥接单元</summary>
        public IFrameworkUnit CreateUnitByBridge<T>(int name, T target)
        {
            return new FrameworkUnitBrige<T>(name, target);
        }

        /// <summary>
        /// 加载框架的管理单元
        /// </summary>
        /// <param name="units"></param>
        public void LoadUnit(params IFrameworkUnit[] units)
        {
            int max = units.Length;
            IFrameworkUnit unit;
            for (int i = 0; i < max; i++)
            {
                unit = units[i];
                if (!mUnits.ContainsKey(unit.Name))
                {
                    mUnits[unit.Name] = unit;
                }
                else { }
            }
        }

        /// <summary>
        /// 重新加载框架的管理单元
        /// </summary>
        /// <param name="units"></param>
        public void ReloadUnit(params IFrameworkUnit[] units)
        {
            int max = units.Length;
            IFrameworkUnit unit;
            for (int i = 0; i < max; i++)
            {
                unit = units[i];
                mUnits[unit.Name] = unit;
            }
        }

        public T GetUnit<T>(int name)
        {
            if (mUnits == default)
            {
                return default;
            }
            else { }

            IFrameworkUnit unit = mUnits[name];

            T result;
            if (unit is FrameworkUnitBrige<T> bridge)
            {
                result = bridge.Unit;
            }
            else
            {
                result = (T)unit;
            }
            return result;
        }

        public void OnUpdateComponentReady(Action handler)
        {
            mUpdateCompReadyEvent += handler;
        }

        public void RemoveUpdateComponentReady(Action handler)
        {
            mUpdateCompReadyEvent -= handler;
        }

        public void OnFrameworkStartUp(Action<bool> handler)
        {
            mFrameworkStartUpEvent += handler;
        }

        public void RemoveFrameworkStartUp(Action<bool> handler)
        {
            mFrameworkStartUpEvent -= handler;
        }

        public void Init(int ticks, IFrameworkUnit[] units = default, Action onStartUp = default)
        {
            Application.targetFrameRate = ticks <= 0 ? 10 : ticks;
            
            Updates.Init();
            Updates.SyncToFrame(mOnNewFrame);
            Updates.SyncToFrame(onStartUp);

            //加载各个控制单元
            LoadUnit(units);

            //通知其他功能，帧更新组件已就绪
            mUpdateCompReadyEvent?.Invoke();
            //通知其他功能，框架已启动
            mFrameworkStartUpEvent?.Invoke(true);

            //清除定制框架对一些临时组件或对象的引用
            mOnNewFrame = default;
        }

        /// <summary>
        /// 初始化一个定制框架
        /// </summary>
        /// <param name="app">定制框架的对象</param>
        /// <param name="ticks">子线程的运行帧率</param>
        /// <param name="onStartUp">定制框架启动后的回调函数</param>
        public void InitByCustomCore(ICustomCore app, int ticks, Action onStartUp = default)
        {
            if (App == default)
            {
                IsCustomCoreMode = true;

                App = app;
                App.SetUpdatesComponent(Updates);
                App.SyncToUpdates(mOnNewFrame);
                App.SyncToUpdates(onStartUp);
                App.Run(ticks);

                //加载各个控制单元
                LoadUnit(App.FrameworkUnits);

                //清除定制框架对一些临时组件或对象的引用
                Updates = default;
                mOnNewFrame = default;
            }
            else { }
        }

        /// <summary>
        /// 下一帧需要执行的函数
        /// </summary>
        /// <param name="method"></param>
        public void CallOnNextFrame(Action method)
        {
            if (IsCustomCoreMode)
            {
                if (App != default)
                {
                    App.SyncToUpdates(method);
                }
                else
                {
                    mOnNewFrame += method;
                }
            }
            else
            {
                if (IsStarted)
                {
                    Updates.SyncToFrame(mOnNewFrame);
                }
                else
                {
                    mOnNewFrame += method;
                }
            }
        }
    }

    public static class FrameworkExtensions
    {
        public static T Unit<T>(this int unitType)
        {
            return Framework.Instance.GetUnit<T>(unitType);
        }
    }
}
