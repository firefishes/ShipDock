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

        public override void InitFieldsFromConfig(IConfig config)
        {
            base.InitFieldsFromConfig(config);

            PeaceOfficer peaceOfficer = config as PeaceOfficer;
            int[] properties = peaceOfficer.GetRolePropertyValues();

            if (IsInited)
            {
                mIntFieldSource.Clear();
            }
            else
            {
                SetDefaultIntData(FieldsConsts.IntFieldsRole);
            }

            int max = properties.Length;
            for (int i = 0; i < max; i++)
            {
                mIntFieldSource.Add(properties[i]);
            }

            FillValues(!IsInited);
        }
    }
}
