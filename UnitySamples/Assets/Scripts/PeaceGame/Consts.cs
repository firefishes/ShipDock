using ShipDock.Applications;
using ShipDock.Config;
using System.Collections.Generic;

namespace Peace
{
    /// <summary>
    /// ������
    /// </summary>
    public static class Consts
    {
        #region ���ݲ������
        /// <summary>ս������</summary>
        public const int D_BATTLE = 0;
        /// <summary>�������</summary>
        public const int D_PLAYER = 1;
        /// <summary>��������</summary>
        public const int D_CONFIGS = 2;
        /// <summary>��������</summary>
        public const int D_LEGION = 3;
        /// <summary>��������</summary>
        public const int D_TROOPS = 4;
        #endregion

        #region ģ����
        public const int M_MESSAGE = 2000;
        public const int M_BATTLE = 2001;
        public const int M_WORLD = 2002;
        public const int M_DATA = 2003;
        #endregion

        #region ��Ϣ��
        //public const int N_MSG_ADD = 10000;
        //public const int N_MSG_QUEUE = 10001;
        #endregion

        #region �ܵ���Ϣ��
        public const int MSG_GAME_READY = 20000;
        public const int MSG_ENTER_BATTLE = 20001;
        public const int MSG_ADD_UPDATER = 20002;
        public const int MSG_RM_UPDATER = 20003;
        #endregion

        #region ������Ϣ��
        public const int DN_NEW_GAME_CREATED = 30000;
        #endregion

        #region ����
        public const int CONF_GROUP_CONFIGS = 1;

        public const string CONF_EQUIPMENT = "equipments";
        public const string CONF_ORGANIZATIONS = "organizations";
        #endregion

        #region ��Դ����
        public const string AB_CONFIGS = "peace/configs";
        public const string AB_LOGOIN = "peace/ui_login";
        public const string AB_UI_MAIN = "peace/ui_main";
        #endregion

        #region UI��Դ��
        public const string U_LOGIN = "UILogin";
        public const string U_HEADQUARTERS = "UIHeadquarters";
        #endregion

        #region UIģ����
        public const string UM_LOGIN = "UMLogin";
        public const string UM_HEADQUARTERS = "UMHeadquarters";
        #endregion

        #region ����
        /// <summary>��ξ��</summary>
        public const int ORG_TYPE_P = 1;
        /// <summary>��ξ��</summary>
        public const int ORG_TYPE_C = 2;
        /// <summary>��ξ��</summary>
        public const int ORG_TYPE_B = 3;
        /// <summary>��У��</summary>
        public const int ORG_TYPE_R = 4;
        /// <summary>��У��</summary>
        public const int ORG_TYPE_BR = 5;
        /// <summary>��У��</summary>
        public const int ORG_TYPE_D = 6;
        /// <summary>׼����</summary>
        public const int ORG_TYPE_A = 7;
        /// <summary>�ٽ���</summary>
        public const int ORG_TYPE_GA = 8;
        /// <summary>�н���</summary>
        public const int ORG_TYPE_MGA = 9;
        /// <summary>�Ͻ���</summary>
        public const int ORG_TYPE_LGA = 10;
        /// <summary>�����Ͻ���</summary>
        public const int ORG_TYPE_FA = 11;
        /// <summary>һ���Ͻ���</summary>
        public const int ORG_TYPE_MFA = 12;
        /// <summary>�ص��Ͻ���</summary>
        public const int ORG_TYPE_LFA = 13;
        #endregion

        #region ��������
        /// <summary>���沿��</summary>
        public const int TROOP_TYPE_COMMON = 1;
        /// <summary>��첿��</summary>
        public const int TROOP_TYPE_RECONNOITRE = 2;
        /// <summary>��������</summary>
        public const int TROOP_TYPE_FIREPOWER = 3;
        /// <summary>���䲿��</summary>
        public const int TROOP_TYPE_TRANSPORT = 4;
        /// <summary>װ�ײ���</summary>
        public const int TROOP_TYPE_ARMOURED = 5;
        /// <summary>��������</summary>
        public const int TROOP_TYPE_NAVY = 6;
        /// <summary>���첿��</summary>
        public const int TROOP_TYPE_SPACE_AIR = 7;
        #endregion

        /// <summary>ECS����������</summary>
        public const int ECS_CONTEXT_PEACE = 1;

        /// <summary>����ϵͳ</summary>
        public const int SYSTEM_WORLD = 1;

        /// <summary>λ�����</summary>
        public const int COMP_MOVEMENT = 1;
    }
}