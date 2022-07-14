using ShipDock.Pooling;

namespace ShipDock.Notices
{
    /// <summary>
    /// 
    /// 携带一个参数的消息，参数类型为泛型定义的类型
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParamNotice<T> : Notice, IParamNotice<T>
    {
        protected override void Purge()
        {
            base.Purge();

            ParamValue = default;
        }

        public override void ToPool()
        {
            Pooling<ParamNotice<T>>.To(this);
        }

        public virtual T ParamValue { get; set; }
    }

    /// <summary>
    /// 携带一个参数的消息接口
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParamNotice<T> : INotice
    {
        T ParamValue { get; set; }
    }
}
