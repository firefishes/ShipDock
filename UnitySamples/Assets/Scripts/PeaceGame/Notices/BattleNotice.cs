using ShipDock.Pooling;

namespace Peace
{
    public class BattleNotice : PeaceNotice
    {
        public override void ToPool()
        {
            base.ToPool();

            Pooling<BattleNotice>.To(this);
        }
    }
}