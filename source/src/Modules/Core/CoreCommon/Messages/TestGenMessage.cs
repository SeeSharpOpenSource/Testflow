using System.Runtime.Serialization;
using Testflow.MasterCore.Common;

namespace Testflow.MasterCore.Message.Messages
{
    /// <summary>
    /// 测试生成报告消息
    /// </summary>
    public class TestGenMessage : MessageBase
    {
        public int SequenecIndex { get; set; }

        public TestGenState State { get; set; }

        public TestGenMessage(string name, int id, int sequenceIndex, TestGenState state) : base(name, id, MessageType.TestGen)
        {
            this.SequenecIndex = sequenceIndex;
            this.State = state;
        }

        public TestGenMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.SequenecIndex = (int) info.GetValue("SequenceIndex", typeof(int));
            this.State = (TestGenState) info.GetValue("State", typeof(TestGenState));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("SequenecIndex", SequenecIndex);
            info.AddValue("State", State);
        }
    }
}