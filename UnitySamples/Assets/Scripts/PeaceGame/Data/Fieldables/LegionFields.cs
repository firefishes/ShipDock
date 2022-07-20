using System.Collections.Generic;
using ShipDock.Config;
using ShipDock.Interfaces;
using ShipDock.Pooling;
using ShipDock.Tools;

namespace Peace
{

    /// <summary>
    /// ���������ֶ�
    /// </summary>
    public class LegionFields : BaseFields, ITroopFields, ILegionFields
    {
        private static List<int> newIntFields;

        /// <summary>������Դ</summary>
        private ResourcesFields mResources;
        /// <summary>������ؼ�</summary>
        private VolumeGroupControl mVolumeGroupControl;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsLegion);

        public override void Dispose()
        {
            base.Dispose();

            mVolumeGroupControl?.Dispose();

            mResources?.Dispose();
            mResources = default;
        }

        protected override void Purge()
        {
            base.Purge();

            mVolumeGroupControl?.Reset();
        }

        public override void ToPool()
        {
            mResources?.ToPool();

            Pooling<LegionFields>.To(this);
        }

        protected override void Init()
        {
            if (IsInited)
            {
                IDAdvanced();
                AfterFilledData();
            }
            else
            {
                mVolumeGroupControl = new VolumeGroupControl();

                SetDefaultIntData(FieldsConsts.IntFieldsLegion);

                FillValues(true);
            }
        }

        protected override void AfterFilledData()
        {
            base.AfterFilledData();

            mResources = Pooling<ResourcesFields>.From();

            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;
            group.AddValueVolume(FieldsConsts.F_TROOPS);
            group.AddValueVolume(FieldsConsts.F_CREDIT_POINT);
            group.AddValueVolume(FieldsConsts.F_METAL);
            group.AddValueVolume(FieldsConsts.F_ENERGY);
            group.AddValueVolume(FieldsConsts.F_SUPPLIES);
        }

        #region ���ų�����Դ
        public void SetCreditPoint(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_CREDIT_POINT, current, mResources, max);
        }

        public int GetCreditPoint()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_CREDIT_POINT, mResources);
        }

        public void SetMetal(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_METAL, current, mResources, max);
        }

        public int GetMetal()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_METAL, mResources);
        }

        public void SetEnergy(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_ENERGY, current, mResources, max);
        }

        public int GetEnergy()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_ENERGY, mResources);
        }

        public void SetSupplies(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_SUPPLIES, current, mResources, max);
        }

        public int GetSupplies()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_SUPPLIES, mResources);
        }
        #endregion

        #region ���ű���
        public void SetTroops(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_TROOPS, current, this, max);
        }

        public int GetTroops()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_TROOPS, this);
        }
        #endregion

        #region ��������
        public void SetPrestige(int value)
        {
            SetIntData(FieldsConsts.F_PRESTIGE, value);
        }

        public int GetPrestige()
        {
            return GetIntData(FieldsConsts.F_PRESTIGE);
        }
        #endregion

        #region ���ž��� ID �� ���� ID
        public void SetLegionOfficerID(int officerID)
        {
        }

        public void SetLegionHeadquartersID(int headquartersID)
        {
        }
        #endregion
    }

    public class VolumeGroupControl : IDispose
    {
        /// <summary>��������������</summary>
        public ValueVolumeGroup VolumeGroup { get; private set; }

        public VolumeGroupControl()
        {
            VolumeGroup = new ValueVolumeGroup();
        }

        public void Reset()
        {
            VolumeGroup?.Reset();
        }

        public void Dispose()
        {
            VolumeGroup?.Dispose();
        }

        #region ��������
        /// <summary>
        /// ���������ֶε�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="current"></param>
        /// <param name="baseFields"></param>
        /// <param name="max"></param>
        public void SetVolumeData<T>(int fieldName, int current, T baseFields = default, int max = -1) where T : BaseFields
        {
            if (max >= 0)
            {
                VolumeGroup.SetVolumeMax(fieldName, max);
            }
            else { }

            VolumeGroup.SetVolumeCurrent(fieldName, current, baseFields);
        }

        /// <summary>
        /// ��ȡ�����ֶε�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="baseFields"></param>
        /// <returns></returns>
        public int GetVolumeData<T>(int fieldName, T baseFields = default, int defaultValue = 0) where T : BaseFields
        {
            return VolumeGroup.GetVolumeCurrent(fieldName, baseFields, defaultValue);
        }
        #endregion
    }
}
