using System;

namespace ShipDock.UI
{
    public static class UILayerType
    {
        public const int NONE = 0;
        public const int WINDOW = 1;
        public const int POPUPS = 2;
        public const int WIDGET = 3;
    }

    public enum UILayerTypeEnum
    {
        None,
        Window,
        Widget,
        Popup,
    }

    public class UIStack : object, IUIStack
    {

        /// <summary>UI栈是否被标记为退出状态</summary>
        public virtual bool IsExited { get; private set; }
        /// <summary>UI栈是否被标记为栈提前状态</summary>
        public virtual bool IsStackAdvanced { get; private set; }
        /// <summary>是否栈式UI模块</summary>
        public virtual bool IsStackable { get; } = true;
        /// <summary>UI资源名（预制体名）</summary>
        public virtual string UIAssetName { get; protected set; }
        /// <summary>UI名，用于UI管理器识别此UI</summary>
        public virtual string Name { get; protected set; }
        /// <summary>UI栈退出时的回调</summary>
        public Action<bool> OnExit { get; set; }

        public virtual void Init() { }

        public virtual void Enter() { }

        public virtual void Exit(bool isDestroy)
        {
            OnExit?.Invoke(isDestroy);

            if (isDestroy)
            {
                IsExited = true;
                OnExit = default;
            }
            else { }
        }

        public virtual void Interrupt() { }

        public virtual void Renew() { }

        public virtual void ResetAdvance()
        {
            IsStackAdvanced = false;
        }

        public virtual void StackAdvance()
        {
            IsStackAdvanced = true;
        }
    }
}