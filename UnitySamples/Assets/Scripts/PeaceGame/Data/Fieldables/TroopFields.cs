using ShipDock.Config;
using System.Collections.Generic;

namespace Peace
{
    public interface ITroopFields
    {
        int GetTroops();
        void SetTroops(int current, int max = -1);
    }

    public interface IBattleElementFields
    {
        int GetStamina();
        void SetStamina(int current);
    }

    /// <summary>
    /// ���������ֶ�
    /// </summary>
    public class TroopFields : BaseFields, ITroopFields, IBattleElementFields
    {
        /// <summary>��������</summary>
        private TroopFields mOwner;
        /// <summary>������ؼ�</summary>
        private VolumeGroupControl mVolumeGroupControl;

        public override List<int> StringFieldNames { get; protected set; } = FieldsConsts.IntFieldsTroops;

        public override void InitFieldsFromConfig(IConfig config)
        {
            base.InitFieldsFromConfig(config);

            mOwner = this;

            mVolumeGroupControl = new VolumeGroupControl();

            ValueVolumeGroup group = mVolumeGroupControl.VolumeGroup;
            group.AddValueVolume(FieldsConsts.F_TROOPS);

            if (config == default)
            {
                SetTroops(0, 0);
            }
            else { }

            FillValues(true);
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
    }
}