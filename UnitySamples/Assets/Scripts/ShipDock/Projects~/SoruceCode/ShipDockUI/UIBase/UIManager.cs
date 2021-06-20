using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.UI
{
    /// <summary>
    /// 
    /// UI管理器
    /// 
    /// </summary>
    public class UIManager
    {
        private IUIStack mCurrent;
        private IUIStack mPrevious;
        private UICacher mUICacher;
        private List<IUIStack> mPopups;
        private Dictionary<string, GameObject> mResourceUIMapper;

        public IUIRoot UIRoot { get; private set; }
        public Action<bool> OnLoadingAlert { get; private set; }
        public Action<UIManager, IUIStack> OnStackableChanged { get; set; }
        public Action<UIManager, IUIStack> OnNonstackChanged { get; set; }

        public int PopupCount
        {
            get
            {
                return mPopups.Count;
            }
        }

        public UIManager()
        {
            mResourceUIMapper = new Dictionary<string, GameObject>();
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
            OnLoadingAlert = default;
        }

        public void SetLoadingAlert(Action<bool> method)
        {
            OnLoadingAlert = method;
        }

        public void SetRoot(IUIRoot root)
        {
            UIRoot = root;
        }

        public T GetUI<T>(string stackName) where T : IUIStack
        {
            return mUICacher.GetUICache<T>(stackName);
        }

        public T OpenResourceUI<T>(string resName) where T : Component
        {
            T result;
            if (mResourceUIMapper.TryGetValue(resName, out GameObject raw))
            {
                if (raw != default)
                {
                    result = raw.GetComponent<T>();
                }
                else
                {
                    mResourceUIMapper.Remove(resName);//排除已被外部销毁的对象，然后重新获取
                    result = OpenResourceUI<T>(resName);
                }
            }
            else
            {
                raw = Resources.Load<GameObject>("ui/".Append(resName));
                raw = UnityEngine.Object.Instantiate(raw, UIRoot.MainCanvas.transform);
                result = raw.GetComponent<T>();
                mResourceUIMapper[resName] = raw;
            }
            return result;
        }

        public void CloseResourceUI(string resName)
        {
            if (mResourceUIMapper.TryGetValue(resName, out GameObject raw))
            {
                mResourceUIMapper.Remove(resName);
                if (raw != default)
                {
                    UnityEngine.Object.Destroy(raw);
                }
                else { }
            }
            else
            {
                Debug.LogError(resName);
            }
        }

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
                    UIStackCurrentEnter(result);//非栈管理方式的界面，直接开启
                    "todo".Log("非栈管理方式的界面需要做层级管理");
                }
            }
            return result;
        }

        public void Close(string name, bool isDestroy = false, Action<IUIStack, bool> onHotFixUIExit = default)
        {
            bool isCurrentStack;
            IUIStack result = mUICacher.RemoveAndCheckUICached(name, out isCurrentStack, out IUIStack removed, isDestroy);
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
            else { }//非栈方式管理的界面的额外处理

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