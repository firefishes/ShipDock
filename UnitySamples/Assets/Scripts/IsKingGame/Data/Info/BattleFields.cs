using System.Collections.Generic;

namespace IsKing
{
    public class BattleFields : DataInfo
    {
        public override List<int> FloatFieldNames { get; protected set; } = Consts.FieldsBattleInfoFloat;
        public override List<int> IntFieldNames { get; protected set; } = Consts.FieldsBattleInfoInt;
    }
}