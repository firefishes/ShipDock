using UnityEngine;

namespace ShipDock
{
    public struct Movement : IECSData
    {
        public Vector3 positionPrev;
        public Vector3 direction;

        private ValueVolume mMoveSpeed;
        private ValueVolume rotateSpeed;

        public Vector3 Scale { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 RotateEuler { get; private set; }
        public Quaternion Rotation { get; private set; }
        public bool IsChanged { get; set; }
        public int id;

        public void SyncFromTransform(Vector3 pos, Vector3 scaleValue, Quaternion rotationValue)
        {
            Position = pos;
            Scale = scaleValue;
            Rotation = rotationValue;
            RotateEuler = Rotation.eulerAngles;
        }

        public void SyncSpeed(float speed, float max)
        {
            mMoveSpeed.Volumn(max);
            mMoveSpeed.Current = speed;
        }

        public void SetSpeed(float speed)
        {
            IsChanged = true;
            mMoveSpeed.Current = speed;
        }

        public float GetSpeed()
        {
            return mMoveSpeed.Current;
        }

        public float GetSpeedMax()
        {
            return mMoveSpeed.Max;
        }

        public void SyncRotateSpeed(float speed, float max)
        {
            rotateSpeed.Volumn(max);
            rotateSpeed.Current = speed;
        }

        public void SetRotateSpeed(float speed)
        {
            IsChanged = true;
            rotateSpeed.Current = speed;
        }

        public float GetRotateSpeed()
        {
            return rotateSpeed.Current;
        }

        public float GetRotateSpeedMax()
        {
            return rotateSpeed.Max;
        }

        public void SetPosition(Vector3 pos)
        {
            IsChanged = true;
            Position = pos;
        }
    }
}