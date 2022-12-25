using ShipDock.ECS;
using ShipDock.Tools;
using UnityEngine;

namespace IsKing
{
    public class IsKingMovementComp : WorldMovementComponent
    {
    }

    public class HeroMovementComp : IsKingMovementComp
    {
        public HeroMovementComp()
        {
            Name = "英雄移动组件";
        }
    }

    public class MonsterMovementComp : IsKingMovementComp
    {
        private float[] mLockTargetDownTime;
        private float[] mLockTargetDownTotalTime;
        private bool[] mWillMoveToTarget;
        private bool[] mWillMelle;
        private Vector3[] mTargetPosition;

        public MonsterMovementComp()
        {
            Name = "怪物移动组件";
        }

        protected override void DropData(ref ILogicData target)
        {
            base.DropData(ref target);

            int index = target.DataIndex;
            mLockTargetDownTime[index] = default;
            mLockTargetDownTotalTime[index] = default;
            mWillMoveToTarget[index] = default;
            mTargetPosition[index] = default;
        }

        protected override void OnResetSuccessive(bool clearOnly = false)
        {
            base.OnResetSuccessive(clearOnly);

            Utils.Reclaim(ref mLockTargetDownTime, clearOnly);
            Utils.Reclaim(ref mLockTargetDownTotalTime, clearOnly);
            Utils.Reclaim(ref mWillMoveToTarget, clearOnly);
            Utils.Reclaim(ref mTargetPosition, clearOnly);
            Utils.Reclaim(ref mWillMelle, clearOnly);
        }

        protected override void UpdateDataStretch(int dataSize)
        {
            base.UpdateDataStretch(dataSize);

            Utils.Stretch(ref mLockTargetDownTime, dataSize);
            Utils.Stretch(ref mLockTargetDownTotalTime, dataSize);
            Utils.Stretch(ref mWillMoveToTarget, dataSize);
            Utils.Stretch(ref mTargetPosition, dataSize);
            Utils.Stretch(ref mWillMelle, dataSize);
        }

        public void WillMelee(int entitas, bool flag)
        {
            UpdateValidWithType(entitas, ref mWillMelle, out _, flag);
        }

        public bool GetWillMelee(int entitas)
        {
            bool result = GetDataValueWithType(entitas, ref mWillMelle, out _);
            return result;
        }

        #region 锁定目标的总时长
        public void LockTargetDownTimeMax(int entitas, float max, float start = 0f)
        {
            start = Mathf.Min(start, max);

            UpdateValidWithType(entitas, ref mLockTargetDownTime, out _, start);
            UpdateValidWithType(entitas, ref mLockTargetDownTotalTime, out _, max);
        }

        public float GetLockTargetDownTimeMax(int entitas)
        {
            float result = GetDataValueWithType(entitas, ref mLockTargetDownTotalTime, out _);
            return result;
        }
        #endregion

        #region 锁定目标的流逝时间
        public void LockTargetDownTime(int entitas, float value = 0f)
        {
            float max = GetLockTargetDownTimeMax(entitas);

            value = Mathf.Min(value, max);

            UpdateValidWithType(entitas, ref mLockTargetDownTime, out _, value);
        }

        public float GetLockTargetDownTime(int entitas)
        {
            float result = GetDataValueWithType(entitas, ref mLockTargetDownTime, out _);
            return result;
        }
        #endregion

        #region 标记是否开始向目标进发
        public void RelockDownTarget(int entitas, bool flag)
        {
            UpdateValidWithType(entitas, ref mWillMoveToTarget, out _, flag);
        }

        public bool GetRelockDownTarget(int entitas)
        {
            bool result = GetDataValueWithType(entitas, ref mWillMoveToTarget, out _);
            return result;
        }
        #endregion

        #region 终点坐标
        public void MoveToPosition(int entitas, Vector3 pos)
        {
            UpdateValidWithType(entitas, ref mTargetPosition, out _, pos);
        }

        public Vector3 GetMoveToPosition(int entitas)
        {
            Vector3 result = GetDataValueWithType(entitas, ref mTargetPosition, out _);
            return result;
        }
        #endregion

        public void UpdateLockTargetDown(int entitas, float deltaTime)
        {
            bool flag = GetRelockDownTarget(entitas);
            if (flag) { }
            else
            {
                float max = GetLockTargetDownTimeMax(entitas);
                float time = GetLockTargetDownTime(entitas);

                //float a = mDeltaTime / UpdatesCacher.UPDATE_CACHER_TIME_SCALE;
                time += deltaTime; //Time.deltaTime;

                bool timeOut = time > max;
                if (timeOut)
                {
                    time -= max;

                    RelockDownTarget(entitas, timeOut);
                }
                else { }

                LockTargetDownTime(entitas, time);
            }
        }
    }
}