using ShipDock.Pooling;
using System.Collections.Generic;

namespace Peace
{
    /// <summary>
    /// æ›µ„ Ù–‘◊÷∂Œ
    /// </summary>
    public class StrongholdFields : BaseFields
    {
        private static List<int> newIntFields;

        public override List<int> IntFieldNames { get; protected set; } = GetNewIntFields(ref newIntFields, FieldsConsts.IntFieldsFortifiedPoint);

        public override void ToPool()
        {
            Pooling<StrongholdFields>.To(this);
        }

        protected override void Init()
        {
            if (IsInited)
            {
                IDAdvanced();
                AfterFilledData();
            }
            else
            {
                SetDefaultIntData(FieldsConsts.IntFieldsFortifiedPoint);
                FillValues(true);
            }
        }

        protected override void AfterFilledData()
        {
            base.AfterFilledData();


        }
    }
}