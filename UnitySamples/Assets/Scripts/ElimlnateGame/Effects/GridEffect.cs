using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using IGridEffectInfo = Elimlnate.IEffectInfo<Elimlnate.ElimlnateGrid, Elimlnate.GridEffectParam>;

namespace Elimlnate
{
    /// <summary>
    /// 作用在单个消除格上的特效，由继承自 ElimlnateEffect 的子类对象管理
    /// </summary>
    public abstract class GridEffect : IDispose
    {
        /// <summary>消除格与特效参数信息组成的数据映射</summary>
        private KeyValueList<ElimlnateGrid, IGridEffectInfo> mMapper;

        public Action OnPlaySound { get; set; }
        /// <summary>前置特效</summary>
        public BatchEffect UpperStrata { get; set; }
        /// <summary>已启动的特效数量</summary>
        public int EffectCount { get; protected set; }

        public GridEffect()
        {
            mMapper = new KeyValueList<ElimlnateGrid, IGridEffectInfo>();
        }

        public void Dispose()
        {
            Clear();
            Purge();

            UpperStrata = default;

            Utils.Reclaim(ref mMapper);
            mMapper = default;
        }

        public virtual void Clear()
        {
            List<IGridEffectInfo> list = mMapper.Values;
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                list[i].GetParam().Curve = default;
                list[i].Clean();
            }
            mMapper.Clear();
            mMapper.TrimExcess();
        }

        protected virtual void Purge()
        {
        }

        /// <summary>从消除格上移除此特效</summary>
        public void Remove(ElimlnateGrid target)
        {
            if ((mMapper != default) && mMapper.ContainsKey(target))
            {
                IGridEffectInfo effect = mMapper.Remove(target);
                effect?.Clean();
            }
            else { }
        }

        /// <summary>
        /// 在消除格上启用此特效
        /// </summary>
        /// <param name="target">消除格</param>
        /// <param name="play">是否播放特效（false表示仅准备数据映射，不播放特效）</param>
        public void Commit(ElimlnateGrid target, bool play = true)
        {
            if(mMapper != default)
            {
                IGridEffectInfo info;
                if (mMapper.ContainsKey(target))
                {
                    info = mMapper[target];
                }
                else
                {
                    info = Create(ref target);
                    mMapper[target] = info;
                }

                if (play)
                {
                    info.Start(ref target);
                }
                else { }
            }
            else { }
        }

        /// <summary>
        /// 获取对应消除格的特效参数信息
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public GridEffectParam GetEffectParam(ElimlnateGrid target)
        {
            return mMapper != default && mMapper.ContainsKey(target) ? mMapper[target].GetParam() : default;
        }

        /// <summary>
        /// 创建一个特效参数信息对象
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected abstract IGridEffectInfo Create(ref ElimlnateGrid target);

        /// <summary>
        /// 在消除格上停止特效
        /// </summary>
        /// <param name="target"></param>
        public void Stop(ElimlnateGrid target)
        {
            if ((mMapper != default) && mMapper.ContainsKey(target))
            {
                IGridEffectInfo info = mMapper[target];
                info.Stop(ref target);
                AfterStop(target);
            }
            else { }
        }

        protected virtual void AfterStop(ElimlnateGrid target) { }
    }
}
