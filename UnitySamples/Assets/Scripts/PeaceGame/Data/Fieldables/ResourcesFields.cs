using System.Collections.Generic;
using ShipDock.Config;
using ShipDock.Tools;

namespace Peace
{
    /// <summary>
    /// ×ÊÔ´ÊôÐÔ×Ö¶Î
    /// </summary>
    public class ResourcesFields : BaseFields
    {
        private static List<int> newIntFields;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsResources);

        public override void InitFieldsFromConfig(IConfig config)
        {
            base.InitFieldsFromConfig(config);

            FillValues(true);
        }
    }
}