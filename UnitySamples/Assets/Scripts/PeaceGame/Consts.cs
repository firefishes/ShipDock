using ShipDock.Applications;
using ShipDock.Config;
using System.Collections.Generic;

namespace Peace
{
    public static class Consts
    {
        #region ���ݲ������
        /// <summary>ս������</summary>
        public const int D_BATTLE = 0;
        /// <summary>�������</summary>
        public const int D_PLAYER = 1;
        /// <summary>��������</summary>
        public const int D_CONFIGS = 2;
        #endregion

        public const int M_MESSAGE = 2000;
        public const int M_BATTLE = 2001;

        public const int N_MSG_ADD = 10000;
        public const int N_MSG_QUEUE = 10001;

        public const int MSG_GAME_READY = 20000;
        public const int MSG_ENTER_BATTLE = 20001;

        public const int CONF_GROUP_CONFIGS = 1;

        public const string CONF_EQUIPMENT = "equipments";
        public const string CONF_ORGANIZATIONS = "organizations";

        public const string AB_CONFIGS = "peace/configs";

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

        /// <summary>��Ϸ���ð����л�ȡ���õ���չ����</summary>
        public static Dictionary<int, ConfigT> GetConfigTable<ConfigT>(this string configName) where ConfigT : IConfig, new()
        {
            ConfigData data = D_CONFIGS.GetData<ConfigData>();
            ConfigsResult configs = data.GetConfigs(CONF_GROUP_CONFIGS);
            Dictionary<int, ConfigT> dic = configs.GetConfigRaw<ConfigT>(configName, out _);
            return dic;
        }
    }
}