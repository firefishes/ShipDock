using System.Collections.Generic;
using ShipDock.Config;
using ShipDock.Interfaces;
using ShipDock.Pooling;
using ShipDock.Tools;

namespace Peace
{

    /// <summary>
    /// 军团属性字段
    /// </summary>
    public class LegionFields : BaseFields, ITroopFields, ILegionFields
    {
        private static List<int> newIntFields;

        /// <summary>归属引用</summary>
        private LegionFields mOwner;
        /// <summary>军团资源</summary>
        private ResourcesFields mResources;
        /// <summary>容量组控件</summary>
        private VolumeGroupControl mVolumeGroupControl;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsLegion);

        protected override void Purge()
        {
            base.Purge();

            mVolumeGroupControl?.Reset();
        }

        public override void ToPool()
        {
            Pooling<LegionFields>.To(this);
        }

        public override void InitFieldsFromConfig(IConfig config)
        {
            base.InitFieldsFromConfig(config);

            if (IsInited) { }
            else
            {
                mOwner = this;

                mResources = new ResourcesFields();

                mVolumeGroupControl = new VolumeGroupControl();

                FillValues(true);
            }

            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;
            group.AddValueVolume(FieldsConsts.F_TROOPS);
            group.AddValueVolume(FieldsConsts.F_CREDIT_POINT);
            group.AddValueVolume(FieldsConsts.F_METAL);
            group.AddValueVolume(FieldsConsts.F_ENERGY);
            group.AddValueVolume(FieldsConsts.F_SUPPLIES);
        }

        public override void Dispose()
        {
            base.Dispose();

            mVolumeGroupControl?.Dispose();

            mResources?.Dispose();
            mResources = default;
            mOwner = default;
        }

        #region 军团常规资源
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

        #region 军团兵力
        public void SetTroops(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_TROOPS, current, ref mOwner, max);
        }

        public int GetTroops()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_TROOPS, ref mOwner);
        }
        #endregion

        #region 军团威望
        public void SetPrestige(int value)
        {
            SetIntData(FieldsConsts.F_PRESTIGE, value);
        }

        public int GetPrestige()
        {
            return GetIntData(FieldsConsts.F_PRESTIGE);
        }
        #endregion

        #region 军团军官 ID 和 基地 ID
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
        /// <summary>军团数据容量组</summary>
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

        #region 容量控制
        /// <summary>
        /// 设置数据字段的容量
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
        /// 获取数据字段的容量
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
