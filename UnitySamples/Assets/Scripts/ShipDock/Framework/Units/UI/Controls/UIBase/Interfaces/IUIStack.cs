using System;

namespace ShipDock
{
    /// <summary>UI堆栈接口</summary>
    public interface IUIStack
    {
        Action<bool> OnExit { get; set; }
        /// <summary>是否已退出</summary>
        bool IsExited { get; }
        /// <summary>是否已标记为栈提前</summary>
        bool IsStackAdvanced { get; }
        /// <summary>在资源包中的名称</summary>
        string UIAssetName { get; }
        /// <summary>模块名（栈名）</summary>
        string Name { get; }
        /// <summary>是否用栈管理</summary>
        bool IsStackable { get; }
        /// <summary>UI所处的层级</summary>
        int UILayer { get; }

        /// <summary>初始化</summary>
        void Init();
        /// <summary>开启</summary>
        void Enter();
        /// <summary>中断</summary>
        void Interrupt();
        /// <summary>标记为栈提前</summary>
        void StackAdvance();
        /// <summary>重置栈提前标记</summary>
        void ResetAdvance();
        /// <summary>唤醒</summary>
        void Renew();
        /// <summary>退出</summary>
        void Exit(bool isDestroy);
    }
}