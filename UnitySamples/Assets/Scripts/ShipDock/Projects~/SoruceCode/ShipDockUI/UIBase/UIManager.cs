using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.UI
{
    /// <summary>
    /// 
    /// UI管理器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class UIManager
    {
        public static string resourcesUIRelativePath = "ui/";

        /// <summary>当前UI栈</summary>
        private IUIStack mCurrent;
        /// <summary>上一个UI栈</summary>
        private IUIStack mPrevious;
        /// <summary>UI栈缓存</summary>
        private UICacher mUICacher;
        /// <summary>弹出式 UI 集合</summary>
        private List<IUIStack> mPopups;
        /// <summary>自Resources目录加载方式打开的 UI 集合</summary>
        private Dictionary<string, ResourcesUI> mResourceUIMapper;

        /// <summary>UI 根节点对象</summary>
        public IUIRoot UIRoot { get; private set; }
        /// <summary>加载 UI 资源包时显示加载提示的回调</summary>
        public Action<bool> OnLoadingShower { get; private set; }
        /// <summary>UI 栈发生变化时的回调</summary>
        public Action<UIManager, IUIStack> OnStackableChanged { get; set; }
        /// <summary>UI 栈为空时的回调</summary>
        public Action<UIManager, IUIStack> OnNonstackChanged { get; set; }

        /// <summary>
        /// 所有弹出式 UI 的数量
        /// </summary>
        public int PopupCount
        {
            get
            {
                return mPopups.Count;
            }
        }

        public UIManager()
        {
            mResourceUIMapper = new Dictionary<string, ResourcesUI>();
            mUICacher = new UICacher();
            mUICacher.Init();

            mPopups = new List<IUIStack>();
        }

        public void Dispose()
        {
            mUICacher.Clear();
            mResourceUIMapper.Clear();
            mPopups?.Clear();

            mCurrent = default;
            mPrevious = default;
            OnLoadingShower = default;
        }

        /// <summary>
        /// 设置加载 UI 资源包时显示加载提示的回调
        /// </summary>
        /// <param name="method"></param>
        public void SetLoadingShower(Action<bool> method)
        {
            OnLoadingShower = method;
        }

        /// <summary>
        /// 设置 UI 根节点对象
        /// </summary>
        /// <param name="root"></param>
        public void SetRoot(IUIRoot root)
        {
            UIRoot = root;
        }

        /// <summary>
        /// 获取指定的 UI 栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stackName"></param>
        /// <returns></returns>
        public T GetUI<T>(string stackName) where T : IUIStack
        {
            return mUICacher.GetUICache<T>(stackName);
        }

        /// <summary>
        /// 打开 Resouces 目录下的 UI
        /// </summary>
        /// <typeparam name="T">UI脚本类</typeparam>
        /// <param name="resName">资源名</param>
        /// <param name="isUnique">是否为唯一的 UI 对象</param>
        /// <param name="isShow">是否显示</param>
        /// <param name="activeSelfControlShow">是否以物体激活属性控制 UI 的显示</param>
        /// <returns></returns>
        public T OpenResourceUI<T>(string resName, bool isUnique = true, bool isShow = true, bool activeSelfControlShow = true) where T : Component
        {
            T result = default;
            GameObject res;
            if (isUnique && mResourceUIMapper.TryGetValue(resName, out ResourcesUI resourcesUIItem))
            {
                if (resourcesUIItem.StackBinded != default)
                {
                    string name = resourcesUIItem.StackBinded.Name;
                    Open<UIStack>(name);//若已绑定过 UI栈则通过 UI 栈做开启操作
                }
                else
                {
                    res = resourcesUIItem.ui;
                    if (res != default)
                    {
                        RefUIComponentAndCheckVisible(ref res, ref result, isShow, activeSelfControlShow);
                    }
                    else
                    {
                        mResourceUIMapper.Remove(resName);//排除已被外部关闭并销毁的对象，然后重新打开一个
                        result = OpenResourceUI<T>(resName, isUnique);
                    }
                }
            }
            else
            {
                res = Resources.Load<GameObject>(resourcesUIRelativePath.Append(resName));
                if (res != default)
                {
                    resourcesUIItem = new ResourcesUI(mResourceUIMapper)
                    {
                        isUnique = isUnique,
                    };

                    string key = isUnique ? resName : res.name;
                    mResourceUIMapper[key] = resourcesUIItem;

                    res = UnityEngine.Object.Instantiate(res, UIRoot.MainCanvas.transform);
                    RefUIComponentAndCheckVisible(ref res, ref result, isShow, activeSelfControlShow);

                    resourcesUIItem.ui = res;
                }
                else
                {
                    throw new Exception("Open resouces UI error, res path name is ".Append(resName));
                }
            }
            return result;
        }

        /// <summary>
        /// 从打开自 Resources 目录下的 UI 获取组件并检测其是否可见
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res">UI</param>
        /// <param name="result"></param>
        /// <param name="isShow"></param>
        /// <param name="activeSelfControlShow"></param>
        private void RefUIComponentAndCheckVisible<T>(ref GameObject res, ref T result, bool isShow, bool activeSelfControlShow) where T : Component
        {
            if (activeSelfControlShow)
            {
                if (isShow && !res.gameObject.activeSelf)
                {
                    res.SetActive(true);
                }
                else { }
            }
            else
            {
                if (isShow)
                {
                    res.transform.localScale = Vector3.one;
                }
                else { }
            }

            result = res.GetComponent<T>();
        }

        /// <summary>
        /// 关闭加载自 Resouces 目录下的 UI
        /// </summary>
        /// <param name="resName">资源名</param>
        /// <param name="willDestroy">关闭时是否销毁</param>
        /// <param name="activeSelfControlHide">是否以物体激活属性控制 UI 的隐藏</param>
        public void CloseResourceUI(string resName, bool willDestroy = true, bool activeSelfControlHide = true)
        {
            GameObject res;
            if (mResourceUIMapper.TryGetValue(resName, out ResourcesUI resouceUIItem))
            {
                if (resouceUIItem.StackBinded != default)
                {
                    string name = resouceUIItem.StackBinded.Name;
                    Close(name, willDestroy);//若已绑定过 UI栈则通过 UI 栈做 关闭/销毁 操作
                }
                else
                {
                    res = resouceUIItem.ui;

                    if (willDestroy)
                    {
                        mResourceUIMapper.Remove(resName);
                        if (res != default)
                        {
                            UnityEngine.Object.Destroy(res);
                        }
                        else { }
                    }
                    else
                    {
                        if (activeSelfControlHide)
                        {
                            res.SetActive(false);
                        }
                        else
                        {
                            res.transform.localScale = Vector3.zero;
                        }
                    }
                }
            }
            else { }
        }

        /// <summary>
        /// 将加载自 Resouces 目录下的 UI 与UI栈绑定，便于额外附加UI栈后统一开启、关闭和销毁
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="res"></param>
        /// <param name="activeSelfControlHide"></param>
        public void BindResourcesUIToStack(IUIStack stack, GameObject res, bool activeSelfControlHide = true)
        {
            var enumer = mResourceUIMapper.GetEnumerator();
            int max = mResourceUIMapper.Count;
            ResourcesUI resourceUI;
            KeyValuePair<string, ResourcesUI> item;
            for (int i = 0; i < max; i++)
            {
                enumer.MoveNext();
                item = enumer.Current;
                resourceUI = item.Value;

                if (resourceUI.isUnique)
                {
                    int id = resourceUI.ui.GetInstanceID();
                    if (id == res.GetInstanceID())
                    {
                        resourceUI.BindToUIStack(stack);//与UI栈绑定
                        break;
                    }
                    else { }
                }
                else { }
            }
            enumer.Dispose();
        }

        /// <summary>
        /// 从资源包中加载 UI，并加入 UI 栈进行管理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stackName"></param>
        /// <param name="creater"></param>
        /// <returns></returns>
        public T Open<T>(string stackName, Func<object> creater = default) where T : IUIStack, new()
        {
            T result = mUICacher.CreateOrGetUICache<T>(stackName, creater);

            if(!result.IsExited)
            {
                if(result.IsStackable)
                {
                    if ((mPrevious != default) && (mPrevious.Name != result.Name))
                    {
                        mPrevious.Interrupt();
                    }
                    else { }

                    mPrevious = mCurrent;
                    mCurrent = mUICacher.StackCurrent;

                    if (mCurrent.IsStackAdvanced)
                    {
                        UIStackCurrentRenew();
                    }
                    else
                    {
                        UIStackCurrentEnter(mCurrent);
                    }
                }
                else
                {
                    if (!mPopups.Contains(result))
                    {
                        mPopups.Add(result);
                    }
                    else { }

                    UIStackCurrentEnter(result);//非栈管理方式的界面，直接开启
                    "todo".Log("非栈管理方式的界面需要做层级管理");
                }
            }
            return result;
        }

        /// <summary>
        /// 关闭由 UI栈管理的 UI
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isDestroy"></param>
        public void Close(string name, bool isDestroy = false)
        {
            IUIStack result = mUICacher.RemoveAndCheckUICached(name, out bool isCurrentStack, out IUIStack removed, isDestroy);
            if (isCurrentStack)
            {
                mPrevious = mCurrent;
                mCurrent = mUICacher.StackCurrent;

                if (mCurrent != default && !mCurrent.IsExited && result != mCurrent)
                {
                    mCurrent?.Renew();
                }
                else { }
            }
            else
            {
                //非栈方式管理界面的额外处理
                if (mPopups.Contains(result))
                {
                    mPopups.Remove(result);
                }
                else { }
            }

            UIStackExit(ref result, isDestroy);
        }

        private void UIStackExit(ref IUIStack result, bool isDestroy)
        {
            "error".Log(result == default, "UI is null when exit.");
            if (result != default)
            {
                result.Exit(isDestroy);//退出界面
            }
            else { }
        }

        private void UIStackCurrentEnter(IUIStack stack)
        {
            "log".Log("UI open ".Append(stack.Name));
            stack.Enter();//界面栈没被提前，说明此界面刚刚打开，已位于栈顶
        }

        private void UIStackCurrentRenew()
        {
            "log".Log("UI renew ".Append(mCurrent.Name));
            mCurrent.Renew();//界面栈被提前后重新唤醒
        }
    }
}