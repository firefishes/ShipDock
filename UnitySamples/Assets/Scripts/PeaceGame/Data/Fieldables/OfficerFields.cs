using System.Collections.Generic;
using ShipDock.Config;
using ShipDock.Pooling;
using StaticConfig;

namespace Peace
{

    /// <summary>
    /// ���������ֶ�
    /// </summary>
    public class OfficerFields : BaseFields, IOfficerFields
    {
        private static List<int> newIntFields;
        private static List<int> newStringFields;

        private int mMilitaryRank;
        private int mCommandRight;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsOfficer);
        public override List<int> StringFieldNames { get; protected set; } = GetNewStringFields(ref newStringFields, FieldsConsts.StringFieldsOfficer);

        public OrganizationFields Organization { get; private set; }

        /// <summary>
        /// ����ָ��Ȩ��
        /// </summary>
        public int OrganizationValue
        {
            get
            {
                return Organization.GetIntData(FieldsConsts.F_ORGANIZATION_VALUE);
            }
        }

        public PeaceOrganizations OrganizationsConfig => throw new System.NotImplementedException();

        /// <summary>
        /// ��������
        /// </summary>
        public RoleFields RoleFields { get; private set; }

        public override void ToPool()
        {
            Pooling<OfficerFields>.To(this);
        }

        public override void InitFieldsFromConfig(IConfig config)
        {
            base.InitFieldsFromConfig(config);

            if (IsInited) { }
            else
            {
                //SetOrganization(100);

                PeaceOfficer peaceOfficer = config as PeaceOfficer;

                RoleFields = new RoleFields();
                RoleFields.InitFieldsFromConfig(peaceOfficer);

                mIntFieldSource.Add(peaceOfficer.gender);
                mIntFieldSource.Add(0);
                mIntFieldSource.Add(0);

                //����ֶ�����
                FillValues(true);
            }
        }

        #region ���Ρ���ְ�����Ӽ�����������Ȩ��ֵ
        private void SetOrganization(int id)
        {
            if (IsInited) { }
            else
            {
                Organization = new OrganizationFields();
            }

            Organization.InitFieldsFromConfig(id);
        }

        public void SetMilitaryRank(int id)
        {
            const int startID = 100;

            SetOrganization(startID + id - 1);

            mMilitaryRank = id;// Organization.OrganizationsConfig.militaryRank;
        }

        public int GetMilitaryRank()
        {
            return mMilitaryRank;
        }

        public string GetStripes()
        {
            return TroopLevelName();
        }

        public string TroopLevelName()
        {
            return Organization.OrganizationsConfig.stripes;
        }

        public void SetOrganizationValue(int value)
        {
            Organization.SetOrganizationValue(value);
        }

        public int GetOrganizationValue()
        {
            return Organization.GetOrganizationValue();
        }
        #endregion
    }
}