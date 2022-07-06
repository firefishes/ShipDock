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
        public override List<int> IntFieldNames { get; protected set; } = FieldsConsts.IntFieldsResources;

        public override void InitFieldsFromConfig(IConfig config)
        {
            base.InitFieldsFromConfig(config);

            FillValues(true);
        }
    }
}