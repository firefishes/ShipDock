using ShipDock.Config;

using ShipDock.Tools;

namespace StaticConfig
{
    public partial class PeaceOfficer : IConfig
    {
        /// <summary>
        /// id
        /// <summary>
        public int id;

        /// <summary>
        /// 名称
        /// <summary>
        public string fullName;
        /// <summary>
        /// 性别
        /// <summary>
        public int gender;
        /// <summary>
        /// 忠诚
        /// <summary>
        public string loyal;
        /// <summary>
        /// 战功
        /// <summary>
        public string achievement;
        /// <summary>
        /// 人物属性
        /// <summary>
        public string roleProperties;
        
        public string CRCValue { get; }

        public int GetID()
        {
            return id;
        }

        public void Parse(ByteBuffer buffer)
        {
            id = buffer.ReadInt();
            fullName = buffer.ReadString();
            gender = buffer.ReadInt();
            loyal = buffer.ReadString();
            achievement = buffer.ReadString();
            roleProperties = buffer.ReadString();
            
        }

    }
}
