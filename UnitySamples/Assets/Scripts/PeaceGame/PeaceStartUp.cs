using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public class PeaceStartUp : ShipDockAppComponent
    {
        public override void ApplicationCloseHandler()
        {
            base.ApplicationCloseHandler();
        }

        public override void GetDataProxyHandler(IParamNotice<IDataProxy[]> param)
        {
            base.GetDataProxyHandler(param);

            param.ParamValue = new IDataProxy[]
            {
                new ConfigData(Consts.D_CONFIGS),
                new PlayerData(),
            };
        }

        public override void EnterGameHandler()
        {
            base.EnterGameHandler();

            PlayerData playerData = Consts.D_PLAYER.GetData<PlayerData>();
            playerData.InitPlayer();

            IModular[] modulars = new IModular[]
            {
                new MessageModular(),
                new BattleModular(),
            };
            DecorativeModulars modluars = ShipDockApp.Instance.AppModulars;
            modluars.AddModular(modulars);

            IParamNotice<string> notice = new ParamNotice<string>()
            {
                ParamValue = "£¡£¡£¡",
            };

            modluars.NotifyModular(Consts.N_MSG_ADD, MessageNotice.Create(Consts.MSG_ENTER_BATTLE, notice));
        }
    }
}