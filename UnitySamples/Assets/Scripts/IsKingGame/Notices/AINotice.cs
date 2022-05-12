using ShipDock.Notices;
using System.Collections.Generic;

namespace IsKing
{
    public class AINotice : Notice
    {
        public int camp;
    }

    public class AIRatioNotice : AINotice
    {
        public List<BattleHeroController> Heros { get; set; }
    }
}