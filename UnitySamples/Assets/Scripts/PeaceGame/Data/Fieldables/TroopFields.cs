using ShipDock.Config;
using System.Collections.Generic;

namespace Peace
{
    /// <summary>
    /// �����ֶνӿ�
    /// </summary>
    public interface ITroopFields
    {
        int GetTroops();
        void SetTroops(int current, int max = -1);
    }

    /// <summary>
    /// ս��Ԫ�ؽӿ�
    /// </summary>
    public interface IBattleElementFields
    {
        int GetStamina();
        void SetStamina(int current);
    }

    /// <summary>
    /// ���ӱ��ƽӿ�
    /// </summary>
    public interface ITroopOrganization
    {
        OrganizationFields Organization { get; }
        string TroopLevelName();
        int OrganizationValue { get; }
    }

    /// <summary>
    /// ���������ֶ�
    /// </summary>
    public class TroopFields : BaseFields, ITroopFields, IBattleElementFields, ITroopOrganization
    {
        private static List<int> newIntFields;

        /// <summary>��������</summary>
        private TroopFields mOwner;
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

        public OrganizationFields Organization { get; private set; }

        public override void InitFieldsFromConfig(IConfig config = default)
        {
            base.InitFieldsFromConfig(config);

            mOwner = this;

            //ʹ��Ĭ��ֵ��ʼ�����������ֶ�����
            SetDefaultIntData(FieldsConsts.IntFieldsTroops);

            //��ʼ���������
            mVolumeGroupControl = new VolumeGroupControl();
            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;

            //�½���������
            group.AddValueVolume(FieldsConsts.F_TROOPS);

            //����ֶ�����
            FillValues(true);

            //��ʼ�����ӱ���
            int orgID = 100;
            if (config == default)
            {
                SetTroops(0, 0);
            }
            else { }

            Organization = new OrganizationFields();
            Organization.InitFieldsFromConfig(orgID);
        }

        #region ���ӱ���
        public void SetTroops(int current, int max = -1)
        {
            mVolumeGroupControl.SetVolumeData(FieldsConsts.F_TROOPS, current, ref mOwner, max);
        }

        public int GetTroops()
        {
            return mVolumeGroupControl.GetVolumeData(FieldsConsts.F_TROOPS, ref mOwner);
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
            return Organization.GetStringData(FieldsConsts.F_ORG_LEVEL_NAME).Append(" ������");
        }
        #endregion
    }
}