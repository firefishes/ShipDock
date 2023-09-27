using ShipDock;
using UnityEngine;

public class Movement : ITenonData
{
    public bool moveToTarget;
    public bool isMoving;
    public bool syncToTrans;
    public float currentSpeed;
    public Vector3 position;
    public Vector3 positionPrev;
    public Vector3 direction;
    public Quaternion rotation;
    public Vector3 scale;
    public Vector3 rotate;

    public ITenon Tenon { get; private set; }

    public void Revert()
    {
        Tenon = default;
    }

    public void SetTenon(ITenon tenon)
    {
        Tenon = tenon;
    }

    public void ToPool()
    {
        Pooling<Movement>.To(this);
    }
}

public class MovementTenon : Tenon, ITenon<Movement>
{
    public Movement Data { get; protected set; }
    public float MoveSpeed { get; protected set; }
    public float MoveSpeedMax { get; protected set; }
    public float RotateSpeed { get; protected set; }
    public override int[] SystemIDs { get; } = new int[] { Consts.TENON_SYSTEM_SHOOT };

    protected override void Purge() 
    {
        Data.ToPool();
        Data = default;
    }

    protected override void BindSystem(ref Tenons tenons, int systemID)
    {
        tenons.BindSystem(this, systemID);
    }

    protected override void DebindSystem(ref Tenons tenons, int systemID)
    {
        tenons.DebindSystem(this, systemID);
    }

    protected override void CreateData()
    {
        base.CreateData();

        Data = Pooling<Movement>.From();
        Data.SetTenon(this);
    }

    protected override void OnTenonFrameInit(float deltaTime)
    {
        base.OnTenonFrameInit(deltaTime);

    }

    public Movement GetData()
    {
        return Data;
    }

    protected override bool CheckAndKeepingDataChanged()
    {
        return true;
    }

    #region 组件其他数据行为
    public void Init(float moveSpeed, float rotateSpeed)
    {
        MoveSpeedMax = moveSpeed;
        RotateSpeed = rotateSpeed;
    }

    public void SetPosition(Vector3 pos)
    {
        Data.position = pos;
        DataValid();
    }

    public Vector3 GetPosition()
    {
        return Data.position;
    }

    public void SetScale(Vector3 scale)
    {
        Data.scale = scale;
        DataValid();
    }

    public Vector3 GetScale()
    {
        return Data.scale;
    }

    public void SetRotation(Quaternion rot)
    {
        Data.rotation = rot;
        DataValid();
    }

    public Quaternion GetRotation()
    {
        return Data.rotation;
    }

    public void SetSpeed(float speed)
    {
        MoveSpeed = speed;
    }
    #endregion
}
