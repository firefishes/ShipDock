using System.Collections;
using System.Collections.Generic;
using ShipDock.Config;
using StaticConfig;
using UnityEngine;

namespace Peace
{
    public class OrganizationFields : BaseFields
    {
        private static List<int> newIntFields;
        private static List<int> newStringFields;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsOrganization);
        public override List<int> StringFieldNames { get; protected set; } = GetNewStringFields(ref newStringFields, FieldsConsts.StringFieldsOrganization);

        public void InitFieldsFromConfig(int id)
        {
            Dictionary<int, PeaceOrganizations> configs = Consts.CONF_ORGANIZATIONS.GetConfigTable<PeaceOrganizations>();
            PeaceOrganizations config = configs[id];

            InitFieldsFromConfig(config);
        }

        public override void InitFieldsFromConfig(IConfig config)
        {
            base.InitFieldsFromConfig(config);

            PeaceOrganizations item = config as PeaceOrganizations;

            mIntFieldSource.Add(item.organaizationValue);
            mIntFieldSource.Add(item.isCommon ? 1 : 0);

            mStringFieldSource.Add(item.levelName);

            FillValues(true);
        }

        protected override string GetNameFieldSource(ref IConfig config)
        {
            return (config as PeaceOrganizations).name;
        }
    }
}
