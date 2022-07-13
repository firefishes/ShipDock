using ShipDock.Config;
using ShipDock.Pooling;
using ShipDock.Tools;
using System.Collections.Generic;

namespace Peace
{

    /// <summary>
    /// 部队属性字段
    /// </summary>
    public class TroopFields : BaseFields, ITroopFields, IBattleElementFields, ITroopOrganization
    {
        private static List<int> newIntFields;

        private string mOrganizationLevelName;
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
        
        /// <summary>部队编制</summary>
        public OrganizationFields Organization { get; private set; }

        public override void ToPool()
        {
            Pooling<TroopFields>.To(this);
        }

        public override void InitFieldsFromConfig(IConfig config = default)
        {
            base.InitFieldsFromConfig(config);

            int orgID = 100;
            mOrganizationLevelName = string.Empty;

            if (IsInited) { }
            else
            {
                mOwner = this;

                //使用默认值初始化所有整型字段数据
                SetDefaultIntData(FieldsConsts.IntFieldsTroops);

                //初始化容量组件
                mVolumeGroupControl = new VolumeGroupControl();

                //填充字段数据
                FillValues(true); 

                //初始化部队编制
                if (config == default)
                {
                    SetTroops(0, 0);
                }
                else { }

                Organization = new OrganizationFields();
            }

            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;

            //新建兵力容量
            group.AddValueVolume(FieldsConsts.F_TROOPS);

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
            if (string.IsNullOrEmpty(mOrganizationLevelName))
            {
                string levelName = Organization.GetStringData(FieldsConsts.F_ORG_LEVEL_NAME);
                mOrganizationLevelName = levelName.Append(" 级部队");
            }
            else { }

            return mOrganizationLevelName;
        }
        #endregion
    }
}