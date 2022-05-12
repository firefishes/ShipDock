﻿using ShipDock.Datas;
using System.Collections.Generic;

namespace IsKing
{
    public class DataInfo : FieldableData
    {
        public DataInfo()
        { 
            FillValues();
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void LogFields()
        {
            int max = StringFieldNames.Count;
            for (int i = 0; i < max; i++)
            {
                LogStringValue(i);
            }

            max = IntFieldNames.Count;
            for (int i = 0; i < max; i++)
            {
                LogIntValue(i);
            }

            max = FloatFieldNames.Count;
            for (int i = 0; i < max; i++)
            {
                LogFloatValue(i);
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        protected void LogStringValue(int i)
        {
            int n = StringFieldNames[i];
            string s = Consts.CommonNames[n];
            "log:   {0} = {1}".Log(!string.IsNullOrEmpty(s), s, GetStringData(n).ToString());
        }

        [System.Diagnostics.Conditional("G_LOG")]
        protected void LogIntValue(int i)
        {
            int n = IntFieldNames[i];
            string s = Consts.FieldNames[n];
            "log:   {0} = {1}".Log(!string.IsNullOrEmpty(s), s, GetIntData(n).ToString());
        }

        [System.Diagnostics.Conditional("G_LOG")]
        protected void LogFloatValue(int i)
        {
            int n = FloatFieldNames[i];
            string s = Consts.FieldNames[n];
            "log:   {0} = {1}".Log(!string.IsNullOrEmpty(s), s, GetFloatData(n).ToString());
        }
    }
}
