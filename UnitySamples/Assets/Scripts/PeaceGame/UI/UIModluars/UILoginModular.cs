using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public class UILoginModular : UIModularImpl<UILogin, ILoginUI>
    {
        public const int UIM_LGOIN_NEW_GAME = 1;

        public override string ABName { get; } = Consts.AB_LOGOIN;
        public override string UIAssetName { get; protected set; } = Consts.U_LOGIN;
        public override string Name { get; protected set; } = Consts.UM_LOGIN;
        public override int UILayer { get; protected set; } = UILayerType.WINDOW;
        public override int[] DataProxyLinks { get; set; } = new int[] { Consts.D_PLAYER };


        protected override void Purge()
        {
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Enter()
        {
            base.Enter();

            PlayerData playerData = Consts.D_PLAYER.GetData<PlayerData>();
            PeaceClientInfo clientInfo = playerData.LocalClient.ClientInfo;

            bool enabled = clientInfo.accountID != "0";
            UIImpl.CheckLoadGameEnabled(enabled);
        }

        public override void OnDataProxyNotify(IDataProxy data, int keyName)
        {
            if (data is PlayerData playerData)
            {
                switch (keyName)
                {
                    case Consts.DN_NEW_GAME_CREATED:
                        PeaceClientInfo clientInfo = playerData.LocalClient.ClientInfo;

                        bool enabled = clientInfo.accountID != "0";
                        UIImpl.CheckLoadGameEnabled(enabled);

                        Consts.UM_LOGIN.Close();
                        break;
                }
            }
            else { }
        }

        protected override void UIModularHandler(INoticeBase<int> param)
        {
            switch (param.Name)
            {
                case UIM_LGOIN_NEW_GAME:
                    PlayerData playerData = Consts.D_PLAYER.GetData<PlayerData>();
                    playerData.LoadNewPlayer();
                    break;
            }

        }
    }

}