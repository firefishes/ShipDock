using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;

namespace ShipDock.Applications
{
    public class HotFixerInteractorAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mRelease_0 = new CrossBindingMethodInfo("Release");
        static CrossBindingMethodInfo<ShipDock.HotFixerUI, ShipDock.HotFixerUIAgent> mInitInteractor_1 = new CrossBindingMethodInfo<ShipDock.HotFixerUI, ShipDock.HotFixerUIAgent>("InitInteractor");
        static CrossBindingMethodInfo<System.Int32, ShipDock.INoticeBase<System.Int32>> mDispatch_2 = new CrossBindingMethodInfo<System.Int32, ShipDock.INoticeBase<System.Int32>>("Dispatch");
        static CrossBindingMethodInfo mUpdateInteractor_3 = new CrossBindingMethodInfo("UpdateInteractor");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ShipDock.HotFixerInteractor);
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

        public class Adapter : ShipDock.HotFixerInteractor, CrossBindingAdaptorType
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

            public override void Release()
            {
                if (mRelease_0.CheckShouldInvokeBase(this.instance))
                    base.Release();
                else
                    mRelease_0.Invoke(this.instance);
            }

            public override void InitInteractor(ShipDock.HotFixerUI UIOwner, ShipDock.HotFixerUIAgent agent)
            {
                if (mInitInteractor_1.CheckShouldInvokeBase(this.instance))
                    base.InitInteractor(UIOwner, agent);
                else
                    mInitInteractor_1.Invoke(this.instance, UIOwner, agent);
            }

            public override void Dispatch(System.Int32 name, ShipDock.INoticeBase<System.Int32> param)
            {
                if (mDispatch_2.CheckShouldInvokeBase(this.instance))
                    base.Dispatch(name, param);
                else
                    mDispatch_2.Invoke(this.instance, name, param);
            }

            public override void UpdateInteractor()
            {
                mUpdateInteractor_3.Invoke(this.instance);
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

