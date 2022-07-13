using ShipDock.Config;
using ShipDock.Pooling;
using ShipDock.Tools;
using System.Collections.Generic;

namespace Peace
{

    /// <summary>
    /// ���������ֶ�
    /// </summary>
    public class TroopFields : BaseFields, ITroopFields, IBattleElementFields, ITroopOrganization
    {
        private static List<int> newIntFields;

        private string mOrganizationLevelName;
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
        
        /// <summary>���ӱ���</summary>
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

                //ʹ��Ĭ��ֵ��ʼ�����������ֶ�����
                SetDefaultIntData(FieldsConsts.IntFieldsTroops);

                //��ʼ���������
                mVolumeGroupControl = new VolumeGroupControl();

                //����ֶ�����
                FillValues(true); 

                //��ʼ�����ӱ���
                if (config == default)
                {
                    SetTroops(0, 0);
                }
                else { }

                Organization = new OrganizationFields();
            }

            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;

            //�½���������
            group.AddValueVolume(FieldsConsts.F_TROOPS);

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
            if (string.IsNullOrEmpty(mOrganizationLevelName))
            {
                string levelName = Organization.GetStringData(FieldsConsts.F_ORG_LEVEL_NAME);
                mOrganizationLevelName = levelName.Append(" ������");
            }
            else { }

            return mOrganizationLevelName;
        }
        #endregion
    }
}