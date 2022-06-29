using ShipDock.Commons;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Tools;

namespace Peace
{
    public class MessageModular : ApplicationModular
    {
        public override int ModularName { get; protected set; } = Consts.M_MESSAGE;

        private bool mHasMessageQueue;
        private DoubleBuffers<IPeaceNotice> mDoubleBuffers;
        private IPeaceNotice mNoticeWillAdd;
        private MethodUpdater mMessageUpdater;

        public override void Purge()
        {
        }

        public override void InitModular()
        {
            base.InitModular();

            mDoubleBuffers = new DoubleBuffers<IPeaceNotice>()
            {
                OnDequeue = OnMessageDequeue,
            };

            mMessageUpdater = new MethodUpdater()
            {
                Update = OnModularUpdate,
            };

            UpdaterNotice.AddSceneUpdater(mMessageUpdater);
        }

        protected override void InitCustomHandlers()
        {
            base.InitCustomHandlers();

            AddNoticeHandler(OnMessageAdd);
        }

        [ModularNoticeListener(Consts.N_MSG_ADD)]
        private void OnMessageAdd(INoticeBase<int> param)
        {
            mNoticeWillAdd = param as IPeaceNotice;
            if (mNoticeWillAdd != default)
            {
                if (mNoticeWillAdd != default)
                {
                    mDoubleBuffers.Enqueue(mNoticeWillAdd);
                }
                else { }
            }
            else { }

            mNoticeWillAdd = default;
        }

        private void OnModularUpdate(int deltaTime)
        {
            mDoubleBuffers.Update(deltaTime);
        }

        private void OnMessageDequeue(int dTime, IPeaceNotice param)
        {
            mHasMessageQueue = param != default;

            if (mHasMessageQueue)
            {
                NotifyModular(Consts.N_MSG_QUEUE, param);
                param.ToPool();
            }
            else { }

            mHasMessageQueue = false;
        }
    }
}
