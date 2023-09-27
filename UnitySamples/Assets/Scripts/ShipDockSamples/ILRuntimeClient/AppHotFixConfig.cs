using ILRuntime.Runtime.Enviorment;

public class AppHotFixConfig : AppHotFixConfigBase
{
    public AppHotFixConfig() : base() { }

    protected override void SetRegisterMethods(AppDomain appdomain)
    {
        base.SetRegisterMethods(appdomain);

        appdomain.DelegateManager.RegisterMethodDelegate<DigitalRubyShared.GestureRecognizer>();        appdomain.DelegateManager.RegisterMethodDelegate<System.String, System.Single, System.Int32>();        appdomain.DelegateManager.RegisterDelegateConvertor<DigitalRubyShared.GestureRecognizerStateUpdatedDelegate>((act) =>
        {
            return new DigitalRubyShared.GestureRecognizerStateUpdatedDelegate((gesture) =>
            {
                ((System.Action<DigitalRubyShared.GestureRecognizer>)act)(gesture);
            });
        });
    }
}