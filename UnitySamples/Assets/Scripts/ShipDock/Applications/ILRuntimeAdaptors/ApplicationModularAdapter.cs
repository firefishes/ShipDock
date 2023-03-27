using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;

namespace ShipDock.Applications
{
    public class ApplicationModularAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Int32[]> mget_ModularNoticeCreate_0 = new CrossBindingFunctionInfo<System.Int32[]>("get_ModularNoticeCreate");
        static CrossBindingFunctionInfo<System.Int32[]> mget_ModularNoticeDecorater_1 = new CrossBindingFunctionInfo<System.Int32[]>("get_ModularNoticeDecorater");
        static CrossBindingFunctionInfo<System.Int32[]> mget_ModularNoticeListener_2 = new CrossBindingFunctionInfo<System.Int32[]>("get_ModularNoticeListener");
        static CrossBindingFunctionInfo<System.Int32> mget_ModularName_3 = new CrossBindingFunctionInfo<System.Int32>("get_ModularName");
        static CrossBindingMethodInfo<System.Int32> mset_ModularName_4 = new CrossBindingMethodInfo<System.Int32>("set_ModularName");
        static CrossBindingFunctionInfo<ShipDock.IAppModulars> mget_Modulars_5 = new CrossBindingFunctionInfo<ShipDock.IAppModulars>("get_Modulars");
        static CrossBindingMethodInfo<ShipDock.IAppModulars> mset_Modulars_6 = new CrossBindingMethodInfo<ShipDock.IAppModulars>("set_Modulars");
        static CrossBindingMethodInfo mDispose_7 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingMethodInfo mInitModular_8 = new CrossBindingMethodInfo("InitModular");
        static CrossBindingMethodInfo mPurge_9 = new CrossBindingMethodInfo("Purge");
        static CrossBindingMethodInfo<ShipDock.INoticeBase<System.Int32>> mNoticesHandler_10 = new CrossBindingMethodInfo<ShipDock.INoticeBase<System.Int32>>("NoticesHandler");
        static CrossBindingFunctionInfo<System.Int32, ShipDock.INoticeBase<System.Int32>> mNoticeCreater_11 = new CrossBindingFunctionInfo<System.Int32, ShipDock.INoticeBase<System.Int32>>("NoticeCreater");
        static CrossBindingMethodInfo<System.Int32, ShipDock.INoticeBase<System.Int32>> mNoticeDecorator_12 = new CrossBindingMethodInfo<System.Int32, ShipDock.INoticeBase<System.Int32>>("NoticeDecorator");
        static CrossBindingFunctionInfo<System.Int32, ShipDock.INoticeBase<System.Int32>, ShipDock.INoticeBase<System.Int32>> mNotifyModular_13 = new CrossBindingFunctionInfo<System.Int32, ShipDock.INoticeBase<System.Int32>, ShipDock.INoticeBase<System.Int32>>("NotifyModular");
        static CrossBindingMethodInfo<ShipDock.IAppModulars> mSetModularManager_14 = new CrossBindingMethodInfo<ShipDock.IAppModulars>("SetModularManager");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ShipDock.ApplicationModular);
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

        public class Adapter : ShipDock.ApplicationModular, CrossBindingAdaptorType
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

            public override void Dispose()
            {
                if (mDispose_7.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_7.Invoke(this.instance);
            }

            public override void InitModular()
            {
                if (mInitModular_8.CheckShouldInvokeBase(this.instance))
                    base.InitModular();
                else
                    mInitModular_8.Invoke(this.instance);
            }

            public override void Purge()
            {
                mPurge_9.Invoke(this.instance);
            }

            public override ShipDock.INoticeBase<System.Int32> NotifyModular(System.Int32 name, ShipDock.INoticeBase<System.Int32> param)
            {
                if (mNotifyModular_13.CheckShouldInvokeBase(this.instance))
                    return base.NotifyModular(name, param);
                else
                    return mNotifyModular_13.Invoke(this.instance, name, param);
            }

            public override void SetModularManager(ShipDock.IAppModulars modulars)
            {
                if (mSetModularManager_14.CheckShouldInvokeBase(this.instance))
                    base.SetModularManager(modulars);
                else
                    mSetModularManager_14.Invoke(this.instance, modulars);
            }

            public override System.Int32 ModularName
            {
                get
                {
                    return mget_ModularName_3.Invoke(this.instance);

                }
                protected set
                {
                    mset_ModularName_4.Invoke(this.instance, value);

                }
            }

            protected override ShipDock.IAppModulars Modulars
            {
                get
                {
                    if (mget_Modulars_5.CheckShouldInvokeBase(this.instance))
                        return base.Modulars;
                    else
                        return mget_Modulars_5.Invoke(this.instance);

                }
                set
                {
                    if (mset_Modulars_6.CheckShouldInvokeBase(this.instance))
                        base.Modulars = value;
                    else
                        mset_Modulars_6.Invoke(this.instance, value);

                }
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

