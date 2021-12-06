using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsKing
{
    public class UIBattleModular : UIModular<UIBattle>
    {

        public const int UI_UPDATE_INTELLIGENCE = 0;
        public const int UI_UPDATE_MORALE = 1;
        public const int UI_UPDATE_TROOPS = 2;

        public override string ABName { get; } = Consts.AB_UI_BATTLE;
        public override string UIAssetName { get; protected set; } = Consts.UI_BATTLE;
        public override string Name { get; protected set; } = Consts.UIM_BATTLE;
        public override int UILayer { get; protected set; } = UILayerType.WINDOW;

        public override int[] DataProxyLinks { get; set; } = new int[]
        {
            Consts.D_BATTLE
        };

        public override void OnDataProxyNotify(IDataProxy data, int keyName)
        {
            if (data is BattleData battleData)
            {
                switch (keyName)
                {
                    case Consts.DN_PLAYER_INTELLIGENTAL_UPDATE:
                        Vector2 value = battleData.CurrentPlayerIntelligental();
                        UI.UpdatePlayerIntelligence(value);
                        break;

                    case Consts.N_GAIN_GENERAL_ORDER:
                        UI.UpdateGeneralOrder();
                        break;

                    case Consts.DN_BATTLE_DATA_UPDATE:
                        Vector2 moraleValue = battleData.CurrentMorale(Consts.CAMP_PLAYER);
                        UI.UpdatePlayerMorale(moraleValue);

                        Vector2Int troopsValue = battleData.CurrentTroops(Consts.CAMP_PLAYER);
                        UI.UpdatePlayerTroops(troopsValue);
                        break;
                }
            }
            else { }
        }

        protected override void Purge()
        {
        }

        protected override void UIModularHandler(INoticeBase<int> param)
        {
        }
    }

}