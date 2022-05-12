using System;
using System.Collections.Generic;
using ShipDock.Applications;
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
        ParamNotice<Queue<CardInfoController>> mGainCardNotice = Pooling<ParamNotice<Queue<CardInfoController>>>.From();

        public void UpdatePlayerIntelligence(Vector2 value)
        {
            mIntelligenceNotice.ParamValue = value;
            mIntelligenceNotice.SetNoticeName(UIBattleModular.UI_UPDATE_INTELLIGENCE);

            UpdatSubgroup("battleIntellige", mIntelligenceNotice);
        }

        internal void UpdatePlayerMorale(Vector2 moraleValue)
        {
            mMoraleNotice.ParamValue = moraleValue;
            mMoraleNotice.SetNoticeName(UIBattleModular.UI_UPDATE_MORALE);

            UpdatSubgroup("battleMorale", mMoraleNotice);
        }

        internal void UpdatePlayerTroops(Vector2Int troopsValue)
        {
            mTroopsNotice.ParamValue = troopsValue;
            mTroopsNotice.SetNoticeName(UIBattleModular.UI_UPDATE_TROOPS);

            UpdatSubgroup("battleTroops", mTroopsNotice);
        }

        internal void UpdateGeneralOrder(ref Queue<CardInfoController> cardQueue)
        {
            mGainCardNotice.ParamValue = cardQueue;
            mGainCardNotice.SetNoticeName(UIBattleModular.UI_UPDATE_BATTLE_CARDS);
            UpdatSubgroup("GeneralOrder", mGainCardNotice);
        }
    }
}