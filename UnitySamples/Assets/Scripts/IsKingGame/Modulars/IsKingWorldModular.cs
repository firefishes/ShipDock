using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Notices;

namespace IsKing
{
    public class IsKingWorldModular : WorldModular
    {
        public IsKingWorldModular() : base(Consts.M_WORLD, Consts.MSG_ADD_UPDATER, Consts.MSG_RM_UPDATER)
        {
        }

        public override void InitModular()
        {
            base.InitModular();

            InitECS();

            //IParamNotice<string> notice = Pooling<ParamNotice<string>>.From();
            //notice.ParamValue = "！！！";

            //AddMessageToQueue(Consts.MSG_GAME_READY, notice);
        }

        private void InitECS()
        {
            //定义通用覆盖检测组件名
            EntityComponent.COMP_NAME_OVERLAY_MAPPER = Consts.COMP_BEHAVIOUR_IDS;

            //创建并激活一个ECS世界上下文
            ShipDockApp app = ShipDockApp.Instance;
            ECSContext contexts = app.ECSContext;
            contexts.CreateContext(Consts.ECS_CONTEXT_PEACE);
            contexts.ActiveECSContext();

            ILogicContext worldContext = contexts.CurrentContext;

            //创建组件
            worldContext.Create<HeroMovementComp>(Consts.COMP_HERO_MOVEMENT);
            worldContext.Create<MonsterMovementComp>(Consts.COMP_MONSTER_MOVEMENT);
            worldContext.Create<BehaviourIDsComponent>(Consts.COMP_BEHAVIOUR_IDS);
            worldContext.Create<RolePropertiesComp>(Consts.COMP_ROLE_PROPERTIES);
            worldContext.Create<AttackableComp>(Consts.COMP_ATTACKABLE);
            worldContext.Create<WorldResourceComp>(Consts.COMP_WORLD_RES);

            //创建系统
            worldContext.Create<IsKingWorldSystem>(Consts.SYSTEM_WORLD, true,
                Consts.COMP_HERO_MOVEMENT,
                Consts.COMP_MONSTER_MOVEMENT);

            worldContext.Create<MeleeSystem>(Consts.SYSTEM_SKILL_MELEE, false, Consts.COMP_ATTACKABLE);

            ILogicEntities logicEntitas = worldContext.AllEntitas;
            #region 角色实体
            logicEntitas.BuildEntitasTemplate(1, new int[] {
                Consts.COMP_ROLE_PROPERTIES,
                Consts.COMP_HERO_MOVEMENT
            });
            logicEntitas.BuildEntitasTemplate(2, new int[] {
                Consts.COMP_ROLE_PROPERTIES,
                Consts.COMP_MONSTER_MOVEMENT
            });
            #endregion

            #region 技能实体
            logicEntitas.BuildEntitasTemplate(3, new int[] { Consts.COMP_ATTACKABLE });
            #endregion

            logicEntitas.MakeChunks();

            //启动ECS
            app.StartECS();
        }

        protected override void SettleMessageQueue(int message, INoticeBase<int> notice)
        {
            base.SettleMessageQueue(message, notice);
        }
    }

}