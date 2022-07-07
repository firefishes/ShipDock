using System.Collections.Generic;
using ShipDock.Config;
using ShipDock.Interfaces;
using ShipDock.Tools;

namespace Peace
{
    /// <summary>
    /// ���������ֶ�
    /// </summary>
    public class LegionFields : BaseFields, ITroopFields
    {
        private static List<int> newIntFields;

        /// <summary>��������</summary>
        private LegionFields mOwner;
        /// <summary>������Դ</summary>
        private ResourcesFields mResources;
        /// <summary>������ؼ�</summary>
        private VolumeGroupControl mVolumeGroupControl;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsLegion);

        public override void InitFieldsFromConfig(IConfig config)
        {
            base.InitFieldsFromConfig(config);

            mOwner = this;

            mResources = new ResourcesFields();

            mVolumeGroupControl = new VolumeGroupControl();

            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;
            group.AddValueVolume(FieldsConsts.F_TROOPS);
            group.AddValueVolume(FieldsConsts.F_CREDIT_POINT);
            group.AddValueVolume(FieldsConsts.F_METAL);
            group.AddValueVolume(FieldsConsts.F_ENERGY);
            group.AddValueVolume(FieldsConsts.F_SUPPLIES);

            FillValues(true);
        }

        public override void Dispose()
        {
            base.Dispose();

            mVolumeGroupControl?.Dispose();

            mOwner = default;
        }

        #region ���ų�����Դ
        public void SetCreditPoint(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_CREDIT_POINT, current, ref mResources, max);
        }

        public int GetCreditPoint()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_CREDIT_POINT, ref mResources);
        }

        public void SetMetal(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_METAL, current, ref mResources, max);
        }

        public int GetMetal()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_METAL, ref mResources);
        }

        public void SetEnergy(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_ENERGY, current, ref mResources, max);
        }

        public int GetEnergy()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_ENERGY, ref mResources);
        }

        public void SetSupplies(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_SUPPLIES, current, ref mResources, max);
        }

        public int GetSupplies()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_SUPPLIES, ref mResources);
        }
        #endregion

        #region ���ű���
        public void SetTroops(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_TROOPS, current, ref mOwner, max);
        }

        public int GetTroops()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_TROOPS, ref mOwner);
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
            SetIntData(FieldsConsts.F_LEGION_OFFICER_ID, officerID);
        }

        public void SetLegionHeadquartersID(int headquartersID)
        {
            SetIntData(FieldsConsts.F_HEADQUARTERS_ID, headquartersID);
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
        public void SetVolumeData<T>(int fieldName, int current, ref T baseFields, int max = -1) where T : BaseFields
        {
            if (max >= 0)
            {
                VolumeGroup.SetVolumeMax(fieldName, max);
            }
            else { }

            VolumeGroup.SetVolumeCurrent(fieldName, current, ref baseFields);
        }

        /// <summary>
        /// ��ȡ�����ֶε�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="baseFields"></param>
        /// <returns></returns>
        public int GetVolumeData<T>(int fieldName, ref T baseFields) where T : BaseFields
        {
            return baseFields.GetIntData(fieldName);
        }
        #endregion
    }
}
