using ShipDock.Config;
using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace Peace
{
    /// <summary>
    /// 基础信息字段
    /// </summary>
    public abstract class BaseFields : PeaceFields
    {

        public bool IsInited { get; private set; }
        public override List<int> IntFieldNames { get; protected set; } = FieldsConsts.IntFieldsBase;
        public override List<int> StringFieldNames { get; protected set; } = FieldsConsts.StringFieldsBase;

        public override void Dispose()
        {
            base.Dispose();

            IsInited = false;
        }

        public virtual void InitFieldsFromConfig(IConfig config)
        {
            int configID = config != default ? config.GetID() : -1;
            mIntFieldSource = new List<int>()
            {
                -1,
                -1,
                configID,
            };

            mStringFieldSource = new List<string>
            {
                GetNameFieldSource(ref config),
            };
        }

        public void SetID(int id)
        {
            SetIntData(FieldsConsts.F_ID, id);
        }

        public void SetSID(int sid)
        {
            SetIntData(FieldsConsts.F_S_ID, sid);
        }

        public void SetName(string name)
        {
            SetStringData(FieldsConsts.F_NAME, name);
        }

        protected virtual string GetNameFieldSource(ref IConfig config)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 容量组
    /// </summary>
    public class ValueVolumeGroup : IDispose
    {
        private KeyValueList<int, ValueVolume> mValueVolumes;

        public ValueVolumeGroup()
        {
            mValueVolumes = new KeyValueList<int, ValueVolume>();
        }

        public void Dispose()
        {
            Utils.Reclaim(ref mValueVolumes);
        }

        /// <summary>
        /// 为指定的数据字段添加容量值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public ValueVolume AddValueVolume(int fieldName)
        {
            ValueVolume result = new ValueVolume();
            mValueVolumes[fieldName] = result;
            return result;
        }

        /// <summary>
        /// 设置容量化数据的上限
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="max"></param>
        public void SetVolumeMax(int fieldName, int max)
        {
            if (mValueVolumes.IsContainsKey(fieldName))
            {
                ValueVolume volume = mValueVolumes[fieldName];
                //max = Math.Max(0, max);
                volume.Volumn(max);
                mValueVolumes[fieldName] = volume;
            }
            else { }
        }

        /// <summary>
        /// 获取容量化数据的上限
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="max"></param>
        public int GetVolumeMax(int fieldName)
        {
            int result = 0;
            if (mValueVolumes.IsContainsKey(fieldName))
            {
                ValueVolume volume = mValueVolumes[fieldName];
                result = volume.Max;
            }
            else { }
            return result;
        }

        /// <summary>
        /// 设置容量化数据的当前值
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="current"></param>
        /// <param name="max"></param>
        public void SetVolumeCurrent<T>(int fieldName, int current, ref T fields) where T : BaseFields
        {
            if (mValueVolumes.IsContainsKey(fieldName))
            {
                ValueVolume volume = mValueVolumes[fieldName];
                volume.Current = current;
                mValueVolumes[fieldName] = volume;
                fields.SetIntData(fieldName, volume.Current);
            }
            else { }
        }
    }

    public struct ValueVolume
    {
        private int mMax;
        private int mCurrent;
        private int mScaler;
        private bool mIsRestrict;

        public int Max
        {
            get
            {
                return mScaler > 0 ? mMax / mScaler : mMax;
            }
            set
            {
                mMax = value * mScaler;
            }
        }

        public int Current
        {
            get
            {
                return mScaler > 0 ? mCurrent / mScaler : mCurrent;
            }
            set
            {
                mCurrent = value * mScaler;

                if (mIsRestrict)
                {
                    mCurrent = Math.Max(0, mCurrent);
                    mCurrent = Math.Min(mCurrent, mMax);
                }
                else { }
            }
        }

        public void Volumn(int maxValue, int scalerValue = 1, bool isRestrict = true)
        {
            mScaler = scalerValue;
            mMax = maxValue * mScaler;
            mCurrent = mMax;
            mIsRestrict = isRestrict;
        }
    }
}
