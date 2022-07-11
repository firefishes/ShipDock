using ShipDock.Config;
using ShipDock.Pooling;
using System.Collections.Generic;

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

            if (IsInited) { }
            else
            {
                FillValues(true);
            }
        }

        public override void ToPool()
        {
            Pooling<ResourcesFields>.To(this);
        }
    }
}