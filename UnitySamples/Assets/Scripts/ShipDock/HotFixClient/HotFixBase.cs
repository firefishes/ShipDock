using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// ILRuntime 热更端对象基类
    /// 
    /// 与主工程的 ILRuntime 热更桥接器组件对接，承接诸如热更 UI、创建于热更端的对象等逻辑的关联
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class HotFixBase
    {
        /// <summary>需要热更端对象传递至主工程热更桥接器的方法集合</summary>
        private Dictionary<string, Action> mHotFixMethods;

        /// <summary>接收主工程对象桥接器组件传递至热更端对象的识别数据，用于创建批量热更端对象时的识别操作</summary>
        public virtual int ReadyID { get; set; }
        /// <summary>主工程中热更桥接器组件初始化完成后调用热更端对象的入口方法（可在热更桥接组件中配置变更）</summary>
        public abstract void ShellInited(MonoBehaviour target);

        /// <summary>
        /// 增加可传递至主工程热更桥接器组件的方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        protected void AddHotFixMethod(string name, Action method)
        {
            mHotFixMethods[name] = method;
        }

        /// <summary>
        /// 子类覆盖此方法，统一添加需要传递至主工程热更桥接器组件的方法
        /// </summary>
        protected virtual void InitHotFixedMethods() { }

        /// <summary>
        /// 主工程热更端桥接组件通过方法名获取此对象上的方法
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Action GetUpdateMethods(string name)
        {
            if (mHotFixMethods == default)
            {
                mHotFixMethods = new Dictionary<string, Action>();
                InitHotFixedMethods();
            }
            else { }

            Action result = default;
            switch (name)
            {
                case "FixedUpdate":
                    result = FixedUpdate;
                    break;
                case "Update":
                    result = Update;
                    break;
                case "LateUpdate":
                    result = LateUpdate;
                    break;
                case "OnDestroy":
                    result = OnDestroy;
                    break;
                default:
                    result = mHotFixMethods[name];
                    break;
            }
            return result;
        }

        /// <summary>
        /// 模拟主工程组件的 OnDestroy 周期方法
        /// </summary>
        protected virtual void OnDestroy()
        {
            mHotFixMethods?.Clear();
            mHotFixMethods = default;
        }

        /// <summary>
        /// 模拟主工程组件的 Update 周期方法
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// 模拟主工程组件的 FixedUpdate 周期方法
        /// </summary>
        public abstract void FixedUpdate();

        /// <summary>
        /// 模拟主工程组件的 LateUpdate 周期方法
        /// </summary>
        public abstract void LateUpdate();
    }
}