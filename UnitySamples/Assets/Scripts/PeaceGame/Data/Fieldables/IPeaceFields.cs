using StaticConfig;

namespace Peace
{
    #region 部队相关接口
    /// <summary>
    /// 部队字段接口
    /// </summary>
    public interface ITroopFields
    {
        int GetTroops();
        void SetTroops(int current, int max = -1);
    }

    /// <summary>
    /// 战场元素接口
    /// </summary>
    public interface IBattleElementFields
    {
        int GetStamina();
        void SetStamina(int current);
    }

    /// <summary>
    /// 部队编制接口
    /// </summary>
    public interface ITroopOrganization
    {
        OrganizationFields Organization { get; }
        string TroopLevelName();
        int OrganizationValue { get; }
    }
    #endregion

    #region 编制相关接口
    public interface IOrganizationFields
    {
        void SetOrganizationValue(int value);
        int GetOrganizationValue();
        PeaceOrganizations OrganizationsConfig { get; }
    }
    #endregion

    #region 军团相关接口
    public interface ILegionFields
    {
        void SetLegionOfficerID(int officerID);
        void SetLegionHeadquartersID(int headquartersID);
    }
    #endregion

    #region 军官相关接口
    /// <summary>
    /// 军官字段接口
    /// </summary>
    public interface IOfficerFields : ITroopOrganization, IOrganizationFields
    {
        RoleFields RoleFields { get; }
        int GetMilitaryRank();
        string GetStripes();
    }
    #endregion
}
