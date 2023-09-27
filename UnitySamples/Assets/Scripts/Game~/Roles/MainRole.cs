using ShipDock;
using System.Data;
using UnityEngine;

public class Role
{
    public RoleResTenon Res { get; private set; }
    public RoleMovementTenon MovementTenon { get; private set; }
    public RoleInputTenon InputTenon { get; protected set; }

    private MethodUpdater mInputUpdater;

    public Role()
    {
        Tenons tenons = ShipDockApp.Instance.Tenons;
        InputTenon = tenons.AddTenonByType<RoleInputTenon>(Consts.TENON_TYPE_ROLE_INPUT);

        Res = tenons.AddTenonByType<RoleResTenon>(Consts.TENON_TYPE_ROLE_RES);
        MovementTenon = tenons.AddTenonByType<RoleMovementTenon>(Consts.TENON_TYPE_ROLE_MOVEMENT);
    }

    public virtual void InitRole(GameObject target)
    {
        mInputUpdater = new MethodUpdater()
        {
            Update = OnInputUpdate,
        };
        UpdaterNotice.AddSceneUpdater(mInputUpdater);

        Res.BindRes(target);

        MovementTenon.InitData(Res);

        InputTenon.SetDataChangeAutoReset(true);
        InputTenon.SetMovementTenon(MovementTenon.GetTenonID());
    }

    protected virtual void OnInputUpdate(float deltaTime)
    {
    }
}

public class EnemyRole : Role
{
    private Role mTargetRole;

    public override void InitRole(GameObject target)
    {
        base.InitRole(target);

    }

    public void SetRoleTarget(Role role)
    {
        mTargetRole = role;
        MovementTenon.MoveTarget = role.MovementTenon;
        MovementTenon.SetSpeed(Res.RoleRes.MoveSpeed);
        MovementTenon.DataValid();

        //UnityEngine.Debug.Log("Res.RoleRes.MoveSpeed "+ Res.RoleRes.MoveSpeed);
    }

    protected override void OnInputUpdate(float deltaTime)
    {
        base.OnInputUpdate(deltaTime);

        if (mTargetRole != default)
        {
            MovementTenon.SetPosition(mTargetRole.MovementTenon.GetPosition());
            MovementTenon.Data.moveToTarget = true;
            MovementTenon.DataValid();
        }
        else { }
    }
}

public class MainRole : Role
{
    public WeaponTenon WeaponTenon { get; private set; }

    public MainRole()
    {
        Tenons tenons = ShipDockApp.Instance.Tenons;
        WeaponTenon = tenons.AddTenonByType<WeaponTenon>(Consts.TENON_TYPE_WEAPON);
    }

    public override void InitRole(GameObject target)
    {
        base.InitRole(target);

        WeaponTenon.InitWeapon(Res);
        InputTenon.SetWeaponTenon(WeaponTenon.GetTenonID());
    }

    protected override void OnInputUpdate(float deltaTime)
    {
        base.OnInputUpdate(deltaTime);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isFire = Input.GetButton("Fire1");
        if (isFire)
        {
            InputTenon.StartAttack();
            InputTenon.StopMove();
        }
        else
        {
            InputTenon.StopAttack();
            if (h != 0f || v != 0f)
            {
                InputTenon.StartMove(new Vector2(h, v));
            }
            else 
            {
                InputTenon.StopMove();
            }
        }
    }
}