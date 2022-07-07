using ShipDock.Config;

using ShipDock.Tools;

namespace StaticConfig
{
    public partial class PeaceOrganizations : IConfig
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
        /// 编制名
        /// <summary>
        public string levelName;
        /// <summary>
        /// 编制消耗
        /// <summary>
        public int organaizationValue;
        /// <summary>
        /// 兵力
        /// <summary>
        public int troops;
        /// <summary>
        /// 装甲单位
        /// <summary>
        public int armoredUnit;
        /// <summary>
        /// 海空单位
        /// <summary>
        public int seaAndAirUnit;
        /// <summary>
        /// 是否基本编制
        /// <summary>
        public bool isCommon;
        
        public string CRCValue { get; }

        public int GetID()
        {
            return id;
        }

        public void Parse(ByteBuffer buffer)
        {
            id = buffer.ReadInt();
            name = buffer.ReadString();
            levelName = buffer.ReadString();
            organaizationValue = buffer.ReadInt();
            troops = buffer.ReadInt();
            armoredUnit = buffer.ReadInt();
            seaAndAirUnit = buffer.ReadInt();
            isCommon = buffer.ReadInt() != 0;
            
        }

    }
}
