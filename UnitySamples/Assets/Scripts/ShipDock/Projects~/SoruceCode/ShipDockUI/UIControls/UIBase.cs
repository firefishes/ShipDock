using ShipDock.Notices;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.UIControls
{
    /// <summary>
    /// 
    /// UI控件基类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class UIBase
    {

        /// <summary>需要初始化的子控件</summary>
        private int mChildrenWillInit;
        /// <summary>控件子元素名称</summary>
        private List<string> mNameReferens;
        /// <summary>控件子元素物体引用</summary>
        private List<GameObject> mUIReferens;
        /// <summary>控件子元素源功能名称</summary>
        private List<string> mNameRaws;
        /// <summary>控件子元素源功能引用</summary>
        private List<IUIRaw> mUIRaws;
        /// <summary>子控件集合</summary>
        private List<UIBase> mChildrenUI;

        /// <summary>控件数据</summary>
        protected UIControlData mData;

        /// <summary>是否已被清除</summary>
        public bool IsClean { get; protected set; }
        /// <summary>是否开启控件的延后执行功能</summary>
        public bool ApplyCallLate { get; set; }
        /// <summary>控件初始化完成后的回调函数</summary>
        public Action OnInited { get; set; }
        /// <summary>控件对应的UI变换组件</summary>
        public RectTransform UITransform { get; protected set; }
        /// <summary>父级控件</summary>
        public UIBase ParentControl { get; private set; }
        /// <summary控件是否已初始化</summary>
        public bool IsInited { get; private set; }

        /// <summary>子控件数量</summary>
        public int ChildrenUICount
        {
            get
            {
                return mChildrenUI != default ? mChildrenUI.Count : 0;
            }
        }

        /// <summary>是否有子控件</summary>
        public bool ChildrenNeedInit
        {
            get
            {
                return mChildrenWillInit > 0;
            }
        }

        /// <summary>控件数据</summary>
        public virtual UIControlData Data
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;

                UIValid();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UIBase()
        {
            ApplyCallLate = true;

            mNameReferens = new List<string>();
            mUIReferens = new List<GameObject>();

            mNameRaws = new List<string>();
            mUIRaws = new List<IUIRaw>();

            mChildrenUI = new List<UIBase>();

            Action<UIBase> clean = (param) => { };
            Action<UIBase> redirect = (param) => { };
            Action<UIBase> childInited = (param) => { };
            Action<UIBase> childAdded = (param) => { };
            Action<UIBase> childRemoved = (param) => { };

            AddUIRaw(UIControlNameRaws.RAW_DURING_CLEAN, clean);//新增自动回收的源功能
            AddUIRaw(UIControlNameRaws.RAW_REDIRECT_CONTROL, redirect);//新增控件主体重定向的源功能
            AddUIRaw(UIControlNameRaws.RAW_CHILDREN_INITED, childInited);//新增子控件初始化完成的源功能
            AddUIRaw(UIControlNameRaws.RAW_CHILDREN_ADDED, childAdded);//新增子控件添加的源功能
            AddUIRaw(UIControlNameRaws.RAW_CHILDREN_REMOVED, childRemoved);//新增子控件移除的源功能

            //子类须在构造函数中主动调用以下代码执行各自的初始化流程
            //Init();
        }

        /// <summary>
        /// 清除控件
        /// </summary>
        public virtual void Clean()
        {
            if (IsClean)
            {
                return;
            }
            else { }

            IsClean = true;
            IsInited = false;

            Purge();

            CallRaw(this, UIControlNameRaws.RAW_DURING_CLEAN);//调用自动回收的方法

            mNameReferens?.Clear();
            mUIReferens?.Clear();
            mNameRaws?.Clear();
            mUIRaws?.Clear();
            mChildrenUI?.Clear();

            OnInited = default;
            UITransform = default;
            ParentControl = default;
        }

        /// <summary>
        /// 控件子类的回收
        /// </summary>
        protected abstract void Purge();

        /// <summary>
        /// 初始化控件，须由各子类在构造函数中主动调用
        /// </summary>
        protected virtual void Init()
        {
            InitUI();
            InitEvents();

            InsertUIRaw<UIBase>(UIControlNameRaws.RAW_REDIRECT_CONTROL, OnRedirect);
            InsertUIRaw<UIBase>(UIControlNameRaws.RAW_CHILDREN_ADDED, OnChildAdded);
            InsertUIRaw<UIBase>(UIControlNameRaws.RAW_CHILDREN_REMOVED, OnChildRemoved);

            if (!ChildrenNeedInit)
            {
                InsertUIRaw<UIBase>(UIControlNameRaws.RAW_CHILDREN_INITED, OnControlInited);
            }
            else { }

            UIValid();
        }

        /// <summary>
        /// 子控件加入时触发
        /// </summary>
        /// <param name="child"></param>
        private void OnChildRemoved(UIBase child)
        {
            if (mChildrenUI.Contains(child))
            {
                mChildrenUI.Remove(child);

                if (mChildrenWillInit > 0)
                {
                    mChildrenWillInit--;
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 子控件被移除时触发
        /// </summary>
        /// <param name="child"></param>
        private void OnChildAdded(UIBase child)
        {
            if (!mChildrenUI.Contains(child))
            {
                mChildrenWillInit++;
                mChildrenUI.Add(child);
            }
            else { }
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        protected abstract void InitUI();

        /// <summary>
        /// 初始化事件
        /// </summary>
        protected abstract void InitEvents();

        /// <summary>
        /// 添加自动回收时需要调用的方法
        /// </summary>
        /// <param name="method"></param>
        protected void AddClean<T>(Action<T> method)
        {
            InsertUIRaw(UIControlNameRaws.RAW_DURING_CLEAN, method);
        }

        /// <summary>
        /// 移除自动回收时需要调用的方法
        /// </summary>
        /// <param name="method"></param>
        protected void RemoveClean<T>(Action<T> method)
        {
            RemoveUIRaw(UIControlNameRaws.RAW_DURING_CLEAN, method);
        }

        /// <summary>
        /// 添加控件子元素引用
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        protected void AddReferenceUI(string name, GameObject target)
        {
            if (target != default)
            {
                mNameReferens.Add(name);
                mUIReferens.Add(target);
            }
            else { }
        }

        /// <summary>
        /// 移除控件子元素引用
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        protected void RemoveReferenceUI(string name, GameObject target)
        {
            if (target != default)
            {
                mNameReferens.Remove(name);
                mUIReferens.Remove(target);
            }
            else { }
        }

        /// <summary>
        /// 添加控件源功能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="method"></param>
        protected void AddUIRaw<T>(string name, Action<T> method)
        {
            if (method != default)
            {
                int index = mNameRaws.IndexOf(name);
                UIRaw<Action<T>> item;
                if (index == -1)
                {
                    item = new UIRaw<Action<T>>(method);

                    mNameRaws.Add(name);
                    mUIRaws.Add(item);
                }
                else
                {
                    item = mUIRaws[index] as UIRaw<Action<T>>;
                    item.raw = default;

                    Action<T> m = (p) => { };
                    m += method;
                    item.raw = m;

                    mNameRaws[index] = name;
                }
            }
            else { }
        }

        /// <summary>
        /// 向指定的源功能添加委托方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <param name="isOverride"></param>
        public void InsertUIRaw<T>(string name, Action<T> method, bool isOverride = false)
        {
            if (method != default)
            {
                int index = mNameRaws.IndexOf(name);
                if (index >= 0)
                {
                    UIRaw<Action<T>> item = mUIRaws[index] as UIRaw<Action<T>>;
                    if (isOverride)
                    {
                        item.raw = method;
                    }
                    else
                    {
                        item.raw += method;
                    }
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 从指定的源功能移除委托方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="method"></param>
        public void RemoveUIRaw<T>(string name, Action<T> method)
        {
            int index = mNameRaws.IndexOf(name);
            if (index >= 0)
            {
                UIRaw<Action<T>> item = mUIRaws[index] as UIRaw<Action<T>>;
                item.raw -= method;
            }
            else { }
        }

        /// <summary>
        /// 设置子元素显示与否
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public virtual void SetVisible(bool value, string name = "")
        {
            int index = mNameReferens.IndexOf(name);
            if (index >= 0)
            {
                mUIReferens[index]?.SetActive(value);
            }
            else { }
        }

        /// <summary>
        /// 调用控件源功能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="name"></param>
        protected void CallRaw<T>(T param, string name)
        {
            Action<T> method = GetUIRaw<T>(name);
            method?.Invoke(param);

            LogCallRaw(method, ref name);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogCallRaw(object param, ref string name)
        {
            "log:Call raw {0}".Log(name);
            "log:Call raw, method is {0}".Log(param != default, param.ToString());
        }

        /// <summary>
        /// 获取控件的源功能引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Action<T> GetUIRaw<T>(string name)
        {
            Action<T> result = default;
            int index = mNameRaws.IndexOf(name);
            if (index >= 0)
            {
                IUIRaw item = mUIRaws[index];
                if (item is UIRaw<Action<T>> target)
                {
                    if (target != default)
                    {
                        result = target.raw;
                    }
                    else { }
                }
                else { }
            }
            else { }

            return result;
        }

        /// <summary>
        /// 标记控件的变更为生效状态
        /// </summary>
        public virtual void UIValid()
        {
            if (ApplyCallLate)
            {
                UpdaterNotice.SceneCallLater(OnNextFrameUpdate);
            }
            else
            {
                OnNextFrameUpdate(0);
            }
        }

        /// <summary>
        /// 更新帧需要执行的逻辑
        /// </summary>
        /// <param name="time"></param>
        private void OnNextFrameUpdate(int time)
        {
            DataChanged();
            PropertiesChanged();
            AfterUIValid();

            if (!IsInited && !ChildrenNeedInit)
            {
                IsInited = true;
                CallRaw(this, UIControlNameRaws.RAW_CHILDREN_INITED);
            }
            else { }
        }

        /// <summary>
        /// 数据发生变化
        /// </summary>
        protected virtual void DataChanged() { }

        /// <summary>
        /// 控件属性发生变更
        /// </summary>
        protected abstract void PropertiesChanged();

        /// <summary>
        /// 处理控件更新后的流程
        /// </summary>
        protected virtual void AfterUIValid() { }

        /// <summary>
        /// 重定向控件
        /// </summary>
        protected void RedirectControl()
        {
            CallRaw(this, UIControlNameRaws.RAW_REDIRECT_CONTROL);//调用重定向的源方法
        }

        /// <summary>
        /// 子类覆盖此方法实现子类控件的重定向功能
        /// </summary>
        protected virtual void OnRedirect(UIBase control)
        {
            UITransform = default;
        }

        /// <summary>
        /// 控件初始化完成
        /// </summary>
        /// <param name="control"></param>
        public virtual void OnControlInited(UIBase control)
        {
            RemoveUIRaw<UIBase>(UIControlNameRaws.RAW_CHILDREN_INITED, OnControlInited);

            if (!ChildrenNeedInit)
            {
                OnInited?.Invoke();
            }
            else { }
        }

        /// <summary>
        /// 自身子控件初始化完成
        /// </summary>
        /// <param name="child"></param>
        private void ChildInited(UIBase child)
        {
            if (ChildrenNeedInit)
            {
                mChildrenWillInit--;
                mChildrenWillInit = Mathf.Max(0, mChildrenWillInit);

                child.RemoveUIRaw<UIBase>(UIControlNameRaws.RAW_CHILDREN_INITED, ChildInited);

                if (!IsInited && !ChildrenNeedInit)
                {
                    IsInited = true;
                    OnAllChildrenInited();
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 所有子控件都完成了初始化流程
        /// </summary>
        protected virtual void OnAllChildrenInited()
        {
            OnInited?.Invoke();
        }

        /// <summary>
        /// 绑定子控件
        /// </summary>
        /// <param name="child"></param>
        protected void BindChildControl(UIBase child)
        {
            if ((child != default) && !mChildrenUI.Contains(child))
            {
                child.BindToControlAsChild(this);
            }
            else { }
        }

        /// <summary>
        /// 作为子控件绑定到其他子控件
        /// </summary>
        /// <param name="parent"></param>
        public void BindToControlAsChild(UIBase parent)
        {
            if (ParentControl != default)
            {
                if (parent != ParentControl)
                {
                    ParentControl.CallRaw(this, UIControlNameRaws.RAW_CHILDREN_REMOVED);
                    RemoveUIRaw<UIBase>(UIControlNameRaws.RAW_CHILDREN_INITED, ParentControl.ChildInited);
                }
                else
                {
                    return;
                }
            }
            else { }

            ParentControl = parent;

            ParentControl.CallRaw(this, UIControlNameRaws.RAW_CHILDREN_ADDED);
            InsertUIRaw<UIBase>(UIControlNameRaws.RAW_CHILDREN_INITED, ParentControl.ChildInited);
        }
    }
}