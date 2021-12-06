using System.Collections.Generic;

namespace IsKing
{
    public class CardFields : DataInfo
    {
        public override List<int> IntFieldNames { get; protected set; } = Consts.FieldsCardInfoInt;
    }

}