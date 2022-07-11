using StaticConfig;

namespace Peace
{
    #region ������ؽӿ�
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
    #endregion

    #region ������ؽӿ�
    public interface IOrganizationFields
    {
        void SetOrganizationValue(int value);
        int GetOrganizationValue();
        PeaceOrganizations OrganizationsConfig { get; }
    }
    #endregion

    #region ������ؽӿ�
    public interface ILegionFields
    {
        void SetLegionOfficerID(int officerID);
        void SetLegionHeadquartersID(int headquartersID);
    }
    #endregion

    #region ������ؽӿ�
    /// <summary>
    /// �����ֶνӿ�
    /// </summary>
    public interface IOfficerFields : ITroopOrganization, IOrganizationFields
    {
        RoleFields RoleFields { get; }
        int GetMilitaryRank();
        string GetStripes();
    }
    #endregion
}
