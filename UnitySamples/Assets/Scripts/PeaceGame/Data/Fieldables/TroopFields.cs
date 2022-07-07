using ShipDock.Config;
using System.Collections.Generic;

namespace Peace
{
    /// <summary>
    /// 部队字段接口
    /// </summary>
    public interface ITroopFields
    {
        int GetTroops();
        void SetTroops(int current, int max = -1);
    }

    /// <summary>
    /// 战场元素接口
    /// </summary>
    public interface IBattleElementFields
    {
        int GetStamina();
        void SetStamina(int current);
    }

    /// <summary>
    /// 部队编制接口
    /// </summary>
    public interface ITroopOrganization
    {
        OrganizationFields Organization { get; }
        string TroopLevelName();
        int OrganizationValue { get; }
    }

    /// <summary>
    /// 部队属性字段
    /// </summary>
    public class TroopFields : BaseFields, ITroopFields, IBattleElementFields, ITroopOrganization
    {
        private static List<int> newIntFields;

        /// <summary>归属引用</summary>
        private TroopFields mOwner;
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

        public OrganizationFields Organization { get; private set; }

        public override void InitFieldsFromConfig(IConfig config = default)
        {
            base.InitFieldsFromConfig(config);

            mOwner = this;

            //使用默认值初始化所有整型字段数据
            SetDefaultIntData(FieldsConsts.IntFieldsTroops);

            //初始化容量组件
            mVolumeGroupControl = new VolumeGroupControl();
            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;

            //新建兵力容量
            group.AddValueVolume(FieldsConsts.F_TROOPS);

            //填充字段数据
            FillValues(true);

            //初始化部队编制
            int orgID = 100;
            if (config == default)
            {
                SetTroops(0, 0);
            }
            else { }

            Organization = new OrganizationFields();
            Organization.InitFieldsFromConfig(orgID);
        }

        #region 部队兵力
        public void SetTroops(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_TROOPS, current, ref mOwner, max);
        }

        public int GetTroops()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_TROOPS, ref mOwner);
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
            return Organization.GetStringData(FieldsConsts.F_ORG_LEVEL_NAME).Append(" 级部队");
        }
        #endregion
    }
}