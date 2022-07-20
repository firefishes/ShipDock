using ShipDock.Datas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    /// <summary>
    /// �����ֶγ���
    /// </summary>
    public static class FieldsConsts
    {
        /// <summary>���ڿͻ��˵� ID</summary>
        public const int F_ID = 1;
        /// <summary>���ڷ���˵� ID</summary>
        public const int F_S_ID = 2;
        /// <summary>����</summary>
        public const int F_NAME = 3;
        /// <summary>���� ID</summary>
        public const int F_CONF_ID = 4;

        /// <summary>�ܲ� ID</summary>
        //public const int F_HEADQUARTERS_ID = 5;
        /// <summary>����ָ�ӹ� ID</summary>
        //public const int F_LEGION_OFFICER_ID = 6;

        /// <summary>���õ�</summary>
        public const int F_CREDIT_POINT = 7;
        /// <summary>����</summary>
        public const int F_METAL = 8;
        /// <summary>��Դ</summary>
        public const int F_ENERGY = 9;
        /// <summary>����</summary>
        public const int F_SUPPLIES = 10;

        /// <summary>���ӱ��</summary>
        public const int F_TROOPS_SERIAL_NUM = 11;
        /// <summary>��������</summary>
        public const int F_TROOPS_TYPE = 12;
        /// <summary>���ӳ��� ID</summary>
        public const int F_TROOPS_OFFICER_ID = 13;
        /// <summary>ս��</summary>
        public const int F_TROOPS_ANNIHILATION = 14;
        /// <summary>����</summary>
        public const int F_TROOPS = 15;
        /// <summary>����</summary>
        public const int F_CRACK = 16;
        /// <summary>��־</summary>
        public const int F_WILL = 17;
        /// <summary>����</summary>
        public const int F_PROVISIONS = 18;
        /// <summary>��װ�� ID</summary>
        public const int F_MAIN_EQUIPMENT_ID = 19;

        /// <summary>���ӱ�������</summary>
        public const int F_ORG_LEVEL_NAME = 20;
        /// <summary>Ȩ��ֵ</summary>
        public const int F_ORGANIZATION_VALUE = 21;
        /// <summary>�Ƿ��������</summary>
        public const int F_IS_BASE_ORGANIZATION = 22;

        /// <summary>����</summary>
        public const int F_FULL_NAME = 23;
        /// <summary>�Ա�</summary>
        public const int F_GENDER = 24;
        /// <summary>�ҳ�</summary>
        public const int F_LOYAL = 25;
        /// <summary>ս��</summary>
        public const int F_ACHIEVEMENT = 26;
        /// <summary>����</summary>
        //public const int F_MILITARY_RANK = 27;
        /// <summary>��ְ</summary>
        //public const int F_STRIPES = 28;
        /// <summary>ָ��Ȩ</summary>
        //public const int F_COMMAND_RIGHT = 29;

        /// <summary>ָ��</summary>
        public const int F_ROLE_COMMAND = 30;
        /// <summary>˼ά</summary>
        public const int F_ROLE_THINKING = 31;
        /// <summary>�鱨</summary>
        public const int F_ROLE_INFORMATION = 32;
        /// <summary>��ѧ</summary>
        public const int F_ROLE_SCIENTIFIC = 33;
        /// <summary>����</summary>
        public const int F_ROLE_SOCIAL = 34;

        /// <summary>�ƻ�</summary>
        public const int F_RUIN = 35;
        /// <summary>ɱ��</summary>
        public const int F_KILL = 36;
        /// <summary>����/summary>
        public const int F_AIR_DEF = 37;
        /// <summary>ǿ��/summary>
        public const int F_INTENSIFY = 38;
        /// <summary>����ս/summary>
        public const int F_ECM_WAR = 39;
        /// <summary>����/summary>
        public const int F_TRANSPORT = 40;
        /// <summary>�Ƽ�/summary>
        public const int F_SCIENCE_TECH = 41;

        /// <summary>����/summary>
        public const int F_INVISIBLE = 42;
        /// <summary>ɨ��/summary>
        public const int F_SCAN = 43;
        /// <summary>̽��/summary>
        public const int F_PROBE = 44;
        /// <summary>��Ұ/summary>
        public const int F_VISION = 45;
        /// <summary>��Ұ/summary>
        public const int F_STORAGE = 46;
        /// <summary>�ܺ�/summary>
        public const int F_ENERY_CONSUMPTION = 47;

        /// <summary>����ʱ��/summary>
        public const int F_BUILD_TIME = 48;
        /// <summary>�ܼ�ֵ/summary>
        public const int F_TOTAL_VALUE = 49;
        /// <summary>�ؽ�����/summary>
        public const int F_REBUILD_COUNT = 50;

        /// <summary>��ҵ/summary>
        public const int F_BUSINESS = 51;
        /// <summary>��ҵ/summary>
        public const int F_MINING = 52;
        /// <summary>��ҵ/summary>
        public const int F_INDUSTRY = 53;
        /// <summary>����/summary>
        public const int F_DEFENCE = 54;
        /// <summary>����/summary>
        public const int F_DEPTH = 55;
        /// <summary>����/summary>
        public const int F_LANDFORM = 56;

        /// <summary>��������/summary>
        public const int F_PRESTIGE = 57;
        /// <summary>�;�/summary>
        public const int F_STAMINA = 58;
        /// <summary>����/summary>
        public const int F_PRECISE = 59;

        #region ������Ϣ�ֶ�
        public readonly static List<int> IntFieldsBase = new List<int>
        {
            F_ID,
            F_S_ID,
            F_CONF_ID,
        };

        public readonly static List<int> StringFieldsBase = new List<int>
        {
            F_NAME,
        };
        #endregion

        #region ������Ϣ�ֶ�
        public readonly static List<int> IntFieldsLegion = new List<int>
        {
            F_PRESTIGE,
            F_TROOPS,
        };
        #endregion

        #region ��Դ��Ϣ�ֶ�
        public readonly static List<int> IntFieldsResources = new List<int>
        {
            F_CREDIT_POINT,
            F_METAL,
            F_ENERGY,
            F_SUPPLIES,
        };
        #endregion

        #region ������Ϣ�ֶ�
        public readonly static List<int> IntFieldsTroops = new List<int>
        {
            F_TROOPS_SERIAL_NUM,
            F_TROOPS_TYPE,
            F_TROOPS_OFFICER_ID,
            F_TROOPS_ANNIHILATION,
            F_TROOPS,
            F_CRACK,
            F_WILL,
            F_PROVISIONS,
            F_MAIN_EQUIPMENT_ID,
            F_STAMINA,
        };
        #endregion

        #region ������Ϣ�ֶ�
        public readonly static List<int> IntFieldsOrganization = new List<int>
        {
            F_ORGANIZATION_VALUE,
            F_IS_BASE_ORGANIZATION,
        };

        public readonly static List<int> StringFieldsOrganization = new List<int>
        {
            F_ORG_LEVEL_NAME,
        };
        #endregion

        #region ������Ϣ�ֶ�
        public readonly static List<int> IntFieldsOfficer = new List<int>
        {
            F_GENDER,
            F_LOYAL,
            F_ACHIEVEMENT,
        };

        public readonly static List<int> StringFieldsOfficer = new List<int>
        {
            F_FULL_NAME,
        };
        #endregion

        #region ����������Ϣ�ֶ�
        public readonly static List<int> IntFieldsRole = new List<int>
        {
            F_ROLE_COMMAND,
            F_ROLE_THINKING,
            F_ROLE_INFORMATION,
            F_ROLE_SCIENTIFIC,
            F_ROLE_SOCIAL,
        };
        #endregion

        #region װ��������Ϣ�ֶ�
        public readonly static List<int> IntFieldsEquipment = new List<int>
        {
            F_RUIN,
            F_KILL,
            F_AIR_DEF,
            F_INTENSIFY,
            F_ECM_WAR,
            F_TRANSPORT,
            F_SCIENCE_TECH,
        };
        #endregion

        #region �ݵ�������Ϣ�ֶ�
        public readonly static List<int> IntFieldsFortifiedPoint = new List<int>
        {
            F_BUSINESS,
            F_MINING,
            F_INDUSTRY,
            F_DEFENCE,
            F_DEPTH,
            F_LANDFORM,
        };
        #endregion
    }

}
