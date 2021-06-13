using DG.Tweening;
using ShipDock.Commons;
using ShipDock.Notices;
using System;
using IGridEffectInfo = Elimlnate.IEffectInfo<Elimlnate.ElimlnateGrid, Elimlnate.GridEffectParam>;

namespace Elimlnate
{
    /// <summary>
    /// 基于DOTween实现的特效参数信息基类
    /// </summary>
    /// <typeparam name="P"></typeparam>
    public class TweenEffectBase<P> : IGridEffectInfo where P : GridEffectParam, new()
    {
        public bool IsPlaying { get; set; }
        public bool ApplyUpdate { get; set; }
        public Tween[] TweenRef { get; set; }
        public P Param { get; private set; }
        public Action<ElimlnateGrid, TweenEffectBase<P>, P> EffectMethod { get; set; }

        public ElimlnateGrid Grid { get; private set; }
        private MethodUpdater mUpdater;

        public TweenEffectBase()
        {
            Param = new P();
        }

        public void ResetTweenRefs()
        {
            if (TweenRef != default)
            {
                int max = TweenRef.Length;
                for (int i = 0; i < max; i++)
                {
                    TweenRef[i]?.Kill();
                }
            }
            else { }
            TweenRef = default;
        }

        public void Clean()
        {
            ElimlnateGrid grid = Grid;
            Stop(ref grid);

            Param?.Clean();
            ResetTweenRefs();

            Param = default;
            Grid = default;
            EffectMethod = default;
        }

        public GridEffectParam GetParam()
        {
            return Param;
        }

        public void Start(ref ElimlnateGrid param)
        {
            Grid = param;
            if (ApplyUpdate)
            {
                if (mUpdater == default)
                {
                    mUpdater = new MethodUpdater()
                    {
                        Update = Update
                    };
                    UpdaterNotice.AddSceneUpdater(mUpdater);
                }
                else { }
            }
            else
            {
                EffectMethod?.Invoke(Grid, this, Param);
            }
        }

        public void Stop(ref ElimlnateGrid param)
        {
            if (ApplyUpdate)
            {
                if (mUpdater != default)
                {
                    UpdaterNotice.RemoveSceneUpdater(mUpdater);
                    mUpdater.Dispose();
                    mUpdater = default;
                }
                else { }
            }
            else
            {
                ResetTweenRefs();
            }
        }

        public virtual void Update(int time)
        {
            EffectMethod?.Invoke(Grid, this, Param);
        }

        public void SetInfoTarget(ElimlnateGrid target)
        {
            Grid = target;
        }

        public ElimlnateGrid GetInfoTarget()
        {
            return Grid;
        }
    }
}
