using System.Collections.Generic;

namespace ShipDock
{
    public interface IFieldableData
    {
        int GetIntData(int fieldName);
        void SetIntData(int fieldName, int value);
        float GetFloatData(int fieldName);
        void SetFloatData(int fieldName, float value);
        string GetStringData(int fieldName);
        void SetStringData(int fieldName, string value);
        List<int> IntFieldNames { get; }
        List<int> FloatFieldNames { get; }
        List<int> StringFieldNames { get; }
    }

}