using ShipDock;

namespace StaticConfig
{
    public partial class PuzzlesConfig : IConfig
    {
        /// <summary>
        /// 配置id
        /// <summary>
        public int id;

        /// <summary>
        /// 问题
        /// <summary>
        public string question;
        /// <summary>
        /// 答案
        /// <summary>
        public string all_answers;
        /// <summary>
        /// 正确答案
        /// <summary>
        public int answer;
        
        public string CRCValue { get; }

        public int GetID()
        {
            return id;
        }

        public void Parse(ByteBuffer buffer)
        {
            id = buffer.ReadInt();
            question = buffer.ReadString();
            all_answers = buffer.ReadString();
            answer = buffer.ReadInt();
            
        }

    }
}
