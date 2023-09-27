using ShipDock;
using UnityEngine;

public class InputTenon : Tenon
{
    private Vector3 mMoving;
    private Vector2 mInputing;
    private bool mIsMoved;
    private RoleMovementTenon mRoleMovementTenon;

    protected override void Purge()
    {
    }

    public void SetMovementTenon(int tenonID)
    {
        Tenons tenons = ShipDockApp.Instance.Tenons;
        mRoleMovementTenon = tenons.GetTenon<RoleMovementTenon>(tenonID);
    }

    protected override void OnTenonFrameReady(float deltaTime)
    {
        base.OnTenonFrameReady(deltaTime);

        float h = mInputing.x;
        float v = mInputing.y;
        Vector3 direction = new(h, 0f, v);

        if (direction != Vector3.zero)
        {
            mIsMoved = true;
            mRoleMovementTenon.SetSpeed(mRoleMovementTenon.MoveSpeedMax);
            Vector3 pos = mRoleMovementTenon.GetPosition();
            mMoving = pos + Time.smoothDeltaTime * mRoleMovementTenon.MoveSpeed * direction;
        }
        else { }
    }

    protected override void OnTenonFrame(float deltaTime)
    {
        base.OnTenonFrame(deltaTime);

        mRoleMovementTenon.Data.isMoving = mIsMoved;
        if (mIsMoved)
        {
            mRoleMovementTenon.SetPosition(mMoving);
        }
        else { }
    }

    public void StopMove()
    {
        mIsMoved = false;
        mInputing = Vector3.zero;
        mRoleMovementTenon.SetSpeed(0f);
        mRoleMovementTenon.DataValid();
    }

    public void StartMove(Vector2 inputValue)
    {
        if (inputValue == Vector2.zero)
        {
            StopMove();
        }
        else
        {
            mInputing = inputValue.normalized;

            DataValid();
        }
    }
}

public class RoleInputTenon : InputTenon
{
    private WeaponTenon mWeaponTenon;

    internal void SetWeaponTenon(int tenonID)
    {
        Tenons tenons = ShipDockApp.Instance.Tenons;
        mWeaponTenon = tenons.GetTenon<WeaponTenon>(tenonID);
    }

    internal void StartAttack()
    {
        mWeaponTenon.Shoot();
    }

    internal void StopAttack()
    {
        mWeaponTenon.StopShoot();
    }
}
