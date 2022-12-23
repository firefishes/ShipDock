using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.ECS
{
    public class WorldMovementComponent : DataComponent<LogicData>
    {
        private float[] mMoveSpeeds;
        private float[] mMoveSpeedMaxs;
        private float[] mMoveSpeedRatios;
        private float[] mAaccelerationTimes;
        private float[] mAaccelerations;
        private bool[] mAllowMoves;
        private Vector3[] mPositions;
        private Vector3[] mLocalPositions;
        private Vector3[] mForwards;
        private Vector3[] mTrackingPositions;
        private Vector3[] mMoveDirections;
        private Quaternion[] mRotations;

        protected override void DropData(ref ILogicData target)
        {
            base.DropData(ref target);

            int index = target.DataIndex;
            mMoveSpeeds[index] = default;
            mMoveSpeedMaxs[index] = default;
            mMoveSpeedRatios[index] = default;
            mAaccelerationTimes[index] = default;
            mAaccelerations[index] = default;
            mAllowMoves[index] = default;
            mPositions[index] = default;
            mLocalPositions[index] = default;
            mForwards[index] = default;
            mTrackingPositions[index] = default;
            mMoveDirections[index] = default;
            mRotations[index] = default;
        }

        protected override void OnResetSuccessive(bool clearOnly = false)
        {
            base.OnResetSuccessive(clearOnly);

            Utils.Reclaim(ref mRotations, clearOnly);
            Utils.Reclaim(ref mMoveSpeeds, clearOnly);
            Utils.Reclaim(ref mMoveSpeedMaxs, clearOnly);
            Utils.Reclaim(ref mMoveSpeedRatios, clearOnly);
            Utils.Reclaim(ref mPositions, clearOnly);
            Utils.Reclaim(ref mLocalPositions, clearOnly);
            Utils.Reclaim(ref mForwards, clearOnly);
            Utils.Reclaim(ref mTrackingPositions, clearOnly);
            Utils.Reclaim(ref mMoveDirections, clearOnly);
            Utils.Reclaim(ref mAaccelerationTimes, clearOnly);
            Utils.Reclaim(ref mAaccelerations, clearOnly);
            Utils.Reclaim(ref mAllowMoves, clearOnly);
        }

        protected override void UpdateDataStretch(int dataSize)
        {
            base.UpdateDataStretch(dataSize);

            Utils.Stretch(ref mRotations, dataSize);
            Utils.Stretch(ref mMoveSpeeds, dataSize);
            Utils.Stretch(ref mMoveSpeedMaxs, dataSize);
            Utils.Stretch(ref mMoveSpeedRatios, dataSize);
            Utils.Stretch(ref mPositions, dataSize);
            Utils.Stretch(ref mLocalPositions, dataSize);
            Utils.Stretch(ref mForwards, dataSize);
            Utils.Stretch(ref mTrackingPositions, dataSize);
            Utils.Stretch(ref mMoveDirections, dataSize);
            Utils.Stretch(ref mAaccelerationTimes, dataSize);
            Utils.Stretch(ref mAaccelerations, dataSize);
            Utils.Stretch(ref mAllowMoves, dataSize);
        }

        public void SyncMovement(int entitas, Transform transform, bool isResetSpeeds = false)
        {
            Position(entitas, transform.position);
            LocalPosition(entitas, transform.localPosition);
            Rotation(entitas, transform.rotation);
            Forward(entitas, transform.forward);
            MoveDirection(entitas, Vector3.zero);

            if (isResetSpeeds)
            {
                MoveSpeed(entitas, 0f);
                MoveSpeedMax(entitas, 0f);
                MoveSpeedRatio(entitas, 1f);
            }
            else { }

        }

        #region 旋转四元数
        public void Rotation(int entitas, Quaternion value)
        {
            UpdateValidWithType(entitas, ref mRotations, out _, value);
        }

        public Quaternion GetRotation(int entitas)
        {
            Quaternion result = GetDataValueWithType(entitas, ref mRotations, out _);
            return result;
        }
        #endregion

        #region 位移速度
        public void MoveSpeed(int entitas, float value, bool isInterdict = true)
        {
            if (isInterdict)
            {
                float maxSpeed = GetMoveSpeedMax(entitas);

                if (maxSpeed > 0f)
                {
                    value = Mathf.Min(value, maxSpeed);
                }
                else { }
            }
            else { }

            UpdateValidWithType(entitas, ref mMoveSpeeds, out _, value);
        }

        public float GetMoveSpeed(int entitas)
        {
            float result = GetDataValueWithType(entitas, ref mMoveSpeeds, out _);
            return result;
        }
        #endregion

        #region 最大位移速度
        public void MoveSpeedMax(int entitas, float value)
        {
            UpdateValidWithType(entitas, ref mMoveSpeedMaxs, out _, value);
        }

        public float GetMoveSpeedMax(int entitas)
        {
            float result = GetDataValueWithType(entitas, ref mMoveSpeedMaxs, out _);
            return result;
        }
        #endregion

        #region 移动步幅
        public void MoveSpeedRatio(int entitas, float value)
        {
            UpdateValidWithType(entitas, ref mMoveSpeedRatios, out _, value);
        }

        public float GetMoveSpeedRatio(int entitas)
        {
            float result = GetDataValueWithType(entitas, ref mMoveSpeedRatios, out _);
            return result;
        }
        #endregion

        #region 当前坐标
        public void LocalPosition(int entitas, Vector3 value)
        {
            UpdateValidWithType(entitas, ref mLocalPositions, out _, value);
        }

        public Vector3 GetLocalPosition(int entitas)
        {
            Vector3 result = GetDataValueWithType(entitas, ref mLocalPositions, out _);
            return result;
        }
        #endregion

        #region 世界坐标
        public void Position(int entitas, Vector3 value)
        {
            UpdateValidWithType(entitas, ref mPositions, out _, value);
        }

        public Vector3 GetPosition(int entitas)
        {
            Vector3 result = GetDataValueWithType(entitas, ref mPositions, out _);
            return result;
        }
        #endregion

        #region 正方向
        public void Forward(int entitas, Vector3 value)
        {
            UpdateValidWithType(entitas, ref mForwards, out _, value);
        }

        public Vector3 GetForward(int entitas)
        {
            Vector3 result = GetDataValueWithType(entitas, ref mForwards, out _);
            return result;
        }
        #endregion

        #region 导向坐标
        public Vector3 TrackingPosition(int entitas, Vector3 value)
        {
            Vector3 result = Vector3.zero;
            UpdateValidWithType(entitas, ref mTrackingPositions, out int dataIndex, value);

            if (dataIndex != int.MaxValue)
            {
                Vector3 pos = mPositions[dataIndex];
                result = value - pos;
            }
            else { }

            return result;
        }

        public Vector3 GetTrackingPosition(int entitas)
        {
            Vector3 result = GetDataValueWithType(entitas, ref mTrackingPositions, out _);
            return result;
        }
        #endregion

        #region 位移方向
        public void MoveDirection(int entitas, Vector3 value)
        {
            UpdateValidWithType(entitas, ref mMoveDirections, out _, value);
        }

        public Vector3 GetMoveDirection(int entitas, bool isNormalized = true)
        {
            Vector3 result = GetDataValueWithType(entitas, ref mMoveDirections, out _);
            return isNormalized ? result.normalized : result;
        }
        #endregion

        #region 加速时间
        public void AaccelerationTime(int entitas, float time)
        {
            UpdateValidWithType(entitas, ref mAaccelerationTimes, out _, time);
        }

        public float GetAaccelerationTimes(int entitas)
        {
            float result = GetDataValueWithType(entitas, ref mAaccelerationTimes, out _);
            return result;
        }
        #endregion

        #region 加速度
        public void Aacceleration(int entitas, float time)
        {
            UpdateValidWithType(entitas, ref mAaccelerationTimes, out _, time);
        }

        public float GetAacceleration(int entitas)
        {
            float result = GetDataValueWithType(entitas, ref mAaccelerationTimes, out _);
            return result;
        }
        #endregion

        #region 是否可移动
        public void ShouldMove(int entitas, bool flag)
        {
            UpdateValidWithType(entitas, ref mAllowMoves, out _, flag);
        }

        public bool ShouldMove(int entitas)
        {
            bool result = GetDataValueWithType(entitas, ref mAllowMoves, out _);
            return result;
        }
        #endregion
    }
}
