using ShipDock.Notices;

namespace IsKing
{
    public class HeroNotice : Notice
    {
        public int id;
        public int camp;
        public BattleHeroController heroController;

        public override void ToPool()
        {
            base.ToPool();

            heroController = default;
        }
    }
}

