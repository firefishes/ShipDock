#define _G_LOG

using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public static class ShipDockECSSetting
    {
        /// <summary>是否启用合并更新模式</summary>
        public static bool isMergeUpdateMode = false;
        /// <summary>是否启用帧后更新模式</summary>
        public static bool isUpdateByCallLate = false;
    }

    public class ECSContext : IReclaim
    {
        public IShipDockComponentContext CurrentContext { get; private set; }

        private int mFrameTimeInScene;
        private KeyValueList<int, IShipDockComponentContext> mMapper;

        public ECSContext(int frameTimeInScene)
        {
            mFrameTimeInScene = frameTimeInScene;
            mMapper = new KeyValueList<int, IShipDockComponentContext>();
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
                mMapper[name] = new ShipDockComponentContext()
                {
                    FrameTimeInScene = mFrameTimeInScene,
                };
            }
        }

        public IShipDockComponentContext GetContext(int name = int.MaxValue)
        {
            IShipDockComponentContext context = default;
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
    public class ShipDockComponentContext : IShipDockComponentContext
    {
        private int mFinalUpdateTime;
        private IShipDockEntitas mFinalUpdateEntitas;
        /// <summary>当前执行更新的系统</summary>
        private IShipDockComponent mSystem;
        /// <summary>所有组件及系统</summary>
        private List<IShipDockComponent> mComponents;
        /// <summary>需要在子线程中更新的组件 ID 列表</summary>
        private List<int> mUpdateByTicks;
        /// <summary>需要在主线程中更新的组件 ID 列表</summary>
        private List<int> mUpdateByScene;
        /// <summary>被标记为须要删除的组件 ID 列表</summary>
        private List<int> mDeletedComponents;
        /// <summary>组件名与组件自动ID的映射 </summary>
        private KeyValueList<int, int> mNameAutoIDMapper;
        /// <summary>组件对象映射 </summary>
        private IntegerMapper<IShipDockComponent> mMapper;
        #region 帧末调用队列相关
        private Action<int, IShipDockEntitas> mFinalUpdateMethod;
        private DoubleBuffers<int> mQueueUpdateTime;
        private DoubleBuffers<IShipDockEntitas> mQueueUpdateEntitas;
        private DoubleBuffers<Action<int, IShipDockEntitas>> mQueueUpdateExecute;
        #endregion

        /// <summary>预更新</summary>
        public Action<List<int>, bool> PreUpdate { get; set; }
        /// <summary>重填充系统的关联组件的回调</summary>
        public Action<int, IShipDockComponent, IShipDockComponentContext> RelateComponentsReFiller { get; set; }
        public int CountTime { get; private set; }
        public int FrameTimeInScene { get; set; }

        public ShipDockComponentContext()
        {
            mSystem = default;
            mNameAutoIDMapper = new KeyValueList<int, int>();
            mMapper = new IntegerMapper<IShipDockComponent>();

            mDeletedComponents = new List<int>();
            mUpdateByTicks = new List<int>();
            mUpdateByScene = new List<int>();
            mComponents = new List<IShipDockComponent>();

            mQueueUpdateTime = new DoubleBuffers<int>();
            mQueueUpdateEntitas = new DoubleBuffers<IShipDockEntitas>();
            mQueueUpdateExecute = new DoubleBuffers<Action<int, IShipDockEntitas>>();

            mQueueUpdateExecute.OnDequeue += OnQueueUpdateExecute;
        }

        #region 销毁和重置
        public void Dispose()
        {
            Utils.Reclaim(ref mUpdateByTicks);
            Utils.Reclaim(ref mUpdateByScene);
            Utils.Reclaim(ref mDeletedComponents);
            Utils.Reclaim(ref mComponents);
            Utils.Reclaim(mQueueUpdateTime);
            Utils.Reclaim(mQueueUpdateEntitas);
            Utils.Reclaim(mQueueUpdateExecute);
            Utils.Reclaim(mMapper);
            Utils.Reclaim(mNameAutoIDMapper);
            RelateComponentsReFiller = default;
            PreUpdate = default;
            mSystem = default;
        }
        #endregion

        #region 创建组件或系统
        public int Create<T>(T target, int name, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IShipDockComponent
        {
            SetComponentUpdateMode(target, isUpdateByScene, willRelateComponents);
            AddComponentToMapper(name, ref target, out int autoID);

            return autoID;
        }

        public int Create<T>(int name, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IShipDockComponent, new()
        {
            T target = new T();

            int result = Create(target, name, isUpdateByScene, willRelateComponents);
            
            return result;
        }

        private void AddComponentToMapper<T>(int name, ref T target, out int autoID) where T : IShipDockComponent
        {
            autoID = mMapper.Add(target, out int statu);
            if (statu == 0)
            {
                mNameAutoIDMapper[name] = autoID;

                target.SetComponentID(autoID);
                target.Init(this);
                RelateComponentsReFiller?.Invoke(name, target, this);

                "log: Add ECS component {0}".Log(name.ToString());
            }
            else
            {
                autoID = -1;
            }
        }
        #endregion

        /// <summary>
        /// 设置组件的更新模式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">组件</param>
        /// <param name="isUpdateByScene">是否使用场景主线程更新</param>
        /// <param name="willRelateComponents">需要关联的组件</param>
        private void SetComponentUpdateMode<T>(T target, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IShipDockComponent
        {
            bool isSystem = target.IsSystem;
            if (isSystem)
            {
                //若为系统则设置关联的组件
                ISystemComponent system = target as ISystemComponent;
                system.RelateComponents = willRelateComponents;
            }
            else { }

            target.SetSceneUpdate(isUpdateByScene);
            target.OnFinalUpdateForTime = OnFinalUpdateForTime;
            target.OnFinalUpdateForEntitas = OnFinalUpdateForEntitas;
            target.OnFinalUpdateForExecute = OnFinalUpdateForExecute;

            mComponents.Add(target);

            int index = mComponents.Count - 1;
            if (isSystem)
            {
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
            }
            else { }
        }

        /// <summary>
        /// 创建一个新实体，根据参数指定的组件名向实体添加组件后返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameArgs"></param>
        /// <returns></returns>
        public T GetEntitasWithComponents<T>(params int[] nameArgs) where T : IShipDockEntitas, new()
        {
            T result = new T();

            int max = nameArgs.Length;
            int name;
            IShipDockComponent component;
            for (int i = 0; i < max; i++)
            {
                name = nameArgs[i];
                component = RefComponentByName(name);
                if (component != default)
                {
                    result.AddComponent(component);
                }
                else { }
            }
            return result;
        }

        /// <summary>
        /// 通过组件名获取一个已创建的组件，但此组件有可能不在某个实体，或没被某系统绑定
        /// </summary>
        public IShipDockComponent RefComponentByName(int name)
        {
            IShipDockComponent component = default;
            if (mNameAutoIDMapper.IsContainsKey(name))
            {
                int id = mNameAutoIDMapper[name];
                component = mMapper.Get(id);
            }
            else { }
            return component;
        }

        #region 帧末更新队列相关
        private void OnFinalUpdateForExecute(Action<int, IShipDockEntitas> method)
        {
            mQueueUpdateExecute.Enqueue(method, false);
        }

        private void OnFinalUpdateForEntitas(IShipDockEntitas entitas)
        {
            mQueueUpdateEntitas.Enqueue(entitas, false);
        }

        private void OnFinalUpdateForTime(int time)
        {
            mQueueUpdateTime.Enqueue(time, false);
        }

        /// <summary>
        /// 执行所有需要帧末执行的队列业务
        /// </summary>
        /// <param name="time"></param>
        private void FinalUpdate(int time)
        {
            mQueueUpdateTime.Step(time);
            mQueueUpdateEntitas.Step(time);
            mQueueUpdateExecute.Step(time);
            mFinalUpdateEntitas = default;
            mFinalUpdateMethod = default;
        }

        /// <summary>
        /// 提供于外部双缓冲更新器使用的回调方法
        /// </summary>
        /// <param name="time"></param>
        /// <param name="current"></param>
        private void OnQueueUpdateExecute(int time, Action<int, IShipDockEntitas> current)
        {
            mFinalUpdateTime = mQueueUpdateTime.Current;
            mFinalUpdateEntitas = mQueueUpdateEntitas.Current;
            mFinalUpdateMethod = current;

            if (mFinalUpdateEntitas == default)
            {
                mFinalUpdateMethod.Invoke(mFinalUpdateTime, default);
            }
            else
            {
                if (!mFinalUpdateEntitas.WillDestroy && (mFinalUpdateEntitas.ID != int.MaxValue))
                {
                    mFinalUpdateMethod.Invoke(mFinalUpdateTime, mFinalUpdateEntitas);
                }
                else { }
            }
        }
        #endregion

        #region 移除、销毁组件相关
        /// <summary>
        /// 标记需要移除的组件
        /// </summary>
        public void RemoveComponent(IShipDockComponent target)
        {
            if (!mDeletedComponents.Contains(target.ID))
            {
                mDeletedComponents.Add(target.ID);
            }
            else { }
        }

        /// <summary>
        /// 销毁已标记为可移除的组件
        /// </summary>
        public void RemoveSingedComponents()
        {
            IShipDockComponent target;
            int max = mDeletedComponents.Count;
            for (int i = 0; i < max; i++)
            {
                int id = mDeletedComponents[i];
                target = mMapper.Get(id);

                RemoveComponentAndClear(ref target, id);

                target.Reclaim();
            }

            if (max > 0)
            {
                mDeletedComponents.Clear();
            }
            else { }
        }

        /// <summary>
        /// 移除组件并清理组件相关的映射数据
        /// </summary>
        /// <param name="target"></param>
        /// <param name="aid"></param>
        private void RemoveComponentAndClear(ref IShipDockComponent target, int aid)
        {
            RemoveNameFromIDMapper(aid);

            int compIndex = mComponents.IndexOf(target);
            List<int> updateList = target.IsSceneUpdate ? mUpdateByScene : mUpdateByTicks;
            updateList.Remove(compIndex);
            mMapper.Remove(target, out int statu);

            mComponents.Remove(target);
        }

        private void RemoveNameFromIDMapper(int id)
        {
            int index = mNameAutoIDMapper.Values.IndexOf(id);

            id = mNameAutoIDMapper.Keys[index];
            mNameAutoIDMapper.Remove(id);

            if (index >= 0)
            {
                mNameAutoIDMapper.Remove(id);
            }
            else { }
        }
        #endregion

        #region 根据当前的执行顺序获取组件
        private void RefComponentByIndex(int value, ref int compIndex, ref List<int> updateList, ref IShipDockComponent comp)
        {
            compIndex = updateList[value];
            comp = compIndex < mComponents.Count ? mComponents[compIndex] : default;
        }

        public void RefComponentByIndex(int value, bool isSceneUpdate, ref IShipDockComponent comp)
        {
            int index = 0;
            List<int> list = isSceneUpdate ? mUpdateByScene : mUpdateByTicks;
            RefComponentByIndex(value, ref index, ref list, ref comp);
        }
        #endregion

        #region 子线程更新模式相关
        /// <summary>
        /// 更新组件的同时检测需要释放的组件（运行于子线程，暂时没用到）
        /// </summary>
        public void UpdateAndFreeComponents(int time, Action<Action<int>> method = default)
        {
            int compIndex = 0;
            int max = mUpdateByTicks.Count;
            for (int i = 0; i < max; i++)
            {
                RefComponentByIndex(i, ref compIndex, ref mUpdateByTicks, ref mSystem);

                if ((mSystem != default) && !mDeletedComponents.Contains(mSystem.ID))
                {
                    if (method == default)
                    {
                        mSystem.UpdateComponent(time);
                        mSystem.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mSystem.UpdateComponent);
                        method.Invoke(mSystem.FreeComponent);
                    }
                }
                else { }
            }

            FinalUpdate(time);

            RemoveSingedComponents();
        }

        /// <summary>
        /// 更新组件（运行于子线程）
        /// </summary>
        public void UpdateComponentUnit(int time, Action<Action<int>> method = default)
        {
            CountTime += time;//与主线程的帧率时间保持一致，避免更新过快

            IShipDockComponent item;
            while (CountTime > FrameTimeInScene)
            {
                PreUpdate?.Invoke(mUpdateByTicks, false);

                int compIndex = 0;
                int max = mUpdateByTicks.Count;
                for (int i = 0; i < max; i++)
                {
                    item = default;

                    RefComponentByIndex(i, ref compIndex, ref mUpdateByTicks, ref item);
                    if (item != default)
                    {
                        if(item.IsSceneUpdate)
                        {
                            continue;
                        }
                        else { }

                        if (item.IsSystemChanged)
                        {
                            if (mDeletedComponents.Contains(item.ID)) { }
                            else
                            {
                                if (method == default)
                                {
                                    item.UpdateComponent(time);
                                }
                                else
                                {
                                    method.Invoke(item.UpdateComponent);
                                }
                            }

                            item.SystemChecked();
                        }
                        else { }
                    }
                    else { }
                }
                FinalUpdate(time);
                CountTime -= FrameTimeInScene;
            }
        }

        /// <summary>
        /// 检测是否有需要释放的组件（运行于子线程）
        /// </summary>
        public void FreeComponentUnit(int time, Action<Action<int>> method = default)
        {
            int compIndex = 0;
            int max = mUpdateByTicks.Count;
            for (int i = 0; i < max; i++)
            {
                RefComponentByIndex(i, ref compIndex, ref mUpdateByTicks, ref mSystem);

                if ((mSystem != default) && !mDeletedComponents.Contains(mSystem.ID))
                {
                    if (method == default)
                    {
                        mSystem.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mSystem.FreeComponent);
                    }
                }
                else { }
            }
        }
        #endregion

        #region 主线程更新模式相关
        /// <summary>
        /// 更新组件的同时检测需要释放的组件（运行于主线程，暂时没用到）
        /// </summary>
        public void UpdateAndFreeComponentsInScene(int time, Action<Action<int>> method = default)
        {
            int compIndex = 0;
            int max = mUpdateByScene.Count;
            for (int i = 0; i < max; i++)
            {
                RefComponentByIndex(i, ref compIndex, ref mUpdateByScene, ref mSystem);

                if ((mSystem != default) && !mDeletedComponents.Contains(mSystem.ID))
                {
                    if (method == default)
                    {
                        mSystem.UpdateComponent(time);
                        mSystem.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mSystem.UpdateComponent);
                        method.Invoke(mSystem.FreeComponent);
                    }
                }
                else { }
            }
            RemoveSingedComponents();
        }

        /// <summary>
        /// 更新组件（主线程）
        /// </summary>
        public void UpdateComponentUnitInScene(int time, Action<Action<int>> method = default)
        {
            int compIndex = 0;
            int max = mUpdateByScene.Count;
            IShipDockComponent item = default;
            for (int i = 0; i < max; i++)
            {
                RefComponentByIndex(i, ref compIndex, ref mUpdateByScene, ref item);

                if ((item != default) && !mDeletedComponents.Contains(item.ID))
                {
                    if (method == default)
                    {
                        item.UpdateComponent(time);
                    }
                    else
                    {
                        method.Invoke(item.UpdateComponent);
                    }
                }
                else { }
            }
        }

        /// <summary>
        /// 检测是否有需要释放的组件（运行于主线程）
        /// </summary>
        public void FreeComponentUnitInScene(int time, Action<Action<int>> method = default)
        {
            int compIndex = 0;
            int max = mUpdateByScene.Count;
            IShipDockComponent item = default;
            for (int i = 0; i < max; i++)
            {
                RefComponentByIndex(i, ref compIndex, ref mUpdateByScene, ref item);

                if ((item != default) && !mDeletedComponents.Contains(item.ID))
                {
                    if (method == default)
                    {
                        item.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(item.FreeComponent);
                    }
                }
                else { }
            }
        }
        #endregion
    }
}