namespace Elimlnate
{
    /// <summary>
    /// 特效信息对象接口，
    /// 用于一定程度解决特效执行时使用匿名函数带来的性能开销和闭包问题
    /// </summary>
    /// <typeparam name="G">消除格类型</typeparam>
    /// <typeparam name="P">消除格特效参数类型</typeparam>
    public interface IEffectInfo<G, P>
    {
        bool ApplyUpdate { get; set; }
        bool IsPlaying { get; set; }
        G GetInfoTarget();
        void SetInfoTarget(G target);
        void Update(int time);
        void Start(ref G param);
        void Stop(ref G param);
        P GetParam();
        void Clean();
    }
}
