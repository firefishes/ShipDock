using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    /// <summary>
    /// ECS 组件
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class ShipDockComponent : IShipDockComponent
    {
        #region TODO 系统特性，需要迁移
        public virtual void SystemChecked()
        {
            IsSystemChanged = true;
        }

        public bool IsSystem { get; protected set; }
        public bool IsSystemChanged { get; set; }
        #endregion
        public int ID { get; private set; } = int.MaxValue;
        public bool IsSceneUpdate { get; private set; }
        #region 帧末更新相关的回调
        public Action<int> OnFinalUpdateForTime { private get; set; }
        public Action<IShipDockEntitas> OnFinalUpdateForEntitas { set; private get; }
        public Action<Action<int, IShipDockEntitas>> OnFinalUpdateForExecute { set; private get; }
        #endregion
        /// <summary>实体规模伸缩回调</summary>
        public Action<IShipDockEntitas, bool> OnEntitasStretch { get; set; }

        private IShipDockEntitas mEntitasItem;
        /// <summary>组件关联的所有实体 ID</summary>
        private List<int> mEntitasIDs;
        /// <summary>组件中需要释放的实体对应的 ID</summary>
        private List<int> mEntitasIDsRelease;
        /// <summary>已废弃的实体 ID 列表</summary>
        private List<int> mEntitasIDsRemoved;
        /// <summary>组件关联的所有实体</summary>
        private IntegerMapper<IShipDockEntitas> mEntitas;

        public ShipDockComponent() { }

        #region 销毁和重置
        public virtual void Reclaim()
        {
            CleanAllEntitas(ref mEntitasIDs);
            CleanAllEntitas(ref mEntitasIDsRelease);

            OnFinalUpdateForTime = default;
            OnFinalUpdateForEntitas = default;
            OnFinalUpdateForExecute = default;
            OnEntitasStretch = default;

            mEntitasItem = default;
            Utils.Reclaim(ref mEntitasIDs);
            Utils.Reclaim(ref mEntitasIDsRelease);
            Utils.Reclaim(mEntitas);
            ID = int.MaxValue;
        }

        /// <summary>
        /// 清除所有实体
        /// </summary>
        /// <param name="list"></param>
        private void CleanAllEntitas(ref List<int> list)
        {
            int id;
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                id = list[i];
                mEntitasItem = GetEntitas(id);
                mEntitasItem.RemoveComponent(this);
            }
        }
        #endregion

        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="context"></param>
        public virtual void Init(IShipDockComponentContext context)
        {
            mEntitasIDs = new List<int>();
            mEntitasIDsRemoved = new List<int>();
            mEntitasIDsRelease = new List<int>();
            mEntitas = new IntegerMapper<IShipDockEntitas>();
        }

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual int SetEntitas(IShipDockEntitas target)
        {
            int aid = mEntitas.Add(target, out int statu);
            if (statu == 0)
            {
                mEntitasIDs.Add(aid);
                OnEntitasStretch?.Invoke(target, false);
            }
            else { }
            return aid;
        }

        /// <summary>
        /// 根据实体 ID 获取组件关联的实体
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public IShipDockEntitas GetEntitas(int aid)
        {
            IShipDockEntitas result = mEntitas.Get(aid, out _);
            return result;
        }

        /// <summary>
        /// 以out参数方式获取组件关联的实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entitas"></param>
        public void GetEntitasRef(int id, out IShipDockEntitas entitas)
        {
            entitas = mEntitas.Get(id, out _);
        }

        /// <summary>
        /// 废弃实体
        /// </summary>
        public virtual int DropEntitas(IShipDockEntitas target, int entitasID)
        {
            if (mEntitasIDsRemoved.Contains(entitasID))
            {
                //ID 参数对应的实体已在废弃列表中
                return 1;
            }
            else
            {
                //调用实体规模伸缩回调
                OnEntitasStretch?.Invoke(target, true);
                //将 ID 参数对应的实体加入废弃列表
                mEntitasIDsRemoved.Add(entitasID);
            }
            return 0;
        }

        /// <summary>
        /// 设置组件名
        /// </summary>
        /// <param name="id"></param>
        public void SetComponentID(int id)
        {
            if (ID == int.MaxValue)
            {
                ID = id;
            }
            else { }
        }

        /// <summary>
        /// 组件的帧更新执行函数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="target"></param>
        public virtual void Execute(int time, ref IShipDockEntitas target) { }

        /// <summary>
        /// 将实体及对应的方法加入帧末执行队列
        /// </summary>
        /// <param name="time"></param>
        /// <param name="entitas"></param>
        /// <param name="method"></param>
        protected void ExecuteInFinal(int time, IShipDockEntitas entitas, Action<int, IShipDockEntitas> method)
        {
            if (ID == int.MaxValue)
            {
                return;
            }
            else { }

            OnFinalUpdateForTime.Invoke(time);
            OnFinalUpdateForEntitas.Invoke(entitas);
            OnFinalUpdateForExecute(method);
        }

        /// <summary>
        /// 组件帧更新方法
        /// </summary>
        /// <param name="time"></param>
        public void UpdateComponent(int time)
        {
            int id;
            int max = (mEntitasIDs != default) ? mEntitasIDs.Count : 0;
            for (int i = 0; i < max; i++)
            {
                if (i >= mEntitasIDs.Count)
                {
                    i = 0;
                    max = mEntitasIDs.Count;
                }
                else { }

                id = mEntitasIDs[i];
                mEntitasItem = GetEntitas(id);
                if (mEntitasItem != default && mEntitasIDsRemoved != default)
                {
                    if (mEntitasItem.WillDestroy || mEntitasIDsRemoved.Contains(id))
                    {
                        if (mEntitasIDsRelease.Contains(id)) { }
                        else
                        {
                            mEntitasIDsRelease.Add(id);
                        }
                    }
                    else
                    {
                        Execute(time, ref mEntitasItem);
                    }
                }
                else
                {
                    if (mEntitasIDsRemoved.Contains(id)) { }
                    else
                    {
                        mEntitasIDsRelease.Add(id);
                    }
                }
            }
            mEntitasItem = default;
            AfterComponentExecuted();
        }

        /// <summary>
        /// 组件更新后的处理
        /// </summary>
        protected virtual void AfterComponentExecuted() { }

        /// <summary>
        /// 检测组件中需要释放的实体
        /// </summary>
        public void FreeComponent(int time)
        {
            int id;
            int max = (mEntitasIDsRelease != default) ? mEntitasIDsRelease.Count : 0;
            for (int i = 0; i < max; i++)
            {
                id = mEntitasIDsRelease[i];
                mEntitasItem = GetEntitas(id);
                if (mEntitasItem != default)
                {
                    FreeEntitas(id, ref mEntitasItem, out int statu);
                }
                else { }

                mEntitasIDsRemoved.Remove(id);
            }
            mEntitasIDsRelease.Clear();
            mEntitasItem = default;
        }

        /// <summary>
        /// 释放实体
        /// </summary>
        protected virtual void FreeEntitas(int mid, ref IShipDockEntitas entitas, out int statu)
        {
            entitas.RemoveComponent(this);//此处在实体无需销毁时可能为重复操作
            mEntitas.Remove(entitas, out statu);
            mEntitasIDs.Remove(mid);
        }

        public void SetSceneUpdate(bool value)
        {
            IsSceneUpdate = value;
        }
    }
}