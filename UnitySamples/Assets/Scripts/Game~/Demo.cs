using ShipDock;
using UnityEngine;

public class Demo : ShipDockAppComponent
{
    [SerializeField]
    private Transform[] m_RandomSpwans;

    public override void InitConfigTypesHandler(IParamNotice<ConfigHelper> param)
    {
        base.InitConfigTypesHandler(param);

        param.ParamValue.AddHolderType<StaticConfig.PuzzlesConfig>("heros");
    }

    public override void EnterGameHandler()
    {
        base.EnterGameHandler();

        Debug.Log("Demo start");

        InitECS();

        //加载场景
        Scenes scenes = new();
        scenes.OnSceneLoaded += (s, sceneManagement) =>
        {
            scenes.OnSceneLoaded = default;
            StartGame();
        };
        scenes.LoadAndClearAnotherScene("GameStart");
    }

    private void InitECS()
    {
        //添加系统
        Tenons tenons = ShipDockApp.Instance.Tenons;
        MovementSystem movementSystem = tenons.AddSystem<MovementSystem>();
        ShootSystem shootSystem = tenons.AddSystem<ShootSystem>();

        //创建系统所需数据
        MovementDataCreater roleMovementDataCreater = new();
        roleMovementDataCreater.OnExecuter(movementSystem.ExecuteMovements);

        MovementDataCreater movementDataCreater = new();
        movementDataCreater.OnExecuter(shootSystem.ExecuteMovements);

        WeaponDataCreater weaponDataCreater = new();
        weaponDataCreater.OnExecuter(shootSystem.ExecuteWeapons);

        BulletDataCreater bulletDataCreater = new();
        bulletDataCreater.OnExecuter(shootSystem.ExecuteBullets);

        //绑定系统数据
        movementSystem.AddDataCreater(Consts.TENON_TYPE_ROLE_MOVEMENT, roleMovementDataCreater);

        shootSystem.AddDataCreater(Consts.TENON_TYPE_WEAPON, weaponDataCreater);
        shootSystem.AddDataCreater(Consts.TENON_TYPE_BULLET, bulletDataCreater);
        shootSystem.AddDataCreater(Consts.TENON_TYPE_MOVEMENT, movementDataCreater);
    }

    private void StartGame()
    {
        //创建主角
        AssetBundles abs = ShipDockApp.Instance.ABs;
        GameObject res = abs.Get<GameObject>("roles", "Hum_JYsni_1");
        GameObject asset = Instantiate(res);
        GameObject resEnemy = abs.Get<GameObject>("roles", "Zom_PThum_2");

        UpdaterNotice.SceneCallLater((t) =>
        {
            //初始化主角
            MainRole mainRole = new();
            mainRole.InitRole(asset);
            mainRole.MovementTenon.Data.syncToTrans = true;
            mainRole.MovementTenon.SetPosition(new Vector3(3.3f, 3.3f, 2.72f));
            mainRole.MovementTenon.SetRotation(Quaternion.Euler(0f, -180f, 0f));
            mainRole.MovementTenon.SetScale(Vector3.one * 0.5f);

            TimeUpdater.New(1f, () => {
                EnemyRole enemyRole = new();
                GameObject assetEnemy = Instantiate(resEnemy);
                enemyRole.InitRole(assetEnemy);
                enemyRole.MovementTenon.Data.syncToTrans = true;
                enemyRole.MovementTenon.SetScale(Vector3.one * 0.5f);

                int len = m_RandomSpwans.Length;
                int index = Utils.RangeRandom(0, len - 1);
                Vector3 pos = m_RandomSpwans[index].position;
                enemyRole.MovementTenon.SetPosition(pos);

                Transform trans = enemyRole.Res.RoleRes.Animator.transform;
                trans.LookAt(pos);

                float h = pos.y + 2.1f;

                MethodUpdater updater = new()
                {
                    Update = (t) =>
                    {
                        Vector3 pos = trans.position;
                        if (trans.position.y >= h)
                        {
                            enemyRole.SetRoleTarget(mainRole);
                        }
                        else
                        {
                            trans.position = new Vector3(pos.x, pos.y + 3f * t, pos.z);
                        }
                    },
                };
                UpdaterNotice.AddSceneUpdater(updater);

            }, default, 0);
        });
    }
}