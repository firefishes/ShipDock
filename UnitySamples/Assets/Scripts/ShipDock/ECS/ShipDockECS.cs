using ShipDock.Tools;

namespace ShipDock.ECS
{
    public class ShipDockECS : Singletons<ShipDockECS>
    {
        public ILogicContext Context
        {
            get
            {
                return mContexts.CurrentContext;
            }
        }

        private ECSContext mContexts;

        public ShipDockECS()
        {
            mContexts = Framework.Instance.GetUnit<ECSContext>(Framework.UNIT_ECS);
        }
    }
}
