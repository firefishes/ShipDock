using ShipDock.Notices;
using ShipDock.Pooling;

namespace IsKing
{
    public class CardNotice : Notice
    {
        public int camp;
        public int cardType;
        public BattleHeroController heroControllerFrom;
        public BattleSkillController skillController;

        public override void ToPool()
        {
            Pooling<CardNotice>.To(this);
        }

        public override void Revert()
        {
            base.Revert();

            camp = Consts.CAMP_NONE;
            cardType = Consts.CARD_TYPE_NONE;
            heroControllerFrom = null;
        }
    }
}