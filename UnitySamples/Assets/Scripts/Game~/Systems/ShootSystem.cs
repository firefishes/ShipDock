using ShipDock;
using System;
using UnityEngine;


public class BulletDataCreater : TenonSystemDatas<Bullet>
{
}

public class WeaponDataCreater : TenonSystemDatas<Weapon>
{
}

public class ShootSystem : TenonSystem
{
    public override int SystemID { get; } = Consts.TENON_SYSTEM_SHOOT;

    public void ExecuteWeapons(int tenonID, Weapon data)
    {
        Animator animator = (data.Tenon as WeaponTenon).ShooterAnimator;
        if (data.isShoot)
        {
            if (data.shotBlends >= 0.5f)
            {
                data.shotGapTime -= Time.deltaTime;
                if (data.shotGapTime <= 0f)
                {
                    data.shotGapTime += data.shotGapTimeMax;
                    (data.Tenon as WeaponTenon).CreateBullete();
                }
                else { }
            }
            else
            {
                data.shotBlends = 0.5f;
                animator.SetFloat("Shot", data.shotBlends);
            }

        }
        else
        {
            if (data.shotBlends != 0f)
            {
                data.shotBlends = 0f;
                animator.SetFloat("Shot", data.shotBlends);
            }
            else { }
        }

        animator.SetBool("IsShot", data.isShoot);
    }

    public void ExecuteBullets(int tenonID, Bullet data)
    {
        BulletTenon tenon = data.Tenon as BulletTenon;
        tenon.SyncBullet();

        Vector3 startPos = tenon.StartPos;
        if (Vector3.Distance(startPos, tenon.Movement.Data.position) > 100f)
        {
            tenon.Drop();
        }
        else { }
    }

    internal void ExecuteMovements(int tenonID, Movement movement)
    {
        movement.position += Time.deltaTime * movement.currentSpeed * movement.direction;
    }

    public override void Execute()
    {
        DuringExecute<Weapon>(Consts.TENON_TYPE_WEAPON);
        DuringExecute<Movement>(Consts.TENON_TYPE_MOVEMENT);
        DuringExecute<Bullet>(Consts.TENON_TYPE_BULLET);
    }
}
