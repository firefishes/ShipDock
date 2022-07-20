using ShipDock.Config;
using ShipDock.Pooling;
using ShipDock.Tools;
using StaticConfig;
using System.Collections.Generic;

namespace Peace
{

    /// <summary>
    /// 部队属性字段
    /// </summary>
    public class TroopFields : BaseFields, ITroopFields, IBattleElementFields, ITroopOrganization, IEquipmentProperties
    {
        private static List<int> newIntFields;

        private string mOrganizationLevelName;
        /// <summary>容量组控件</summary>
        private VolumeGroupControl mVolumeGroupControl;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsTroops);

        public int OrganizationValue
        {
            get
            {
                return Organization.GetIntData(FieldsConsts.F_ORGANIZATION_VALUE);
            }
        }

        /// <summary>人物属性</summary>
        public EquipmentFields EquipmentFields { get; private set; }

        /// <summary>部队编制</summary>
        public OrganizationFields Organization { get; private set; }

        public override void ToPool()
        {
            mVolumeGroupControl?.Reset();

            Organization?.ToPool();
            EquipmentFields?.ToPool();

            Organization = default;
            EquipmentFields = default;

            Pooling<TroopFields>.To(this);
        }

        protected override void Init()
        {
            mOrganizationLevelName = string.Empty;

            if (IsInited)
            {
                IDAdvanced();
                AfterFilledData();
            }
            else
            {
                //使用默认值初始化所有整型字段数据
                SetDefaultIntData(FieldsConsts.IntFieldsTroops);

                //初始化容量组件
                mVolumeGroupControl = new VolumeGroupControl();

                //填充字段数据
                FillValues(true);
            }
        }

        protected override void AfterFilledData()
        {
            base.AfterFilledData();

            //初始化部队编制
            SetTroops(0, 0);

            int orgID = 100;
            Organization = Pooling<OrganizationFields>.From();
            Organization.InitFieldsFromConfig(orgID);

            EquipmentFields = Pooling<EquipmentFields>.From();

            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;
            //新建兵力容量
            group.AddValueVolume(FieldsConsts.F_TROOPS);
        }

        #region 部队兵力
        public void SetTroops(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_TROOPS, current, this, max);
        }

        public int GetTroops()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_TROOPS, this);
        }
        #endregion

        #region 部队耐久
        public int GetStamina()
        {
            return GetIntData(FieldsConsts.F_STAMINA);
        }

        public void SetStamina(int value)
        {
            SetIntData(FieldsConsts.F_STAMINA, value);
        }
        #endregion

        #region 部队级别
        public string TroopLevelName()
        {
            if (string.IsNullOrEmpty(mOrganizationLevelName))
            {
                string levelName = Organization.GetStringData(FieldsConsts.F_ORG_LEVEL_NAME);
                mOrganizationLevelName = levelName.Append(" 级部队");
            }
            else { }

            return mOrganizationLevelName;
        }
        #endregion

        #region 部队属性
        public void SetPropertiesByConfig(PeaceEquipment config)
        {
            EquipmentFields.InitFieldsFromConfig(config);
        }
        #endregion
    }
}