using ShipDock;
using UnityEngine;

public class Weapon : ITenonData
{
    public bool isShoot;
    public float shotBlends;
    public float shotGapTime;
    public float shotGapTimeMax;

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
        Pooling<Weapon>.To(this);
    }
}

public class WeaponTenon : Tenon, ITenon<Weapon>
{
    private GameObject mBulletRes;
    private GameObject mBulletCreated;
    private BulletTenon mBulletTenonCreated;

    public Transform FirePoint { get; private set; }
    public Weapon Data { get; private set; }
    public int ResTenonID { get; private set; }
    public Animator ShooterAnimator { get; protected set; }
    public override int[] SystemIDs { get; } = new int[] { Consts.TENON_SYSTEM_SHOOT };

    protected override void Purge() { }

    //protected override bool CheckAndKeepingDataChanged()
    //{
    //    bool result = base.CheckAndKeepingDataChanged();

    //    if (mIsInited) { }
    //    else
    //    {
    //        mIsInited = true;
    //        result = mIsInited;
    //    }

    //    return result;
    //}

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

        Data = new Weapon();
        Data.SetTenon(this);
    }

    public void InitWeapon(RoleResTenon roleResTenon)
    {
        RoleRes res = roleResTenon.RoleRes;
        ResTenonID = roleResTenon.GetTenonID();
        mBulletRes = res.BulletRes;
        FirePoint = res.WeaponFirePoint;
        ShooterAnimator = res.Animator;
    }

    protected override void OnTenonFrameInit(float deltaTime)
    {
        base.OnTenonFrameInit(deltaTime);

        //Tenons tenons = ShipDockApp.Instance.Tenons;
        //RoleResTenon roleResTenon = tenons.GetTenon<RoleResTenon>(ResTenonID);
        //RoleRes res = roleResTenon.RoleRes;

    }

    protected override void OnTenonFrame(float deltaTime)
    {
        base.OnTenonFrame(deltaTime);
    }

    public Weapon GetData()
    {
        return Data;
    }

    #region 组件其他数据行为
    public void Shoot()
    {
        if (Data.isShoot) { }
        else
        {
            Data.shotGapTimeMax = 0.1f;
            Data.shotGapTime = 0.55f;
            Data.isShoot = true;
        }
        DataValid();
    }

    public void CreateBullete()
    {

        ShipDockApp app = ShipDockApp.Instance;
        mBulletCreated = app.AssetsPooling.FromPool(Consts.POOL_RES_BULLET, ref mBulletRes);
        mBulletTenonCreated = app.Tenons.AddTenonByType<BulletTenon>(Consts.TENON_TYPE_BULLET);
        mBulletTenonCreated.InitBullet(ref app, mBulletCreated, FirePoint.position, FirePoint.forward);
        mBulletTenonCreated = default;
        mBulletCreated = default;
    }

    public void StopShoot()
    {
        Data.isShoot = false;
        DataValid();
    }
    #endregion
}
