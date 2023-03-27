using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;

namespace ShipDock.Applications
{
    public class IDataExtracterAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo<ShipDock.IDataProxy, System.Int32> mOnDataProxyNotify_0 = new CrossBindingMethodInfo<ShipDock.IDataProxy, System.Int32>("OnDataProxyNotify");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ShipDock.IDataExtracter);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : ShipDock.IDataExtracter, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public void OnDataProxyNotify(ShipDock.IDataProxy data, System.Int32 DCName)
            {
                mOnDataProxyNotify_0.Invoke(this.instance, data, DCName);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}

