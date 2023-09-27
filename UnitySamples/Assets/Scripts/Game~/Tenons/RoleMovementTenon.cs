using ShipDock;
using System;
using UnityEngine;

public class RoleMovementTenon : MovementTenon
{
    private bool mIsInited;

    public RoleMovementTenon MoveTarget { get; set; }
    public Transform Trans { get; private set; }
    public int ResTenonID { get; private set; }
    public override int[] SystemIDs { get; } = new int[] { Consts.TENON_SYSTEM_MOVEMENT };

    protected override void Purge()
    {
        base.Purge();

        Trans = default;
    }

    protected override bool CheckAndKeepingDataChanged()
    {
        bool result = base.CheckAndKeepingDataChanged();

        if (mIsInited) { }
        else
        {
            mIsInited = true;
            result = mIsInited;
        }

        return result;
    }

    public void InitData(RoleResTenon roleResTenon)
    {
        RoleRes res = roleResTenon.RoleRes;
        ResTenonID = roleResTenon.GetTenonID();
        
        Trans = res.transform;
        Data.position = Trans.position;
        Data.rotation = Trans.rotation;
        Data.scale = Trans.localScale;

        Init(res.MoveSpeed, res.RotateSpeed);

        DataValid();
    }

    protected override void OnTenonFrameInit(float deltaTime)
    {
        base.OnTenonFrameInit(deltaTime);

        Tenons tenons = ShipDockApp.Instance.Tenons;
        RoleResTenon roleResTenon = tenons.GetTenon<RoleResTenon>(ResTenonID);
        RoleRes res = roleResTenon.RoleRes;

        MoveSpeedMax = res.MoveSpeed;
        RotateSpeed = res.RotateSpeed;
    }

    protected override void OnTenonFrame(float deltaTime)
    {
        base.OnTenonFrame(deltaTime);

        //if (MoveTarget != default)
        //{
        //    Data.direction = MoveTarget.Data.position - Trans.position;
        //    SetSpeed(MoveSpeed);
        //    DataValid();
        //}
        //else { }
    }

    internal void Hide()
    {
        Trans.gameObject.SetActive(false);

    }
}
