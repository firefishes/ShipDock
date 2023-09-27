using ShipDock;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : ITenonData
{
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
        Pooling<Bullet>.To(this);
    }
}

public class BulletTenon : Tenon, ITenon<Bullet>
{
    private GameObject mBulletRes;

    public MovementTenon Movement { get; private set; }
    public Vector3 StartPos { get; set; }
    public Transform FirePoint { get; private set; }
    public Bullet Data { get; private set; }
    public override int[] SystemIDs { get; } = new int[] { Consts.TENON_SYSTEM_SHOOT };

    protected override void Purge()
    {
        Data.ToPool();
        Data = default;

        ShipDockApp app = ShipDockApp.Instance;
        app.AssetsPooling.ToPool(Consts.POOL_RES_BULLET, mBulletRes);
    }

    public override void Drop()
    {
        base.Drop();

        Movement.Drop();
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

        Data = Pooling<Bullet>.From();
        Data.SetTenon(this);
    }

    protected override void OnTenonFrame(float deltaTime)
    {
        base.OnTenonFrame(deltaTime);

        Movement.DataValid();
        DataValid();
    }

    public Bullet GetData()
    {
        return Data;
    }

    #region 组件其他数据行为
    public void InitBullet(ref ShipDockApp app, GameObject bulletRes, Vector3 startPos, Vector3 forward)
    {
        mBulletRes = bulletRes;
        StartPos = startPos;
        
        Movement = app.Tenons.AddTenonByType<MovementTenon>(Consts.TENON_TYPE_MOVEMENT);
        Movement.Init(10f, 0f);
        Movement.SetPosition(startPos);
        Movement.SetScale(Vector3.one * 0.4f);
        Movement.Data.currentSpeed = 30f;
        Movement.Data.direction = forward;

        Quaternion quaternion = Quaternion.LookRotation(forward, Vector3.up);
        Movement.SetRotation(quaternion);
    }

    public void SyncBullet()
    {
        var data = Movement.Data;
        mBulletRes.transform.position = data.position;
        mBulletRes.transform.rotation = data.rotation;
        mBulletRes.transform.localScale = data.scale;
    }
    #endregion
}
