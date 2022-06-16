using ShipDock.Applications;
using ShipDock.Config;
using System.Collections.Generic;
/// <summary>
/// ��̬����
/// </summary>
public static class SampleConsts
{
    /// <summary>���ݴ������е����� ID</summary>
    public const int D_SAMPLE = 1;
    /// <summary>���ؿͻ����˻������е����� ID</summary>
    public const int D_PLAYER = 2;
    /// <summary>MVC�����е����� ID</summary>
    public const int D_SAMPLE_MVC = 3;
    /// <summary>��Ϸ���ð����е����� ID</summary>
    public const int D_CONFIGS = 4;

    #region ��Ϣ�ַ�������ص���Ϣ��
    public const int N_SAMPLE_NOTICE_BY_PARAM = 1000;
    public const int N_SAMPLE_NOTICE_BY_DEFAULT= 1001;
    public const int N_SAMPLE_NOTICE_BY_OBSERVER = 1002;
    #endregion

    /// <summary>�߼�ģ�鰸����Ϣ������ʼ��Ϸ</summary>
    public const int N_SAMPLE_GAME_START = 1003;
    /// <summary>�߼�ģ�鰸����Ϣ��������ؿ�</summary>
    public const int N_SAMPLE_GAME_ENTER_MISSION = 1004;
    /// <summary>�߼�ģ�鰸����Ϣ�������عؿ�</summary>
    public const int N_SAMPLE_GAME_LOAD_MISSION = 1005;
    /// <summary>�߼�ģ�鰸����Ϣ�����ؿ�����</summary>
    public const int N_SAMPLE_GAME_MISSION_FINISHED = 1006;
    /// <summary>�߼�ģ�鰸����Ϣ��������ģ�鰸����ʾ���</summary>
    public const int N_SAMPLE_MODULARS_END = 1007;

    /// <summary>���ݴ������е����ݱ����Ϣ</summary>
    public const int DN_SAMPLE_DATA_NOTIFY = 2000;
    /// <summary>MVC�����е����ݱ����Ϣ</summary>
    public const int DN_SAMPLE_MVC_DATA_CHANGED = 2001;

    /// <summary>�߼�ģ�鰸���е�ģ��������Ϸ����ģ��</summary>
    public const int M_SAMPLE_GAME_START = 3001;
    /// <summary>�߼�ģ�鰸���е�ģ�������ؿ�ģ��</summary>
    public const int M_SAMPLE_GAME_MISSIONS = 3002;

    /// <summary>MVC�����е���Դ����</summary>
    public const string AB_SAMPLES = "sample_res";
    /// <summary>MVC�����е�UIģ����</summary>
    public const string U_SAMPLE = "SampleWindow";

    /// <summary>��Ϸ���ð����е������飺Ĭ����</summary>
    public const int CONF_GROUP_CONFIGS = 4000;

    /// <summary>��Ϸ���ð����е�������</summary>
    public const string CONF_PUZZLES = "puzzles_config";

    /// <summary>��Ϸ���ð����л�ȡ���õ���չ����</summary>
    public static Dictionary<int, ConfigT> GetConfigTable<ConfigT>(this string configName) where ConfigT : IConfig, new()
    {
        ConfigData data = D_CONFIGS.GetData<ConfigData>();
        ConfigsResult configs = data.GetConfigs(CONF_GROUP_CONFIGS);
        Dictionary<int, ConfigT> dic = configs.GetConfigRaw<ConfigT>(configName, out _);
        return dic;
    }
}