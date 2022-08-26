using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public interface ILoadingView
    {
    }

    public class UILoadingModular : UIModularImpl<UILoading, ILoadingView>
    {
        public override string ABName { get; } = Consts.AB_UI_MAIN;
        public override string UIAssetName { get; protected set; } = Consts.U_LOADING;
        public override string Name { get; protected set; } = Consts.UM_LOADING;
        public override int UILayer { get; protected set; } = UILayerType.WINDOW;
        public override int[] DataProxyLinks { get; set; } = new int[] { /*Consts.D_PLAYER */ };

        public override void OnDataProxyNotify(IDataProxy data, int keyName)
        {
        }

        protected override void Purge()
        {
        }

        protected override void UIModularHandler(INoticeBase<int> param)
        {
        }

        public override void Init()
        {
            base.Init();

            Consts.UM_HEADQUARTERS.LoadAndOpenUI<UIHeadquartersModular>((a) => {
                Consts.UM_LOADING.Close();

            }, Consts.AB_UI_HEADQUARTERS);
        }
    }
}
