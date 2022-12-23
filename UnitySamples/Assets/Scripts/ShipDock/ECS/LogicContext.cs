
using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public static class ShipDockECSSetting
    {
        /// <summary>是否启用帧后更新模式</summary>
        public static bool isUpdateByCallLate = false;
    }

    public class ECSContext : IReclaim
    {
        public ILogicContext CurrentContext { get; private set; }

        private int mFrameTimeInScene;
        private KeyValueList<int, ILogicContext> mMapper;

        public ECSContext(int frameTimeInScene)
        {
            mFrameTimeInScene = frameTimeInScene;
            mMapper = new KeyValueList<int, ILogicContext>();
        }

        public void Reclaim()
        {
            CurrentContext = default;
            Utils.Reclaim(ref mMapper, true, true);
        }

        public void CreateContext(int name)
        {
            if (mMapper.ContainsKey(name)) { }
            else
            {
                mMapper[name] = new LogicContext()
                {
                    FrameTimeInScene = mFrameTimeInScene,
                };
            }
        }

        public ILogicContext GetContext(int name = int.MaxValue)
        {
            ILogicContext context = default;
            if (mMapper.Size > 0)
            {
                name = (name == int.MaxValue) ? mMapper.Keys[0] : name;
                context = mMapper[name];
            }
            else { }
            return context;
        }

        public void ActiveECSContext(int name = int.MaxValue)
        {
            CurrentContext = GetContext(name);
        }
    }

    /// <summary>
    /// 
    /// ECS上下文环境
    /// 
    /// </summary>
    public class LogicContext : ILogicContext
    {
        private int mFinalUpdateComp;
        private int mFinalUpdateEntitas;
        private ILogicData mFinalUpdateData;
        /// <summary>当前执行更新的系统</summary>
        private ILogicSystem mSystem;
        /// <summary>所有组件及系统</summary>
        private List<ILogicSystem> mSystems;
        /// <summary>需要在子线程中更新的组件 ID 列表</summary>
        private List<int> mUpdateByTicks;
        /// <summary>需要在主线程中更新的组件 ID 列表</summary>
        private List<int> mUpdateByScene;
        /// <summary>组件对象映射 </summary>
        private KeyValueList<int, ILogicComponent> mComponents;
        #region 帧末调用队列相关
        private Action<int, int, ILogicData> mFinalUpdateMethod;
        private DoubleBuffers<int> mQueueUpdateComp;
        private DoubleBuffers<int> mQueueUpdateEntitas;
        private DoubleBuffers<ILogicData> mQueueUpdateDatas;
        private DoubleBuffers<Action<int, int, ILogicData>> mQueueUpdateExecute;
        #endregion

        public ILogicEntities AllEntitas { get; private set; }

        /// <summary>预更新</summary>
        public Action<List<int>, bool> PreUpdate { get; set; }
        public int CountTime { get; private set; }
        public int FrameTimeInScene { get; set; }

        public LogicContext()
        {
            mSystem = default;
            mSystems = new List<ILogicSystem>();
            mComponents = new KeyValueList<int, ILogicComponent>();

            mUpdateByTicks = new List<int>();
            mUpdateByScene = new List<int>();

            mQueueUpdateEntitas = new DoubleBuffers<int>();
            mQueueUpdateComp = new DoubleBuffers<int>();
            mQueueUpdateDatas = new DoubleBuffers<ILogicData>();
            mQueueUpdateExecute = new DoubleBuffers<Action<int, int, ILogicData>>();
            mQueueUpdateExecute.OnDequeue += OnQueueUpdateExecute;

            AllEntitas = new LogicEntities();
        }

        #region 销毁和重置
        public void Dispose()
        {
            Utils.Reclaim(ref mUpdateByTicks);
            Utils.Reclaim(ref mUpdateByScene);
            Utils.Reclaim(mQueueUpdateComp);
            Utils.Reclaim(mQueueUpdateEntitas);
            Utils.Reclaim(mQueueUpdateExecute);
            Utils.Reclaim(ref mSystems);
            Utils.Reclaim(mComponents);

            PreUpdate = default;
            mSystem = default;
        }
        #endregion

        #region 创建组件或系统
        public int Create<T>(T target, int name, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IECSLogic
        {
            int statu = 0;
            if (target is ILogicSystem system)
            {
                //若为系统则设置关联的组件
                int componentID;
                int max = willRelateComponents.Length;
                for (int i = 0; i < max; i++)
                {
                    componentID = willRelateComponents[i];
                    system.RelateComponent(componentID);
                }

                SetupSystem(name, system, isUpdateByScene);
            }
            else if (target is ILogicComponent component)
            {
                SetupComponent(name, ref component);
            }
            else
            {
                statu = 1;
            }

            if (statu == 0)
            {
                if (target.IsSystem)
                {
                    const string logECSComp = "log: Add ECS system - {0}";
                    logECSComp.Log(target.Name);
                }
                else
                {
                    const string logECSSys = "log: Add ECS component - {0}";
                    logECSSys.Log(target.Name);
                }
            }
            else { }

            return statu;
        }

        public int Create<T>(int logicName, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IECSLogic, new()
        {
            T target = new T();

            int result = Create(target, logicName, isUpdateByScene, willRelateComponents);
            
            return result;
        }

        private bool SetupComponent<T>(int logicName, ref T target) where T : ILogicComponent
        {
            bool result = default;
            ILogicComponent component = mComponents[logicName];
            if (mComponents.ContainsKey(logicName)) { }
            else
            {
                mComponents[logicName] = target;

                target.SetUnitID(logicName);
                target.Init(this);
                target.SetAllEntitas(AllEntitas);

                result = true;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 设置组件的更新模式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">组件</param>
        /// <param name="isUpdateByScene">是否使用场景主线程更新</param>
        private bool SetupSystem<T>(int name, T target, bool isUpdateByScene = false) where T : ILogicSystem
        {
            int index = mSystems.Count;
            mSystems.Add(target);

            target.SetUnitID(name);
            target.Init(this);
            target.SetAllEntitas(AllEntitas);

            target.SetSceneUpdate(isUpdateByScene);

            target.OnFinalUpdateForEntitas = OnFinalUpdateForEntitas;
            target.OnFinalUpdateForComp = OnFinalUpdateForComp;
            target.OnFinalUpdateForData = OnFinalUpdateForData;
            target.OnFinalUpdateForExecute = OnFinalUpdateForExecute;

            if (isUpdateByScene)
            {
                //使用主线程的帧更新组件
                mUpdateByScene.Add(index);
            }
            else
            {
                //使用子线程的帧更新组件
                mUpdateByTicks.Add(index);
            }
            return true;
        }

        /// <summary>
        /// 通过组件名获取一个已创建的组件，但此组件有可能不在某个实体，或没被某系统绑定
        /// </summary>
        public ILogicComponent RefComponentByName(int name)
        {
            return mComponents != default ? mComponents[name] : default;
        }

        #region 帧末更新队列相关
        private void OnFinalUpdateForExecute(Action<int, int, ILogicData> method)
        {
            mQueueUpdateExecute.Enqueue(method, false);
        }

        private void OnFinalUpdateForEntitas(int entitas)
        {
            mQueueUpdateEntitas.Enqueue(entitas, false);
        }

        private void OnFinalUpdateForComp(int compName)
        {
            mQueueUpdateComp.Enqueue(compName, false);
        }

        private void OnFinalUpdateForData(ILogicData data)
        {
            mQueueUpdateDatas.Enqueue(data, false);
        }

        /// <summary>
        /// 执行所有需要帧末执行的队列业务
        /// </summary>
        /// <param name="time"></param>
        private void FinalUpdate(int time)
        {
            mQueueUpdateEntitas.Step(time);
            mQueueUpdateComp.Step(time);
            mQueueUpdateDatas.Step(time);
            mQueueUpdateExecute.Step(time);

            mFinalUpdateEntitas = default;
            mFinalUpdateMethod = default;
        }

        /// <summary>
        /// 提供于外部双缓冲更新器使用的回调方法
        /// </summary>
        /// <param name="time"></param>
        /// <param name="current"></param>
        private void OnQueueUpdateExecute(int time, Action<int, int, ILogicData> current)
        {
            mFinalUpdateEntitas = mQueueUpdateEntitas.Current;
            mFinalUpdateComp = mQueueUpdateComp.Current;
            mFinalUpdateData = mQueueUpdateDatas.Current;

            mFinalUpdateMethod = current;
            mFinalUpdateMethod?.Invoke(mFinalUpdateEntitas, mFinalUpdateComp, mFinalUpdateData);
        }
        #endregion

        #region 根据当前的执行顺序获取组件
        private void FillSystemByIndex(int value, ref int systemIndex, ref List<int> updateList, out ILogicSystem system)
        {
            systemIndex = updateList[value];
            system = (systemIndex < mSystems.Count) ? mSystems[systemIndex] : default;
        }
        #endregion

        #region 更新上下文的所有系统、组件相关
        /// <summary>
        /// 更新系统中的组件（运行于子线程）
        /// </summary>
        public void UpdateECSUnits(int time, Action<Action<int>> method = default)
        {
            //与主线程的帧率时间保持一致，避免更新过快
            CountTime += time;

            ILogicSystem item = default;
            if (CountTime > FrameTimeInScene)
            {
                CountTime -= FrameTimeInScene;
                //CheckAllDropedEntitas();

                PreUpdate?.Invoke(mUpdateByTicks, false);

                int compIndex = 0;
                Action<int> onUpdate;
                int max = mUpdateByTicks.Count;
                for (int i = 0; i < max; i++)
                {
                    FillSystemByIndex(i, ref compIndex, ref mUpdateByTicks, out item);
                    if (item != default)
                    {
                        if(item.IsSceneUpdate)
                        {
                            continue;
                        }
                        else { }

                        if (method == default)
                        {
                            //同步更新
                            item.UpdateComponents(time);
                        }
                        else
                        {
                            //异步更新
                            onUpdate = item.UpdateComponents;
                            method.Invoke(onUpdate);
                        }
                    }
                    else { }
                }

                FinalUpdate(time);
            }
        }

        /// <summary>
        /// 更新系统中的组件（主线程）
        /// </summary>
        public void UpdateECSUnitsInScene(int time, Action<Action<int>> method = default)
        {
            CheckAllDropedEntitas();

            int compIndex = 0;
            Action<int> onUpdate;
            int max = mUpdateByScene.Count;
            ILogicSystem item = default;
            for (int i = 0; i < max; i++)
            {
                FillSystemByIndex(i, ref compIndex, ref mUpdateByScene, out item);

                if (item != default)
                {
                    if (method == default)
                    {
                        //同步更新
                        item.UpdateComponents(time);
                    }
                    else
                    {
                        //异步更新
                        onUpdate = item.UpdateComponents;
                        method.Invoke(onUpdate);
                    }
                }
                else { }
            }

            //CheckAllValidDatas();
        }

        private void CheckAllDropedEntitas()
        {
            ILogicComponent comp;
            List<ILogicComponent> list = mComponents.Values;
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                comp = list[i];
                comp?.CheckAllDropedEntitas();
            }
        }

        private void CheckAllValidDatas()
        {
            ILogicComponent comp;
            List<ILogicComponent> list = mComponents.Values;
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                comp = list[i];
                comp?.CheckAllDataValided();
            }
        }
        #endregion
    }
}