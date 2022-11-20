using ShipDock.Config;

using ShipDock.Tools;

namespace StaticConfig
{
    public partial class IsKingGenerals : IConfig
    {
        /// <summary>
        /// id
        /// <summary>
        public int id;

        /// <summary>
        /// 名称
        /// <summary>
        public string name;
        /// <summary>
        /// 模型 ID
        /// <summary>
        public int modelID;
        /// <summary>
        /// 类别
        /// <summary>
        public int equipmentType;
        /// <summary>
        /// 评价
        /// <summary>
        public int assess;
        /// <summary>
        /// 适用性
        /// <summary>
        public int applyTo;
        /// <summary>
        /// 装备属性
        /// <summary>
        public string propertyValues;
        /// <summary>
        /// 精密
        /// <summary>
        public int precise;
        
        public string CRCValue { get; }

        public int GetID()
        {
            return id;
        }

        public void Parse(ByteBuffer buffer)
        {
            id = buffer.ReadInt();
            name = buffer.ReadString();
            modelID = buffer.ReadInt();
            equipmentType = buffer.ReadInt();
            assess = buffer.ReadInt();
            applyTo = buffer.ReadInt();
            propertyValues = buffer.ReadString();
            precise = buffer.ReadInt();
            
        }

    }
}
