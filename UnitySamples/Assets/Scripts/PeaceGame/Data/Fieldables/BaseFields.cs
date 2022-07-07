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
        /// <summary>
        /// 获取连接了新的整型字段的字段列表
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="vs"></param>
        /// <returns></returns>
        protected static List<int> GetNewIntFields(ref List<int> fields, List<int> vs)
        {
            if (fields == default)
            {
                List<int> defaultFields = FieldsConsts.IntFieldsBase;

                int max = defaultFields.Count;
                fields = new List<int>(max + vs.Count);
                for (int i = 0; i < max; i++)
                {
                    fields.Add(defaultFields[i]);
                }

                max = vs.Count;
                for (int i = 0; i < max; i++)
                {
                    fields.Add(vs[i]);
                }
            }
            else { }

            return fields;
        }

        /// <summary>
        /// 获取连接了新的文本字段的字段列表
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="vs"></param>
        /// <returns></returns>
        protected static List<int> GetNewStringFields(ref List<int> fields, List<int> vs)
        {
            if (fields == default)
            {
                List<int> defaultFields = FieldsConsts.StringFieldsBase;

                int max = defaultFields.Count;
                fields = new List<int>(max + vs.Count);
                for (int i = 0; i < max; i++)
                {
                    fields.Add(defaultFields[i]);
                }

                max = vs.Count;
                for (int i = 0; i < max; i++)
                {
                    fields.Add(vs[i]);
                }
            }
            else { }

            return fields;
        }

        public bool IsInited { get; private set; }
        public override List<int> IntFieldNames { get; protected set; } = FieldsConsts.IntFieldsBase;
        public override List<int> StringFieldNames { get; protected set; } = FieldsConsts.StringFieldsBase;

        public override void Dispose()
        {
            base.Dispose();

            IsInited = false;
        }

        /// <summary>
        /// 通过配置对象进行初始化
        /// </summary>
        /// <param name="config"></param>
        public virtual void InitFieldsFromConfig(IConfig config)
        {
            int configID = config != default ? config.GetID() : -1;
            mIntFieldSource = new List<int>()
            {
                //客户端 ID
                -1,
                //服务端 ID
                -1,
                //配置 ID
                configID,
            };

            //设置名称字段数据源
            mStringFieldSource = new List<string>
            {
                GetNameFieldSource(ref config),
            };
        }

        protected override void AfterFilledData()
        {
            base.AfterFilledData();

            SetIntData(FieldsConsts.F_ID, InstanceID);
        }

        public int GetID()
        {
            return GetIntData(FieldsConsts.F_ID);
        }

        public void SetSID(int sid)
        {
            SetIntData(FieldsConsts.F_S_ID, sid);
        }

        public void SetName(string name)
        {
            SetStringData(FieldsConsts.F_NAME, name);
        }

        /// <summary>
        /// 获取名称字段的数据源
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected virtual string GetNameFieldSource(ref IConfig config)
        {
            return string.Empty;
        }

        /// <summary>
        /// 使用默认值设置整型字段数据
        /// </summary>
        /// <param name="fields"></param>
        protected void SetDefaultIntData(List<int> fields)
        {
            if (fields != default)
            {
                int max = fields.Count;
                List<int> list = new List<int>(max);
                for (int i = 0; i < max; i++)
                {
                    list.Add(default);
                }
                mIntFieldSource.Contact(list);
            }
            else { }
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
