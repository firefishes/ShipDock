using ShipDock.Config;
using ShipDock.Pooling;
using ShipDock.Tools;
using StaticConfig;
using System.Collections.Generic;

namespace Peace
{

    /// <summary>
    /// ���������ֶ�
    /// </summary>
    public class TroopFields : BaseFields, ITroopFields, IBattleElementFields, ITroopOrganization, IEquipmentProperties
    {
        private static List<int> newIntFields;

        private string mOrganizationLevelName;
        /// <summary>������ؼ�</summary>
        private VolumeGroupControl mVolumeGroupControl;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsTroops);

        public int OrganizationValue
        {
            get
            {
                return Organization.GetIntData(FieldsConsts.F_ORGANIZATION_VALUE);
            }
        }

        /// <summary>��������</summary>
        public EquipmentFields EquipmentFields { get; private set; }

        /// <summary>���ӱ���</summary>
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
                //ʹ��Ĭ��ֵ��ʼ�����������ֶ�����
                SetDefaultIntData(FieldsConsts.IntFieldsTroops);

                //��ʼ���������
                mVolumeGroupControl = new VolumeGroupControl();

                //����ֶ�����
                FillValues(true);
            }
        }

        protected override void AfterFilledData()
        {
            base.AfterFilledData();

            //��ʼ�����ӱ���
            SetTroops(0, 0);

            int orgID = 100;
            Organization = Pooling<OrganizationFields>.From();
            Organization.InitFieldsFromConfig(orgID);

            EquipmentFields = Pooling<EquipmentFields>.From();

            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;
            //�½���������
            group.AddValueVolume(FieldsConsts.F_TROOPS);
        }

        #region ���ӱ���
        public void SetTroops(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_TROOPS, current, this, max);
        }

        public int GetTroops()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_TROOPS, this);
        }
        #endregion

        #region �����;�
        public int GetStamina()
        {
            return GetIntData(FieldsConsts.F_STAMINA);
        }

        public void SetStamina(int value)
        {
            SetIntData(FieldsConsts.F_STAMINA, value);
        }
        #endregion

        #region ���Ӽ���
        public string TroopLevelName()
        {
            if (string.IsNullOrEmpty(mOrganizationLevelName))
            {
                string levelName = Organization.GetStringData(FieldsConsts.F_ORG_LEVEL_NAME);
                mOrganizationLevelName = levelName.Append(" ������");
            }
            else { }

            return mOrganizationLevelName;
        }
        #endregion

        #region ��������
        public void SetPropertiesByConfig(PeaceEquipment config)
        {
            EquipmentFields.InitFieldsFromConfig(config);
        }
        #endregion
    }
}