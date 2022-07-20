using ShipDock.Config;
using ShipDock.Pooling;
using StaticConfig;
using System.Collections.Generic;

namespace Peace
{
    /// <summary>
    /// »ÀŒÔ Ù–‘◊÷∂Œ
    /// </summary>
    public class RoleFields : BaseFields
    {
        private static List<int> newIntFields;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsRole);

        public override void ToPool()
        {
            Pooling<RoleFields>.To(this);
        }

        protected override void Init()
        {
            if (IsInited)
            {
                mIntFieldSource.Clear();

                IDAdvanced();
                AfterFilledData();
            }
            else
            {
                SetDefaultIntData(FieldsConsts.IntFieldsRole);

                FillValues(true);
            }
        }

        protected override void AfterFilledData()
        {
            base.AfterFilledData();

            PeaceOfficer peaceOfficer = Config as PeaceOfficer;
            int[] properties = peaceOfficer != default ? peaceOfficer.GetRolePropertyValues() : default;

            int max = properties != default ? properties.Length : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    SetIntData(FieldsConsts.IntFieldsRole[i], properties[i]);
                }
            }
            else { }
        }
    }
}
