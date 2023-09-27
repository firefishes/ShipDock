using ShipDock;
using UnityEngine;
using UnityEngine.UIElements;


public class MovementDataCreater : TenonSystemDatas<Movement>
{
}

public class MovementSystem : TenonSystem
{
    private Transform mTrans;
    private RoleMovementTenon mRoleMovementTenon;

    public override int SystemID { get; } = Consts.TENON_SYSTEM_MOVEMENT;

    public void ExecuteMovements(int tenonID, Movement movement)
    {
        mRoleMovementTenon = mTenons.GetTenon<RoleMovementTenon>(tenonID);

        mTrans = mRoleMovementTenon.Trans;
        mTrans.position = Vector3.Lerp(mTrans.position, movement.position, mRoleMovementTenon.MoveSpeed * Time.smoothDeltaTime);

        mTrans.localScale = movement.scale;

        int resTenonID = mRoleMovementTenon.ResTenonID;
        RoleResTenon roleResTenon = mTenons.GetTenon<RoleResTenon>(resTenonID);

        const string KEY_IS_MOVE = "IsMove";
        const string KEY_MOVE = "Move";
        const string KEY_SPEED = "Speed";
        bool flag = movement.isMoving || movement.moveToTarget;
        roleResTenon.RoleRes.Animator.SetBool(KEY_IS_MOVE, flag);
        roleResTenon.RoleRes.Animator.SetFloat(KEY_MOVE, flag ? 1f : 0f);
        roleResTenon.RoleRes.Animator.SetFloat(KEY_SPEED, flag ? 1f : 0f);

        if (flag)
        {
            if (movement.moveToTarget)
            {
                if (Vector3.Distance(mTrans.position, movement.position) <= 0.1f)
                {
                    mRoleMovementTenon.Hide();
                }
                else { }
            }
            else { }
            movement.moveToTarget = false;
            Quaternion targetRotation = Quaternion.LookRotation(movement.position - movement.positionPrev, Vector3.up);
            targetRotation = Quaternion.Lerp(mTrans.rotation, targetRotation, mRoleMovementTenon.RotateSpeed * Time.smoothDeltaTime);
            mTrans.rotation = targetRotation;

            movement.rotation = targetRotation;
        }
        else
        {
            if (movement.syncToTrans)
            {
                movement.syncToTrans = false;
                mTrans.position = movement.position;
                mTrans.rotation = movement.rotation;
                mTrans.localScale = movement.scale;
            }
            else
            {
                movement.position = mTrans.position;
            }
        }

        movement.positionPrev = mTrans.position;

        mTrans = default;
        mRoleMovementTenon = default;
    }

    public override void Execute()
    {
        DuringExecute<Movement>(Consts.TENON_TYPE_ROLE_MOVEMENT);
    }
}
