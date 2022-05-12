using ShipDock.Tools;
using System.Collections.Generic;


namespace IsKing
{
    public abstract class AIExectuter
    {
        public abstract void Execute();
    }

    public class AIGeneralIntoBattleRatio : AIExectuter
    {
        public List<BattleHeroController> Heros { get; set; }
        public float Ratio { get; set; }
        public int HeroID { get; set; }
        public BattleHeroController HeroController { get; set; }

        public override void Execute()
        {
            int index = Utils.UnityRangeRandom(0, Heros.Count);

            HeroController = Heros[index];
            HeroID = HeroController.Info.GetIntData(Consts.FN_ID);

            "log:AI chooset player card from {0} heros, selected index is {1}, id is {2}".Log(Heros.Count.ToString(), index.ToString(), HeroID.ToString());
        }
    }

}