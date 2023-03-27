using UnityEngine;

namespace ShipDock
{
    /// <summary>
    /// 子弹数据
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class BulletData : IReclaim
    {
        /// <summary>射线</summary>
        private Ray mRay;
        /// <summary>射线的命中信息</summary>
        private RaycastHit mRayHit;

        /// <summary>是否已命中目标</summary>
        public bool IsHit { get; set; }
        /// <summary>命中事件是否已提交</summary>
        public bool IsHitCommited { get; set; }
        /// <summary>是否超出射程</summary>
        public bool OutOfShotRange { get; private set; }
        /// <summary>射线遮罩层</summary>
        public int RayMask { get; set; }
        /// <summary>命中目标所处的场景层级</summary>
        public int HitLayer { get; set; }
        /// <summary>命中目标的实例 ID</summary>
        public int HitID { get; set; }
        /// <summary>出膛速度</summary>
        public float MuzzleVelocity { get; set; }
        /// <summary>最大射程</summary>
        public float ShotRange { get; set; } = 100f;
        /// <summary>射线长度</summary>
        public float RayDistance { get; set; }
        /// <summary>命中方向</summary>
        public Vector3 HitDirection { get; set; }
        /// <summary>初始坐标</summary>
        public Vector3 InitPosition { get; set; }
        /// <summary>下一帧的预计坐标</summary>
        public Vector3 NextFramePos { get; set; }
        /// <summary>命中坐标</summary>
        public Vector3 HitPosition { get; set; }
        /// <summary>子弹命中数据缓存器</summary>
        public ICommonBulletChecker CommonBulletChecker { get; set; }

        public void Reclaim()
        {
            CommonBulletChecker = default;
        }

        public void Reset()
        {
            IsHit = default;
            IsHitCommited = default;
            OutOfShotRange = default;
            HitID = default;
            HitLayer = default;
        }

        /// <summary>
        /// 更新子弹数据
        /// </summary>
        /// <param name="time"></param>
        /// <param name="postion"></param>
        /// <param name="moveDirection"></param>
        /// <returns></returns>
        public Vector3 UpdateBulletData(float time, Vector3 postion, Vector3 moveDirection)
        {
            Vector3 currentPos = postion;
            Vector3 nextFramePos = postion + (moveDirection * MuzzleVelocity * time);

            RayDistance = Vector3.Magnitude(nextFramePos - currentPos);

            Vector3? result = null;
            if (RayDistance > 0)
            {
                if (IsHit)
                {
                    result = HitPosition;
                }
                else
                {
                    NextFramePos = nextFramePos;
                    result = nextFramePos;
                }
            }
            else
            {
                const string bulletHitLog = "Log: Bullet is hit, position is ({0})";
                bulletHitLog.Log(HitPosition.ToString());
            }
            return result.Value;
        }

        /// <summary>
        /// 检测子弹是否命中
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="postion"></param>
        /// <param name="moveDirection"></param>
        /// <param name="bulletBehaviour"></param>
        public void CheckRaycast(ref Transform transform, Vector3 postion, Vector3 moveDirection, ref IBulletBehaviour bulletBehaviour)
        {
            OutOfShotRange = false;

            if ((RayDistance > 0f) && !IsHit && !IsHitCommited)
            {
                transform.position = postion;
                OutOfShotRange = Vector3.Distance(postion, InitPosition) > ShotRange;
            }
            else { }

            if (IsHit || OutOfShotRange || IsHitCommited)
            {
                bulletBehaviour.BulletFinish();
            }
            else
            {
                mRay = bulletBehaviour.Ray;
                mRayHit = bulletBehaviour.RayHit;
                bool isHit = Utils.Raycast(postion, moveDirection, out mRay, out mRayHit, RayDistance, RayMask);
                if (isHit)
                {
                    GameObject hitTarget = mRayHit.transform.gameObject;
                    bulletBehaviour.HitTarget = hitTarget;

                    int id = hitTarget.GetInstanceID();
                    int hitLayer = hitTarget.layer;
                    Vector3 point = mRayHit.point;

                    RefreshHitParams(hitLayer, id, point, moveDirection);

                    bulletBehaviour?.AfterDoHit(this);

                    if (IsHitCommited) { }
                    else
                    {
                        IsHitCommited = true;
                        
                        //缓存并处理命中后的操作
                        CommonBulletChecker?.CacheBulletData(this);
                    }
                }
                else { }
            }
        }

        /// <summary>
        /// 刷新命中的结果参数
        /// </summary>
        /// <param name="hitLayer"></param>
        /// <param name="id"></param>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        public void RefreshHitParams(int hitLayer, int id, Vector3 point, Vector3 direction)
        {
            IsHit = true;
            HitID = id;
            HitLayer = hitLayer;
            HitPosition = point;
            HitDirection = direction;
        }
    }
}