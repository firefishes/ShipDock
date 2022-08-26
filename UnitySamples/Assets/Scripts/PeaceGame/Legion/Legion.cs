using System.Collections.Generic;
using UnityEngine;

namespace Peace
{
    public interface ILegion
    {
        int ID { get; }
        int SID { get; }
        int RelationType { get; }

        void SetRelationType(int type);
    }

    public static class LegionRelationType
    {
        /// <summary>我军</summary>
        public const int LEGION_SELF = 0;
        /// <summary>中立</summary>
        public const int LEGION_NEUTRAL = 1;
        /// <summary>敌军</summary>
        public const int LEGION_ENEMY = 2;
        /// <summary>友军</summary>
        public const int LEGION_ALLY = 3;
    }

    /// <summary>
    /// 军团
    /// </summary>
    public class Legion : ILegion
    {
        private LegionFields mPlayerLegion;
        private List<int> mStrongholdIDs;

        public int ID
        {
            get
            {
                return mPlayerLegion.GetID();
            }
        }

        public int SID
        {
            get
            {
                return mPlayerLegion.GetIntData(FieldsConsts.F_S_ID);
            }
        }

        public int RelationType { get; private set; }

        public void Init()
        {
            mPlayerLegion = new LegionFields();
            mPlayerLegion.InitFields();
        }

        public void SetRelationType(int type)
        {
            RelationType = type;
        }

        public void SyncPlayerLegion(int id)
        {
            mPlayerLegion.SetIntData(FieldsConsts.F_ID, id);
        }

        public void SyncLegionOfficerID(int officerID, int headquartersID)
        {
            mPlayerLegion.SetLegionOfficerID(officerID);
            mPlayerLegion.SetLegionHeadquartersID(headquartersID);
        }

        public void SyncPlayerResource(int credit, Vector2Int metal, Vector2Int energy, Vector2Int supplies)
        {
            mPlayerLegion.SetCreditPoint(credit);
            mPlayerLegion.SetMetal(metal.x, metal.y);
            mPlayerLegion.SetEnergy(energy.x, energy.y);
            mPlayerLegion.SetSupplies(supplies.x, supplies.y);
        }

        public void SyncLegionTroops(Vector2Int troops)
        {
            mPlayerLegion.SetTroops(troops.x, troops.y);
        }

        public void SyncLegionPrestige(int prestige)
        {
            mPlayerLegion.SetPrestige(prestige);
        }
    }
}

