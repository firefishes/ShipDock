using ShipDock.Datas;
using ShipDock.Interfaces;

namespace ShipDock.Tools
{
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

        public void Reset()
        {
            Utils.Reclaim(ref mValueVolumes, false);
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
        public void SetVolumeCurrent<T>(int fieldName, int current, ref T fields) where T : FieldableData
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
}