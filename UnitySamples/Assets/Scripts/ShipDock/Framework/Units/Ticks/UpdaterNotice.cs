using System;

namespace ShipDock
{
    public class UpdaterNotice : ParamNotice<IUpdate>
    {
        private static Notifications<int> sender;

        private static Notifications<int> Notificater
        {
            get
            {
                if (sender == default)
                {
                    sender = NotificatonsInt.Instance.Notificater;
                }
                else { }

                return sender;
            }
        }

        public override void ToPool()
        {
            Pooling<UpdaterNotice>.To(this);
        }

        public static void AddUpdate(IUpdate target)
        {
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = target;
            notice.SetNoticeName(ShipDockConsts.NOTICE_ADD_UPDATE);
            Notificater.Broadcast(notice);
            notice.ToPool();
        }

        public static void RemoveUpdate(IUpdate target)
        {
            UpdaterNotice notice = Pooling<UpdaterNotice>.From();
            notice.ParamValue = target;
            notice.SetNoticeName(ShipDockConsts.NOTICE_REMOVE_UPDATE);
            Notificater.Broadcast(notice);
            notice.ToPool();
        }

        public static void SceneCallLater(Action<float> target)
        {
            Framework.Instance.Updates?.AddCallLate(target);
        }

        public static void AddSceneUpdate(IUpdate target)
        {
            Framework.Instance.Updates?.AddUpdate(target);
        }

        public static void RemoveSceneUpdate(IUpdate target)
        {
            Framework.Instance.Updates?.RemoveUpdate(target);
        }

        public static void AddSceneFixedUpdate(IUpdate target)
        {
            Framework.Instance.Updates?.AddFixedUpdate(target);
        }

        public static void RemoveSceneFixedUpdate(IUpdate target)
        {
            Framework.Instance.Updates?.RemoveFixedUpdate(target);
        }

        public static void AddSceneLateUpdate(IUpdate target)
        {
            Framework.Instance.Updates?.AddLateUpdate(target);
        }

        public static void RemoveSceneLateUpdate(IUpdate target)
        {
            Framework.Instance.Updates.RemoveLateUpdate(target);
        }
    }
}
