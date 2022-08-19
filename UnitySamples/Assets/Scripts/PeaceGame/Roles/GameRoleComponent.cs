using ShipDock.Applications;
using ShipDock.Commons;
using ShipDock.Interfaces;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Pooling;

namespace Peace
{
    public interface IWorldUpdate// : IWorldIntercatable
    {
        void RemoveFromWorld();
        void AddToWorld();
        MethodUpdater RoleUpdater { get; }
    }

    public interface IWorldElement : IWorldUpdate, IPoolable, IReclaim
    {
    }

    public abstract class WorldElement : IWorldElement
    {
        public MethodUpdater RoleUpdater { get; private set; }

        public void Reclaim()
        {
            Purge();

            RoleUpdater?.Reclaim();
            RoleUpdater = default;
        }

        protected abstract void Purge();

        public virtual void Revert() { }

        public abstract void ToPool();

        public void AddToWorld()
        {
            IParamNotice<IUpdate> notice = Pooling<ParamNotice<IUpdate>>.From();
            notice.ParamValue = RoleUpdater;
            MessageModular.AddMessage(Consts.MSG_ADD_UPDATER, notice);
        }

        public void RemoveFromWorld()
        {
            IParamNotice<IUpdate> notice = Pooling<ParamNotice<IUpdate>>.From();
            notice.ParamValue = RoleUpdater;
            MessageModular.AddMessage(Consts.MSG_RM_UPDATER, notice);
        }
    }

    public class GameRoleComponent : RoleComponent, IWorldUpdate
    {
        protected override void OnInit()
        {
            base.OnInit();

            AddToWorld();

            //WorldInteracter.Init<>();
        }

        protected override void Purge()
        {
            base.Purge();

            RemoveFromWorld();
        }

        private void OnEnable()
        {
            AddToWorld();
        }

        private void OnDisable()
        {
            RemoveFromWorld();
        }

        public void AddToWorld()
        {
            IParamNotice<IUpdate> notice = Pooling<ParamNotice<IUpdate>>.From();
            notice.ParamValue = RoleUpdater;
            MessageModular.AddMessage(Consts.MSG_ADD_UPDATER, notice);
        }

        public void RemoveFromWorld()
        {
            IParamNotice<IUpdate> notice = Pooling<ParamNotice<IUpdate>>.From();
            notice.ParamValue = RoleUpdater;
            MessageModular.AddMessage(Consts.MSG_RM_UPDATER, notice);
        }

        public override void OnUpdate(int time)
        {
            base.OnUpdate(time);
        }
    }

}