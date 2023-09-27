namespace ShipDock
{
    /// <summary>
    /// ������
    /// </summary>
    public class ValueVolumeGroup : IReclaim
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

        public void Reclaim()
        {
            Utils.Reclaim(ref mValueVolumes);
        }

        /// <summary>
        /// Ϊָ���������ֶ��������ֵ
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
        /// �������������ݵ�����
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
        /// ��ȡ���������ݵ�����
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="max"></param>
        public float GetVolumeMax(int fieldName)
        {
            float result = 0;
            if (mValueVolumes.IsContainsKey(fieldName))
            {
                ValueVolume volume = mValueVolumes[fieldName];
                result = volume.Max;
            }
            else { }
            return result;
        }

        /// <summary>
        /// �������������ݵĵ�ǰֵ
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="current"></param>
        /// <param name="max"></param>
        public void SetVolumeCurrent<T>(int fieldName, int current, T fields = default) where T : FieldableData
        {
            if (mValueVolumes.IsContainsKey(fieldName))
            {
                ValueVolume volume = mValueVolumes[fieldName];
                volume.Current = current;
                mValueVolumes[fieldName] = volume;
                if (fields != default)
                {
                    fields.SetIntData(fieldName, (int)volume.Current);
                }
                else { }
            }
            else { }
        }

        public float GetVolumeCurrent<T>(int fieldName, T fields = default, float defaultValue = 0) where T : FieldableData
        {
            float result = defaultValue;
            if (mValueVolumes.IsContainsKey(fieldName))
            {
                ValueVolume volume = mValueVolumes[fieldName];
                result = volume.Current;
                mValueVolumes[fieldName] = volume;
                if (fields != default)
                {
                    result = fields.GetFloatData(fieldName);
                }
                else { }
            }
            else { }
            return result;
        }
    }
}