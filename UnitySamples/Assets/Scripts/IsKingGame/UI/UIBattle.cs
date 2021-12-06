using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.UI;
using UnityEngine;

namespace IsKing
{
    public class UIBattle : UIContainer
    {
        ParamNotice<Vector2> mIntelligenceNotice = Pooling<ParamNotice<Vector2>>.From();
        ParamNotice<Vector2> mMoraleNotice = Pooling<ParamNotice<Vector2>>.From();
        ParamNotice<Vector2Int> mTroopsNotice = Pooling<ParamNotice<Vector2Int>>.From();

        public void UpdatePlayerIntelligence(Vector2 value)
        {
            mIntelligenceNotice.ParamValue = value;
            mIntelligenceNotice.SetNoticeName(UIBattleModular.UI_UPDATE_INTELLIGENCE);

            this.Dispatch(mIntelligenceNotice);

            UpdatSubgroup("battleIntellige");
        }

        internal void UpdatePlayerMorale(Vector2 moraleValue)
        {
            mMoraleNotice.ParamValue = moraleValue;
            mMoraleNotice.SetNoticeName(UIBattleModular.UI_UPDATE_MORALE);

            this.Dispatch(mMoraleNotice);

            UpdatSubgroup("battleMorale");
        }

        internal void UpdatePlayerTroops(Vector2Int troopsValue)
        {
            mTroopsNotice.ParamValue = troopsValue;
            mTroopsNotice.SetNoticeName(UIBattleModular.UI_UPDATE_TROOPS);

            this.Dispatch(mTroopsNotice);

            UpdatSubgroup("battleTroops");
        }

        internal void UpdateGeneralOrder()
        {
            UpdatSubgroup("GeneralOrder");
        }
    }
}