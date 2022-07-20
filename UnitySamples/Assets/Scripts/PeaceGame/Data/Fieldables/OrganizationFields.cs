using ShipDock.Config;
using ShipDock.Pooling;
using StaticConfig;
using System.Collections.Generic;

namespace Peace
{

    /// <summary>
    /// 编制属性字段
    /// </summary>
    public class OrganizationFields : BaseFields, IOrganizationFields
    {
        private static List<int> newIntFields;
        private static List<int> newStringFields;

        public PeaceOrganizations OrganizationsConfig { get; private set; }

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsOrganization);
        public override List<int> StringFieldNames { get; protected set; } = GetNewStringFields(ref newStringFields, FieldsConsts.StringFieldsOrganization);

        public override void ToPool()
        {
            OrganizationsConfig = default;

            Pooling<OrganizationFields>.To(this);
        }

        public void InitFieldsFromConfig(int id)
        {
            Dictionary<int, PeaceOrganizations> configs = Consts.CONF_ORGANIZATIONS.GetConfigTable<PeaceOrganizations>();
            PeaceOrganizations config = configs[id];

            InitFieldsFromConfig(config);
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
                SetDefaultIntData(FieldsConsts.IntFieldsOrganization);

                mStringFieldSource.Add(string.Empty);

                FillValues(true);
            }
        }

        protected override void AfterFilledData()
        {
            base.AfterFilledData();

            OrganizationsConfig = Config as PeaceOrganizations;

            int organizationsValue = OrganizationsConfig.organaizationValue;
            int flag = OrganizationsConfig.isCommon ? 1 : 0;
            string levelName = OrganizationsConfig.levelName;

            SetIntData(FieldsConsts.F_ORGANIZATION_VALUE, organizationsValue);
            SetIntData(FieldsConsts.F_IS_BASE_ORGANIZATION, flag);

            SetStringData(FieldsConsts.F_ORG_LEVEL_NAME, levelName);
        }

        protected override string GetNameFieldSource(ref IConfig config)
        {
            return (config as PeaceOrganizations).name;
        }

        #region 编制权限值
        public void SetOrganizationValue(int value)
        {
            SetIntData(FieldsConsts.F_ORGANIZATION_VALUE, value);
        }

        public int GetOrganizationValue()
        {
            return GetIntData(FieldsConsts.F_ORGANIZATION_VALUE);
        }
        #endregion
    }
}
