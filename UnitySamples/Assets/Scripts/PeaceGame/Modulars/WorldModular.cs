using ShipDock.Applications;
using ShipDock.Commons;
using ShipDock.Notices;
using ShipDock.Pooling;

namespace Peace
{
    public class WorldModular : BaseModular
    {
        private MethodUpdater mWorldUpdater;

        public WorldModular() : base(Consts.M_WORLD)
        {
        }

        public override void InitModular()
        {
            base.InitModular();

            mWorldUpdater = new MethodUpdater()
            {
                Update = OnWorldUpdate,
                LateUpdate = OnWorldLateUpdate,
                FixedUpdate = OnWorldFixedUpdate,
            };

            UpdaterNotice.AddSceneUpdater(mWorldUpdater);

            ShipDockApp.Instance.StartECS();
        }

        private void OnWorldFixedUpdate(int dTime)
        {
        }

        private void OnWorldLateUpdate()
        {
        }

        private void OnWorldUpdate(int dTime)
        {
            IParamNotice<string> notice = Pooling<ParamNotice<string>>.From();
            notice.ParamValue = "£¡£¡£¡";

            AddMessageToQueue(Consts.MSG_GAME_READY, notice);
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
        }
    }

}